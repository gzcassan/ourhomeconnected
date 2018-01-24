using OHC.Core.Events;
using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.AreaObservers
{
    public interface ILivingroomObserver : IAreaObserver
    {
        Double GetCurrentTemperature();
        Task SwitchOnLightForSunset();
        Task SwitchOffLights();
        Task StoreSensorDataAsync(MySensorsDataMessage message);
    }
}
