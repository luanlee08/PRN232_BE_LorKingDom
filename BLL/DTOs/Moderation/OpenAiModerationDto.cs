namespace BLL.DTOs.Moderation
{
    public class OpenAiModerationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public List<OmniModerationResult> Results { get; set; } = new();
    }

    public class OmniModerationResult
    {
        public bool Flagged { get; set; }
        public OmniCategories Categories { get; set; } = new();
        public OmniCategoryScores CategoryScores { get; set; } = new();
        public Dictionary<string, List<string>> CategoryAppliedInputTypes { get; set; } = new();
    }
}
