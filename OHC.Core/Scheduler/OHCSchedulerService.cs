using Hangfire;
using Innovative.SolarCalculator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using OHC.Core.Settings;
using Microsoft.Extensions.Options;
using WhenDoJobs.Core.Interfaces;
using OHC.Core.Interfaces;

namespace OHC.Core.Scheduler
{
    public class OHCSchedulerService : IOHCSchedulerService, IHostedService
    {
        private ILogger<OHCSchedulerService> logger;
        private SchedulerSettings settings;
        private IWhenDoQueueProvider queueProvider;

        public OHCSchedulerService(IWhenDoQueueProvider queueProvider, IOptions<SchedulerSettings> settings, ILogger<OHCSchedulerService> logger)
        {
            this.logger = logger;
            this.settings = settings.Value;
            this.queueProvider = queueProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting HomeApplicationService");

            CreateSunsetEventJob();
            SwitchOffLightsDuringNightJob();

            await Task.CompletedTask;
        }

        private void SwitchOffLightsDuringNightJob()
        {
            var cron = Cron.Daily(settings.NightTime.Hours, settings.NightTime.Minutes);
            RecurringJob.AddOrUpdate<IWhenDoQueueProvider>(queue => queue.EnqueueMessage(new TimeEvent(TimeEventType.Night, settings.NightTime)), cron, TimeZoneInfo.Local);
        }


        private void CreateSunsetEventJob()
        {
            var cron = Cron.Daily(11, 55);
            RecurringJob.AddOrUpdate<IOHCSchedulerService>((ss) => ss.CreateSunsetEventJobForToday(), cron, TimeZoneInfo.Local);

            //we don't want to miss the sunset event if the recurring job is created after it's trigger time for today
            var now = DateTime.Now;

            var sunset = OHCHelpers.CalculateSunset(now, settings.TimezoneId, settings.Latitude, settings.Longitude);
            if (now > new DateTime(now.Year, now.Month, now.Day, 11, 55, 00) && now < sunset)
            {
                logger.LogDebug("Server started after recurring job and before sunset, so a one-off job will be created for today.");
                CreateScheduledSunsetEventJob(now);
            }
            else
            {
                logger.LogDebug("No need to create one-off job, because now: {now} is < {1155} or later than sunset at {sunset}",
                    now.ToString(), new DateTime(now.Year, now.Month, now.Day, 11, 55, 00), sunset.ToString());
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping HomeApplicationService");
            await Task.CompletedTask;
        }

        public void CreateSunsetEventJobForToday()
        {
            CreateScheduledSunsetEventJob(DateTimeOffset.Now);
        }

        public void CreateScheduledSunsetEventJob(DateTimeOffset date)
        {
            try
            {
                var sunsetTime = OHCHelpers.CalculateSunset(date, settings.TimezoneId, settings.Latitude, settings.Longitude);
                logger.LogInformation("Scheduling new sunset event for {time}", sunsetTime.ToString());
                BackgroundJob.Schedule<IWhenDoQueueProvider>(queue => queue.EnqueueMessage(new TimeEvent(TimeEventType.Sunset, sunsetTime.TimeOfDay)), sunsetTime);
                
            }
            catch(InvalidOperationException ex)
            {
                logger.LogError(ex, "Invalid timezone in settings: {timezone}", settings.TimezoneId);
                throw;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error scheduling sunset event.");
                throw;
            }
        }
    }
}
