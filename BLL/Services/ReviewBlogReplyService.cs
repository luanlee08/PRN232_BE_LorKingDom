using BLL.DTOs;
using BLL.DTOs.ReviewBlogReply;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ReviewBlogReplyService
    : IReviewBlogReplyService
    {
        private readonly IReviewBlogReplyRepository _repo;
        private readonly IReviewBlogRepository _reviewRepo;

        public ReviewBlogReplyService(
            IReviewBlogReplyRepository repo,
            IReviewBlogRepository reviewRepo)
        {
            _repo = repo;
            _reviewRepo = reviewRepo;
        }

        public async Task<ApiResponse<bool>> CreateAsync(
            int accountId,
            CreateReviewBlogReplyRequest request)
        {
            var review = await _reviewRepo
                .GetByIdAsync(request.ReviewBlogId);

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

            var reply = new ReviewBlogReply
            {
                ReviewBlogId = request.ReviewBlogId,
                AccountId = accountId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(reply);
            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Reply thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<List<ReviewBlogReplyResponse>>>
            GetByReviewIdAsync(int reviewBlogId)
        {
            var replies = await _repo
                .GetByReviewIdAsync(reviewBlogId);

            var result = replies.Select(r => new ReviewBlogReplyResponse
            {
                ReplyBlogId = r.ReplyBlogId,
                AccountId = r.AccountId,
                AccountName = string.IsNullOrEmpty(r.Account.AccountName)
                    ? r.Account.Email
                    : r.Account.AccountName,
                Content = r.Content,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new ApiResponse<List<ReviewBlogReplyResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Data = result
            };
        }
    }
}
