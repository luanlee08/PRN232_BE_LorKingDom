namespace BLL.DTOs.Moderation
{
    public class ModerationLayer1Result
    {
        public bool IsViolated { get; set; }
        public List<string> ViolationReasons { get; set; } = new();
        public string Stage => "RuleBased";
    }
}
