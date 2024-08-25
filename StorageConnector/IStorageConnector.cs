namespace StorageConnector;

public interface IStorageConnector
{
    Task<string> GetDataAsync(string key);
    Task<bool> CreateDataAsync(string key, string value);
    Task<bool> UpdateDataAsync(string key, string value);
    Task<bool> DeleteDataAsync(string key);
}
