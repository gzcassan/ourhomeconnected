using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using OHC.Core.Infrastructure;
using OHC.Core.MySensors;
using OHC.Core.MySensorsGateway;
using System.Linq.Expressions;
using OHC.Storage;
using Microsoft.Extensions.Logging;
using OHC.Storage.Interfaces;

namespace OHC.Core.AreaObservers
{
    public class BathroomObserver : IBathroomObserver
    { 
        private IEventAggregator eventAggregator;
        private ISensorDataService sensorDataService;
        private ILogger<BathroomObserver> logger;
        private static readonly int[] roomNodes = { 0 };

        public BathroomObserver(IEventAggregator eventAggregator, ILogger<BathroomObserver> logger, ISensorDataService sensorDataService)
        {
            this.sensorDataService = sensorDataService;
            this.logger = logger;
            this.eventAggregator = eventAggregator;
        }

        private async Task OnPresentationReceived(MySensorsPresentationMessage message)
        {
            await Task.CompletedTask;
        }

        private async Task OnTempDataReceived(MySensorsDataMessage message)
        {
            await Task.CompletedTask;
        }

        public async Task<bool> OnHumidityDataReceived(MySensorsDataMessage data)
        {
            return await Task.FromResult(true);
        }

        public Task StartAsync()
        {
            logger.LogInformation("Starting BathroomObserver service");
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            logger.LogInformation("Stopping BathroomObserver service");
            return Task.CompletedTask;
        }
    }
}
