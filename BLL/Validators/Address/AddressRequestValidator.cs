using BLL.DTOs.Address;
using FluentValidation;

namespace BLL.Validators.Address
{
    public class AddressRequestValidator : AbstractValidator<AddressRequestDTO>
    {
        public AddressRequestValidator()
        {
            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Địa chỉ không được để trống")
                .MaximumLength(500).WithMessage("Địa chỉ tối đa 500 ký tự");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Thành phố không được để trống")
                .MaximumLength(100).WithMessage("Thành phố tối đa 100 ký tự");

            RuleFor(x => x.Ward)
                .MaximumLength(100).WithMessage("Phường/Xã tối đa 100 ký tự");
        }
    }
}
