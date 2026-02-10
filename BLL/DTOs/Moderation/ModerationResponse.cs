namespace BLL.DTOs.Moderation
{
    public class ModerationResponse
    {
        public RuleBasedResponse RuleBasedResult { get; set; } = new();
        public AiModerationResponse? AiResult { get; set; }
        public ModerationDecisionResponse Decision { get; set; } = new();

        public bool IsApproved => Decision.Status == "Approved";
        public bool IsRejected => Decision.Status == "Rejected";
        public bool IsUnderReview => Decision.Status == "UnderReview";
    }
}
