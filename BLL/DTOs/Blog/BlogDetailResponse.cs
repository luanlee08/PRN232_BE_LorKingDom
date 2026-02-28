namespace BLL.DTOs.Blog
{
    public class BlogDetailResponse
    {
        public int BlogPostId { get; set; }
        public string BlogTitle { get; set; } = null!;
        public string BlogContent { get; set; } = null!;
        public string? BlogThumbnail { get; set; }
        public string BlogCategory { get; set; } = null!;
        public string AuthorEmail { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
