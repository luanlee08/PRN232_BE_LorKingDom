namespace BLL.DTOs.Moderation
{
    public class ModerationFinalResult
    {
        public string Status { get; set; } = "Pending"; // Rejected | Pending | Approved
        public decimal FinalScore { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Stage => "Decision";
    }
}
