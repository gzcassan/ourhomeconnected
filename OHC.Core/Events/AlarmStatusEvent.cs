using OHC.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.Events
{
    public class AlarmStatusEvent : IOhcEvent
    {
        public AlarmStatus AlarmStatus { get; set; }

        public AlarmStatusEvent(bool enabled)
        {
            AlarmStatus = AlarmStatus.Enabled;
        }

        public string ToEventDescription()
        {
            return $"Alarm status is now: {AlarmStatus.ToString()}";
        }
    }
}
