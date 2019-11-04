using DockerGui.Core.Containers;
using DockerGui.Core.Sentries;
using DockerGui.EfCore;
using DockerGui.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DockerGui.Infrastructure
{
    public class DependencyInjection
    {
        public static void Injection(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IRedisContext, RedisContext>();
            services.AddSingleton<IMySqlContext, MySqlContext>();

            services.AddSingleton<IContainerCore, ContainerCore>();
            services.AddSingleton<ISentry, Sentry>();
        }
    }
}