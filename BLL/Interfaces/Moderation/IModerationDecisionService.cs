using BLL.DTOs.Moderation;

namespace BLL.Interfaces.Moderation
{
    public interface IModerationDecisionService
    {
        Task<ModerationDecisionResponse> MakeDecisionAsync(
            RuleBasedResponse ruleResult,
            AiModerationResponse aiResult);
    }
}
