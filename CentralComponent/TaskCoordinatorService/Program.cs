namespace TaskCoordinatorService;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<TaskCoordinatorWorker>();
                services.AddSingleton<IMessageBroker>(sp =>
                    new KafkaMessageBroker(
                        sp.GetRequiredService<ILogger<KafkaMessageBroker>>(),
                        Environment.GetEnvironmentVariable("KAFKA_BROKER"), // Broker list
                        Environment.GetEnvironmentVariable("KAFKA_TASKS_TOPIC")));   // Kafka topic
            });
}
