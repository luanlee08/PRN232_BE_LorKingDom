using DAL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DALServiceCollectionExtensions
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, string? connectionString)
        {
            return services;
        }
    }
}
