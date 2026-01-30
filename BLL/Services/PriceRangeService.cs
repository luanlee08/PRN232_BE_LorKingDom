using BLL.DTOs;
using BLL.DTOs.PriceRanges;
using BLL.Interfaces;
using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PriceRangeService : IPriceRangeService
    {
        private readonly IPriceRangeRepository _repo;

        public PriceRangeService(IPriceRangeRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<PriceRangeResponse>>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();

            return new ApiResponse<List<PriceRangeResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách PriceRange",
                Data = items.Select(x => new PriceRangeResponse
                {
                    PriceRangeId = x.PriceRangeId,
                    PriceRangeMin = x.PriceRangeMin,
                    PriceRangeMax = x.PriceRangeMax,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
