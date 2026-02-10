using BLL.DTOs;
using BLL.DTOs.Cart;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;

namespace BLL.Services
{
    public class CartServices : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;

        public CartServices(ICartRepository cartRepo, IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task<ApiResponse<CartDto>> GetCartAsync(int accountId)
        {
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);

            if (cart == null)
            {
                // Create new cart if doesn't exist
                cart = await _cartRepo.CreateCartAsync(accountId);
            }

            var cartDto = MapToDto(cart);

            return new ApiResponse<CartDto>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy giỏ hàng thành công",
                Data = cartDto
            };
        }

        public async Task<ApiResponse<CartDto>> AddToCartAsync(AddToCartRequest request, int accountId)
        {
            // Validate product existence and availability
            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy sản phẩm"
                };
            }

            if (product.IsDeleted || product.ProductStatus != "Available")
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Sản phẩm không khả dụng"
                };
            }

            if (request.Quantity <= 0)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Số lượng phải lớn hơn 0"
                };
            }

            if (request.Quantity > product.Quantity)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Số lượng vượt quá tồn kho. Còn lại: {product.Quantity}"
                };
            }

            // Get or create cart
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            if (cart == null)
            {
                cart = await _cartRepo.CreateCartAsync(accountId);
            }

            // Check if product already in cart
            var existingCartItem = await _cartRepo.GetCartItemByProductIdAsync(cart.CartId, request.ProductId);

            if (existingCartItem != null)
            {
                // Update quantity
                var newQuantity = existingCartItem.Quantity + request.Quantity;

                if (newQuantity > product.Quantity)
                {
                    return new ApiResponse<CartDto>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = $"Tổng số lượng vượt quá tồn kho. Còn lại: {product.Quantity}"
                    };
                }

                existingCartItem.Quantity = newQuantity;
                existingCartItem.PriceAtThatTime = product.Price;
                await _cartRepo.UpdateCartItemAsync(existingCartItem);
            }
            else
            {
                // Add new cart item
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    PriceAtThatTime = product.Price,
                    Status = "Active",
                    AddedAt = DateTime.UtcNow
                };

                await _cartRepo.AddCartItemAsync(cartItem);
            }

            // Update cart timestamp
            await _cartRepo.UpdateCartAsync(cart);

            // Reload cart with items
            cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            var cartDto = MapToDto(cart!);

            return new ApiResponse<CartDto>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Thêm vào giỏ hàng thành công",
                Data = cartDto
            };
        }

        public async Task<ApiResponse<CartDto>> UpdateCartItemAsync(UpdateCartItemRequest request, int accountId)
        {
            if (request.Quantity <= 0)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Số lượng phải lớn hơn 0"
                };
            }

            var cartItem = await _cartRepo.GetCartItemByIdAsync(request.CartItemId);
            if (cartItem == null)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy sản phẩm trong giỏ hàng"
                };
            }

            // Verify cart ownership
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            if (cart == null || cart.CartId != cartItem.CartId)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền cập nhật giỏ hàng này"
                };
            }

            // Validate stock
            var product = await _productRepo.GetByIdAsync(cartItem.ProductId);
            if (product == null || product.IsDeleted || product.ProductStatus != "Available")
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Sản phẩm không khả dụng"
                };
            }

            if (request.Quantity > product.Quantity)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Số lượng vượt quá tồn kho. Còn lại: {product.Quantity}"
                };
            }

            // Update cart item
            cartItem.Quantity = request.Quantity;
            cartItem.PriceAtThatTime = product.Price;
            await _cartRepo.UpdateCartItemAsync(cartItem);

            // Update cart timestamp
            await _cartRepo.UpdateCartAsync(cart);

            // Reload cart
            cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            var cartDto = MapToDto(cart!);

            return new ApiResponse<CartDto>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật giỏ hàng thành công",
                Data = cartDto
            };
        }

        public async Task<ApiResponse<CartDto>> IncrementCartItemAsync(int cartItemId, int accountId)
        {
            var cartItem = await _cartRepo.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy sản phẩm trong giỏ hàng"
                };
            }

            // Verify cart ownership
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            if (cart == null || cart.CartId != cartItem.CartId)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền cập nhật giỏ hàng này"
                };
            }

            // Validate stock
            var product = await _productRepo.GetByIdAsync(cartItem.ProductId);
            if (product == null || product.IsDeleted || product.ProductStatus != "Available")
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Sản phẩm không khả dụng"
                };
            }

            var newQuantity = cartItem.Quantity + 1;
            if (newQuantity > product.Quantity)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Số lượng vượt quá tồn kho. Còn lại: {product.Quantity}"
                };
            }

            // Update cart item
            cartItem.Quantity = newQuantity;
            cartItem.PriceAtThatTime = product.Price;
            await _cartRepo.UpdateCartItemAsync(cartItem);

            // Update cart timestamp
            await _cartRepo.UpdateCartAsync(cart);

            // Reload cart
            cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            var cartDto = MapToDto(cart!);

            return new ApiResponse<CartDto>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Tăng số lượng thành công",
                Data = cartDto
            };
        }

        public async Task<ApiResponse<CartDto>> DecrementCartItemAsync(int cartItemId, int accountId)
        {
            var cartItem = await _cartRepo.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy sản phẩm trong giỏ hàng"
                };
            }

            // Verify cart ownership
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            if (cart == null || cart.CartId != cartItem.CartId)
            {
                return new ApiResponse<CartDto>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền cập nhật giỏ hàng này"
                };
            }

            // If quantity = 1, remove item instead
            if (cartItem.Quantity <= 1)
            {
                await _cartRepo.DeleteCartItemAsync(cartItemId);
                await _cartRepo.UpdateCartAsync(cart);

                // Reload cart
                cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
                var cartDto = MapToDto(cart!);

                return new ApiResponse<CartDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Đã xóa sản phẩm khỏi giỏ hàng",
                    Data = cartDto
                };
            }

            // Validate product still available (optional check)
            var product = await _productRepo.GetByIdAsync(cartItem.ProductId);
            if (product != null && !product.IsDeleted)
            {
                cartItem.PriceAtThatTime = product.Price; // Update price
            }

            // Decrease quantity
            cartItem.Quantity -= 1;
            await _cartRepo.UpdateCartItemAsync(cartItem);

            // Update cart timestamp
            await _cartRepo.UpdateCartAsync(cart);

            // Reload cart
            cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            var cartDtoResult = MapToDto(cart!);

            return new ApiResponse<CartDto>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Giảm số lượng thành công",
                Data = cartDtoResult
            };
        }

        public async Task<ApiResponse<object>> RemoveCartItemAsync(int cartItemId, int accountId)
        {
            var cartItem = await _cartRepo.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy sản phẩm trong giỏ hàng"
                };
            }

            // Verify cart ownership
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            if (cart == null || cart.CartId != cartItem.CartId)
            {
                return new ApiResponse<object>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền xóa sản phẩm này"
                };
            }

            await _cartRepo.DeleteCartItemAsync(cartItemId);
            await _cartRepo.UpdateCartAsync(cart);

            return new ApiResponse<object>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Xóa sản phẩm khỏi giỏ hàng thành công"
            };
        }

        public async Task<ApiResponse<object>> ClearCartAsync(int accountId)
        {
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);
            if (cart == null)
            {
                return new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy giỏ hàng"
                };
            }

            await _cartRepo.DeleteAllCartItemsAsync(cart.CartId);
            await _cartRepo.UpdateCartAsync(cart);

            return new ApiResponse<object>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Xóa tất cả sản phẩm thành công"
            };
        }

        private CartDto MapToDto(Cart cart)
        {
            return new CartDto
            {
                CartId = cart.CartId,
                AccountId = cart.AccountId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.ProductName,
                    ProductSku = ci.Product.Sku,
                    PriceAtThatTime = ci.PriceAtThatTime,
                    Quantity = ci.Quantity,
                    Status = ci.Status,
                    AddedAt = ci.AddedAt,
                    AvailableStock = ci.Product.Quantity,
                    MainImageUrl = ci.Product.ProductImages
                        .FirstOrDefault(img => img.IsMain)?.ImageUrl
                }).ToList()
            };
        }
    }
}
