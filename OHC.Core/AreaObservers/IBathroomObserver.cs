using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.AreaObservers
{
    public interface IBathroomObserver : IAreaObserver
    {
        Task<bool> OnHumidityDataReceived(MySensorsDataMessage data);
    }
}
