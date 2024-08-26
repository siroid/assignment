using MonitoringService;
using StorageConnector;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MonitoringWorker>();
builder.Services.AddSingleton<IMonitoringService, MonitoringService.MonitoringService>();
builder.Services.AddSingleton<IStorageConnector, StorageConnector.StorageConnector>();

var host = builder.Build();

host.Run();