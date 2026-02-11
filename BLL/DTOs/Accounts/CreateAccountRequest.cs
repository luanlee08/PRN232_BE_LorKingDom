using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Accounts
{
    public class CreateAccountRequest
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên tài khoản tối đa 100 ký tự")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(255, ErrorMessage = "Email tối đa 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "RoleId không được để trống")]
        public int RoleId { get; set; }

        public string Status { get; set; } = "Active";
    }
}
