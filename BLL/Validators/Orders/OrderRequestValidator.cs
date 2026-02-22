using BLL.DTOs.Orders;
using FluentValidation;

namespace BLL.Validators.Orders
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Vui lòng chọn phương thức thanh toán")
                .Must(BeValidPaymentMethod)
                .WithMessage("Phương thức thanh toán không hợp lệ. Chỉ chấp nhận: COD, Wallet, VNPay, MoMo, Sepay");

            RuleFor(x => x.ShippingMethod)
                .NotEmpty()
                .WithMessage("Vui lòng chọn phương thức vận chuyển")
                .Must(BeValidShippingMethod)
                .WithMessage("Phương thức vận chuyển không hợp lệ. Chỉ chấp nhận: Express, Standard, Economy");

            // Address validation - either AddressId or all shipping details required
            When(x => !x.AddressId.HasValue, () =>
            {
                RuleFor(x => x.ShippingName)
                    .NotEmpty()
                    .WithMessage("Vui lòng nhập tên người nhận")
                    .MaximumLength(100)
                    .WithMessage("Tên người nhận không được vượt quá 100 ký tự");

                RuleFor(x => x.ShippingPhone)
                    .NotEmpty()
                    .WithMessage("Vui lòng nhập số điện thoại người nhận")
                    .Matches(@"^(0|\+84)[0-9]{9,10}$")
                    .WithMessage("Số điện thoại không hợp lệ");

                RuleFor(x => x.ShippingAddressLine)
                    .NotEmpty()
                    .WithMessage("Vui lòng nhập địa chỉ giao hàng")
                    .MaximumLength(500)
                    .WithMessage("Địa chỉ không được vượt quá 500 ký tự");

                RuleFor(x => x.ShippingCity)
                    .NotEmpty()
                    .WithMessage("Vui lòng nhập thành phố/tỉnh")
                    .MaximumLength(100)
                    .WithMessage("Tên thành phố/tỉnh không được vượt quá 100 ký tự");

                RuleFor(x => x.ShippingWard)
                    .NotEmpty()
                    .WithMessage("Vui lòng nhập phường/xã")
                    .MaximumLength(100)
                    .WithMessage("Tên phường/xã không được vượt quá 100 ký tự");
            });

            // Hybrid payment validation
            When(x => x.PaidByWalletAmount > 0 || x.PaidByExternalAmount > 0, () =>
            {
                RuleFor(x => x.PaidByWalletAmount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Số tiền thanh toán bằng ví không được âm");

                RuleFor(x => x.PaidByExternalAmount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Số tiền thanh toán bằng cổng thanh toán không được âm");
            });

            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Ghi chú không được vượt quá 500 ký tự");
        }

        private bool BeValidPaymentMethod(string paymentMethod)
        {
            var validMethods = new[]
            {
                PaymentMethods.COD,
                PaymentMethods.Wallet,
                PaymentMethods.VNPay,
                PaymentMethods.MoMo,
                PaymentMethods.Sepay
            };

            return validMethods.Contains(paymentMethod);
        }

        private bool BeValidShippingMethod(string shippingMethod)
        {
            var validMethods = new[]
            {
                ShippingMethods.Express,
                ShippingMethods.Standard,
                ShippingMethods.Economy
            };

            return validMethods.Contains(shippingMethod);
        }
    }
}
