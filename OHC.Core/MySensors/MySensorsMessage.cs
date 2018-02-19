using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.MySensors
{
    public abstract class MySensorsMessage : IOHCEvent
    {
        public int NodeId { get; set; }
        public int SensorId { get; set; }
        public bool Ack { get; set; }
        public string Payload { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }

        public abstract string ToEventDescription();

        public abstract string ToMySensorsTopicString();
        //{
        //    return $"{NodeId};{SensorId};{(int)MessageType};{(Ack ? "1" : "0")};{SubType};{Payload}\n";
        //}
    }
}
