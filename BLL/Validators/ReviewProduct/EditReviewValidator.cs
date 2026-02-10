using BLL.DTOs.ReviewProduct;
using FluentValidation;

namespace BLL.Validators.ReviewProduct
{
    public class EditReviewValidator : AbstractValidator<EditReviewRequest>
    {
        public EditReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating phải từ 1 đến 5 sao");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Nội dung review không được để trống")
                .MinimumLength(10).WithMessage("Review tối thiểu 10 ký tự")
                .MaximumLength(500).WithMessage("Review tối đa 500 ký tự");

            RuleFor(x => x.NewImages)
                .Must(images => images == null || images.Count <= 5)
                .WithMessage("Tối đa 3 ảnh mới");
        }
    }
}
