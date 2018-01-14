using OHC.Core.Infrastructure;
using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using System.Linq;
using OHC.Core.Settings;
using OHC.Storage.Interfaces;
using OHC.Core.MySensors;
using Hangfire;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace OHC.Core.AreaObservers
{
    public class LivingroomObserver : ILivingroomObserver
    {
        private IEventAggregator eventAggregator;
        private ISensorDataService sensorDataService;
        private ILogger<LivingroomObserver> logger;
        private PhilipsHueSettings hueSettings;
        private LivingroomSettings livingroomSettings;
        private AlarmStatus AlarmStatus;
        private int[] areaNodes = { 0 };



        public LivingroomObserver(IOptions<PhilipsHueSettings> hueSettings, IEventAggregator eventAggregator, ILogger<LivingroomObserver> logger, ISensorDataService sensorDataService, IOptions<LivingroomSettings> livingroomSettings)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.hueSettings = hueSettings.Value;
            this.sensorDataService = sensorDataService;
            this.livingroomSettings = livingroomSettings.Value;
        }

        public Task StartAsync()
        {
            logger.LogInformation("Starting LivingroomObserver service");
            this.eventAggregator.GetEvent<AlarmStatusEvent>().Subscribe((evt) => this.OnAlarmStatusChanged(evt));
            this.eventAggregator.GetEvent<SunsetEvent>().Subscribe((evt) => this.OnSunsetStart(evt));
            this.eventAggregator.GetEvent<MySensorsDataMessage>()
                .Where(m => areaNodes.Contains(m.NodeId)) 
                .Subscribe(message => OnSensorDataReceived(message));

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            logger.LogInformation("Stopping LivingroomObserver service");
            return Task.CompletedTask;
        }

        private void OnSunsetStart(SunsetEvent evt)
        {
            logger.LogDebug("Sunset (at {time}) started event received, switching on lights", evt.SunsetTime.ToString());
            BackgroundJob.Schedule<ILivingroomObserver>((lo) => lo.SwitchOnLightForSunset(), TimeSpan.FromMinutes(livingroomSettings.SunsetLightOnDelayInMinutes));
        }

        public async Task SwitchOnLightForSunset()
        { 
            if (AlarmStatus != AlarmStatus.Activated)
            {
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
                    logger.LogError(ex, "Unable to switch on lights due to exception");
                }
            }
            else
            {
                logger.LogWarning("Alarm activated, can't switch on lights.");
            }
        }

        private void OnAlarmStatusChanged(AlarmStatusEvent ev)
        {
            this.AlarmStatus = ev.AlarmStatus;
        }

        public void OnSensorDataReceived(MySensorsDataMessage message)
        {
            logger.LogDebug("OnSensorDataReceived is called");
            BackgroundJob.Enqueue<ILivingroomObserver>(sm => sm.StoreSensorDataAsync(message));
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task StoreSensorDataAsync(MySensorsDataMessage message)
        {
            try
            {
                await sensorDataService.SaveSensorDataReadingAsync(message.ToStorage());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to store SensorData in cloud", message.ToEventDescription());
                throw;
            }
        }
    }
}
