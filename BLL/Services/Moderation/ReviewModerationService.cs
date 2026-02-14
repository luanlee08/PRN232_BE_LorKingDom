using BLL.DTOs.Moderation;
using BLL.Interfaces.Moderation;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Moderation
{
    public class ReviewModerationService : IReviewModerationService
    {
        private readonly IModerationLayer1Service _layer1;
        private readonly IModerationLayer2Service _layer2;
        private readonly ILogger<ReviewModerationService> _logger;

        public ReviewModerationService(
            IModerationLayer1Service layer1,
            IModerationLayer2Service layer2,
            ILogger<ReviewModerationService> logger)
        {
            _layer1 = layer1;
            _layer2 = layer2;
            _logger = logger;
        }

        public async Task<ModerationReport> ModerateAsync(ReviewModerationRequest request)
        {
            var result = new ModerationReport();

            try
            {
                _logger.LogInformation(
                    $"Review Moderation Pipeline | AccountId: {request.AccountId}, ProductId: {request.ProductId}");

                // === TẦNG 1: Rule-Based Filter ===
                result.RuleBasedResult = await _layer1.CheckAsync(request.ReviewText);

                if (result.RuleBasedResult.IsViolated)
                {
                    _logger.LogWarning($"Layer 1 violation for AccountId: {request.AccountId}");

                    result.Decision = new ModerationFinalResult
                    {
                        Status = "Rejected",
                        FinalScore = 1.0m,
                        Reason = string.Join("; ", result.RuleBasedResult.ViolationReasons)
                    };
                    return result;
                }

                // === TẦNG 2: AI Omni-Moderation ===
                result.AiResult = await _layer2.AnalyzeAsync(request);

                // === TẦNG 3: Decision Making ===
                result.Decision = MakeDecision(result.RuleBasedResult, result.AiResult);

                _logger.LogInformation(
                    $"Moderation Finished | AccountId: {request.AccountId} | Status: {result.Decision.Status}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Moderation failed for AccountId: {request.AccountId}");

                result.Decision = new ModerationFinalResult
                {
                    Status = "Pending",
                    FinalScore = 0.5m,
                    Reason = "Lỗi hệ thống trong quá trình kiểm duyệt."
                };

                return result;
            }
        }

        private ModerationFinalResult MakeDecision(ModerationLayer1Result l1, ModerationLayer2Result l2)
        {
            if (l1.IsViolated)
            {
                return new ModerationFinalResult
                {
                    Status = "Rejected",
                    FinalScore = 1.0m,
                    Reason = string.Join("; ", l1.ViolationReasons)
                };
            }

            if (l2.IsFlagged)
            {
                return new ModerationFinalResult
                {
                    Status = "Rejected",
                    FinalScore = 1.0m,
                    Reason = BuildAiRejectionReason(l2)
                };
            }

            return new ModerationFinalResult
            {
                Status = "Approved",
                FinalScore = 0.0m,
                Reason = "Đạt tiêu chuẩn cộng đồng"
            };
        }

        private string BuildAiRejectionReason(ModerationLayer2Result aiResult)
        {
            var reasons = new List<string>();
            var cat = aiResult.Categories;

            if (cat.Harassment || cat.HarassmentThreatening) reasons.Add("Quấy rối");
            if (cat.Violence || cat.ViolenceGraphic) reasons.Add("Bạo lực");
            if (cat.Hate || cat.HateThreatening) reasons.Add("Ngôn từ thù ghét");
            if (cat.Sexual || cat.SexualMinors) reasons.Add("Nội dung nhạy cảm");
            if (cat.Illicit || cat.IllicitViolent) reasons.Add("Nội dung bất hợp pháp");
            if (cat.SelfHarm || cat.SelfHarmIntent || cat.SelfHarmInstructions) reasons.Add("Tự hại");

            return reasons.Any()
                ? $"Vi phạm: {string.Join(", ", reasons)}"
                : "Nội dung vi phạm chính sách cộng đồng (AI)";
        }
    }
}
