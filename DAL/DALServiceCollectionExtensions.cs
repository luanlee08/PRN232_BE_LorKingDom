using DAL.Interface;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DALServiceCollectionExtensions
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<AspLorKingDomContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            //Nhánh Product
            services.AddScoped<ISuperCategoryRepository, SuperCategoryRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IMaterialRepository, MaterialRepository>();
            services.AddScoped<IAgeRepository, AgeRepository>();
            services.AddScoped<IOriginRepository, OriginRepository>();
            services.AddScoped<IPriceRangeRepository, PriceRangeRepository>();
            services.AddScoped<ISexRepository, SexRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            return services;
        }
    }
}
