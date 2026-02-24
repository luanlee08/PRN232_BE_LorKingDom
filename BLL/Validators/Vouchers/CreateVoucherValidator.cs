using BLL.DTOs.Vouchers;
using FluentValidation;

namespace BLL.Validators.Vouchers
{
    public class CreateVoucherValidator : AbstractValidator<CreateVoucherRequest>
    {
        public CreateVoucherValidator()
        {
            RuleFor(x => x.VoucherTypeId)
                .NotEmpty().WithMessage("Voucher type is required")
                .GreaterThan(0).WithMessage("Invalid voucher type");

            RuleFor(x => x.VoucherCode)
                .NotEmpty().WithMessage("Voucher code is required")
                .MaximumLength(30).WithMessage("Voucher code must not exceed 30 characters")
                .Matches("^[A-Z0-9_-]+$").WithMessage("Voucher code must contain only uppercase letters, numbers, hyphens, and underscores");

            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("Discount type is required")
                .Must(x => new[] { "Fixed", "Percentage" }.Contains(x))
                .WithMessage("Discount type must be 'Fixed' or 'Percentage'");

            // Fixed: DiscountValue > 0
            RuleFor(x => x.DiscountValue)
                .NotEmpty().WithMessage("Discount value is required")
                .GreaterThan(0).WithMessage("Discount value must be greater than 0");

            // Percentage: DiscountValue must be 0 < x <= 100
            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(100)
                .When(x => x.DiscountType == "Percentage")
                .WithMessage("Discount value must be between 1 and 100 for Percentage type");

            // Fixed: DiscountValue < MinOrderAmount
            RuleFor(x => x.DiscountValue)
                .Must((x, discount) =>
                    x.DiscountType != "Fixed"
                    || !x.MinOrderAmount.HasValue
                    || discount < x.MinOrderAmount.Value
                )
                .WithMessage("Discount value must be less than minimum order amount");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinOrderAmount.HasValue)
                .WithMessage("Min order amount must be greater than or equal to 0");

            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThan(0)
                .When(x => x.MaxDiscountAmount.HasValue)
                .WithMessage("Max discount amount must be greater than 0");

            RuleFor(x => x.UsageLimitPerUser)
                .GreaterThan(0)
                .When(x => x.UsageLimitPerUser.HasValue)
                .WithMessage("Usage limit per user must be greater than 0");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(x => new[] { "Active", "Inactive", "Expired" }.Contains(x))
                .WithMessage("Status must be Active, Inactive, or Expired");
        }
    }
}
