﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class Switch: HomeDevice
    {
        public bool Open { get; set; }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}