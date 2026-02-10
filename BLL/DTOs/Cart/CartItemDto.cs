namespace BLL.DTOs.Cart
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductSku { get; set; }
        public decimal PriceAtThatTime { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
        public decimal SubTotal => PriceAtThatTime * Quantity;
        public int AvailableStock { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
