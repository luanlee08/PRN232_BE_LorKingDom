using BLL.DTOs.Cart;
using FluentValidation;

namespace BLL.Validators.Cart
{
    public class UpdateCartItemRequestValidator : AbstractValidator<UpdateCartItemRequest>
    {
        public UpdateCartItemRequestValidator()
        {
            RuleFor(x => x.CartItemId)
                .GreaterThan(0)
                .WithMessage("CartItemId phải lớn hơn 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Số lượng phải lớn hơn 0")
                .LessThanOrEqualTo(100)
                .WithMessage("Số lượng không được vượt quá 100");
        }
    }
}
