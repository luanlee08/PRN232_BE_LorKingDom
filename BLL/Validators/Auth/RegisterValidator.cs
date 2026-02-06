using BLL.DTOs.Auth;
using FluentValidation;

namespace BLL.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("Tên tài khoản không được để trống")
                .MinimumLength(2).WithMessage("Tên tài khoản phải có ít nhất 2 ký tự")
                .MaximumLength(100).WithMessage("Tên tài khoản tối đa 100 ký tự");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không đúng định dạng")
                .MaximumLength(255).WithMessage("Email tối đa 255 ký tự");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                .MaximumLength(100).WithMessage("Mật khẩu tối đa 100 ký tự");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Số điện thoại không đúng định dạng");
        }
    }
}