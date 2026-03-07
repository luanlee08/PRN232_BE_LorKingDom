using BLL.DTOs.SuperCategories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators.SuperCategory
{
    public class UpdateSuperCategoryValidator
           : AbstractValidator<UpdateSuperCategoryRequest>
    {
        public UpdateSuperCategoryValidator()
        {
            RuleFor(x => x.SuperCategoryName)
                .NotEmpty()
                    .WithMessage("Tên super category không được để trống")
                .MaximumLength(255)
                    .WithMessage("Tên super category không được vượt quá 255 ký tự");
        }
    }
}
