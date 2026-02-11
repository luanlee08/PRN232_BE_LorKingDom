using Microsoft.AspNetCore.Http;

namespace BLL.DTOs.ReviewProduct
{
    public class AddReviewRequest
    {
        public int OrderDetailId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<IFormFile>? Images { get; set; }
    }
}
