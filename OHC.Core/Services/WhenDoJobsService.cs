using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OHC.Core.Interfaces;
using OHC.Core.Settings;
using OHC.Core.WhenDoProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Models;

namespace OHC.Core.Services
{
    public class WhenDoJobsService : HostedService, IWhenDoJobsService
    {
        private IWhenDoEngine whenDoJobEngine;
        private ApplicationSettings settings;
        private ILogger<WhenDoJobsService> logger;

        public WhenDoJobsService(IWhenDoEngine whenDoJobEngine, IOptions<ApplicationSettings> settings, ILogger<WhenDoJobsService> logger)
        {
            this.logger = logger;
            this.whenDoJobEngine = whenDoJobEngine;
            this.settings = settings.Value;
            ConfigureJobConfigWatcher();
        }

        private void ConfigureJobConfigWatcher()
        {
            if (!string.IsNullOrEmpty(settings.WhenDoJobsPath))
            {
                var watcher = new FileSystemWatcher(settings.WhenDoJobsPath, "*.json")
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
                };

                watcher.Changed += new FileSystemEventHandler(OnWhenDoJobChanged);
                watcher.Created += new FileSystemEventHandler(OnWhenDoJobChanged);
                watcher.Deleted += new FileSystemEventHandler(OnWhenDoJobChanged);

                watcher.EnableRaisingEvents = true;
            }
        }

        // Define the event handlers.
        private void OnWhenDoJobChanged(object source, FileSystemEventArgs e)
        {
            logger.LogInformation("Job {job} in folder {folder} was added/changed/deleted, will try to re-register job.", e.Name, settings.WhenDoJobsPath);
            RegisterJobsAsync().Wait();
        }

        private async Task RegisterJobsAsync()
        {
            await whenDoJobEngine.ClearJobsAsync();
            var files = Directory.EnumerateFiles(settings.WhenDoJobsPath, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var job = JsonConvert.DeserializeObject<JobDefinition>(File.ReadAllText(file)); //TODO: do we need exclusive lock?
                    await whenDoJobEngine.RegisterJobAsync(job);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, $"Unable to register job {file}");
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            whenDoJobEngine.RegisterExpressionProvider("DateTime", typeof(OHCDateTimeProvider));
            await RegisterJobsAsync();
            await whenDoJobEngine.RunAsync(cancellationToken);
        }
    }
}
