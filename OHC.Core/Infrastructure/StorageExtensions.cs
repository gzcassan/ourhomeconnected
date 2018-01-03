using OHC.Core.MySensors;
using OHC.Storage.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OHC.Core.Infrastructure
{
    public static class StorageExtensions
    {
        public static SensorDataReading ToStorage(this MySensorsDataMessage message)
        {

            var sensorData = new SensorDataReading()
            {
                PartitionKey = $"{message.NodeId}-{message.SensorId}-{message.SensorDataType.ToString()}",
                RowKey = message.DateTimeOffset.UtcDateTime.ToString("yyyyMMddHHmmssfffffff"),
                DateTimeOffset = message.DateTimeOffset.ToString("o", new CultureInfo("nl-NL")),
                NodeId = message.NodeId,
                SensorId = message.SensorId,
                SensorDataType = message.SensorDataType.ToString(),
                Data = message.Payload
            };

            return sensorData;
        }
    }
}
