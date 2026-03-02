    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using BLL.DTOs.Products;
    using BLL.DTOs;
    using BLL.Interfaces;
    using DAL.Models;
    using DAL.Interface;
    using Microsoft.AspNetCore.Http;

    namespace BLL.Services
    {
        public class ProductService : IProductService
        {
            private readonly IPriceRangeRepository _priceRangeRepo;
            private readonly IProductRepository _repo;
            private readonly IProductImageService _imageSvc;

            public ProductService(
                IProductRepository repo,
                IProductImageService imageSvc,
                IPriceRangeRepository priceRangeRepo)
            {
                _repo = repo;
                _imageSvc = imageSvc;
                _priceRangeRepo = priceRangeRepo;
            }

            /* ========================== GET ADMIN PAGED ========================== */

            public async Task<ApiResponse<PagedResult<ProductDto>>> GetAdminAsync(ProductQuery query)
            {
                var (items, total) = await _repo.QueryAdminPagedAsync(
                    query.Keyword,
                    query.Page,
                    query.PageSize
                );

                return new ApiResponse<PagedResult<ProductDto>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Danh sách sản phẩm (Admin)",
                    Data = new PagedResult<ProductDto>
                    {
                        Items = items.Select(Map).ToList(),
                        TotalCount = total,
                        Page = query.Page,
                        PageSize = query.PageSize
                    }
                };
            }

            /* ========================== GET STOREFRONT PAGED ========================== */

            public async Task<ApiResponse<PagedResult<ProductCardDto>>> GetStorefrontAsync(ProductQuery query)
            {
                var (items, total) = await _repo.QueryStorefrontPagedAsync(
                    query.Keyword,
                    query.Page,
                    query.PageSize
                );

                return new ApiResponse<PagedResult<ProductCardDto>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Danh sách sản phẩm cửa hàng",
                    Data = new PagedResult<ProductCardDto>
                    {
                        Items = items.Select(MapToCard).ToList(),
                        TotalCount = total,
                        Page = query.Page,
                        PageSize = query.PageSize
                    }
                };
            }

            /* ========================== GET BY ID ========================== */

            public async Task<ApiResponse<ProductDto>> GetByIdAsync(int id)
            {
                var entity = await _repo.GetByIdAsync(id);

                if (entity == null)
                {
                    return new ApiResponse<ProductDto>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy sản phẩm"
                    };
                }

                return new ApiResponse<ProductDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Chi tiết sản phẩm",
                    Data = Map(entity)
                };
            }

            /* ========================== CREATE ========================== */

            public async Task<ApiResponse<int>> CreateAsync(CreateProductRequest request)
            {
                if (await _repo.ExistsByNameAsync(request.ProductName.Trim()))
                {
                    return new ApiResponse<int>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Tên sản phẩm đã tồn tại",
                    };
                }

                var status = request.ProductStatus == "Discontinued"
                    ? "Discontinued"
                    : (request.StockQuantity > 0 ? "Available" : "OutOfStock");
                var priceRanges = await _priceRangeRepo.GetAllAsync();

                var matchedRange = priceRanges
                    .FirstOrDefault(pr =>
                        request.Price >= pr.PriceRangeMin &&
                        request.Price <= pr.PriceRangeMax
                    );

                if (matchedRange == null)
                {
                    return new ApiResponse<int>
                    {
                        Status = 400,
                        Message = "Giá không thuộc khoảng giá nào"
                    };
                }
                var entity = new Product
                {
                    Sku = GenerateSku(),
                    ProductName = request.ProductName.Trim(),
                    CategoryId = request.CategoryId,
                    MaterialId = request.MaterialId,
                    AgeId = request.AgeId,
                    SexId = request.SexId,
                    PriceRangeId = matchedRange.PriceRangeId,
                    BrandId = request.BrandId,
                    OriginId = request.OriginId,
                    Price = request.Price,
                    Quantity = request.StockQuantity,
                    ProductStatus = status,
                    Description = request.DescriptionHtml,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                Console.WriteLine($"AgeId received: {request.AgeId}");

                await _repo.AddAsync(entity);
                if (request.MainImage == null)
                {
                    return new ApiResponse<int>
                    {
                        Status = 400,
                        Message = "Ảnh chính là bắt buộc"
                    };
                }

                if (request.SecondaryImages != null && request.SecondaryImages.Count > 6)
                {
                    return new ApiResponse<int>
                    {
                        Status = 400,
                        Message = "Tối đa 6 ảnh phụ"
                    };
                }

                // Giao cho ImageService xử lý upload + lưu DB
                await _imageSvc.AddImagesAsync(
                    entity.ProductId,
                    entity.Sku,
                    request.MainImage,
                    request.SecondaryImages ?? new List<IFormFile>()
                );

         


                return new ApiResponse<int>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = "Tạo sản phẩm thành công",
                    Data = entity.ProductId
                };
            }

            /* ========================== UPDATE ========================== */

            public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateProductRequest request)
            {
                var entity = await _repo.GetByIdAsync(id);

                if (entity == null)
                {
                    return new ApiResponse<bool>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy sản phẩm",
                        Data = false
                    };
                }

                if (await _repo.ExistsByNameAsync(request.ProductName.Trim(), id))
                {
                    return new ApiResponse<bool>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Tên sản phẩm đã tồn tại",
                        Data = false
                    };
                }
                var priceRanges = await _priceRangeRepo.GetAllAsync();

                var matchedRange = priceRanges
                    .FirstOrDefault(pr =>
                        request.Price >= pr.PriceRangeMin &&
                        request.Price <= pr.PriceRangeMax
                    );

                if (matchedRange == null)
                {
                    return new ApiResponse<bool>
                    {
                        Status = 400,
                        Message = "Giá không thuộc khoảng giá nào",
                        Data = false
                    };
                }


                entity.ProductName = request.ProductName.Trim();
                entity.CategoryId = request.CategoryId;
                entity.MaterialId = request.MaterialId;
                entity.AgeId = request.AgeId;
                entity.SexId = request.SexId;
                entity.PriceRangeId = matchedRange.PriceRangeId;
                entity.BrandId = request.BrandId;
                entity.OriginId = request.OriginId;
                entity.Price = request.Price;
                entity.Quantity = request.StockQuantity;
                entity.Description = request.DescriptionHtml;
                entity.UpdatedAt = DateTime.UtcNow;

                if (request.ProductStatus == "Discontinued")
                {
                    entity.ProductStatus = "Discontinued";
                    entity.IsDeleted = true;
                }
                else
                {
                    entity.IsDeleted = false;
                    entity.ProductStatus =
                        request.StockQuantity > 0 ? "Available" : "OutOfStock";
                }

                await _repo.UpdateAsync(entity);

                if (
                    request.NewMainImage != null ||
                    request.NewSecondaryImages?.Any() == true ||
                    request.KeepSecondaryUrls != null
                )
                {
                    await _imageSvc.UpsertImagesAsync(
                         entity.ProductId,
                         entity.Sku,                
                         request.NewMainImage,         
                         request.NewSecondaryImages,    
                         request.KeepSecondaryUrls      
                     );


                    //// 👉 Upload file mới
                    //if (request.NewMainImage != null || request.NewSecondaryImages?.Any() == true)
                    //{
                    //    await _imageSvc.AddImagesAsync(
                    //        entity.ProductId,
                    //        entity.Sku,
                    //        request.NewMainImage ?? null!,
                    //        request.NewSecondaryImages ?? new List<IFormFile>()
                    //    );
                    //}
                }

                return new ApiResponse<bool>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Cập nhật sản phẩm thành công",
                    Data = true
                };
            }
            private static ProductCardDto MapToCard(Product x) => new()
            {
                Id = x.ProductId,
                ProductName = x.ProductName,
                MainImageUrl = x.ProductImages?
            .FirstOrDefault(pi => pi.IsMain)?.ImageUrl,
                Price = x.Price,
                StockQuantity = x.Quantity
            };

            /* ========================== MAPPING ========================== */

            private static ProductDto Map(Product x) => new()
            {
                Id = x.ProductId,
                Sku = x.Sku,
                ProductName = x.ProductName,

                CategoryId = x.CategoryId,
                MaterialId = x.MaterialId,
                AgeId = x.AgeId,
                SexId = x.SexId,
                PriceRangeId = x.PriceRangeId,
                BrandId = x.BrandId,
                OriginId = x.OriginId,

                Price = x.Price,
                StockQuantity = x.Quantity,
                ProductStatus = x.ProductStatus,
                DescriptionHtml = x.Description,
                IsDeleted = x.IsDeleted,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,

                CategoryName = x.Category?.CategoryName,
                BrandName = x.Brand?.BrandName,
                MaterialName = x.Material?.MaterialName,
                AgeRange = x.Age?.AgeRange,
                SexName = x.Sex?.SexName,
                OriginName = x.Origin?.OriginName,

                MainImageUrl = x.ProductImages?.FirstOrDefault(pi => pi.IsMain)?.ImageUrl,
                SecondaryImageUrls = x.ProductImages?
                    .Where(pi => !pi.IsMain)
                    .Select(pi => pi.ImageUrl)
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .ToList() ?? new List<string>()
            };

            /* ========================== SKU ========================== */

            private static string GenerateSku()
            {
                const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                using var rng = RandomNumberGenerator.Create();

                var data = new byte[6];
                rng.GetBytes(data);

                var sb = new StringBuilder(6);
                foreach (var b in data)
                    sb.Append(alphabet[b % alphabet.Length]);

                return sb.ToString();
            }

      
    }
    }