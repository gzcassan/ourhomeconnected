using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.MySensorsGateway
{
    public class MySensorsCommand : MySensorsMessage
    {

        //public MySensorsCommand() { }

        //public static MySensorsCommand Create(string topic, string payload)
        //{
        //    var fields = topic.Split('/');
        //    if (fields.Length < 6)
        //        throw new ArgumentException("Topic does not seem to be a valid topic");

        //    var message = new MySensorsCommand()
        //    {
        //        DateTime = DateTimeOffset.Now,
        //        NodeId = Int32.Parse(fields[1]),
        //        SensorId = Int32.Parse(fields[2]),
        //        MessageType = (MessageType)Int32.Parse(fields[3]),
        //        Ack = fields[4] == "1",
        //        SubType = Int32.Parse(fields[5]),
        //        Payload = payload
        //    };

        //    return message;
        //}

        public override string ToEventDescription()
        {
            throw new NotImplementedException();
        }

        public override string ToMySensorsTopicString()
        {
            throw new NotImplementedException();
        }
    }
}
