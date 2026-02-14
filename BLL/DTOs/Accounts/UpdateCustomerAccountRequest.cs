using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Accounts
{
    public class UpdateCustomerAccountRequest
    {
        [Required(ErrorMessage = "Status không được để trống")]
        public string Status { get; set; } = "Active";
    }
}
