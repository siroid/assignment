using dotnet_etcd;
using Newtonsoft.Json;
using StorageAdapters;

public class EtcdStorageAdapter : IStorageAdapter
{
    private readonly EtcdClient _etcdClient;
    private bool _disposed = false;

    public EtcdStorageAdapter()
    {
        _etcdClient = new EtcdClient("http://localhost:2379"); // Replace with your Etcd server address
    }

    public async Task CreateAsync<T>(string key, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        await _etcdClient.PutAsync(key, jsonValue).ConfigureAwait(false);
    }

    public async Task<T> ReadAsync<T>(string key)
    {
        var response = await _etcdClient.GetValAsync(key).ConfigureAwait(false);
        if (string.IsNullOrEmpty(response)) return default;
        return JsonConvert.DeserializeObject<T>(response);
    }

    public async Task UpdateAsync<T>(string key, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        await _etcdClient.PutAsync(key, jsonValue).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string key)
    {
        await _etcdClient.DeleteAsync(key).ConfigureAwait(false);
    }

    // Implement IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _etcdClient.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~EtcdStorageAdapter()
    {
        Dispose(false);
    }
}
