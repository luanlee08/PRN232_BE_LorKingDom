using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Categories
{
    public class CategoryQuery
    {
        public string? Keyword { get; set; }
        public int? SuperCategoryId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
