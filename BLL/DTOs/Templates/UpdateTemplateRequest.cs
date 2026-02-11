using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Templates
{
    public class UpdateTemplateRequest
    {
        [Required(ErrorMessage = "TemplateCode là bắt buộc")]
        [MaxLength(50, ErrorMessage = "TemplateCode không được vượt quá 50 ký tự")]
        public string TemplateCode { get; set; } = null!;

        [Required(ErrorMessage = "TitleTemplate là bắt buộc")]
        [MaxLength(200, ErrorMessage = "TitleTemplate không được vượt quá 200 ký tự")]
        public string TitleTemplate { get; set; } = null!;

        [Required(ErrorMessage = "MessageTemplate là bắt buộc")]
        public string MessageTemplate { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
