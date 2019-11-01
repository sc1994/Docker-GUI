using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DockerGui.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                      .ConfigureWebHostDefaults(webBuilder =>
                      {
                          webBuilder.UseUrls("http://*:5000")
                                    .UseStartup<Startup>();
                      });
        }
    }
}
