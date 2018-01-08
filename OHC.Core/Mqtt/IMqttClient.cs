using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.Mqtt
{
    public interface IMqttClient
    {
        Task ConnectAsync(MqttSettings settings);
        Task DisconnectAsync();
        bool IsConnected { get; }

        Task SubscribeTopicAsync(string topic);
        IObservable<(string, string)> OnDataReceived { get; }
        Task PublishAsync(string topic, string payload);
    }
}
