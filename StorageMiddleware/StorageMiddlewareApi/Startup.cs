using CacheProvider;

namespace StorageMiddlewareApi;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();

        string selectedPersistedStorage = Environment.GetEnvironmentVariable("SELECTED_PERSISTED_STORAGE") ?? "cassandra";
        switch (selectedPersistedStorage)
        {
            case "cassandra":
                {
                    services.AddSingleton<IStorageAdapter, CassandraStorageAdapter>();
                    break;
                }
            case "etcd":
                {
                    services.AddSingleton<IStorageAdapter, EtcdStorageAdapter>();
                    break;
                }
            case "couchbase":
                {
                    services.AddSingleton<IStorageAdapter, CouchbaseStorageAdapter>();
                    break;
                }
        }

        string selectedCacheStorage = Environment.GetEnvironmentVariable("SELECTED_CACHE_STORAGE") ?? "redis";
        switch (selectedCacheStorage)
        {
            case "redis":
                {
                    services.AddSingleton<ICacheProvider, RedisCacheProvider>();
                    break;
                }
            case "ignite":
                {
                    services.AddSingleton<ICacheProvider, IgniteCacheProvider>();
                    break;
                }
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<StorageMiddlewareApi.StorageServiceApi>();
        });
    }
}
