﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class TemperatureHumiditySensor : HomeDevice
    {

        public int CelciusTemperature { get; set; }
        public int Humidity { get; set; }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}