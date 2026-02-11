namespace BLL.DTOs.Accounts
{
    public class AccountQuery
    {
        public string? Keyword { get; set; }
        public int? RoleId { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
