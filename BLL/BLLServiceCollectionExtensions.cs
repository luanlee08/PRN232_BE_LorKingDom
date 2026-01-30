using BLL.Interfaces;
using BLL.Services;
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

            // Đăng ký FluentValidation cho assembly
            services.AddValidatorsFromAssemblyContaining<CreateSuperCategoryValidator>();

            return services;
        }


    }
}
