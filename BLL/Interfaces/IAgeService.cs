using BLL.DTOs;
using BLL.DTOs.Ages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAgeService
    {
        Task<ApiResponse<List<AgeResponse>>> GetAllAsync();
    }
}
