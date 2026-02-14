namespace BLL.DTOs.ReviewProduct
{
    public class ReviewSummaryResponse
    {
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
}
