using DAL.Infrastructure.Email;
using DAL.Infrastructure.Redis;
using DAL.Interface;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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

            // Redis
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var redisConnection = configuration.GetConnectionString("Redis")
                    ?? "localhost:6379";
                return ConnectionMultiplexer.Connect(redisConnection);
            });
            services.AddScoped<IRedisService, RedisService>();

            // Email
            services.AddScoped<IEmailService, EmailService>();

            // Auth
            services.AddScoped<IAccountRepository, AccountRepository>();

            // Address
            services.AddScoped<IAddressRepositories, AddressRepositories>();

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

            // Voucher
            services.AddScoped<IVoucherRepository, VoucherRepository>();

            return services;
        }
    }
}
