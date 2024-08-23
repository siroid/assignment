using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace CacheProvider;

public class RedisCacheProvider : ICacheProvider
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _redisDatabase;
    private bool _disposed = false;

    public RedisCacheProvider()
    {
        var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
        var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
        var connectionMultiplexer = ConnectionMultiplexer.Connect($"{redisHost}:{redisPort}");
        _redisDatabase = connectionMultiplexer.GetDatabase();
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _redisDatabase.StringGetAsync(key);
        return value.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        await _redisDatabase.StringSetAsync(key, jsonValue, expiration);
    }

    public async Task RemoveAsync(string key)
    {
        await _redisDatabase.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _redisDatabase.KeyExistsAsync(key);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connectionMultiplexer?.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~RedisCacheProvider()
    {
        Dispose(false);
    }
}
