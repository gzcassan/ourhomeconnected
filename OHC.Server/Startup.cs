using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OHC.Core.Mqtt;
using OHC.Core.MySensorsGateway;
using OHC.Core.Infrastructure;
using OHC.Core.AreaObservers;
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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using OHC.Server.Auth;
using OHC.Drivers.PhilipsHue;
using OHC.Server.Controllers;
using OHC.Drivers.NefitEasy;

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

            //https://andrewlock.net/reloading-strongly-typed-options-in-asp-net-core-1-1-0/
            services.Configure<SchedulerSettings>(options => configuration.GetSection("Scheduler").Bind(options));
            services.Configure<MqttSettings>(options => configuration.GetSection("MQTT").Bind(options));
            services.Configure<PhilipsHueSettings>(options => configuration.GetSection("PhilipsHue").Bind(options));
            services.Configure<LivingroomSettings>(options => configuration.GetSection("Areas:Livingroom").Bind(options));
            services.Configure<HomeSettings>(options => configuration.GetSection("Areas:Home").Bind(options));
            services.Configure<NefitEasySettings>(options => configuration.GetSection("NefitEasy").Bind(options));

            var sp = services.BuildServiceProvider();

            //https://joonasw.net/view/aspnet-core-di-deep-dive
            //services.AddTransient<IDataService, DataService>((ctx) =>
            //{
            //    IOtherService svc = ctx.GetService<IOtherService>();
            //    //IOtherService svc = ctx.GetRequiredService<IOtherService>();
            //    return new DataService(svc);
            //});

            sp = services.BuildServiceProvider();

            var eventAggregator = new EventAggregator(sp.GetRequiredService<ILogger<EventAggregator>>());
            services.AddSingleton<IEventAggregator>(prov => eventAggregator);

            services.AddSingleton<IMqttClient, MqttClient>();

            services.Configure<AzureTableSettings>(options => configuration.GetSection("AzureTableStorage").Bind(options));

            services.AddTransient<IAzureTableStorage<SensorDataReading>>(
                provider => new AzureTableStorage<SensorDataReading>(sp.GetRequiredService<IOptions<AzureTableSettings>>().Value));
            services.AddTransient<ISensorDataService, SensorDataService>();

            sp = services.BuildServiceProvider();

            var gateway = new MySensorsGateway(sp.GetRequiredService<IMqttClient>(), eventAggregator,
                sp.GetRequiredService<ILogger<MySensorsGateway>>(), sp.GetRequiredService<IOptions<MqttSettings>>());
            services.AddSingleton<IHostedService>(provider => gateway);
            services.AddSingleton<IMySensorsGateway>(provider => gateway);

            services.AddSingleton<INefitEasyClient>(provider => new NefitEasyClient(sp.GetRequiredService<IOptions<NefitEasySettings>>().Value));

            services.AddSingleton<IPhilipsHueFactory>(
                provider => new PhilipsHueFactory(sp.GetRequiredService<IOptions<PhilipsHueSettings>>().Value, sp.GetRequiredService<ILoggerFactory>()));
            services.AddSingleton<IHomeObserver, HomeObserver>();
            services.AddSingleton<IBathroomObserver, BathroomObserver>();
            services.AddSingleton<ILivingroomObserver, LivingroomObserver>();
            services.AddSingleton<IMasterBedroomObserver, MasterBedroomObserver>();

            sp = services.BuildServiceProvider();

            var ss = new OHCApplicationService(sp.GetRequiredService<IOptions<SchedulerSettings>>(), eventAggregator, sp.GetRequiredService<ILogger<OHCApplicationService>>(),
                sp.GetRequiredService<IHomeObserver>(),
                sp.GetRequiredService<ILivingroomObserver>(),
                sp.GetRequiredService<IBathroomObserver>(),
                sp.GetRequiredService<IMasterBedroomObserver>()
                );
            services.AddSingleton<IHostedService>(prov => ss);
            services.AddSingleton<IOHCApplicationService>(prov => ss);

            services.AddScoped<HomeController>(prov => new HomeController(eventAggregator, sp.GetRequiredService<ILogger<HomeController>>()));


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
            })
            // Call custom authentication extension method
            .AddCustomAuth(options =>
            {
                // Configure password for authentication
                options.Account.Add("daniel", "C88F7B47006444A19B5E27D80344C600");
                options.Account.Add("anca", "763C021BBF574004BE0F064294D699F5");
            });

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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseAuthentication();


            var options = new BackgroundJobServerOptions { WorkerCount = Environment.ProcessorCount * 4 };
            app.UseHangfireServer(options);

            var list = new List<IDashboardAuthorizationFilter>() { new HangfireAuthFilter() };
            app.UseHangfireDashboard(options: new DashboardOptions() { Authorization = list });
            //TODO: Check this for authentication: https://stackoverflow.com/questions/41623551/asp-net-core-mvc-hangfire-custom-authentication

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Default}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Default", action = "Index" });
            });
        }
    }
}
