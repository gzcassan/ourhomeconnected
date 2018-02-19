using OHC.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.Events
{
    public class AlarmStatusEvent : IOHCEvent
    {
        public AlarmStatus AlarmStatus { get; set; }

        public AlarmStatusEvent(AlarmStatus status)
        {
            AlarmStatus = status;
        }

        public string ToEventDescription()
        {
            return $"Alarm status is now: {AlarmStatus.ToString()}";
        }
    }
}
