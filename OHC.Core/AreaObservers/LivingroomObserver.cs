using OHC.Core.Infrastructure;
using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using OHC.Core.Settings;
using OHC.Storage.Interfaces;
using OHC.Core.MySensors;
using Hangfire;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using System.Threading;
using OHC.Drivers.PhilipsHue;

namespace OHC.Core.AreaObservers
{
    public class LivingroomObserver : ILivingroomObserver
    {
        private IEventAggregator eventAggregator;
        private ISensorDataService sensorDataService;
        private ILogger<LivingroomObserver> logger;
        private LivingroomSettings livingroomSettings;
        private IPhilipsHueFactory philipsHueFactory;
        private AlarmStatus AlarmStatus;
        private int[] areaNodes = { 0 };
        ReaderWriterLock temperatureLock = new ReaderWriterLock();

        public double CurrentTemperature { get; private set; }
        public double CurrentHumidity { get; private set; }



        public LivingroomObserver(IPhilipsHueFactory philipsHueFactory, IEventAggregator eventAggregator, ILogger<LivingroomObserver> logger, 
            ISensorDataService sensorDataService, IOptions<LivingroomSettings> livingroomSettings)
        {
            this.eventAggregator = eventAggregator;
            this.philipsHueFactory = philipsHueFactory;
            this.logger = logger;
            this.sensorDataService = sensorDataService;
            this.livingroomSettings = livingroomSettings.Value;
        }

        public Task StartAsync()
        {
            logger.LogInformation("Starting LivingroomObserver service");
            this.eventAggregator.GetEvent<AlarmStatusEvent>().Subscribe((evt) => this.OnAlarmStatusChanged(evt));
            this.eventAggregator.GetEvent<SunsetEvent>().Subscribe((evt) => this.OnSunsetStart(evt));
            this.eventAggregator.GetEvent<MySensorsDataMessage>()
                .Where(m => m.SensorDataType == SensorDataType.V_TEMP)
                .Subscribe(message => OnTemperatureUpdate(message));
            this.eventAggregator.GetEvent<MySensorsDataMessage>()
                .Where(m => areaNodes.Contains(m.NodeId))
                .Subscribe(message => OnSensorDataReceived(message));
            this.eventAggregator.GetEvent<HomeStatusEvent>()
                .Where(ev => ev.Status == HomeStatus.GoingToSleep)
                .Subscribe(ev => ScheduleOnGoingToSleep());

            RecurringJob.AddOrUpdate<LivingroomObserver>((lo) => lo.SwitchOffLights(), "30 00 * * *", TimeZoneInfo.Local);

            return Task.CompletedTask;
        }

        public void ScheduleOnGoingToSleep()
        {
            logger.LogInformation("Going to sleep, switching off lights in {minutes} minutes", livingroomSettings.GoingToSleepDelayInMinutes);
            BackgroundJob.Schedule<ILivingroomObserver>((lo) => lo.SwitchOffLights(),
                DateTimeOffset.Now.Add(TimeSpan.FromMinutes(livingroomSettings.GoingToSleepDelayInMinutes)));
        }

        public async Task SwitchOffLights()
        {
            try
            {
                var hueClient = await philipsHueFactory.GetInstance();
                await hueClient.SwitchOnOffAsync(false, "Living room");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Unable to switch off lights");
                //send notification
            }
        }

        public void OnTemperatureUpdate(MySensorsDataMessage message)
        {
            temperatureLock.AcquireWriterLock(1000);
            CurrentTemperature = Double.Parse(message.Payload);
            temperatureLock.ReleaseWriterLock();
        }

        public Double GetCurrentTemperature()
        {
            temperatureLock.AcquireReaderLock(1000);
            var temp = CurrentTemperature;
            temperatureLock.ReleaseReaderLock();
            return temp;
        }

        public Task StopAsync()
        {
            logger.LogInformation("Stopping LivingroomObserver service");
            return Task.CompletedTask;
        }

        private void OnSunsetStart(SunsetEvent evt)
        {
            logger.LogInformation("Sunset (at {time}) started event received, switching on lights", evt.SunsetTime.ToString());
            BackgroundJob.Schedule<ILivingroomObserver>((lo) => lo.SwitchOnLightForSunset(), TimeSpan.FromMinutes(livingroomSettings.SunsetLightOnDelayInMinutes));
        }

        public async Task SwitchOnLightForSunset()
        {
            if (AlarmStatus != AlarmStatus.Activated)
            {
                var client = await philipsHueFactory.GetInstance();
                try
                {
                    await client.SwitchToSceneAsync(livingroomSettings.SunsetScene, "Living room");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unable to switch to sunset scene {scene}", livingroomSettings.SunsetScene);
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
