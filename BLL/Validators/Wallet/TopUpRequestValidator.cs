using BLL.DTOs.Wallet;
using FluentValidation;

namespace BLL.Validators.Wallet
{
    public class TopUpRequestValidator : AbstractValidator<TopUpRequestDTO>
    {
        private static readonly string[] ValidGateways = ["VNPay", "MoMo", "Sepay"];

        public TopUpRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(10_000)
                .WithMessage("Số tiền nạp tối thiểu là 10,000 VND")
                .LessThanOrEqualTo(10_000_000)
                .WithMessage("Số tiền nạp tối đa là 10,000,000 VND");

            RuleFor(x => x.Gateway)
                .NotEmpty()
                .WithMessage("Vui lòng chọn cổng thanh toán")
                .Must(g => ValidGateways.Contains(g))
                .WithMessage("Cổng thanh toán không hợp lệ. Chỉ chấp nhận: VNPay, MoMo, Sepay");

            RuleFor(x => x.ReturnUrl)
                .NotEmpty()
                .WithMessage("ReturnUrl không được để trống")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("ReturnUrl không phải là URL hợp lệ");
        }
    }
}
