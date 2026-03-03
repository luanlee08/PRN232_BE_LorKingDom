using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IReviewBlogReplyRepository
    {
        Task AddAsync(ReviewBlogReply reply);
        Task<List<ReviewBlogReply>> GetByReviewIdAsync(int reviewBlogId);
        Task SaveChangesAsync();
    }
}
