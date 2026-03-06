using BLL.DTOs.Materials;
using FluentValidation;

namespace BLL.Validators.Materials
{
    public class CreateMaterialValidator : AbstractValidator<CreateMaterialRequest>
    {
        public CreateMaterialValidator()
        {
            RuleFor(x => x.MaterialName)
                .NotEmpty().WithMessage("Tên chất liệu không được để trống")
                .MaximumLength(255).WithMessage("Tên chất liệu không được vượt quá 255 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
