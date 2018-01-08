﻿using MQTTnet;
using MQTTnet.ManagedClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using MQTTnet.Client;
using System.Diagnostics;
using MQTTnet.Diagnostics;

namespace OHC.MqttService
{
    public class MqttClient : IMqttClient//, IHostedServer
    {
        private IManagedMqttClient client;
        int test = 0;

        public MqttClient()
        {
            MqttNetGlobalLogger.LogMessagePublished += (s, e) =>
            {
                var trace = $">> [{e.TraceMessage.Timestamp:O}] [{e.TraceMessage.ThreadId}] [{e.TraceMessage.Source}] [{e.TraceMessage.Level}]: {e.TraceMessage.Message}";
                if (e.TraceMessage.Exception != null)
                {
                    trace += Environment.NewLine + e.TraceMessage.Exception.ToString();
                }
                Debug.WriteLine(trace);
            };
        }

        private void Client_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            test++;
            Debug.WriteLine(e.ApplicationMessage.Topic);
        }

        public Task SubscribeTopicAsync(string topic)
        {
            client = new MqttFactory().CreateManagedMqttClient();
            client.ApplicationMessageReceived += Client_ApplicationMessageReceived;
            return client.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
        }

        public bool IsConnected
        {
            get
            {
                return client.IsConnected;
            }
        }

        //public IObservable<(string,string)> OnDataReceived
        //{
        //    get
        //    {
        //        return Observable
        //            .FromEventPattern<MqttApplicationMessageReceivedEventArgs>(
        //            m => client.ApplicationMessageReceived += m,
        //            m => client.ApplicationMessageReceived -= m)
        //            .Select(x => 
        //                (Topic: x.EventArgs.ApplicationMessage.Topic, Payload: System.Text.Encoding.Default.GetString(x.EventArgs.ApplicationMessage.Payload)));
        //    }
        //}

        public Task PublishAsync(string topic, string payload)
        {
            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .WithPayload(payload)
                .Build();

            return client.PublishAsync(msg);
        }

        public async Task ConnectAsync(MqttSettings settings)
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(settings.ClientId)
                    .WithCredentials(settings.Username, settings.Password)
                    .WithTcpServer(settings.Host)
                    .Build())
                .Build();

            await client.StartAsync(options);
        }

        public Task DisconnectAsync()
        {
            return client.StopAsync();
        }



        
    }
}
