using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace OHC.Core.Events
{
    public enum TimeEventType { Sunset, Sunrise, BedTime, Night }

    public class TimeEvent : IWhenDoMessageContext
    {
        public TimeSpan Time { get; private set; }
        public TimeEventType Type { get; set; }

        public TimeEvent(TimeEventType type, TimeSpan time)
        {
            this.Type = type;
            this.Time = time;
        }
    }
}
