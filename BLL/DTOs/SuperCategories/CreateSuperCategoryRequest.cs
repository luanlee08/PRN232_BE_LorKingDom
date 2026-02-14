using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.SuperCategories
{
    public class CreateSuperCategoryRequest
    {
        public string SuperCategoryName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
