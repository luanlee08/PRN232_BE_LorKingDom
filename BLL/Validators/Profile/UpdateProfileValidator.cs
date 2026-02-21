using BLL.DTOs.Profile;
using FluentValidation;

namespace BLL.Validators.Profile
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("Tên tài khoản không được để trống")
                .MaximumLength(100).WithMessage("Tên tài khoản tối đa 100 ký tự");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15).WithMessage("Số điện thoại tối đa 15 ký tự")
                .Matches(@"^[0-9+\-\s()]*$").WithMessage("Số điện thoại không hợp lệ")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
