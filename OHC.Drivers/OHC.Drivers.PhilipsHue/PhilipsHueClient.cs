using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHC.Drivers.PhilipsHue
{
    public class PhilipsHueClient : IPhilipsHueClient
    {
        ILocalHueClient client;
        PhilipsHueSettings settings;
        ILogger<PhilipsHueClient> logger;
        IReadOnlyCollection<Group> groups;

        internal PhilipsHueClient(PhilipsHueSettings hueSettings, ILogger<PhilipsHueClient> logger)
        {
            this.settings = hueSettings;
            this.logger = logger;
            client = new LocalHueClient(hueSettings.Host);
        }

        public async Task Initialize()
        {
            client.Initialize(settings.Key);
            groups = await client.GetGroupsAsync();
        }

        public async Task SwitchOnOffAsync(bool on, string area)
        {
            try
            {                
                var group = groups.Single(x => x.Name == area);

                var command = new LightCommand() { On = on };
                var result = await client.SendGroupCommandAsync(command, group.Id);

                if (!result.HasErrors())
                {
                    logger.LogInformation("Succesfully switched on lights.");
                }
                else
                {
                    logger.LogError("Unable to switch on lights", result.Errors.FirstOrDefault().Error.Description);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to switch OFF lights due to exception");
            }

        }

        public async Task SwitchToSceneAsync(string scene, string area)
        {
                var scenes = await client.GetScenesAsync();
                var sunset = scenes.Single(x => x.Name == scene);

                var group = groups.Single(x => x.Name == area);

                var result = await client.RecallSceneAsync(sunset.Id, group.Id);

                if (!result.HasErrors())
                {
                    logger.LogInformation("Succesfully switched on lights.");
                }
                else
                {
                    logger.LogError("Unable to switch to scene {scene}, error: {error}", scene, result.Errors.FirstOrDefault().Error.Description);
                }
        }
    }
}
