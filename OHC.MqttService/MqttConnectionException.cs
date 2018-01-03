using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.MqttService
{
    public class MqttConnectionException : Exception
    {
        public MqttSettings MqttSettings { get; set; }

        public MqttConnectionException(string message, MqttSettings settings) : base(message)
        {
            this.MqttSettings = settings;
        }
    }
}
