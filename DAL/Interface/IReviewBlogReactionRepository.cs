using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IReviewBlogReactionRepository
    {
        Task<ReviewBlogReaction?> GetAsync(int reviewBlogId, int accountId);

        Task AddAsync(ReviewBlogReaction reaction);

        Task SaveChangesAsync();
        Task RemoveAsync(ReviewBlogReaction reaction);
        Task<List<ReviewBlogReaction>>
    GetByReviewBlogIdAsync(int reviewBlogId);
        Task<int> CountByTypeAsync(int reviewBlogId, string type);

        Task<string?> GetUserReactionAsync(int reviewBlogId, int accountId);

    }
}
