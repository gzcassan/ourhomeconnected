using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.RoomManagers
{
    public interface IBathroomManager
    {
        Task<bool> OnHumidityDataReceived(MySensorsDataMessage data);
    }
}
