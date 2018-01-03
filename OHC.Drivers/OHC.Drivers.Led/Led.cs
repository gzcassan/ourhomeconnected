using System;
using System.Threading.Tasks;

namespace OHC.Drivers.Led
{
    public class Led : ILed
    {
        public Task Blink(int duration)
        {
            throw new NotImplementedException();
        }

        public Task Off()
        {
            throw new NotImplementedException();
        }

        public Task On()
        {
            throw new NotImplementedException();
        }

        public Task Toggle()
        {
            throw new NotImplementedException();
        }
    }
}
