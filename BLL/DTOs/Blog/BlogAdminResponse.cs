namespace BLL.DTOs.Blog
{
    public class BlogAdminResponse
    {
        public int BlogPostId { get; set; }
        public string BlogTitle { get; set; } = null!;
        public string BlogContent { get; set; } = null!;  
        public string? BlogThumbnail { get; set; }

        public int CategoryId { get; set; }
        public string BlogCategory { get; set; } = null!;
        public string AuthorEmail { get; set; } = null!;
        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
