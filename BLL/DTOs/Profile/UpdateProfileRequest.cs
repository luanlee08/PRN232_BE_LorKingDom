using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Profile
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên tài khoản tối đa 100 ký tự")]
        public string AccountName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        public string? PhoneNumber { get; set; }
    }
}
