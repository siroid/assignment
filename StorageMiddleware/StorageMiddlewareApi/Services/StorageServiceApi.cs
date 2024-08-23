using Grpc.Core;
using System.Threading.Tasks;
using StorageMiddlewareApi;

namespace StorageMiddlewareApi;

public class StorageServiceApi : StorageMiddlewareApi.StorageService.StorageServiceBase
{
    public override Task<GetDataResponse> GetData(GetDataRequest request, ServerCallContext context)
    {
        // Implement your logic to get data here
        return Task.FromResult(new GetDataResponse { Data = "Sample Data for ID " + request.Id });
    }

    public override Task<CreateDataResponse> CreateData(CreateDataRequest request, ServerCallContext context)
    {
        // Implement your logic to create data here
        return Task.FromResult(new CreateDataResponse { Success = true });
    }

    public override Task<UpdateDataResponse> UpdateData(UpdateDataRequest request, ServerCallContext context)
    {
        // Implement your logic to update data here
        return Task.FromResult(new UpdateDataResponse { Success = true });
    }

    public override Task<DeleteDataResponse> DeleteData(DeleteDataRequest request, ServerCallContext context)
    {
        // Implement your logic to delete data here
        return Task.FromResult(new DeleteDataResponse { Success = true });
    }
}
