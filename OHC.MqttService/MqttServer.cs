using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OHC.MqttService
{
    public class MqttServer : IHostedService
    {
        IMqttServer server;

        public MqttServer()
        {
            server = new MqttFactory().CreateMqttServer();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            MqttServerOptions opt = new MqttServerOptions();
            return server.StartAsync(opt);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return server.StopAsync();
        }
    }
}
