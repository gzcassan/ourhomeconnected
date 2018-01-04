using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace OHC.Server
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) //move this to appsettings
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .CreateLogger();

            try
            {
                Log.Information("Machine: {machine}", Environment.MachineName);
                Log.Information("Platform: {platform}, version: {version}, 64bit: {64bit}", 
                    Environment.OSVersion.Platform.ToString(), Environment.OSVersion.VersionString, Environment.Is64BitOperatingSystem.ToString());
                Log.Information("Processor count: {processors}", Environment.ProcessorCount.ToString());
                Log.Information("Current directory: {directory}", Environment.CurrentDirectory);
                
                Log.Information("Starting web host");
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:8000")
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
