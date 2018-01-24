//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace OHC.Core
//{
//    interface IArea
//    {
//        Action OnTemperatureUpdate(double temp);
//    }

//    class Area : IArea
//    {
//        public double CurrentTemperature { get; set; }

//        delegate Tempupdate(int node, double temp);

//        public void OnTemperatureUpdate(int node, double temp)
//        {
//            this.Tempupdate(temp);
//        }
//    }

//    class Manager
//    {
//        public IHeatingManager HeatingManager { get; set; }
//        private void tempupdate()
//        {
//            var area = new Area();
//            area.Tempupdate = (int node, double temp) =>
//            {
//                if(temp > 20.0)
//                    HeatingManager.SwitchOff();
//            }
//            }
//    }
//}
