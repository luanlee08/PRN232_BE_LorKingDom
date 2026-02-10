using Microsoft.AspNetCore.Http;

namespace BLL.DTOs.ReviewProduct
{
    public class EditReviewRequest
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<IFormFile>? NewImages { get; set; }
        public List<string>? KeepImageUrls { get; set; }
    }
}
