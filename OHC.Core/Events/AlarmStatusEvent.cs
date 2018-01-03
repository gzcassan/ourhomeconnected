using OHC.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.Events
{
    public class AlarmStatusEvent : IOhcEvent
    {
        public bool AlarmEnabled { get; set; }

        public AlarmStatusEvent(bool enabled)
        {
            AlarmEnabled = enabled;
        }

        public string ToEventDescription()
        {
            var status = AlarmEnabled ? "enabled" : "disabled";
            return $"Alarm status is now: {status}";
        }
    }
}
