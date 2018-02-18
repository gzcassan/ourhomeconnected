using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Events
{
    public class ResidentsStatusEvent : IOhcEvent
    {
        public ResidentsStatus Status { get; set; }

        public ResidentsStatusEvent() {}

        public ResidentsStatusEvent(ResidentsStatus status)
        {
            Status = status;
        }

        public string ToEventDescription()
        {
            return $"Home status set to {Status.ToString()}";
        }
    }
}
