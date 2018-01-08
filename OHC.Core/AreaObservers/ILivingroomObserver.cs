using OHC.Core.Events;
using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.AreaObservers
{
    public interface ILivingroomObserver
    {
        Task SwitchOnLightForSunset();
        Task StoreSensorDataAsync(MySensorsDataMessage message);
    }
}
