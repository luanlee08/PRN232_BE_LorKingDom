using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IReviewBlogRepository
    {
        Task AddAsync(ReviewBlog review);

        Task<ReviewBlog?> GetByIdAsync(int id);

        Task<List<ReviewBlog>> GetByBlogIdAsync(int blogPostId);

        IQueryable<ReviewBlog> GetQueryable();

        Task SaveChangesAsync();

    }
}
