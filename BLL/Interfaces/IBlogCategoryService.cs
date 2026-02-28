using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOs;
using BLL.DTOs.Blog;

namespace BLL.Interfaces
{
    public interface IBlogCategoryService
    {
        Task<ApiResponse<List<BlogCategoryResponse>>> GetAllAsync();
    }
}

