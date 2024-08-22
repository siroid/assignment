using EventHandlerService;
using StorageConnector;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IStorageConnector, StorageConnector.StorageConnector>();

var host = builder.Build();
host.Run();
