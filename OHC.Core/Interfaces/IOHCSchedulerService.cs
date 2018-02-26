using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Interfaces
{
    public interface IOHCSchedulerService
    {
        void CreateScheduledSunsetEventJob(DateTimeOffset date);
        void CreateSunsetEventJobForToday();
    }
}
