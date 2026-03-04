    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BLL.DTOs.Products;
    using BLL.DTOs;

    namespace BLL.Interfaces
    {
        public interface IProductService
        {
            Task<ApiResponse<PagedResult<ProductDto>>> GetAdminAsync(ProductQuery query);

            Task<ApiResponse<PagedResult<ProductCardDto>>> GetStorefrontAsync(ProductQuery query);

            Task<ApiResponse<ProductDto>> GetByIdAsync(int id);

            Task<ApiResponse<int>> CreateAsync(CreateProductRequest request);

            Task<ApiResponse<bool>> UpdateAsync(int id, UpdateProductRequest request);

    }
    }
