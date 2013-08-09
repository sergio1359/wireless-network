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
    public class Light: HomeDevice
    {
        public bool IsOn { get; set; }

        public Light() : base() 
        {
            base.ConnectorCapable = ConnectorType.SwitchLOW;
        }

        public OperationMessage On()
        {
            return OperationMessage.LogicWrite(Id, LogicWriteValues.Set, 0);
        }

        public OperationMessage Off()
        {
            return OperationMessage.LogicWrite(Id, LogicWriteValues.Clear, 0);
        }

        public OperationMessage OnTime(byte seconds)
        {
            return OperationMessage.LogicWrite(Id, LogicWriteValues.Set, seconds);
        }

        public OperationMessage Switch()
        {
            return OperationMessage.LogicSwitch(Id, 0);
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
