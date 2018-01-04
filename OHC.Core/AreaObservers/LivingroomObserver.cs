using OHC.Core.Infrastructure;
using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using System.Linq;
using OHC.Core.Settings;

namespace OHC.Core.AreaObservers
{
    public class LivingroomObserver : ILivingroomObserver
    {
        private IEventAggregator eventAggregator;
        private ILogger<LivingroomObserver> logger;
        private PhilipsHueSettings hueSettings;
        private bool alarmEnabled;
        private bool alarmActivated;

        public LivingroomObserver(PhilipsHueSettings hueSettings, IEventAggregator eventAggregator, ILogger<LivingroomObserver> logger)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.hueSettings = hueSettings;

            this.eventAggregator.GetEvent<AlarmStatusEvent>().Subscribe((evt) => this.OnAlarmStatusChanged(evt));
            this.eventAggregator.GetEvent<SunsetEvent>().Subscribe(async (evt) => await this.OnSunsetStart(evt));
        }

        private async Task OnSunsetStart(SunsetEvent evt)
        {
            if (!alarmActivated)
            {
                logger.LogDebug("Sunset (at {time}) started event received, switching on lights", evt.SunsetTime.ToString());

                try
                {
                    var client = new LocalHueClient(hueSettings.Host);
                    client.Initialize(hueSettings.Key);

                    var scenes = await client.GetScenesAsync();
                    var sunset = scenes.Single(x => x.Name == hueSettings.SunsetScene);

                    var groups = await client.GetGroupsAsync();
                    var group = groups.Single(x => x.Name == "Living room");

                    var result = await client.RecallSceneAsync(sunset.Id, group.Id);

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
                    logger.LogError(ex, "Unable to switch on lights due to error");
                }
            }
            else
            {
                logger.LogWarning("Alarm activated, can't switch on lights.");
            }
        }

        private void OnAlarmStatusChanged(AlarmStatusEvent ev)
        {
            this.alarmEnabled = ev.AlarmEnabled;
        }
    }
}
