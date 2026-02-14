using BLL.DTOs.Moderation;

namespace BLL.Interfaces.Moderation
{
    public interface IModerationLayer2Service
    {
        Task<ModerationLayer2Result> AnalyzeAsync(ReviewModerationRequest request);
    }
}
