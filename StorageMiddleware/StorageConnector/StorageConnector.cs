using Grpc.Net.Client;

namespace StorageConnector;

public class StorageConnector : IStorageConnector
{
    private readonly StorageService.StorageServiceClient _client;

    public StorageConnector()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:7000");
        _client = new StorageService.StorageServiceClient(channel);
    }

    public async Task<string> GetDataAsync(string id)
    {
        var request = new GetDataRequest { Key = id };
        var response = await _client.GetDataAsync(request);
        return response.Value;
    }

    public async Task<bool> CreateDataAsync(string data)
    {
        var request = new CreateDataRequest { Value = data };
        var response = await _client.CreateDataAsync(request);
        return response.Success;
    }

    public async Task<bool> UpdateDataAsync(string id, string data)
    {
        var request = new UpdateDataRequest { Key = id, Value = data };
        var response = await _client.UpdateDataAsync(request);
        return response.Success;
    }

    public async Task<bool> DeleteDataAsync(string id)
    {
        var request = new DeleteDataRequest { Key = id };
        var response = await _client.DeleteDataAsync(request);
        return response.Success;
    }
}