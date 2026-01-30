using BLL.DTOs;
using BLL.DTOs.Origins;
using BLL.Interfaces;
using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OriginService : IOriginService
    {
        private readonly IOriginRepository _repo;

        public OriginService(IOriginRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<OriginResponse>>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();

            return new ApiResponse<List<OriginResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Origin",
                Data = items.Select(x => new OriginResponse
                {
                    OriginId = x.OriginId,
                    OriginName = x.OriginName,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
