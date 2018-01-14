using Microsoft.Extensions.Logging;
using OHC.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.AreaObservers
{
    public class HomeObserver : IHomeObserver
    {
        ILogger<HomeObserver> logger;
        IEventAggregator eventAggregator;

        public HomeObserver(IEventAggregator eventAggregator, ILogger<HomeObserver> logger)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
        }
        public Task StartAsync()
        {
            logger.LogInformation("Starting HomeObserver");
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            logger.LogInformation("Stopping HomeObserver");
            return Task.CompletedTask;

        }
    }
}
