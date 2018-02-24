using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Drivers.PhilipsHue
{
    public class PhilipsHueFactory : IPhilipsHueFactory
    {
        PhilipsHueSettings settings;
        ILoggerFactory loggerFactory;

        public PhilipsHueFactory(PhilipsHueSettings settings, ILoggerFactory loggerFactory)
        {
            this.settings = settings;
            this.loggerFactory = loggerFactory;
        }

        public async Task<IPhilipsHueClient> GetInstanceAsync()
        {
            var client = new PhilipsHueClient(settings, loggerFactory.CreateLogger<PhilipsHueClient>());
            await client.InitializeAsync();
            return client;
        }
    }
}
