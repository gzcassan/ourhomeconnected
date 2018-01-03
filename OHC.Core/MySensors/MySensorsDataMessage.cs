using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.MySensors
{
    public class MySensorsDataMessage : MySensorsMessage
    {
        public SensorDataType SensorDataType { get; set; }

        public MySensorsDataMessage(int nodeId, int sensorId, SensorDataType sensorDataType, bool ack, string payload)
        {
            this.DateTimeOffset = DateTimeOffset.Now;
            this.NodeId = nodeId;
            this.SensorId = sensorId;
            this.SensorDataType = sensorDataType;
            this.Ack = ack;
            this.Payload = payload;
        }

        public override string ToEventDescription()
        {
            return $"Sensor data: NodeId {NodeId};SensorId {SensorId};Ack {(Ack ? "1" : "0")};SensorDataType {SensorDataType.ToString()};Payload {Payload}";
        }

        public override string ToMySensorsTopicString()
        {
            throw new NotImplementedException();
        }
    }
}
