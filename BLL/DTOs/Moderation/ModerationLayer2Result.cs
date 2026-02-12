namespace BLL.DTOs.Moderation
{
    public class ModerationLayer2Result
    {
        // Direct categories from OpenAI
        public bool IsFlagged { get; set; }
        public OmniCategories Categories { get; set; } = new();
        public OmniCategoryScores CategoryScores { get; set; } = new();
        public string Stage => "AiOmniModeration";
    }
}
