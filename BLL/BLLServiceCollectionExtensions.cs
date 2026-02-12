using BLL.Interfaces;
using BLL.Interfaces.Moderation;
using BLL.Services;
using BLL.Services.Moderation;
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

            // Cart
            services.AddScoped<ICartService, CartServices>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();

            // Voucher
            services.AddScoped<IVoucherService, VoucherService>();

            // Review product         
            services.AddScoped<IReviewProductService, ReviewProductService>();
            services.AddScoped<IReviewReactionService, ReviewProductReactionService>();

            // Cloudinary
            services.AddSingleton<ICloudinaryService, CloudinaryService>();

            // Moderate product
            services.AddScoped<IReviewModerationService, ReviewModerationService>();
            services.AddScoped<IModerationLayer1Service, ModerationLayer1Service>();
            services.AddScoped<IModerationLayer2Service, ModerationLayer2Service>();

            // Notification
            services.AddScoped<INotificationService, NotificationService>();

            // Template
            services.AddScoped<ITemplateService, TemplateService>();

            // Đăng ký FluentValidation cho assembly (Đk 1 cái là đủ, mấy cái còn lại ăn theo)
            services.AddValidatorsFromAssemblyContaining<CreateSuperCategoryValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

            return services;
        }
    }
}
