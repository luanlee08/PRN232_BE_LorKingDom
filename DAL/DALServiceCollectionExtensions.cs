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

            // SupperCategories
            services.AddScoped<ISuperCategoryRepository, SuperCategoryRepository>();
            //Categories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            //Brand
            services.AddScoped<IBrandRepository, BrandRepository>();

            return services;
        }
    }
}
