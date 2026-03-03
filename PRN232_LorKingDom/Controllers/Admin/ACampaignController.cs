using BLL.DTOs;
using BLL.DTOs.Campaigns;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/campaigns")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ACampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ILogger<ACampaignController> _logger;

        public ACampaignController(ICampaignService campaignService, ILogger<ACampaignController> logger)
        {
            _campaignService = campaignService;
            _logger = logger;
        }

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        /// <summary>Get paginated campaign list with filters</summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CampaignResponse>>>> GetCampaigns([FromQuery] CampaignQuery query)
        {
            var result = await _campaignService.GetCampaignsAsync(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>Get campaign detail with analytics (recipients + click timeline)</summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<CampaignDetailResponse>>> GetCampaignById(int id)
        {
            var result = await _campaignService.GetCampaignByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>Create a new campaign as Draft (or Scheduled if scheduledAt is set)</summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CampaignResponse>>> CreateCampaign([FromBody] CreateCampaignRequest request)
        {
            var accountId = GetCurrentAccountId();
            if (accountId == 0) return Unauthorized();

            var result = await _campaignService.CreateCampaignAsync(request, accountId);
            return StatusCode(result.Status, result);
        }

        /// <summary>Update a Draft campaign</summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<CampaignResponse>>> UpdateCampaign(int id, [FromBody] UpdateCampaignRequest request)
        {
            var result = await _campaignService.UpdateCampaignAsync(id, request);
            return StatusCode(result.Status, result);
        }

        /// <summary>Delete a Draft or Completed campaign</summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCampaign(int id)
        {
            var result = await _campaignService.DeleteCampaignAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>Trigger immediate send of a campaign</summary>
        [HttpPost("{id:int}/send")]
        public async Task<ActionResult<ApiResponse<bool>>> SendCampaign(int id)
        {
            var accountId = GetCurrentAccountId();
            if (accountId == 0) return Unauthorized();

            var result = await _campaignService.SendCampaignAsync(id, accountId);
            return StatusCode(result.Status, result);
        }

        /// <summary>Duplicate a campaign as a new Draft</summary>
        [HttpPost("{id:int}/duplicate")]
        public async Task<ActionResult<ApiResponse<CampaignResponse>>> DuplicateCampaign(int id)
        {
            var accountId = GetCurrentAccountId();
            if (accountId == 0) return Unauthorized();

            var result = await _campaignService.DuplicateCampaignAsync(id, accountId);
            return StatusCode(result.Status, result);
        }
    }
}
