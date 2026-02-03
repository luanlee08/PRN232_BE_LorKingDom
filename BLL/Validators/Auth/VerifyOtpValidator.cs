using BLL.DTOs.Auth;
using FluentValidation;

namespace BLL.Validators.Auth
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpRequest>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không đúng định dạng");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("Mã OTP không được để trống")
                .Length(6).WithMessage("Mã OTP phải có 6 ký tự");
        }
    }
}