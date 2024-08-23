using Couchbase;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Newtonsoft.Json;
using StorageAdapters;

public class CouchbaseStorageAdapter : IStorageAdapter
{
    private readonly IBucket _bucket;
    private readonly ICouchbaseCollection _collection;
    private bool _disposed = false;

    public CouchbaseStorageAdapter()
    {
        var cluster = Cluster.ConnectAsync("couchbase://localhost", "username", "password").GetAwaiter().GetResult(); // Replace with your Couchbase server address and credentials
        _bucket = cluster.BucketAsync("my_bucket").GetAwaiter().GetResult(); // Replace 'my_bucket' with your bucket name
        _collection = _bucket.DefaultCollection();
    }

    public async Task CreateAsync<T>(string key, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        await _collection.InsertAsync(key, jsonValue).ConfigureAwait(false);
    }

    public async Task<T> ReadAsync<T>(string key)
    {
        try
        {
            var result = await _collection.GetAsync(key).ConfigureAwait(false);
            var jsonValue = result.ContentAs<string>();
            return JsonConvert.DeserializeObject<T>(jsonValue);
        }
        catch (DocumentNotFoundException)
        {
            return default;
        }
    }

    public async Task UpdateAsync<T>(string key, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        await _collection.UpsertAsync(key, jsonValue).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string key)
    {
        await _collection.RemoveAsync(key).ConfigureAwait(false);
    }

    // Implement IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _bucket?.DisposeAsync().AsTask().Wait();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CouchbaseStorageAdapter()
    {
        Dispose(false);
    }
}
