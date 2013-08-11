#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace SmartHome.DataLayer.HomeDevices
{
    public class PresenceSensor: HomeDevice
    {
        public byte Sensibility { get; set; }
        public const int DEFAULT_SENSIBILITY = 10;

        public PresenceSensor() : base() 
        {
            base.Operations = new List<Operation>();
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
