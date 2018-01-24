using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Events
{
    public class HomeStatusEvent : IOhcEvent
    {
        public HomeStatus Status { get; set; }

        public HomeStatusEvent() {}

        public HomeStatusEvent(HomeStatus status)
        {
            Status = status;
        }

        public string ToEventDescription()
        {
            return $"Home status set to {Status.ToString()}";
        }
    }
}
