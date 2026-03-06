using System;
using System.Collections.Generic;
using System.Linq;
using BLL.DTOs.Products;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BLL.Validators.Products
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10MB
        private const int MaxSecondaryImages = 6;
        private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif"
        };

        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "Available",
            "OutOfStock",
            "Discontinued"
        };

        public CreateProductRequestValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(255).WithMessage("Tên sản phẩm không được vượt quá 255 ký tự");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Danh mục không hợp lệ");

            RuleFor(x => x.MaterialId)
                .GreaterThan(0).WithMessage("Chất liệu không hợp lệ");

            RuleFor(x => x.AgeId)
                .GreaterThan(0).WithMessage("Độ tuổi không hợp lệ");

            RuleFor(x => x.SexId)
                .GreaterThan(0).WithMessage("Giới tính không hợp lệ");

            RuleFor(x => x.BrandId)
                .GreaterThan(0).WithMessage("Thương hiệu không hợp lệ");

            RuleFor(x => x.OriginId)
                .GreaterThan(0).WithMessage("Xuất xứ không hợp lệ");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Giá sản phẩm phải lớn hơn 0");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho không được âm");

            RuleFor(x => x.ProductStatus)
                .Must(status => string.IsNullOrWhiteSpace(status) || AllowedStatuses.Contains(status))
                .WithMessage("Trạng thái sản phẩm không hợp lệ");

            RuleFor(x => x.MainImage)
                .NotNull().WithMessage("Ảnh chính là bắt buộc")
                .Must(file => file is not null && IsValidImageFile(file))
                .WithMessage("Ảnh chính không hợp lệ (JPG/PNG/WEBP/GIF, tối đa 10MB)");

            RuleFor(x => x.SecondaryImages)
                .Must(images => images == null || images.Count <= MaxSecondaryImages)
                .WithMessage("Tối đa 6 ảnh phụ");

            RuleForEach(x => x.SecondaryImages!)
                .Must(file => file is not null && IsValidImageFile(file))
                .WithMessage("Ảnh phụ không hợp lệ (JPG/PNG/WEBP/GIF, tối đa 10MB)")
                .When(x => x.SecondaryImages is { Count: > 0 });
        }

        private static bool IsValidImageFile(IFormFile file)
        {
            return file.Length > 0
                   && file.Length <= MaxImageSizeBytes
                   && AllowedImageTypes.Contains(file.ContentType);
        }
    }
}
