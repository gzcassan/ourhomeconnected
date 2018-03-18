using Microsoft.Extensions.Logging;
using OHC.Core.Interfaces;
using OHC.Core.MySensors;
using OHC.Storage.Interfaces;
using OHC.Storage.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace OHC.WhenDoJobs.CommandHandlers
{
    public class SensorMessagePersistenceCommandHandler : IWhenDoCommandHandler
    {
        private ISensorDataService sensorDataService;
        private ILogger<SensorMessagePersistenceCommandHandler> logger;

        public SensorMessagePersistenceCommandHandler(ISensorDataService sensorDataService, ILogger<SensorMessagePersistenceCommandHandler> logger)
        {
            this.sensorDataService = sensorDataService;
            this.logger = logger;
        }

        public async Task SaveAsync(IWhenDoMessage context)
        {
            try
            {
                var message = (MySensorsDataMessage)context;
                await sensorDataService.SaveAsync(message.ToStorage());
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Error storing sensor data message in storage {sensorDataService.Id}", context);
            }
        }
    }

    public static class StorageExtensions
    {
        public static SensorDataMessage ToStorage(this MySensorsDataMessage message)
        {

            var sensorData = new SensorDataMessage()
            {
                PartitionKey = $"{message.NodeId}-{message.SensorId}-{message.SensorDataType.ToString()}",
                RowKey = message.DateTimeOffset.UtcDateTime.ToString("yyyyMMddHHmmssfffffff"),
                DateTimeOffset = message.DateTimeOffset.ToString("o", new CultureInfo("nl-NL")), //TODO: read localisation info from setting
                NodeId = message.NodeId,
                SensorId = message.SensorId,
                SensorDataType = message.SensorDataType.ToString(),
                Data = message.Payload
            };

            return sensorData;
        }
    }
}
