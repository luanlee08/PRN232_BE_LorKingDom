using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class ProductImageService : IProductImageService
    {
        private const int MAX_SECONDARY = 6;
        private readonly IProductImageRepository _repo;

        public ProductImageService(IProductImageRepository repo)
        {
            _repo = repo;
        }

        public async Task AddImagesAsync(
            int productId,
            string sku,
            IFormFile mainImage,
            IEnumerable<IFormFile> secondaryImages)
        {
            if (productId <= 0) throw new ArgumentException("ProductId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU không hợp lệ.");
            if (mainImage == null) throw new ArgumentException("Ảnh chính là bắt buộc.");

            await _repo.ExecuteInTransactionAsync(async () =>
            {
                // clear main cũ
                await _repo.UnsetMainAsync(productId);

                // MAIN IMAGE
                var mainUrl = await SaveFileAsync(sku, mainImage, true);
                await _repo.AddAsync(new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = mainUrl,
                    IsMain = true
                });

                // SECONDARY IMAGES
                var list = secondaryImages?.Take(MAX_SECONDARY).ToList() ?? new();

                foreach (var file in list)
                {
                    var url = await SaveFileAsync(sku, file);
                    await _repo.AddAsync(new ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = url,
                        IsMain = false
                    });
                }
            });
        }

        public async Task UpsertImagesAsync(
            int productId,
            string sku,
            IFormFile? newMainImage,
            List<IFormFile>? newSecondaryImages,
            List<string>? keepSecondaryUrls
        )
        {
            if (productId <= 0)
                throw new ArgumentException("ProductId không hợp lệ");

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU không hợp lệ");

            var uploadFolder = Path.Combine("wwwroot", "uploads", "products", sku);
            Directory.CreateDirectory(uploadFolder);

            await _repo.ExecuteInTransactionAsync(async () =>
            {
                /* ===================== MAIN IMAGE ===================== */
                if (newMainImage != null)
                {
                    // 1. unset main cũ (DB)
                    await _repo.UnsetMainAsync(productId);

                    // 2. xóa file main cũ (optional nhưng NÊN)
                    var oldMain = await _repo.GetMainAsync(productId);
                    if (oldMain != null)
                    {
                        var oldPath = Path.Combine("wwwroot", oldMain.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }

                    // 3. lưu file main mới
                    var mainUrl = await SaveFileAsync(sku, newMainImage, isMain: true);

                    // 4. lưu DB
                    await _repo.AddAsync(new ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = mainUrl,
                        IsMain = true
                    });
                }

                /* ===================== SECONDARY IMAGES ===================== */

                var currentSubs = await _repo.GetByProductIdAsync(productId);
                var currentSecondary = currentSubs.Where(x => !x.IsMain).ToList();

                var keepSet = new HashSet<string>(
                    keepSecondaryUrls ?? new List<string>(),
                    StringComparer.OrdinalIgnoreCase
                );

                // 1. xác định ảnh cần xóa (KHÔNG nằm trong keep)
                var toDelete = currentSecondary
                    .Where(x => !keepSet.Contains(x.ImageUrl))
                    .ToList();

                // 2. xóa DB + file
                foreach (var img in toDelete)
                {
                    var path = Path.Combine("wwwroot", img.ImageUrl.TrimStart('/'));
                    if (File.Exists(path))
                        File.Delete(path);
                }

                if (toDelete.Any())
                    _repo.RemoveRange(toDelete);

                // 3. thêm ảnh mới (nếu có)
                if (newSecondaryImages != null && newSecondaryImages.Any())
                {
                    var remainSlots = MAX_SECONDARY - (currentSecondary.Count - toDelete.Count);
                    if (remainSlots < 0) remainSlots = 0;

                    var filesToAdd = newSecondaryImages.Take(remainSlots);

                    var entities = new List<ProductImage>();

                    foreach (var file in filesToAdd)
                    {
                        var url = await SaveFileAsync(sku, file);
                        entities.Add(new ProductImage
                        {
                            ProductId = productId,
                            ImageUrl = url,
                            IsMain = false
                        });
                    }

                    if (entities.Any())
                        await _repo.AddRangeAsync(entities);
                }

                await _repo.SaveChangesAsync();
            });
        }


        private async Task<string> SaveFileAsync(
            string sku,
            IFormFile file,
            bool isMain = false)
        {
            var ext = Path.GetExtension(file.FileName);
            var fileName = isMain
                ? $"main{ext}"
                : $"{Guid.NewGuid()}{ext}";

            var folder = Path.Combine("wwwroot", "uploads", "products", sku);
            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/products/{sku}/{fileName}";
        }
    }
}