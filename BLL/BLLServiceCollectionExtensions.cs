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

            // Đăng ký FluentValidation cho assembly
            services.AddValidatorsFromAssemblyContaining<CreateSuperCategoryValidator>();

            return services;
        }


    }
}
