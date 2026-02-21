using BLL.DTOs.Address;
using FluentValidation;

namespace BLL.Validators.Address
{
    public class AddressRequestValidator : AbstractValidator<AddressRequestDTO>
    {
        public AddressRequestValidator()
        {
            RuleFor(x => x.RecipientName)
                .NotEmpty().WithMessage("Tên người nhận không được để trống")
                .MaximumLength(100).WithMessage("Tên người nhận tối đa 100 ký tự");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(0|\+84)[0-9]{9,10}$").WithMessage("Số điện thoại không hợp lệ (VD: 0901234567 hoặc +84901234567)");

            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Địa chỉ không được để trống")
                .MaximumLength(500).WithMessage("Địa chỉ tối đa 500 ký tự");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Thành phố không được để trống")
                .MaximumLength(100).WithMessage("Thành phố tối đa 100 ký tự");

            RuleFor(x => x.Ward)
                .MaximumLength(100).WithMessage("Phường/Xã tối đa 100 ký tự");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("Quận/Huyện không được để trống")
                .MaximumLength(100).WithMessage("Quận/Huyện tối đa 100 ký tự");
        }
    }
}
