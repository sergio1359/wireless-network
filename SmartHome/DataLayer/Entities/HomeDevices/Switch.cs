#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace SmartHome.DataLayer.HomeDevices
{
    public class Switch: HomeDevice
    {
        public bool Open { get; set; }

        public Switch() : base() 
        {
            base.Operations = new List<Operation>();
            base.ConnectorCapable = ConnectorType.LogicInput;
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
