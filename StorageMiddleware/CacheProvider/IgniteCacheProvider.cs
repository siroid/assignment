using System;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Cache;
using Newtonsoft.Json;
using Apache.Ignite.Core.Client.Cache;

namespace CacheProvider;

public class IgniteCacheProvider : ICacheProvider
{
    private readonly IIgniteClient _igniteClient;
    private readonly ICacheClient<string, string> _igniteCache;

    private bool _disposed = false;

    public IgniteCacheProvider()
    {
        var igniteHost = Environment.GetEnvironmentVariable("IGNITE_HOST") ?? "localhost";
        var ignitePort = Environment.GetEnvironmentVariable("IGNITE_PORT") ?? "10800";
        var cfg = new IgniteClientConfiguration
        {
            Endpoints = new[] { $"{igniteHost}:{ignitePort}" },
            SocketTimeout = TimeSpan.FromSeconds(30),
            EnablePartitionAwareness = true
        };

        // Start Ignite client using the configuration
        _igniteClient = Ignition.StartClient(cfg);

        // Initialize the cache
        _igniteCache = _igniteClient.GetOrCreateCache<string, string>("defaultCache");
    }

    public async Task<T> GetAsync<T>(string key)
    {
        // var jsonValue = await Task.FromResult(_igniteCache.Get(key));
        // return jsonValue == null ? default : JsonConvert.DeserializeObject<T>(jsonValue);
        return default(T);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        // var jsonValue = JsonConvert.SerializeObject(value);
        // _igniteCache.Put(key, jsonValue);
        // await Task.CompletedTask; // Ignite doesn't support expiration directly in version 2.16.0
    }

    public async Task RemoveAsync(string key)
    {
        // _igniteCache.Remove(key);
        // await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return default(bool);
        // return await Task.FromResult(_igniteCache.ContainsKey(key));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _igniteClient?.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~IgniteCacheProvider()
    {
        Dispose(false);
    }
}
