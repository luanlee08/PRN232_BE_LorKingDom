using BLL.DTOs;
using BLL.DTOs.Address;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace PRN232_LorKingDom.Controllers.Customer
{
    [Route("api/addresses")]
    [ApiController]
    public class CAddressesController : ControllerBase
    {
        private readonly IAddressServices _services;
        int accountId = 6;
        public CAddressesController(IAddressServices services)
        {
            _services = services;
        }

        // GET: api/addresses
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _services.GetAllAsync(accountId);
            return StatusCode(result.Status, result);
        }

        // GET api/addresses/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _services.GetByIdAsync(id, 6);
            return StatusCode(result.Status, result);
        }

        // POST api/addresses
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddressRequestDTO AddressDTO)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _services.CreateAsync(AddressDTO, accountId);
            return StatusCode(result.Status, result);
        }

        // PUT api/addresses/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AddressUpdateRequestDTO updateDTO)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            // Ensure the ID in the URL matches the ID in the body
            if (id != updateDTO.AddressId)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    StatusMessage = "BAD_REQUEST",
                    Message = "ID trong URL không khớp với ID trong dữ liệu"
                });
            }

            var result = await _services.UpdateAsync(updateDTO, accountId);
            return StatusCode(result.Status, result);
        }

        // PATCH api/addresses/{id}/set-default
        [Authorize]
        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _services.SetDefaultAsync(id, accountId);
            return StatusCode(result.Status, result);
        }

        // DELETE api/addresses/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _services.DeleteAsync(id, accountId);
            return StatusCode(result.Status, result);
        }
    }
}
