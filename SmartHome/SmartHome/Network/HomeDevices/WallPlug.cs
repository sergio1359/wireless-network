﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class WallPlug: Light
    {
        public enum WallPlugType
        {
            AirFreshener,
            LightPoint,
            Matamosquitos,
            Fan,
            Heater,
            Speaker,
            Other,
            None,
        }
        
        public WallPlugType Type { get; set; }

        public WallPlug(string name) : base(name) { }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
