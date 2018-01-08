using Hangfire;
using Innovative.SolarCalculator;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OHC.Core.Events;
using OHC.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using OHC.Core.Settings;

namespace OHC.Core.Scheduler
{
    public class SchedulerService : ISchedulerService, IHostedService
    { 
        private ILogger<SchedulerService> logger;
        private IEventAggregator eventAggregator;
        private SchedulerSettings settings;

        
        public SchedulerService(SchedulerSettings settings, IEventAggregator eventAggregator, ILogger<SchedulerService> logger)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.settings = settings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting SchedulerService");
            RecurringJob.AddOrUpdate<ISchedulerService>((ss) => ss.CreateRecurringSunsetEventJobForToday(), "55 11 * * *", TimeZoneInfo.Local);

            //we don't want to miss the sunset event if the recurring job is created after it's trigger time for today
            var now = DateTime.Now;
            var sunset = CalculateSunset(now, settings.Latitude, settings.Longitude);
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
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void CreateRecurringSunsetEventJobForToday()
        {
            CreateScheduledSunsetEventJob(DateTimeOffset.Now);            
        }

        public void CreateScheduledSunsetEventJob(DateTimeOffset date)
        {
            var sunsetTime = CalculateSunset(date, settings.Latitude, settings.Longitude);
            logger.LogInformation("Scheduling new sunset event for {time}", sunsetTime.ToString());
            BackgroundJob.Schedule<ISchedulerService>((ss) => ss.TriggerSunsetEvent(sunsetTime), sunsetTime);
        }

        public void TriggerSunsetEvent(DateTimeOffset sunsetTime)
        {
            eventAggregator.Publish<SunsetEvent>(new SunsetEvent(sunsetTime));
        }

        public void CleanupLogFiles()
        {
            //TODO: get logging folder and clean up
        }
           

        private DateTimeOffset CalculateSunset(DateTimeOffset date, double lat, double lng)
        {
            try
            {
                var tz = TimeZoneInfo.GetSystemTimeZones().Single(x => x.Id == settings.TimezoneId);
                logger.LogDebug("Timezone for sunset calculation: {timezone}", tz.DisplayName);

                var time = new SolarTimes(date, lat, lng);
                DateTime sunrise = TimeZoneInfo.ConvertTimeFromUtc(time.Sunset.ToUniversalTime(), tz);
                return (DateTimeOffset)sunrise;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogCritical(ex, "Invalid timezone in settings: {timezone}", settings.TimezoneId);
                throw;
            }
        }        
    }
}
