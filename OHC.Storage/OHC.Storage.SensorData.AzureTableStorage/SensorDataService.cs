using OHC.Storage.Interfaces;
using OHC.Storage.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Storage.SensorData.AzureTableStorage
{
    public class SensorDataService : ISensorDataService
    {
        private readonly IAzureTableStorage<SensorDataReading> repository;

        public SensorDataService(IAzureTableStorage<SensorDataReading> repository)
        {
            this.repository = repository;
        }

        public async Task SaveSensorDataReadingAsync(SensorDataReading item)
        {
                await this.repository.Insert(item);
        }
    }
}
