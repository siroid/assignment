using RoboticArm;

namespace RoboticArmService
{
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
                    services.AddHostedService<RoboticArmWorker>();
                    services.AddSingleton<ITaskHandler, TaskHandler>();
                    services.AddSingleton<ITelemetryService, TelemetryService>();
                });
    }
}