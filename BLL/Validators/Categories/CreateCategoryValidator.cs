using BLL.DTOs.Categories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators.Categories
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .MaximumLength(255).WithMessage("Tên danh mục không được vượt quá 255 ký tự");

            RuleFor(x => x.SuperCategoryId)
                .GreaterThan(0).WithMessage("Super category không hợp lệ");
        }
    }
}
