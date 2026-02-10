using BLL.DTOs;
using BLL.DTOs.Moderation;
using BLL.DTOs.ReviewProduct;
using BLL.Interfaces;
using BLL.Interfaces.Moderation;
using DAL.Infrastructure;
using DAL.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class ReviewProductService : IReviewProductService
    {
        private readonly IReviewProductRepository _reviewRepo;
        private readonly IReviewProductImageRepository _imageRepo;
        private readonly IReviewProductReactionRepository _reactionRepo;
        private readonly IReviewModerationLogRepository _moderationLogRepo;
        private readonly IReviewModerationOrchestrator _moderationOrchestrator;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewProductService(
            IReviewProductRepository reviewRepo,
            IReviewProductImageRepository imageRepo,
            IReviewProductReactionRepository reactionRepo,
            IReviewModerationLogRepository moderationLogRepo,
            IReviewModerationOrchestrator moderationOrchestrator,
            ICloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork)
        {
            _reviewRepo = reviewRepo;
            _imageRepo = imageRepo;
            _reactionRepo = reactionRepo;
            _moderationLogRepo = moderationLogRepo;
            _moderationOrchestrator = moderationOrchestrator;
            _cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
        }

        // === ADD REVIEW ===
        public async Task<ApiResponse<ReviewResponse>> AddReviewAsync(
            AddReviewRequest request, int accountId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // === STEP 1: Validate OrderDetail ===
                // ...

                // Check đã review chưa
                // ...

                // === STEP 2: Upload Images ===
                var imageUrls = new List<string>();
                if (request.Images?.Any() == true)
                {
                    foreach (var image in request.Images)
                    {
                        var url = await SaveImageAsync(image, 1);
                        imageUrls.Add(url);
                    }
                }

                // === STEP 3: Run Moderation ===
                var moderationRequest = new ModerationRequest
                {
                    ReviewText = request.Comment,
                    ImageUrls = imageUrls,
                    AccountId = accountId,
                    ProductId = 1
                };
                var moderationResult = await _moderationOrchestrator.ModerateAsync(moderationRequest);

                // === STEP 4: Create Review ===
                var review = new ReviewProduct
                {
                    AccountId = accountId,
                    ProductId = 1,
                    OrderDetailId = request.OrderDetailId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    Status = moderationResult.Decision.Status,
                    ModerationScore = moderationResult.Decision.FinalScore,
                    ModerationDetail = moderationResult.Decision.Reason,
                    Visibility = moderationResult.IsApproved ? "Public" : "Private",
                    EditCount = 0,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _reviewRepo.AddAsync(review);

                // === STEP 5: Save Images ===
                if (imageUrls.Any())
                {
                    var images = imageUrls.Select(url => new ReviewProductImage
                    {
                        ReviewProductId = review.ReviewProductId,
                        ImageUrl = url,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                    await _imageRepo.AddRangeAsync(images);
                }

                // === STEP 6: Log Moderation ===
                await SaveModerationLogAsync(review.ReviewProductId, moderationResult);

                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<ReviewResponse>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = GetStatusMessage(moderationResult.Decision.Status),
                    Data = await MapToResponseAsync(review, accountId)
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return new ApiResponse<ReviewResponse>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = $"Có lỗi xảy ra khi tạo review: {ex.Message}"
                };
            }
        }

        // === EDIT REVIEW ===
        public async Task<ApiResponse<ReviewResponse>> EditReviewAsync(
            int reviewId, EditReviewRequest request, int accountId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // === STEP 1: Validate ===
                var review = await _reviewRepo.GetByIdAsync(reviewId);

                if (review == null || review.IsDeleted)
                {
                    return new ApiResponse<ReviewResponse>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy review"
                    };
                }

                if (!await _reviewRepo.CanEditAsync(reviewId, accountId))
                {
                    return new ApiResponse<ReviewResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Review chỉ được sửa 1 lần trong 3 ngày đầu"
                    };
                }

                // === STEP 2: Handle Images ===
                var currentImages = await _imageRepo.GetByReviewIdAsync(reviewId);
                var keepSet = new HashSet<string>(
                    request.KeepImageUrls ?? new List<string>(),
                    StringComparer.OrdinalIgnoreCase);

                // Delete images not in keep list
                var toDelete = currentImages.Where(i => !keepSet.Contains(i.ImageUrl)).ToList();
                if (toDelete.Any())
                {
                    foreach (var img in toDelete)
                    {
                        DeleteImageFile(img.ImageUrl);
                    }
                    await _imageRepo.DeleteRange(toDelete);
                }

                // Upload new images
                var newImageUrls = new List<string>();
                if (request.NewImages?.Any() == true)
                {
                    foreach (var image in request.NewImages)
                    {
                        var url = await SaveImageAsync(image, review.ProductId);
                        newImageUrls.Add(url);
                    }

                    var newImages = newImageUrls.Select(url => new ReviewProductImage
                    {
                        ReviewProductId = reviewId,
                        ImageUrl = url,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                    await _imageRepo.AddRangeAsync(newImages);
                }

                // === STEP 3: Re-run Moderation ===
                var allImageUrls = keepSet.Concat(newImageUrls).ToList();
                var moderationRequest = new ModerationRequest
                {
                    ReviewText = request.Comment,
                    ImageUrls = allImageUrls,
                    AccountId = accountId,
                    ProductId = review.ProductId
                };
                var moderationResult = await _moderationOrchestrator.ModerateAsync(moderationRequest);

                // === STEP 4: Update Review ===
                review.Rating = request.Rating;
                review.Comment = request.Comment;
                review.Status = moderationResult.Decision.Status;
                review.ModerationScore = moderationResult.Decision.FinalScore;
                review.ModerationDetail = moderationResult.Decision.Reason;
                review.Visibility = moderationResult.IsApproved ? "Public" : "Private";
                review.UpdatedAt = DateTime.UtcNow;

                await _reviewRepo.UpdateAsync(review);
                await _reviewRepo.IncrementEditCountAsync(reviewId);

                // === STEP 5: Log Moderation ===
                await SaveModerationLogAsync(reviewId, moderationResult);

                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<ReviewResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = GetStatusMessage(moderationResult.Decision.Status),
                    Data = await MapToResponseAsync(review, accountId)
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                return new ApiResponse<ReviewResponse>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = $"Có lỗi xảy ra khi chỉnh sửa review: {ex.Message}"
                };
            }
        }

        // === GET REVIEWS ===
        public async Task<ApiResponse<PagedResult<ReviewResponse>>> GetReviewsAsync(
            ReviewListQuery query, int? currentAccountId = null)
        {
            var (items, total) = await _reviewRepo.GetPagedAsync(
                query.ProductId,
                query.Page,
                query.PageSize,
                query.Status);

            var responses = new List<ReviewResponse>();
            foreach (var review in items)
            {
                responses.Add(await MapToResponseAsync(review, currentAccountId));
            }

            return new ApiResponse<PagedResult<ReviewResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách review",
                Data = new PagedResult<ReviewResponse>
                {
                    Items = responses,
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        // === GET REVIEW BY ID ===
        public async Task<ApiResponse<ReviewResponse>> GetReviewByIdAsync(
            int reviewId,
            int? currentAccountId = null)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId);

            if (review == null || review.IsDeleted)
            {
                return new ApiResponse<ReviewResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy review"
                };
            }

            // Check visibility
            if (review.Visibility == "Private" && review.AccountId != currentAccountId)
            {
                return new ApiResponse<ReviewResponse>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Review này không công khai"
                };
            }

            return new ApiResponse<ReviewResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Chi tiết review",
                Data = await MapToResponseAsync(review, currentAccountId)
            };
        }

        // === HELPERS ===
        private async Task<string> SaveImageAsync(IFormFile file, int productId)
        {
            // Upload to Cloudinary instead of local filesystem
            var folder = $"reviews/{productId}";
            var imageUrl = await _cloudinaryService.UploadImageAsync(file, folder);
            return imageUrl;
        }

        private async void DeleteImageFile(string imageUrl)
        {
            try
            {
                // Delete from Cloudinary only (new uploads)
                var publicId = _cloudinaryService.ExtractPublicId(imageUrl);
                await _cloudinaryService.DeleteImageAsync(publicId);
            }
            catch
            {
                // Ignore deletion errors (old local images or already deleted)
            }
        }

        private async Task SaveModerationLogAsync(int reviewId, ModerationResponse result)
        {
            var log = new ReviewModerationLog
            {
                ReviewProductId = reviewId,
                Stage = result.Decision.Stage,
                Score = result.Decision.FinalScore,
                Result = result.Decision.Status,
                Details = result.Decision.Reason,
                //RawData = System.Text.Json.JsonSerializer.Serialize(result),
                CreatedAt = DateTime.UtcNow
            };

            await _moderationLogRepo.AddAsync(log);
        }

        private async Task<ReviewResponse> MapToResponseAsync(
            ReviewProduct review, int? currentAccountId)
        {
            var likeCount = await _reactionRepo.GetLikeCountAsync(review.ReviewProductId);
            var dislikeCount = await _reactionRepo.GetDislikeCountAsync(review.ReviewProductId);

            bool isLiked = false;
            if (currentAccountId.HasValue)
            {
                var reaction = await _reactionRepo.GetReactionAsync(
                    review.ReviewProductId,
                    currentAccountId.Value);
                isLiked = reaction?.ReactionType == "Like";
            }

            return new ReviewResponse
            {
                ReviewProductId = review.ReviewProductId,
                AccountId = review.AccountId,
                AccountName = review.Account?.AccountName ?? "Unknown",
                AccountImage = review.Account?.Image,
                ProductId = review.ProductId,
                ProductName = review.Product?.ProductName ?? "Unknown",
                Rating = review.Rating,
                Comment = review.Comment ?? "No comment",
                ImageUrls = review.ReviewProductImages
                    .Where(i => !i.IsDeleted)
                    .Select(i => i.ImageUrl)
                    .ToList(),
                Status = review.Status,
                ModerationDetail = review.ModerationDetail,
                LikeCount = likeCount,
                DislikeCount = dislikeCount,
                IsLikedByCurrentUser = isLiked,
                EditCount = review.EditCount,
                CanEdit = await _reviewRepo.CanEditAsync(review.ReviewProductId, review.AccountId),
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        private string GetStatusMessage(string status)
        {
            return status switch
            {
                "Approved" => "Review của bạn đã được duyệt và hiển thị công khai",
                "Rejected" => "Review của bạn vi phạm chính sách và không được duyệt",
                "UnderReview" => "Review của bạn đang được kiểm duyệt",
                _ => "Review đã được gửi"
            };
        }
    }
}
