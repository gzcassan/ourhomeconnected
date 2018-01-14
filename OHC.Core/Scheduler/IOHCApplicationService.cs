﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Scheduler
{
    public interface IOHCApplicationService
    {
        void CreateScheduledSunsetEventJob(DateTimeOffset date);
        void TriggerSunsetEvent(DateTimeOffset sunsetTime);
        void CreateRecurringSunsetEventJobForToday();
    }
}