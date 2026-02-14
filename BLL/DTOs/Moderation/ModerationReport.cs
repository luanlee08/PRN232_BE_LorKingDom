namespace BLL.DTOs.Moderation
{
    public class ModerationReport
    {
        public ModerationLayer1Result RuleBasedResult { get; set; } = new();
        public ModerationLayer2Result? AiResult { get; set; }
        public ModerationFinalResult Decision { get; set; } = new();

        public bool IsApproved => Decision.Status == "Approved";
        public bool IsRejected => Decision.Status == "Rejected";
        public bool IsPending => Decision.Status == "Pending";
    }
}
