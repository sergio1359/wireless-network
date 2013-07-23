﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class Button: HomeDevice
    {
        public Button(string Name)
        {
            base.Name = Name;
            base.Operations = new List<Operation>();
        }
    
        public void Push()
        {
            
        }
    }
}