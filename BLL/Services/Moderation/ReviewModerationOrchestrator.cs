using BLL.DTOs.Moderation;
using BLL.Interfaces.Moderation;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Moderation
{
    public class ReviewModerationOrchestrator : IReviewModerationOrchestrator
    {
        private readonly IRuleBasedFilterService _ruleFilter;
        private readonly IAiOmniModerationService _aiModeration;
        private readonly IModerationDecisionService _decision;
        private readonly ILogger<ReviewModerationOrchestrator> _logger;

        public ReviewModerationOrchestrator(
            IRuleBasedFilterService ruleFilter,
            IAiOmniModerationService aiModeration,
            IModerationDecisionService decision,
            ILogger<ReviewModerationOrchestrator> logger)
        {
            _ruleFilter = ruleFilter;
            _aiModeration = aiModeration;
            _decision = decision;
            _logger = logger;
        }

        public async Task<ModerationResponse> ModerateAsync(ModerationRequest request)
        {
            var result = new ModerationResponse();

            try
            {
                _logger.LogInformation(
                    $"Starting moderation for AccountId: {request.AccountId}, ProductId: {request.ProductId}");

                // === TẦNG 1: Rule-Based Filter ===
                result.RuleBasedResult = await _ruleFilter.CheckAsync(request.ReviewText);

                // Nếu vi phạm rule → Dừng ngay, không gọi AI
                if (result.RuleBasedResult.IsViolated)
                {
                    _logger.LogWarning(
                        $"Rule-based violation detected for AccountId: {request.AccountId}. " +
                        $"Reasons: {string.Join(", ", result.RuleBasedResult.ViolationReasons)}");

                    result.Decision = new ModerationDecisionResponse
                    {
                        Status = "Rejected",
                        FinalScore = 1.0m,
                        Reason = string.Join("; ", result.RuleBasedResult.ViolationReasons)
                    };

                    return result;
                }

                // === TẦNG 2: AI Omni-Moderation ===
                result.AiResult = await _aiModeration.AnalyzeAsync(request);

                _logger.LogInformation(
                    "AI Moderation completed. Flagged: {Flagged}",
                    result.AiResult.IsFlagged);

                // === TẦNG 3: Decision Making ===
                result.Decision = await _decision.MakeDecisionAsync(
                    result.RuleBasedResult,
                    result.AiResult);

                _logger.LogInformation(
                    "Final Decision: {Status} for AccountId: {AccountId}",
                    result.Decision.Status,
                    request.AccountId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Moderation pipeline failed for AccountId: {request.AccountId}");

                // Fallback: Mark as UnderReview
                result.Decision = new ModerationDecisionResponse
                {
                    Status = "UnderReview",
                    FinalScore = 0.5m,
                    Reason = "Lỗi hệ thống. Review cần kiểm tra thủ công."
                };

                return result;
            }
        }
    }
}
