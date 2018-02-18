using Microsoft.Extensions.Logging;
using OHC.Core.Events;
using OHC.Core.Infrastructure;
using OHC.Drivers.PhilipsHue;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.AreaObservers
{
    public class MasterBedroomObserver : IMasterBedroomObserver
    {
        ILogger<MasterBedroomObserver> logger;
        IPhilipsHueFactory hueFactory;
        IEventAggregator eventAggregator;

        public MasterBedroomObserver(IPhilipsHueFactory factory, IEventAggregator eventAggregator, ILogger<MasterBedroomObserver> logger)
        {
            this.hueFactory = factory;
            this.eventAggregator = eventAggregator;
            this.logger = logger;
        }

        public Task StartAsync()
        {
            logger.LogInformation("Starting MasterBedroomObserver");

            this.eventAggregator.GetEvent<ResidentsStatusEvent>()
               .Where(ev => ev.Status == ResidentsStatus.GoingToSleep)
               .Subscribe(async ev => await OnGoingToSleep());

            return Task.CompletedTask;
        }

        private async Task OnGoingToSleep()
        {
            try
            {
                var client = await hueFactory.GetInstance();
                await client.SwitchOnOffAsync(true, "Bedroom");
            }
            catch(Exception ex)
            {
                logger.LogCritical(ex, "Unable to switch on lights for going to sleep");
            }
        }

        public Task StopAsync()
        {
            logger.LogInformation("Stopping MasterBedroomObserver");
            return Task.CompletedTask;
        }
    }
}
