#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Light : HomeDevice
    {
        public bool IsOn { get; set; }

        public Light()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchLOW;
        }
    }
}
