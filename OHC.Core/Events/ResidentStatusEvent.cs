using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace OHC.Core.Events
{
    public class ResidentStatusEvent : IOHCEvent, IWhenDoMessage
    {
        public ResidentsStatus Status { get; set; }

        public ResidentStatusEvent() {}

        public ResidentStatusEvent(ResidentsStatus status)
        {
            Status = status;
        }

        public string ToEventDescription()
        {
            return $"Resident status set to {Status.ToString()}";
        }
    }
}
