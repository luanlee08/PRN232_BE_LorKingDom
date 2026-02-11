using BLL.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly ILogger<CloudinaryService> _logger;
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(ILogger<CloudinaryService> logger, Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder,

                    // Optimization
                    Transformation = new Transformation()
                        .Width(1000).Crop("scale").Chain()
                        .Quality("auto:best")    // Tự động nén
                        .FetchFormat("auto"), // Tự chọn WebP / AVIF nếu hỗ trợ

                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    _logger.LogError($"Cloudinary upload error: {uploadResult.Error.Message}");
                    throw new Exception($"Image upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload image to Cloudinary. Folder: {folder}");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    return false;
                }

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result == "ok" || result.Result == "not found")
                {
                    return true;
                }

                _logger.LogWarning($"Cloudinary deletion failed. PublicId: {publicId}, Result: {result.Result}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete image from Cloudinary. PublicId: {publicId}");
                return false;
            }
        }

        public string ExtractPublicId(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || !imageUrl.Contains("cloudinary.com"))
                {
                    return string.Empty;
                }

                // Example URL: https://res.cloudinary.com/dvjvmadao/image/upload/v1234567890/reviews/123/abc.jpg
                // We need: reviews/123/abc

                var uri = new Uri(imageUrl);
                var pathSegments = uri.AbsolutePath.Split('/');

                // Find "upload" or "upload/v{version}" and take everything after it
                var uploadIndex = Array.FindIndex(pathSegments, s => s == "upload");
                if (uploadIndex == -1 || uploadIndex >= pathSegments.Length - 1)
                {
                    return string.Empty;
                }

                // Skip "upload" and optional version (v1234567890)
                var startIndex = uploadIndex + 1;
                if (pathSegments[startIndex].StartsWith("v") && pathSegments[startIndex].Length > 1)
                {
                    startIndex++; // Skip version segment
                }

                // Join remaining segments and remove file extension
                var publicIdWithExt = string.Join("/", pathSegments.Skip(startIndex));
                var lastDotIndex = publicIdWithExt.LastIndexOf('.');

                return lastDotIndex > 0
                    ? publicIdWithExt.Substring(0, lastDotIndex)
                    : publicIdWithExt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to extract public ID from URL: {imageUrl}");
                return string.Empty;
            }
        }
    }
}
