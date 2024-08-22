namespace StorageConnector;

public interface IStorageConnector
{
    Task<string> GetDataAsync(string id);
    Task<bool> CreateDataAsync(string data);
    Task<bool> UpdateDataAsync(string id, string data);
    Task<bool> DeleteDataAsync(string id);
}
