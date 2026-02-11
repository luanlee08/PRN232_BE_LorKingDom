using BLL.DTOs.Moderation;

namespace BLL.Interfaces.Moderation
{
    public interface IRuleBasedFilterService
    {
        Task<RuleBasedResponse> CheckAsync(string text);
    }
}
