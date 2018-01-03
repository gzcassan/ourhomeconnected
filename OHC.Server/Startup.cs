using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OHC.MqttService;
using OHC.Core.MySensorsGateway;
using OHC.Core.Infrastructure;
using OHC.Core.RoomManagers;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Logging;
using Hangfire.Dashboard;
using OHC.Server.Hangfire;
using System.IO;
using OHC.Core.Scheduler;
using OHC.Storage.SensorData.AzureTableStorage;
using OHC.Storage.Models;
using OHC.Storage.Interfaces;
using OHC.Core.Settings;

namespace OHC.Server
{
    public class Startup
    {
        private IConfiguration configuration;
        private IServiceProvider serviceProvider;

        public Startup(IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            this.serviceProvider = serviceProvider;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            this.configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseMemoryStorage());

            services.AddTransient<IAzureTableStorage<SensorDataReading>>(factory =>
            {
                return new AzureTableStorage<SensorDataReading>(
                    new AzureTableSettings(
                        storageAccount: configuration["AzureStorage:StorageAccount"],
                        storageKey: configuration["AzureStorage:StorageKey"],
                        tableName: configuration["AzureStorage:TableName"]));
            });

            services.AddTransient<ISensorDataService, SensorDataService>();

            services.AddSingleton<IEventAggregator, EventAggregator>();

            services.AddSingleton<IMqttClient, MqttClient>();

            var sp = services.BuildServiceProvider();
            var gateway = new MySensorsGateway(sp.GetRequiredService<IMqttClient>(), sp.GetRequiredService<IEventAggregator>(), sp.GetRequiredService<ILogger<MySensorsGateway>>(),
                new MqttSettings(
                        host: configuration["MQTT:Host"],
                        clientId: configuration["MQTT:ClientId"],
                        username: configuration["MQTT:Username"],
                        password: configuration["MQTT:Password"]));

            services.AddSingleton<IHostedService>(provider => gateway);
            services.AddSingleton<IMySensorsGateway>(provider => gateway);

            var bm = new BathroomManager(sp.GetRequiredService<IEventAggregator>(), sp.GetRequiredService<ILogger<BathroomManager>>());
            services.AddSingleton<IBathroomManager>(provider => bm);

            var lr = new LivingroomManager(
                new PhilipsHueSettings(
                    host: configuration["PhilipsHue:Host"],
                    key: configuration["PhilipsHue:Key"],
                    sunsetScene: configuration["PhilipsHue:SunsetScene"]),
                sp.GetRequiredService<IEventAggregator>(), sp.GetRequiredService<ILogger<LivingroomManager>>());
            services.AddSingleton<ILivingroomManager>(provider => lr);

            var so = new StorageObserver(sp.GetRequiredService<ILogger<StorageObserver>>(), sp.GetRequiredService<IEventAggregator>(), sp.GetRequiredService<ISensorDataService>());
            //TODO: Weird!! Find out why IStorageManager needs to be first????
            services.AddSingleton<IStorageObserver>(provider => so);
            services.AddSingleton<IHostedService>(provider => so);

            var ss = new SchedulerService(configuration["Scheduler:TimezoneId"], sp.GetRequiredService<IEventAggregator>(), sp.GetRequiredService<ILogger<SchedulerService>>());
            services.AddTransient<IHostedService>(prov => ss);
            services.AddTransient<ISchedulerService>(prov => ss);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            
            var options = new BackgroundJobServerOptions { WorkerCount = Environment.ProcessorCount * 5 };
            app.UseHangfireServer(options);

            var list = new List<IDashboardAuthorizationFilter>() { new HangfireAuthFilter() };
            app.UseHangfireDashboard(options: new DashboardOptions() { Authorization = list });
            //app.UseHangfireDashboard(); //Check this for authentication: https://stackoverflow.com/questions/41623551/asp-net-core-mvc-hangfire-custom-authentication

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
