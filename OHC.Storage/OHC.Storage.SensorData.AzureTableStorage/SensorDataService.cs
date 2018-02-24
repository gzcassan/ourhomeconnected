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
        private readonly IAzureTableStorage<SensorDataMessage> repository;

        public string Id => "AzureTableStorage";

        public SensorDataService(IAzureTableStorage<SensorDataMessage> repository)
        {
            this.repository = repository;
        }

        public Task SaveAsync(SensorDataMessage message)
        {
            return repository.Insert(message);
        }

        public TObject GetCurrent<TType, TObject>()
        {
            throw new NotImplementedException();
        }
    }
}
