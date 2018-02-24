using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OHC.Core.Interfaces;
using OHC.Core.Settings;
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
            RegisterJobs();

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
            RegisterJobs();
            logger.LogInformation("Job {job} in folder {folder} was added/changed/deleted, re-registered jobs.", e.Name, settings.WhenDoJobsPath);
        }

        private void RegisterJobs()
        {
            whenDoJobEngine.ClearJobRegister();
            var files = Directory.EnumerateFiles(settings.WhenDoJobsPath);
            foreach (var file in files)
            {
                var job = JsonConvert.DeserializeObject<JobDefinition>(File.ReadAllText(file)); //TODO: do we need exclusive lock?
                try
                {
                    whenDoJobEngine.RegisterJob(job);
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, $"Unable to register job {file}");
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return whenDoJobEngine.RunAsync(cancellationToken);
        }
    }
}
