using DAL.Models;

namespace DAL.Interface
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByAccountIdAsync(int accountId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemByProductIdAsync(int cartId, int productId);
        Task<Cart> CreateCartAsync(int accountId);
        Task AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(int cartItemId);
        Task DeleteAllCartItemsAsync(int cartId);
        Task UpdateCartAsync(Cart cart);
    }
}
