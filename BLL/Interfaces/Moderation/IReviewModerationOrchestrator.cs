using BLL.DTOs.Moderation;

namespace BLL.Interfaces.Moderation
{
    public interface IReviewModerationOrchestrator
    {
        Task<ModerationResponse> ModerateAsync(ModerationRequest request);
    }
}
