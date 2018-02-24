using OHC.Drivers.PhilipsHue;
using System;
using System.Threading.Tasks;
using WhenDoJobs.Core.Interfaces;

namespace OHC.WhenDoJobs.CommandHandlers
{
    public class PhilipsHueCommandHandler : IWhenDoCommandHandler
    {
        private IPhilipsHueFactory factory;

        public PhilipsHueCommandHandler(IPhilipsHueFactory factory)
        {
            this.factory = factory;
        }

        public async Task SwitchOnAsync(string area)
        {
            var client = await factory.GetInstanceAsync();
            await client.SwitchOnOffAsync(true, area);
        }

        public async Task SwitchOffAsync(string area)
        {
            var client = await factory.GetInstanceAsync();
            await client.SwitchOnOffAsync(false, area);
        }

        public async Task SwitchOnScene(string scene, string area)
        {
            var client = await factory.GetInstanceAsync();
            await client.SwitchToSceneAsync(scene, area);
        }
    }
}
