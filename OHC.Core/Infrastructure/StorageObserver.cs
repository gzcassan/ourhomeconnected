using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OHC.Core.Infrastructure;
using Hangfire;
using OHC.Storage.Interfaces;

namespace OHC.Core.Infrastructure
{
    public class StorageObserver : IHostedService, IStorageObserver
    {
        ILogger<StorageObserver> logger;
        IEventAggregator eventAggregator;
        ISensorDataService sensorDataService;

        public StorageObserver(ILogger<StorageObserver> logger, IEventAggregator eventAggregator, ISensorDataService sensorDataService)
        {
            this.sensorDataService = sensorDataService;
            this.logger = logger;
            this.eventAggregator = eventAggregator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Starting StorageObserver");
            eventAggregator.GetEvent<MySensorsDataMessage>().Subscribe(message => OnSensorDataReceived(message));
            return Task.CompletedTask;
        }

        public void OnSensorDataReceived(MySensorsDataMessage message)
        {
            BackgroundJob.Enqueue<IStorageObserver>((sm) => sm.StoreSensorData(message));
        }

        public void StoreSensorData(MySensorsDataMessage message)
        {
            try
            {
                sensorDataService.AddSensorDataReading(message.ToStorage());
            }
            catch (Exception ex)
            {
                logger.LogError("Unable to store SensorData in cloud", message.ToEventDescription(), ex);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Stopping StorageObserver");
            return Task.CompletedTask;
        }
    }
}
