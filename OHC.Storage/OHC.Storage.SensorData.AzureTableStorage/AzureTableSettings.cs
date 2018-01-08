using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Storage.SensorData.AzureTableStorage
{
    public class AzureTableSettings
    {
        public string StorageAccount { get; set; }
        public string StorageKey { get; set; }
        public string TableName { get; set; }
    }
}
