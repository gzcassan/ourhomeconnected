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
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Logging;
using Hangfire.Dashboard;
using OHC.Server.Hangfire;
using System.IO;
using OHC.Storage.SensorData.AzureTableStorage;
using OHC.Storage.Interfaces;
using OHC.Core.Settings;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using OHC.Server.Auth;
using OHC.Drivers.PhilipsHue;
using OHC.Drivers.NefitEasy;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core;
using OHC.Core.Services;
using OHC.WhenDoJobs.CommandHandlers;
using OHC.Storage.Models;
using WhenDoJobs.Core.Services;
using Hangfire.Common;
using Newtonsoft.Json;

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
            services.AddWhenDoJob(x => x.UseExternalHangfireServer());

            //https://andrewlock.net/reloading-strongly-typed-options-in-asp-net-core-1-1-0/
            services.Configure<LocationSettings>(options => configuration.GetSection("Scheduler").Bind(options));
            services.Configure<MqttSettings>(options => configuration.GetSection("MQTT").Bind(options));
            services.Configure<PhilipsHueSettings>(options => configuration.GetSection("PhilipsHue").Bind(options));
            services.Configure<NefitEasySettings>(options => configuration.GetSection("NefitEasy").Bind(options));
            services.Configure<ApplicationSettings>(options => configuration.GetSection("Application").Bind(options));
            services.Configure<AzureTableSettings>(options => configuration.GetSection("AzureTableStorage").Bind(options));
            var sp = services.BuildServiceProvider();

            services.AddSingleton<IMqttClient>(
                provider => new MqttClient(sp.GetRequiredService<ILogger<MqttClient>>(), sp.GetRequiredService<IOptions<MqttSettings>>().Value));
            services.AddSingleton<IWhenDoEngine, WhenDoEngine>();
            services.AddSingleton<IHostedService, WhenDoJobsService>();
            services.AddSingleton<IHostedService, MySensorsGateway>();

            services.AddTransient<IAzureTableStorage<SensorDataMessage>>(
                provider => new AzureTableStorage<SensorDataMessage>(sp.GetRequiredService<IOptions<AzureTableSettings>>().Value));
            services.AddTransient<ISensorDataService, SensorDataService>();

            services.AddTransient<INefitEasyClient>(
                provider => new NefitEasyClient(sp.GetRequiredService<IOptions<NefitEasySettings>>().Value));
            services.AddTransient<IPhilipsHueFactory>(
                provider => new PhilipsHueFactory(sp.GetRequiredService<IOptions<PhilipsHueSettings>>().Value, sp.GetRequiredService<ILoggerFactory>()));

            services.AddTransient<PhilipsHueCommandHandler>();
            services.AddTransient<NefitHeatingCommandHandler>();
            services.AddTransient<SensorMessagePersistenceCommandHandler>();

            //sp = services.BuildServiceProvider();



            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
            })
            .AddCustomAuth(options =>
            {
                // Configure password for authentication
                options.Account.Add("daniel", "C88F7B47006444A19B5E27D80344C600");
                options.Account.Add("anca", "763C021BBF574004BE0F064294D699F5");
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            GlobalConfiguration.Configuration.UseActivator(new HangfireJobActivator(serviceProvider));
            JobHelper.SetSerializerSettings(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var whenDo = app.ApplicationServices.GetRequiredService<IWhenDoEngine>();
            whenDo.RegisterCommandHandler<PhilipsHueCommandHandler>("PhilipsHue");
            whenDo.RegisterCommandHandler<SensorMessagePersistenceCommandHandler>("SensorMessagePersistence");
            whenDo.RegisterCommandHandler<NefitHeatingCommandHandler>("NefitHeating");

            JobStorage.Current = new MemoryStorage();
            var options = new BackgroundJobServerOptions { WorkerCount = Environment.ProcessorCount * 3 };
            app.UseHangfireServer(options);            

            var list = new List<IDashboardAuthorizationFilter>() { new HangfireAuthFilter() };
            app.UseHangfireDashboard(options: new DashboardOptions() { Authorization = list });
            //TODO: Check this for authentication: https://stackoverflow.com/questions/41623551/asp-net-core-mvc-hangfire-custom-authentication


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
