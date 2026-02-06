using BLL.DTOs;
using BLL.DTOs.Sexes;
using BLL.Interfaces;
using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class SexService : ISexService
    {
        private readonly ISexRepository _repo;

        public SexService(ISexRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<SexResponse>>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();

            return new ApiResponse<List<SexResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Sex",
                Data = items.Select(x => new SexResponse
                {
                    SexId = x.SexId,
                    SexName = x.SexName,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
