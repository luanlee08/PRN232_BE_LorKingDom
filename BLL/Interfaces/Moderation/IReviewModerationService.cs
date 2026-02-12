using BLL.DTOs.Moderation;

namespace BLL.Interfaces.Moderation
{
    public interface IReviewModerationService
    {
        Task<ModerationReport> ModerateAsync(ReviewModerationRequest request);
    }
}
