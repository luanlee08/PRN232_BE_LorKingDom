namespace BLL.DTOs.Templates
{
    public class TemplateResponse
    {
        public short TemplateId { get; set; }
        public string TemplateCode { get; set; } = null!;
        public string TitleTemplate { get; set; } = null!;
        public string MessageTemplate { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
