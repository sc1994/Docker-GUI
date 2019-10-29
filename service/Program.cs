using DockerGui.Cores.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DockerGui
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        // .UseUrls("http://*:5003")
                        .UseStartup<Startup>();
                });
    }
}
