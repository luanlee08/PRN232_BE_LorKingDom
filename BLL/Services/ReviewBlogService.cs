using BLL.DTOs;
using BLL.DTOs.ReviewBlog;
using BLL.DTOs.ReviewBlogReply;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ReviewBlogService : IReviewBlogService
    {
        private readonly IReviewBlogRepository _reviewRepo;
        private readonly IBlogRepository _blogRepo;

        public ReviewBlogService(
            IReviewBlogRepository reviewRepo,
            IBlogRepository blogRepo)
        {
            _reviewRepo = reviewRepo;
            _blogRepo = blogRepo;
        }

        /* ================= CREATE ================= */

        public async Task<ApiResponse<bool>> CreateAsync(
            int accountId,
            CreateReviewBlogRequest request)
        {
            var blog = await _blogRepo.GetPublicByIdAsync(request.BlogPostId);

            if (blog == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Blog không tồn tại",
                    Data = false
                };
            }

            var review = new ReviewBlog
            {
                BlogPostId = request.BlogPostId,
                AccountId = accountId,
                Rating = request.Rating,
                Comment = request.Comment,
                IsBlocked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Đánh giá thành công",
                Data = true
            };
        }

        /* ================= GET BY BLOG ================= */

        public async Task<ApiResponse<List<ReviewBlogResponse>>>
    GetByBlogIdAsync(int blogPostId)
        {
            var reviews = await _reviewRepo
                .GetByBlogIdAsync(blogPostId);

            var result = reviews.Select(r => new ReviewBlogResponse
            {
                ReviewBlogId = r.ReviewBlogId,
                AccountId = r.AccountId,

                CustomerName = string.IsNullOrEmpty(r.Account?.AccountName)
                    ? r.Account?.Email
                    : r.Account.AccountName,

                Rating = r.Rating,

                LikeCount = r.ReviewBlogReactions?
                    .Count(x => x.ReactionType == "Like") ?? 0,

                DislikeCount = r.ReviewBlogReactions?
                    .Count(x => x.ReactionType == "Dislike") ?? 0,

                Comment = r.Comment,
                IsBlocked = r.IsBlocked,
                CreatedAt = r.CreatedAt,

                Replies = r.ReviewBlogReplies?
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => new ReviewBlogReplyResponse
                    {
                        ReplyBlogId = x.ReplyBlogId,
                        AccountId = x.AccountId,
                        AccountName = string.IsNullOrEmpty(x.Account?.AccountName)
                            ? x.Account?.Email
                            : x.Account.AccountName,
                        Content = x.Content,
                        CreatedAt = x.CreatedAt
                    }).ToList() ?? new List<ReviewBlogReplyResponse>()
            }).ToList();

            return new ApiResponse<List<ReviewBlogResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách đánh giá",
                Data = result
            };
        }
        /* ================= ADMIN ================= */

        public async Task<ApiResponse<PagedResult<ReviewBlogAdminDto>>>
    GetAllAsync(int page, int pageSize)
        {
            var query = _reviewRepo.GetQueryable()
                .OrderByDescending(x => x.CreatedAt);

            var total = await query.CountAsync();

            var data = await query
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(x => new ReviewBlogAdminDto
    {
        ReviewBlogId = x.ReviewBlogId,

        AccountId = x.AccountId, // 🔥 thêm

        AccountName = string.IsNullOrEmpty(x.Account.AccountName)
            ? x.Account.Email
            : x.Account.AccountName,  // 🔥 lấy tên hoặc email fallback

        AccountEmail = x.Account.Email,

        BlogTitle = x.BlogPost.BlogTitle,

        Rating = x.Rating,
        Comment = x.Comment,

        IsBlocked = x.IsBlocked,
        CreatedAt = x.CreatedAt
    })
    .ToListAsync();

            return new ApiResponse<PagedResult<ReviewBlogAdminDto>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách review thành công",
                Data = new PagedResult<ReviewBlogAdminDto>
                {
                    Items = data,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize
                }
            };
        }

        public async Task<ApiResponse<bool>> BlockAsync(
            int reviewId,
            bool isBlocked)
        {
            var review = await _reviewRepo.GetByIdAsync(reviewId);

            if (review == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Review không tồn tại",
                    Data = false
                };
            }

            review.IsBlocked = isBlocked;

            await _reviewRepo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = isBlocked
                    ? "Đã block review"
                    : "Đã mở block review",
                Data = true
            };
        }

    }
}
