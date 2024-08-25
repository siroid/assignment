namespace TaskCoordinatorService;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<TaskCoordinatorWorker>();
                services.AddSingleton<IMessageBroker>(sp =>
                    new KafkaMessageBroker(
                        sp.GetRequiredService<ILogger<KafkaMessageBroker>>(),
                        "kafka:9092", // Broker list
                        "task-topic"));   // Kafka topic
            });
}
