using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Infrastructure
{
    public interface IStorageObserver
    {
        void StoreSensorData(MySensorsDataMessage message);
        void OnSensorDataReceived(MySensorsDataMessage message);
    }
}
