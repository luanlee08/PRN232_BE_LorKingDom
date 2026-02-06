using BLL.DTOs;
using BLL.DTOs.Sexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ISexService
    {
        Task<ApiResponse<List<SexResponse>>> GetAllAsync();
    }
}
