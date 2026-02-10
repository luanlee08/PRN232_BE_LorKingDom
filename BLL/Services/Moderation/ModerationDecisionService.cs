using BLL.DTOs.Moderation;
using BLL.Interfaces.Moderation;

namespace BLL.Services.Moderation
{
    public class ModerationDecisionService : IModerationDecisionService
    {
        public Task<ModerationDecisionResponse> MakeDecisionAsync(
            RuleBasedResponse ruleResult, AiModerationResponse aiResult)
        {
            var decision = new ModerationDecisionResponse();

            // === CHECK 1: Rule-Based Filter ===
            if (ruleResult.IsViolated)
            {
                decision.Status = "Rejected";
                decision.FinalScore = 1.0m;
                decision.Reason = string.Join("; ", ruleResult.ViolationReasons);
                return Task.FromResult(decision);
            }

            // === CHECK 2: OpenAI Flagged ===
            if (aiResult.IsFlagged)
            {
                decision.Status = "Rejected";
                decision.FinalScore = 1.0m;
                decision.Reason = BuildRejectionReason(aiResult);
                return Task.FromResult(decision);
            }

            // === APPROVED ===
            decision.Status = "Approved";
            decision.FinalScore = 0.0m;
            decision.Reason = "Review đạt tiêu chuẩn và được phê duyệt";
            return Task.FromResult(decision);
        }

        // === BUILD REJECTION REASON FROM CATEGORIES ===
        private string BuildRejectionReason(AiModerationResponse aiResult)
        {
            var reasons = new List<string>();
            var cat = aiResult.Categories;

            // Check categories trực tiếp từ OpenAI
            if (cat.Harassment || cat.HarassmentThreatening)
                reasons.Add("Quấy rối");

            if (cat.Violence || cat.ViolenceGraphic)
                reasons.Add("Bạo lực");

            if (cat.Hate || cat.HateThreatening)
                reasons.Add("Ngôn từ thù ghét");

            if (cat.Sexual || cat.SexualMinors)
                reasons.Add("Nội dung nhạy cảm");

            if (cat.Illicit || cat.IllicitViolent)
                reasons.Add("Nội dung bất hợp pháp");

            if (cat.SelfHarm || cat.SelfHarmIntent || cat.SelfHarmInstructions)
                reasons.Add("Tự hại");

            return reasons.Any()
                ? $"Vi phạm: {string.Join(", ", reasons)}"
                : "Nội dung vi phạm chính sách cộng đồng";
        }
    }
}
