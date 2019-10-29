using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using DockerGui.Hubs;
using DockerGui.Cores.Sentries;
using DockerGui.Cores.Containers;
using AutoMapper;
using Hangfire;
using DockerGui.Configs;
using DockerGui.Repositories;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IHostApplicationLifetime;
using System.Net.Http;

namespace DockerGui
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration
        )
        {
            Configuration = configuration;
            ConfigurationRoot = (IConfigurationRoot)configuration;
        }
        public Startup(
            IConfiguration configuration,
            IConfigurationRoot configurationRoot,
            IHostEnvironment env
        )
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", true, true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            builder.AddEnvironmentVariables();

            configuration = builder.Build();
            Configuration = configuration;
            ConfigurationRoot = configurationRoot;
        }
        public IConfiguration Configuration { get; }
        public IConfigurationRoot ConfigurationRoot { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddControllers(opetion =>
            {
                // opetion.Filters.Add(new CorsAuthorizationFilterFactory(""));
            });

            services.AddSignalR(opetion =>
            {
                // TODO:config
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Docker Gui",
                    Version = "v1"
                });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IContainerCore, ContainerCore>();
            services.AddSingleton<ISentry, Sentry>();
            services.AddSingleton<IRedis, Redis>();

            services.AddHangfire(config =>
            {
                config.UseRedisStorage(new Redis(Configuration).Connection, new Hangfire.Redis.RedisStorageOptions
                {
                    Db = 1
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              IApplicationLifetime applicationLifetime)
        {
            app.UseCors(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.WithOrigins(
                    "http://localhost:8081",
                    "http://localhost:8082",
                    "http://localhost:8083",
                    "http://localhost:8084",
                    "app://."
                );
                x.AllowCredentials();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHangfireDashboard("/job");
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = "docker ui job server"
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BaseHub>("/pull");
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("../swagger/v1/swagger.json", "Docker Gui V1");
            });

            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopped.Register(OnStopped);
        }

        private void OnStarted()
        {
            using var http = new HttpClient();
            _ = http.GetAsync("http://localhost:5000/v1/sentry/start");
        }
        private void OnStopped()
        {
            var manager = new RecurringJobManager(JobStorage.Current);
            foreach (var item in StaticValue.CONTAINERS)
            {
                manager.RemoveIfExists($"stats_{item.ID}");
            }
        }
    }
}