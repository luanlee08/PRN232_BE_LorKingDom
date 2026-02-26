using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Interface
{
    public interface IBlogRepository
    {
        Task AddAsync(BlogPost blog);

        Task<(List<BlogPost> Data, int Total)> SearchForAdminAsync(string? keyword, int page, int pageSize);
        Task<BlogPost?> GetByIdAsync(int blogId);


        Task<(List<BlogPost>, int)> GetPublicAsync(
            string? keyword,
            int? categoryId,
            int page,
            int pageSize
        );
        Task<BlogPost?> GetPublicDetailAsync(int blogId);
        Task<BlogPost?> GetPublicByIdAsync(int blogId);

        Task<List<BlogPost>> GetRecentAsync(int limit);

        Task SaveChangesAsync();
    }
}
