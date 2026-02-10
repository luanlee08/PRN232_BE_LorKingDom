using BLL.DTOs.ReviewProduct;
using FluentValidation;

namespace BLL.Validators.ReviewProduct
{
    public class AddReviewValidator : AbstractValidator<AddReviewRequest>
    {
        public AddReviewValidator()
        {
            RuleFor(x => x.OrderDetailId)
                .GreaterThan(0).WithMessage("OrderDetailId không hợp lệ");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating phải từ 1 đến 5 sao");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Nội dung review không được để trống")
                .MinimumLength(10).WithMessage("Review tối thiểu 10 ký tự")
                .MaximumLength(500).WithMessage("Review tối đa 500 ký tự");

            RuleFor(x => x.Images)
                .Must(images => images == null || images.Count <= 3)
                .WithMessage("Tối đa 3 ảnh");
        }
    }
}
