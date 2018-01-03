using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.Core.MySensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Tests.MySensors
{
    [TestClass]
    public class MySensorsPresentationMessageTest
    {
        [TestMethod]
        public void CreateMySensorsPresentationMessageTest()
        {
            var nodeId = 0;
            var sensorId = 1;
            var sensorType = SensorType.S_BARO;
            var ack = true;
            var payload = "1234";

            var message = new MySensorsPresentationMessage(nodeId, sensorId, sensorType, ack, payload);

            Assert.AreEqual(nodeId, message.NodeId);
            Assert.AreEqual(sensorId, message.SensorId);
            Assert.AreEqual(sensorType, message.SensorType);
            Assert.AreEqual(ack, message.Ack);
            Assert.AreEqual(payload, message.Payload);
        }
    }
}
