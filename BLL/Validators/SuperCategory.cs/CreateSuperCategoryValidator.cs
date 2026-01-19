using BLL.DTOs.SuperCategories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators.SuperCategory
{
    public class CreateSuperCategoryValidator
        : AbstractValidator<CreateSuperCategoryRequest>
    {
        public CreateSuperCategoryValidator()
        {
            RuleFor(x => x.SuperCategoryName)
                .NotEmpty()
                    .WithMessage("Tên SuperCategory không được để trống")
                .MaximumLength(255)
                    .WithMessage("Tên SuperCategory tối đa 255 ký tự1");
        }
    }
}
