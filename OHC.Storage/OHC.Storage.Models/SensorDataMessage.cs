using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Storage.Models
{
    public class SensorDataMessage : ITableEntity //TODO: Consider this solution: http://www.amithegde.com/2015/06/decoupling-tableentity-while-using-azure-storage.html
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int NodeId { get; set; }
        public int SensorId { get; set; }
        public string SensorDataType { get; set; }
        public string DateTimeOffset { get; set; }
        public string Data { get; set; }

        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            TableEntity.ReadUserObject(this, properties, operationContext);
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return TableEntity.WriteUserObject(this, operationContext);
        }
    }
}
