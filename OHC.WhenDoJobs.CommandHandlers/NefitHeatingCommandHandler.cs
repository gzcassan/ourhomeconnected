using OHC.Drivers.NefitEasy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public Task SetTemperatureAsync(double temperature)
        {
            return client.SetScheduleOverruleTempAsync(temperature);
        }
    }
}
