using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Settings
{
    public class SchedulerSettings
    {
        public string TimezoneId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public TimeSpan NightTime { get; set; }
        public TimeSpan BedTime { get; set; }
    }
}
