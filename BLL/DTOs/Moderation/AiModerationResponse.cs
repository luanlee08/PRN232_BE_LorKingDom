namespace BLL.DTOs.Moderation
{
    public class AiModerationResponse
    {
        // Direct categories from OpenAI
        public bool IsFlagged { get; set; }
        public OmniCategories Categories { get; set; } = new();
        public OmniCategoryScores CategoryScores { get; set; } = new();
        public string Stage => "AiOmniModeration";
    }
}
