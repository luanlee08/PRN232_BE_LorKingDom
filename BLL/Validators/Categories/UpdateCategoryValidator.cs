using BLL.DTOs.Categories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Validators.Categories
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.SuperCategoryId)
                .GreaterThan(0);
        }
    }
}
