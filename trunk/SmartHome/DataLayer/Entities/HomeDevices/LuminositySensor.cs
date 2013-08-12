#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class LuminositySensor : HomeDevice
    {
        public byte Sensibility { get; set; }

        public LuminositySensor()
            : base()
        {

        }
    }
}
