#region Using Statements
using SmartHome.Comunications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages; 
#endregion

namespace SmartHome.Network.HomeDevices
{
    public class Button: HomeDevice
    {
        public Button() : base() 
        {
            base.ConnectorCapable = ConnectorType.LogicInput;
        }

    }
}
