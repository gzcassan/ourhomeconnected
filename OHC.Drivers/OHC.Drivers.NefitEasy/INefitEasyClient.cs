using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Drivers.NefitEasy
{
    public interface INefitEasyClient
    {
        Task SetScheduleOverruleTemp(double temp, int durationInMinutes = 0);
    }
}
