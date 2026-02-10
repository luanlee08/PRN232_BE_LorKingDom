namespace BLL.DTOs.Moderation
{
    public class ModerationDecisionResponse
    {
        public string Status { get; set; } = "UnderReview"; // Rejected | UnderReview | Approved
        public decimal FinalScore { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Stage => "Decision";
    }
}
