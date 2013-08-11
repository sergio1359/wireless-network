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
    public class Button: HomeDevice
    {
        public Button() : base() 
        {
            base.ConnectorCapable = ConnectorTypes.LogicInput;
        }

    }
}
