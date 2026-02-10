using BLL.DTOs;
using BLL.DTOs.Cart;

namespace BLL.Interfaces
{
    public interface ICartService
    {
        Task<ApiResponse<CartDto>> GetCartAsync(int accountId);
        Task<ApiResponse<CartDto>> AddToCartAsync(AddToCartRequest request, int accountId);
        Task<ApiResponse<CartDto>> UpdateCartItemAsync(UpdateCartItemRequest request, int accountId);
        Task<ApiResponse<CartDto>> IncrementCartItemAsync(int cartItemId, int accountId);
        Task<ApiResponse<CartDto>> DecrementCartItemAsync(int cartItemId, int accountId);
        Task<ApiResponse<object>> RemoveCartItemAsync(int cartItemId, int accountId);
        Task<ApiResponse<object>> ClearCartAsync(int accountId);
    }
}
