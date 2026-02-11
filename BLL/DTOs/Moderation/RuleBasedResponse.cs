namespace BLL.DTOs.Moderation
{
    public class RuleBasedResponse
    {
        public bool IsViolated { get; set; }
        public List<string> ViolationReasons { get; set; } = new();
        public string Stage => "RuleBased";
    }
}
