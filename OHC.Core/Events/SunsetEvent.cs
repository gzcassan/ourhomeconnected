using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Events
{
    public class SunsetEvent : IOHCEvent
    {
        public DateTimeOffset SunsetTime { get; private set; }

        public SunsetEvent(DateTimeOffset time)
        {
            SunsetTime = time;
        }

        public string ToEventDescription()
        {
            return $"Sunset is starting at: {SunsetTime.ToString()}";
        }
    }
}
