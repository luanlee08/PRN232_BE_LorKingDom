using BLL.DTOs.Moderation;

namespace BLL.Interfaces.Moderation
{
    public interface IModerationLayer1Service
    {
        Task<ModerationLayer1Result> CheckAsync(string text);
    }
}
