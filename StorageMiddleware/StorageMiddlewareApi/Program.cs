using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace StorageMiddlewareApi; 

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
                webBuilder.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(80, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2; // Support both HTTP/1.1 and HTTP/2
                    });
                });
                webBuilder.UseStartup<Startup>();
            });
}
