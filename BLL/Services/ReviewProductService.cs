using BLL.DTOs;
using BLL.DTOs.Moderation;
using BLL.DTOs.ReviewProduct;
using BLL.Interfaces;
using BLL.Interfaces.Moderation;
using BLL.Interfaces.Notification;
using DAL.Infrastructure;
using DAL.Interface;
using DAL.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class ReviewProductService : IReviewProductService
    {
        private readonly IReviewProductRepository _reviewRepo;
        private readonly IReviewProductImageRepository _imageRepo;
        private readonly IReviewProductReactionRepository _reactionRepo;
        private readonly IReviewProductReplyRepository _replyRepo;
        private readonly IReviewModerationLogRepository _moderationLogRepo;
        private readonly IReviewModerationService _moderationService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly INotificationCommandService _notificationCommandService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewProductService(
            IReviewProductRepository reviewRepo,
            IReviewProductImageRepository imageRepo,
            IReviewProductReactionRepository reactionRepo,
            IReviewProductReplyRepository replyRepo,
            IReviewModerationLogRepository moderationLogRepo,
            IReviewModerationService moderationService,
            ICloudinaryService cloudinaryService,
            INotificationCommandService notificationCommandService,
            IBackgroundJobClient backgroundJobClient,
            IUnitOfWork unitOfWork)
        {
            _reviewRepo = reviewRepo;
            _imageRepo = imageRepo;
            _reactionRepo = reactionRepo;
            _replyRepo = replyRepo;
            _moderationLogRepo = moderationLogRepo;
            _moderationService = moderationService;
            _cloudinaryService = cloudinaryService;
            _notificationCommandService = notificationCommandService;
            _backgroundJobClient = backgroundJobClient;
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

                // === STEP 2: Upload Images in Parallel ===
                var imageUrls = new List<string>();
                if (request.Images?.Any() == true)
                {
                    var uploadTasks = request.Images.Select(image => SaveImageAsync(image, 1));
                    imageUrls = (await Task.WhenAll(uploadTasks)).ToList();
                }

                // === STEP 3: Create Review with Pending status ===
                var review = new ReviewProduct
                {
                    AccountId = accountId,
                    ProductId = 1,
                    OrderDetailId = request.OrderDetailId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    Status = "Pending", // Start with Pending status
                    ModerationScore = null,
                    ModerationDetail = null,
                    Visibility = "AuthorOnly", // Only author can see initially (preview mode)
                    EditCount = 0,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _reviewRepo.AddAsync(review);

                // === STEP 4: Save Images ===
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

                await _unitOfWork.CommitTransactionAsync();

                // === STEP 5: Enqueue Background Moderation Job ===
                _backgroundJobClient.Enqueue(() => ProcessReviewModerationAsync(review.ReviewProductId));

                return new ApiResponse<ReviewResponse>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = GetStatusMessage("Pending"),
                    Data = await MapToResponseAsync(review, accountId, isNewReview: true)
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

        // === BACKGROUND JOB: Process Review Moderation ===
        public async Task ProcessReviewModerationAsync(int reviewId)
        {
            try
            {
                // Retrieve review from database
                var review = await _reviewRepo.GetByIdAsync(reviewId);
                if (review == null || review.IsDeleted)
                {
                    throw new Exception($"Review {reviewId} not found or deleted");
                }

                // Get image URLs
                var images = await _imageRepo.GetByReviewIdAsync(reviewId);
                var imageUrls = images.Select(i => i.ImageUrl).ToList();

                // Run 3-layer moderation
                var moderationRequest = new ReviewModerationRequest
                {
                    ReviewText = review.Comment ?? string.Empty,
                    ImageUrls = imageUrls,
                    AccountId = review.AccountId,
                    ProductId = review.ProductId
                };
                var moderationResult = await _moderationService.ModerateAsync(moderationRequest);

                // Update review with moderation result
                review.Status = moderationResult.Decision.Status;
                review.ModerationScore = moderationResult.Decision.FinalScore;
                review.ModerationDetail = moderationResult.Decision.Reason;
                review.Visibility = moderationResult.IsApproved ? "Public" : "AuthorOnly";
                review.UpdatedAt = DateTime.UtcNow;

                await _reviewRepo.UpdateAsync(review);

                // Save moderation logs
                await SaveModerationLogAsync(reviewId, moderationResult);

                // If rejected, send notification to user
                if (moderationResult.Decision.Status == "Rejected")
                {
                    var productName = review.Product?.ProductName ?? "sản phẩm";
                    var reason = moderationResult.Decision.Reason ?? "Vi phạm chính sách nội dung";

                    await _notificationCommandService.SendReviewRejectionNotificationAsync(
                        reviewId,
                        review.AccountId,
                        productName,
                        reason);
                }
            }
            catch (Exception ex)
            {
                // Log error and let Hangfire retry
                throw new Exception($"Failed to process review moderation for ReviewId {reviewId}: {ex.Message}", ex);
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

                // Upload new images in parallel
                var newImageUrls = new List<string>();
                if (request.NewImages?.Any() == true)
                {
                    var uploadTasks = request.NewImages.Select(image => SaveImageAsync(image, review.ProductId));
                    newImageUrls = (await Task.WhenAll(uploadTasks)).ToList();

                    var newImages = newImageUrls.Select(url => new ReviewProductImage
                    {
                        ReviewProductId = reviewId,
                        ImageUrl = url,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                    await _imageRepo.AddRangeAsync(newImages);
                }

                // === STEP 3: Update Review and Reset to Pending ===
                review.Rating = request.Rating;
                review.Comment = request.Comment;
                review.Status = "Pending"; // Reset to Pending for re-moderation
                review.ModerationScore = null;
                review.ModerationDetail = null;
                review.Visibility = "AuthorOnly"; // Author-only while being re-moderated
                review.UpdatedAt = DateTime.UtcNow;

                await _reviewRepo.UpdateAsync(review);
                await _reviewRepo.IncrementEditCountAsync(reviewId);

                await _unitOfWork.CommitTransactionAsync();

                // === STEP 4: Enqueue Background Re-Moderation Job ===
                _backgroundJobClient.Enqueue(() => ProcessReviewModerationAsync(reviewId));

                return new ApiResponse<ReviewResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = GetStatusMessage("Pending"),
                    Data = await MapToResponseAsync(review, accountId, isNewReview: true)
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
                query.Status,
                query.Rating,
                currentAccountId);

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
            if (review.Visibility == "AuthorOnly" && review.AccountId != currentAccountId)
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

        // === GET REVIEW SUMMARY ===
        public async Task<ApiResponse<ReviewSummaryResponse>> GetReviewSummaryAsync(int productId)
        {
            var (averageRating, totalReviews, ratingDistribution) = await _reviewRepo.GetReviewSummaryAsync(productId);

            return new ApiResponse<ReviewSummaryResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Thống kê đánh giá sản phẩm",
                Data = new ReviewSummaryResponse
                {
                    AverageRating = averageRating,
                    TotalReviews = totalReviews,
                    RatingDistribution = ratingDistribution
                }
            };
        }

        // === GET MY REVIEW HISTORY ===
        public async Task<ApiResponse<List<ReviewResponse>>> GetMyReviewHistoryAsync(int productId, int accountId)
        {
            var reviews = await _reviewRepo.GetMyReviewHistoryAsync(productId, accountId);

            var responses = new List<ReviewResponse>();
            foreach (var review in reviews)
            {
                responses.Add(await MapToResponseAsync(review, accountId));
            }

            return new ApiResponse<List<ReviewResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lịch sử đánh giá của bạn",
                Data = responses
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

        private async Task SaveModerationLogAsync(int reviewId, ModerationReport result)
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
            ReviewProduct review, int? currentAccountId, bool isNewReview = false)
        {
            // For new reviews, we can skip DB calls for reactions and edit permissions
            int likeCount = 0;
            int dislikeCount = 0;
            bool isLiked = false;
            bool canEdit = true;

            if (!isNewReview)
            {
                likeCount = await _reactionRepo.GetLikeCountAsync(review.ReviewProductId);
                dislikeCount = await _reactionRepo.GetDislikeCountAsync(review.ReviewProductId);

                if (currentAccountId.HasValue)
                {
                    var reaction = await _reactionRepo.GetReactionAsync(
                        review.ReviewProductId,
                        currentAccountId.Value);
                    isLiked = reaction?.ReactionType == "Like";
                }
                canEdit = await _reviewRepo.CanEditAsync(review.ReviewProductId, review.AccountId);
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
                ImageUrls = review.ReviewProductImages?
                    .Where(i => !i.IsDeleted)
                    .Select(i => i.ImageUrl)
                    .ToList() ?? new List<string>(),
                Status = review.Status,
                ModerationDetail = review.ModerationDetail,
                LikeCount = likeCount,
                DislikeCount = dislikeCount,
                IsLikedByCurrentUser = isLiked,
                EditCount = review.EditCount,
                CanEdit = canEdit,
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
                "Pending" => "Review của bạn đang được kiểm duyệt",
                _ => "Review đã được gửi"
            };
        }

        // === ADMIN METHODS ===
        public async Task<ApiResponse<PagedResult<ReviewResponse>>> GetAdminReviewsAsync(AdminReviewListQuery query)
        {
            var (items, total) = await _reviewRepo.GetAdminPagedAsync(
                query.ProductId,
                query.Status,
                query.Rating,
                query.SearchKeyword,
                query.FromDate,
                query.ToDate,
                query.Page,
                query.PageSize);

            var responses = new List<ReviewResponse>();
            foreach (var review in items)
            {
                responses.Add(await MapToResponseAsync(review, null));
            }

            return new ApiResponse<PagedResult<ReviewResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách review (Admin)",
                Data = new PagedResult<ReviewResponse>
                {
                    Items = responses,
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<ReviewResponse>> AdminUpdateReviewAsync(int reviewId, AdminUpdateReviewRequest request)
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

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                review.Status = request.Status;
            }

            if (!string.IsNullOrEmpty(request.Visibility))
            {
                review.Visibility = request.Visibility;
            }

            if (request.ModerationDetail != null)
            {
                review.ModerationDetail = request.ModerationDetail;
            }

            review.UpdatedAt = DateTime.UtcNow;
            await _reviewRepo.UpdateAsync(review);

            return new ApiResponse<ReviewResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật review thành công",
                Data = await MapToResponseAsync(review, null)
            };
        }

        public async Task<ApiResponse<object>> AdminSoftDeleteReviewAsync(int reviewId)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId);

            if (review == null || review.IsDeleted)
            {
                return new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy review"
                };
            }

            await _reviewRepo.SoftDeleteAsync(reviewId);

            return new ApiResponse<object>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Xóa review thành công"
            };
        }

        public async Task<ApiResponse<ReplyResponse>> AddReplyAsync(AddReplyRequest request, int adminAccountId)
        {
            var review = await _reviewRepo.GetByIdAsync(request.ReviewProductId);

            if (review == null || review.IsDeleted)
            {
                return new ApiResponse<ReplyResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy review"
                };
            }

            var reply = new ReviewProductReply
            {
                ReviewProductId = request.ReviewProductId,
                AccountId = adminAccountId,
                Content = request.ReplyText,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            var savedReply = await _replyRepo.AddAsync(reply);

            // Get reply with account info
            var replyWithAccount = await _replyRepo.GetByIdAsync(savedReply.ReplyProductId);

            return new ApiResponse<ReplyResponse>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Đã trả lời review",
                Data = new ReplyResponse
                {
                    ReviewProductReplyId = replyWithAccount!.ReplyProductId,
                    ReviewProductId = replyWithAccount.ReviewProductId,
                    AccountId = replyWithAccount.AccountId,
                    AccountName = replyWithAccount.Account?.AccountName ?? "Admin",
                    AccountImage = replyWithAccount.Account?.Image,
                    ReplyText = replyWithAccount.Content,
                    CreatedAt = replyWithAccount.CreatedAt
                }
            };
        }
    }
}
