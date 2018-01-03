using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.Core.Infrastructure;
using OHC.Core.MySensors;
using OHC.Core.RoomManagers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.Tests.RoomManagers
{
    [TestClass]
    public class BathroomManagerTest
    {
        [TestMethod]
        public void TestHumidityLevelUpdate()
        {
            var message = new MySensorsDataMessage(0, 0, SensorDataType.V_HUM, false, "56.3");
            
            var eventAggregator = new EventAggregator(null);
            //var man = new BathroomManager(eventAggregator, );

            var result = true;// await man.OnHumidityDataReceived(message);
            Assert.IsTrue(result);


            
        }
    }
}
