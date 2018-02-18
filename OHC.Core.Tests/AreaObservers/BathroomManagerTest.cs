using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.Core.Infrastructure;
using OHC.Core.MySensors;
using OHC.Core.AreaObservers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.Tests.AreaObservers
{
    [TestClass]
    public class BathroomObserverTest
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

        [TestMethod]
        public void TestCurrentTemperature()
        {
            //var room = new LivingroomObserver(null, null, null, null, null);
            //Parallel.For(0, 100000, (i) => room.OnTemperatureUpdate(new MySensorsDataMessage(0, 0, SensorDataType.V_TEMP, false, "15.0")));
        }
    }
}
