using System;
using System.Threading.Tasks;

namespace DAL.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}