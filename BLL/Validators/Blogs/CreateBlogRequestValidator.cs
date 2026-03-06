using System;
using System.Linq;
using BLL.DTOs.Blog;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BLL.Validators.Blogs
{
    public class CreateBlogRequestValidator : AbstractValidator<CreateBlogRequest>
    {
        private const long MaxThumbnailSizeBytes = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedImageTypes =
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif"
        };

        public CreateBlogRequestValidator()
        {
            RuleFor(x => x.BlogTitle)
                .NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(255).WithMessage("Tiêu đề không được vượt quá 255 ký tự");

            RuleFor(x => x.BlogContent)
                .NotEmpty().WithMessage("Nội dung không được để trống");

            RuleFor(x => x.BlogCategoryId)
                .GreaterThan(0).WithMessage("Thể loại không hợp lệ");

            RuleFor(x => x.BlogThumbnail)
                .NotNull().WithMessage("Ảnh đại diện là bắt buộc")
                .Must(file => file is not null && file.Length > 0)
                .WithMessage("Ảnh đại diện không hợp lệ")
                .Must(file => file is not null && file.Length <= MaxThumbnailSizeBytes)
                .WithMessage("Ảnh đại diện tối đa 5MB")
                .Must(file => file is not null && IsAllowedContentType(file))
                .WithMessage("Ảnh đại diện chỉ hỗ trợ JPG, PNG, WEBP, GIF");
        }

        private static bool IsAllowedContentType(IFormFile file)
        {
            return AllowedImageTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase);
        }
    }
}
