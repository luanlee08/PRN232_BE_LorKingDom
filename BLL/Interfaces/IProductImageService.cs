using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces
{
    public interface IProductImageService
    {
        Task AddImagesAsync(
       int productId,
       string sku,
       IFormFile mainImage,
       IEnumerable<IFormFile> secondaryImages);


        Task UpsertImagesAsync(
                int productId,
                string sku,
                IFormFile? newMainImage,
                List<IFormFile>? newSecondaryImages,
                List<string>? keepSecondaryUrls
            );
    }

}