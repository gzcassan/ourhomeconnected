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

namespace OHC.Core.AreaObservers
{
    public class BathroomObserver : IBathroomObserver
    { 
        private IEventAggregator eventAggregator;
        private ILogger<BathroomObserver> logger;
        private static readonly int[] roomNodes = { 0 };

        public BathroomObserver(IEventAggregator eventAggregator, ILogger<BathroomObserver> logger)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;

            /*
            eventAggregator.GetEvent<MySensorsDataMessage>()
                .Where(m => Array.Exists<int>(roomNodes, x => x == m.NodeId))
                .Where(m => m.SensorDataType == SensorDataType.V_HUM)
                .Subscribe(async message => await OnHumidityDataReceived(message));

            eventAggregator.GetEvent<MySensorsDataMessage>()
                .Where(m => Array.Exists<int>(roomNodes, x => x == m.NodeId))
                .Where(m => m.SensorDataType == SensorDataType.V_TEMP)
                .Subscribe(async message => await OnTempDataReceived(message));

            eventAggregator.GetEvent<MySensorsPresentationMessage>()
                .Where(m => Array.Exists<int>(roomNodes, x => x == m.NodeId))
                .Subscribe(async message => await OnPresentationReceived(message));
            */
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

    }
}
