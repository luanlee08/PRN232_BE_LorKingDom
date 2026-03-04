public class ReviewBlogAdminDto
{
    public int ReviewBlogId { get; set; }

    public int AccountId { get; set; }           
    public string AccountName { get; set; } = "";
    public string AccountEmail { get; set; } = "";

    public string BlogTitle { get; set; } = "";

    public int Rating { get; set; }

    public string? Comment { get; set; }    

    public bool IsBlocked { get; set; }

    public DateTime CreatedAt { get; set; }
}