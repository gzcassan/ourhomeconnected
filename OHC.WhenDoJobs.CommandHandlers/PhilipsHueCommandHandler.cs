using OHC.Drivers.PhilipsHue;
using System;
using System.Threading.Tasks;

namespace OHC.WhenDoJobs.CommandHandlers
{
    public class PhilipsHueCommandHandler
    {
        private IPhilipsHueClient client;

        public PhilipsHueCommandHandler(IPhilipsHueClient client)
        {
            this.client = client;
        }

        public async Task SwitchOnAsync(string area)
        {
            await client.SwitchOnOffAsync(true, area);
        }

        public async Task SwitchOffAsync(string area)
        {
            await client.SwitchOnOffAsync(false, area);
        }

        public async Task SwitchOnScene(string scene, string area)
        {
            await client.SwitchToSceneAsync(scene, area);
        }
    }
}
