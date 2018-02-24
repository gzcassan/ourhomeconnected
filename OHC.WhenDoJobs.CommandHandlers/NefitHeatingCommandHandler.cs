using OHC.Drivers.NefitEasy;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;

namespace OHC.WhenDoJobs.CommandHandlers
{
    public class NefitHeatingCommandHandler : IWhenDoCommandHandler
    {
        private INefitEasyClient client;

        public NefitHeatingCommandHandler(INefitEasyClient client)
        {
            this.client = client;
        }

        public void SetTemperatureAsync(double temperature)
        {
            client.SetScheduleOverruleTemp(temperature);
        }
    }
}
