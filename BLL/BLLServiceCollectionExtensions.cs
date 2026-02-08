using BLL.Interfaces;
using BLL.Services;
using BLL.Validators.Address;
using BLL.Validators.Auth;
using BLL.Validators.SuperCategory;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class BLLServiceCollectionExtensions
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddScoped<ISuperCategoryService, SuperCategoryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IAgeService, AgeService>();
            services.AddScoped<IOriginService, OriginService>();
            services.AddScoped<IPriceRangeService, PriceRangeService>();
            services.AddScoped<ISexService, SexService>();

            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // Address
            services.AddScoped<IAddressServices, AddressServices>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();

            // Voucher
            services.AddScoped<IVoucherService, VoucherService>();

            // Đăng ký FluentValidation cho assembly
            services.AddValidatorsFromAssemblyContaining<CreateSuperCategoryValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

            return services;
        }


    }
}
