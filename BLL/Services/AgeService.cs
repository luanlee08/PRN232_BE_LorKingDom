using BLL.DTOs;
using BLL.DTOs.Ages;
using BLL.Interfaces;
using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AgeService : IAgeService
    {
        private readonly IAgeRepository _repo;

        public AgeService(IAgeRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<AgeResponse>>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();

            return new ApiResponse<List<AgeResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Age",
                Data = items.Select(x => new AgeResponse
                {
                    AgeId = x.AgeId,
                    AgeRange = x.AgeRange,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
