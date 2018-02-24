using OHC.Storage.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OHC.Storage.Interfaces
{

    public interface ISensorDataService
    {
        string Id { get; }
        Task SaveAsync(SensorDataMessage message);
        TObject GetCurrent<TType, TObject>();
    }
}

