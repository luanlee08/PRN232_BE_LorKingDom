using BLL.DTOs;
using BLL.DTOs.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IOriginService
    {
        Task<ApiResponse<List<OriginResponse>>> GetAllAsync();
    }
}
