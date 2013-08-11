#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace SmartHome.DataLayer.HomeDevices
{
    public class HumiditySensor : HomeDevice
    {
        public int Humidity { get; set; }

        public HumiditySensor() : base() 
        {
            
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
