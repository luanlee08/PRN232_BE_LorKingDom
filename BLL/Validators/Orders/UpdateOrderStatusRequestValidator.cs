using BLL.DTOs.Orders;
using FluentValidation;

namespace BLL.Validators.Orders;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.StatusId)
            .NotEmpty()
            .WithMessage("Trạng thái không được để trống");

        RuleFor(x => x.StatusId)
            .InclusiveBetween(1, 6)
            .WithMessage("Trạng thái không hợp lệ. Không thể chuyển sang trạng thái Refunded (7)");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Note))
            .WithMessage("Ghi chú không được vượt quá 500 ký tự");
    }
}
