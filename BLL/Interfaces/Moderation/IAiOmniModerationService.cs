using BLL.DTOs.Moderation;

namespace BLL.Interfaces.Moderation
{
    public interface IAiOmniModerationService
    {
        Task<AiModerationResponse> AnalyzeAsync(ModerationRequest request);
    }
}
