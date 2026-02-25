using BLL.DTOs.Vouchers;
using FluentValidation;

namespace BLL.Validators.Vouchers
{
    public class UpdateVoucherValidator : AbstractValidator<UpdateVoucherRequest>
    {
        public UpdateVoucherValidator()
        {
            RuleFor(x => x.VoucherTypeId)
                .GreaterThan(0).WithMessage("Invalid voucher type")
                .When(x => x.VoucherTypeId.HasValue);

            RuleFor(x => x.VoucherCode)
                .MaximumLength(30).WithMessage("Voucher code must not exceed 30 characters")
                .Matches("^[A-Z0-9_-]+$").WithMessage("Voucher code must contain only uppercase letters, numbers, hyphens, and underscores")
                .When(x => !string.IsNullOrEmpty(x.VoucherCode));

            RuleFor(x => x.DiscountType)
                .Must(x => new[] { "Fixed", "Percentage" }.Contains(x))
                .When(x => !string.IsNullOrEmpty(x.DiscountType))
                .WithMessage("Discount type must be 'Fixed' or 'Percentage'");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than 0")
                .When(x => x.DiscountValue.HasValue);

            // Percentage: DiscountValue <= 100
            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(100)
                .When(x => x.DiscountValue.HasValue && x.DiscountType == "Percentage")
                .WithMessage("Discount value must be between 1 and 100 for Percentage type");

            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThan(0)
                .When(x => x.MaxDiscountAmount.HasValue)
                .WithMessage("Max discount amount must be greater than 0");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinOrderAmount.HasValue)
                .WithMessage("Min order amount must be greater than or equal to 0");

            // Fixed: DiscountValue < MinOrderAmount (only when both provided)
            RuleFor(x => x)
                .Must(x =>
                    x.DiscountType != "Fixed"
                    || !x.MinOrderAmount.HasValue
                    || !x.DiscountValue.HasValue
                    || x.DiscountValue < x.MinOrderAmount.Value
                )
                .WithMessage("Discount value must be less than minimum order amount");

            RuleFor(x => x.UsageLimitPerUser)
                .GreaterThan(0)
                .When(x => x.UsageLimitPerUser.HasValue)
                .WithMessage("Usage limit per user must be greater than 0");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate.GetValueOrDefault())
                .When(x => x.EndDate.HasValue && x.StartDate.HasValue)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Status)
                .Must(x => new[] { "Active", "Inactive", "Expired" }.Contains(x))
                .When(x => !string.IsNullOrEmpty(x.Status))
                .WithMessage("Status must be Active, Inactive, or Expired");
        }
    }
}
