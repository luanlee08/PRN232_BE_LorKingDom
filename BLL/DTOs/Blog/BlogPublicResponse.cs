namespace BLL.DTOs.Blog
{
    public class BlogPublicResponse
    {
        public int BlogPostId { get; set; }
        public string BlogTitle { get; set; } = null!;
        public string BlogExcerpt { get; set; } = null!;
        public string? BlogThumbnail { get; set; }
        public string BlogCategory { get; set; } = null!;
        public string AuthorEmail { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }


}
