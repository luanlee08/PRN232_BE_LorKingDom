using BLL.DTOs;
using BLL.DTOs.Campaigns;

namespace BLL.Interfaces
{
    public interface ICampaignService
    {
        Task<ApiResponse<PagedResult<CampaignResponse>>> GetCampaignsAsync(CampaignQuery query);

        Task<ApiResponse<CampaignDetailResponse>> GetCampaignByIdAsync(int id);

        Task<ApiResponse<CampaignResponse>> CreateCampaignAsync(CreateCampaignRequest request, int createdByAccountId);

        Task<ApiResponse<CampaignResponse>> UpdateCampaignAsync(int id, UpdateCampaignRequest request);

        Task<ApiResponse<bool>> DeleteCampaignAsync(int id);

        /// <summary>Trigger immediate send or schedule via Hangfire. Sets status to Processing / Scheduled.</summary>
        Task<ApiResponse<bool>> SendCampaignAsync(int id, int triggeredByAccountId);

        /// <summary>Clone a campaign as a new Draft.</summary>
        Task<ApiResponse<CampaignResponse>> DuplicateCampaignAsync(int id, int createdByAccountId);

        /// <summary>Log a click or read action for analytics.</summary>
        Task<ApiResponse<bool>> RecordActionAsync(RecordActionRequest request);
    }
}
