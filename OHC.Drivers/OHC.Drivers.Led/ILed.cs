using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Drivers.Led
{
    public interface ILed
    {
        Task Blink(int duration);
        Task Off();
        Task On();
        Task Toggle();
    }
}
