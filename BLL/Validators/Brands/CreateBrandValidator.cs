using BLL.DTOs.Brands;
using FluentValidation;

namespace BLL.Validators.Brands
{
    public class CreateBrandValidator : AbstractValidator<CreateBrandRequest>
    {
        public CreateBrandValidator()
        {
            RuleFor(x => x.BrandName)
                .NotEmpty().WithMessage("Tên thương hiệu không được để trống")
                .MaximumLength(255).WithMessage("Tên thương hiệu không được vượt quá 255 ký tự");
        }
    }
}
