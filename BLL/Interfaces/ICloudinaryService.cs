using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);

        Task<bool> DeleteImageAsync(string publicId);

        string ExtractPublicId(string imageUrl);
    }
}
