namespace StorageAdapters;

public interface IStorageAdapter : IDisposable
{
    Task CreateAsync<T>(string key, T value);
    Task<T> ReadAsync<T>(string key);
    Task UpdateAsync<T>(string key, T value);
    Task DeleteAsync(string key);
}
