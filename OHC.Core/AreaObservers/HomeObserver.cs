using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OHC.Core.Events;
using OHC.Core.Infrastructure;
using OHC.Core.Settings;
using OHC.Drivers.NefitEasy;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.AreaObservers
{
    public class HomeObserver : IHomeObserver
    {
        ILogger<HomeObserver> logger;
        IEventAggregator eventAggregator;
        INefitEasyClient nefitEasyClient;
        HomeSettings homeSettings;

        public HomeObserver(INefitEasyClient nefitEasyClient, IOptions<HomeSettings> homeSettings, IEventAggregator eventAggregator, ILogger<HomeObserver> logger)
        {
            this.nefitEasyClient = nefitEasyClient;
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.homeSettings = homeSettings.Value;
        }

        public Task StartAsync()
        {
            logger.LogInformation("Starting HomeObserver");
            this.eventAggregator.GetEvent<HomeStatusEvent>()
                .Where(ev => ev.Status == HomeStatus.GoingToSleep)
                .Subscribe(async ev => await OnGoingToSleep());

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            logger.LogInformation("Stopping HomeObserver");
            return Task.CompletedTask;
        }

        private async Task OnGoingToSleep()
        {
            try
            {
                await nefitEasyClient.SetScheduleOverruleTemp(homeSettings.NightTemperature);
            }
            catch(Exception ex)
            {
                logger.LogCritical("Unable to set heating to {temp}", homeSettings.NightTemperature, ex);
            }
        }
    }
}
