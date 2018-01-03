using OHC.Storage.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OHC.Storage.Interfaces
{

    public interface ISensorDataService
    {
        Task AddSensorDataReading(SensorDataReading item);
    }
}

