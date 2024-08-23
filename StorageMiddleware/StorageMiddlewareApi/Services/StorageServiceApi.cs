using Grpc.Core;
using StorageAdapters;
using CacheProvider;

namespace StorageMiddlewareApi;

public class StorageServiceApi : StorageService.StorageServiceBase
{
    private readonly IStorageAdapter storageAdapter;
    private readonly ICacheProvider cacheProvider;

    public StorageServiceApi(IStorageAdapter storageAdapter, ICacheProvider cacheProvider)
    {
        this.storageAdapter = storageAdapter;
        this.cacheProvider = cacheProvider;
    }

    public override async Task<GetDataResponse> GetData(GetDataRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Key))
            return new GetDataResponse { Value = "There is no data with specific key", Found = false };

        string value = await cacheProvider.GetAsync<string>(request.Key);

        if (value == null)
            value = await storageAdapter.ReadAsync<string>(request.Key);

        if (value == null)
            return new GetDataResponse() { };

        await cacheProvider.SetAsync(request.Key, value, TimeSpan.FromHours(1));

        return new GetDataResponse { Value = value, Found = true };
    }

    public override async Task<CreateDataResponse> CreateData(CreateDataRequest request, ServerCallContext context)
    {
        try
        {
            await storageAdapter.CreateAsync(request.Key, request.Value);
            return new CreateDataResponse() { Success = true };
        }
        catch (Exception)
        {
            return new CreateDataResponse() { Success = false };
        }
    }

    public override async Task<UpdateDataResponse> UpdateData(UpdateDataRequest request, ServerCallContext context)
    {
        try
        {
            await storageAdapter.UpdateAsync(request.Key, request.Value);
            return new UpdateDataResponse { Success = true };
        }
        catch (Exception)
        {
            return new UpdateDataResponse { Success = false };
        }
    }

    public override async Task<DeleteDataResponse> DeleteData(DeleteDataRequest request, ServerCallContext context)
    {
        try
        {
            await storageAdapter.DeleteAsync(request.Key);
            return new DeleteDataResponse() { Success = true };
        }
        catch (Exception)
        {
            // Implement your logic to delete data here
            return new DeleteDataResponse { Success = false };
        }
    }
}
