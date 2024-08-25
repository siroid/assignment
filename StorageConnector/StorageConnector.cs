using Grpc.Net.Client;

namespace StorageConnector;

public class StorageConnector : IStorageConnector
{
    private readonly StorageService.StorageServiceClient _client;

    public StorageConnector()
    {
        var grpcServerAddress = Environment.GetEnvironmentVariable("GRPC_SERVER_ADDRESS");
        var channel = GrpcChannel.ForAddress(grpcServerAddress);

        _client = new StorageService.StorageServiceClient(channel);
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

    public async Task<string> GetDataAsync(string key)
    {
        var request = new GetDataRequest { Key = key };
        var response = await _client.GetDataAsync(request);
        return response.Value;
    }

    public async Task<bool> CreateDataAsync(string key, string value)
    {
        var request = new CreateDataRequest { Key = key, Value = value };
        var response = await _client.CreateDataAsync(request);
        return response.Success;
    }
}