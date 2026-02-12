using BLL.DTOs.Orders;
using FluentValidation;

namespace BLL.Validators.Orders;

public class OrderQueryValidator : AbstractValidator<OrderQuery>
{
    public OrderQueryValidator()
    {
        //RuleFor(x => x.Page)
        //    .GreaterThanOrEqualTo(1)
        //    .WithMessage("Số trang phải lớn hơn hoặc bằng 1");

        //RuleFor(x => x.PageSize)
        //    .GreaterThanOrEqualTo(10)
        //    .WithMessage("Kích thước trang phải lớn hơn hoặc bằng 10")
        //    .LessThanOrEqualTo(100)
        //    .WithMessage("Kích thước trang không được vượt quá 100");

        RuleFor(x => x.StatusId)
            .InclusiveBetween(1, 7)
            .When(x => x.StatusId.HasValue)
            .WithMessage("Trạng thái không hợp lệ");

        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrEmpty(x) ||
                      new[] { "OrderDate", "Status", "TotalAmount" }.Contains(x))
            .WithMessage("SortBy chỉ chấp nhận: OrderDate, Status, TotalAmount");
    }
}
