using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using OHC.MqttService;
using System.Threading;
using Microsoft.Extensions.Hosting;
using OHC.Core.Infrastructure;
using OHC.Core.MySensors;
using Microsoft.Extensions.Logging;

namespace OHC.Core.MySensorsGateway
{
    public class MySensorsGateway : IMySensorsGateway, IHostedService
    {
        private static string TOPIC = "#";
        //private static string TOPIC = "mysensors-out-test/#";

        private IMqttClient client;
        private MqttSettings mqttSettings;
        private IEventAggregator eventAggregator;
        private ILogger logger;

        public MySensorsGateway(IMqttClient client, IEventAggregator eventAggregator, ILogger<MySensorsGateway> logger, MqttSettings settings)
        {
            this.client = client;
            this.mqttSettings = settings;
            this.logger = logger;
            this.eventAggregator = eventAggregator;

            eventAggregator.GetEvent<MySensorsCommand>().Subscribe(async command => await SendMessage(command));

            client.OnDataReceived.Subscribe(x =>
            {
                CreateAndPublishMessage(x.Item1, x.Item2);
            });
        }

        private void CreateAndPublishMessage(string topic, string payload)
        {
            var fields = topic.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length < 6)
                throw new ArgumentException("Topic does not seem to be a valid topic");

            var nodeId = Int32.Parse(fields[1]);
            var sensorId = Int32.Parse(fields[2]);
            var messageType = (MessageType)Int32.Parse(fields[3]);
            var ack = fields[4] == "1";

            if(messageType == MessageType.C_SET || messageType == MessageType.C_REQ)
            {
                var type = Int32.Parse(fields[5]);
                var sensorDataType = (SensorDataType) type;
                var message = new MySensorsDataMessage(nodeId, sensorId, sensorDataType, ack, payload);
                eventAggregator.Publish<MySensorsDataMessage>(message);
            }
            else if(messageType == MessageType.C_PRESENTATION)
            {
                var sensorType = (SensorType)Int32.Parse(fields[5]);
                var message = new MySensorsPresentationMessage(nodeId, sensorId, sensorType, ack, payload);
                eventAggregator.Publish<MySensorsPresentationMessage>(message);
            }
        }

        public async Task SendMessage(MySensorsCommand command)
        {
            await client.PublishAsync(command.ToMySensorsTopicString(), command.Payload);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting MySensorsGateway");
            int attempts = 0;
            try
            {
                while (!client.IsConnected && attempts < 1) //TODO: Figure out why the mqttclient is not set to connected and if that works increase the nofAttempts
                {
                    attempts++;
                    await this.ConnectAsync();
                }

                //if (!client.IsConnected)
                //    throw new MqttConnectionException($"Unable to connect to MqttServer after {attempts} attempts", mqttSettings);

                await client.SubscribeTopicAsync(TOPIC);
                
            }
            catch(Exception ex)
            {
                logger.LogCritical("Unable to connect to MQTT", ex);
            }


            //TODO: Remove?
            if (cancellationToken.IsCancellationRequested)
                await StopAsync(cancellationToken);
            //OnSensorDataReceived.Subscribe(xs => Client_ApplicationMessageReceived(xs));
        }

        private async Task ConnectAsync()
        {
            logger.LogInformation("Connecting to MQTT");
            await client.ConnectAsync(mqttSettings);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping MySensorsGateway");
            return client.DisconnectAsync();
        }
    }
}
