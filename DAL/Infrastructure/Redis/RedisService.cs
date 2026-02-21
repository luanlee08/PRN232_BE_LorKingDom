using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DAL.Infrastructure.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private IDatabase? _database;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            try
            {
                if (_redis.IsConnected)
                {
                    _database = _redis.GetDatabase();
                }
            }
            catch
            {
                _database = null; // Redis unavailable
            }
        }

        private bool IsRedisAvailable()
        {
            try
            {
                if (_database == null && _redis.IsConnected)
                {
                    _database = _redis.GetDatabase();
                }
                return _database != null && _redis.IsConnected;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (!IsRedisAvailable()) return false;

            try
            {
                if (expiry.HasValue)
                {
                    return await _database!.StringSetAsync(key, value, expiry.Value);
                }
                else
                {
                    return await _database!.StringSetAsync(key, value);
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetAsync(string key)
        {
            if (!IsRedisAvailable()) return null;

            try
            {
                var value = await _database!.StringGetAsync(key);
                return value.HasValue ? value.ToString() : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            if (!IsRedisAvailable()) return false;

            try
            {
                return await _database!.KeyDeleteAsync(key);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (!IsRedisAvailable()) return false;

            try
            {
                return await _database!.KeyExistsAsync(key);
            }
            catch
            {
                return false;
            }
        }
    }
}