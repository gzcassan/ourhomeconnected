using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Drivers.PhilipsHue
{
    public interface IPhilipsHueFactory
    {
        Task<IPhilipsHueClient> GetInstanceAsync();
    }
}
