using BLL.DTOs;
using BLL.DTOs.PriceRanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPriceRangeService
    {
        Task<ApiResponse<List<PriceRangeResponse>>> GetAllAsync();
    }
}
