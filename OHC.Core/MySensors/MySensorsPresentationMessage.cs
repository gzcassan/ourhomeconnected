using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.MySensors
{
    public class MySensorsPresentationMessage : MySensorsMessage
    {
        public SensorType SensorType { get; set; }

        public MySensorsPresentationMessage(int nodeId, int sensorId, SensorType sensorType, bool ack, string payload)
        {
            this.DateTimeOffset = DateTimeOffset.Now;
            this.NodeId = nodeId;
            this.SensorId = sensorId;
            this.SensorType = sensorType;
            this.Ack = ack;
            this.Payload = payload;
        }
        public override string ToEventDescription()
        {
            return $"Sensor presentation: NodeId {NodeId};SensorId {SensorId};Ack {(Ack ? "1" : "0")};SensorType {SensorType.ToString()};Payload {Payload}";
        }

        public override string ToMySensorsTopicString()
        {
            throw new NotImplementedException();
        }
    }
}
