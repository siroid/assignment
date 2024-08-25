using EventHandlerService;
using StorageConnector;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<EventHandlerWorker>();
builder.Services.AddSingleton<IStorageConnector, StorageConnector.StorageConnector>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

var host = builder.Build();
host.Run();
