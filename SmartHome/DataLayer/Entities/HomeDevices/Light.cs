#region Using Statements
using SmartHome.Comunications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages;
#endregion

namespace SmartHome.DataLayer.HomeDevices
{
    public class Light: HomeDevice
    {
        public bool IsOn { get; set; }

        public Light() : base() 
        {
            base.ConnectorCapable = ConnectorType.SwitchLOW;
        }

        [OperationAttribute]
        public OperationMessage On()
        {
            return OperationMessage.LogicWrite(Id, LogicWriteValues.Set, 0);
        }

        [OperationAttribute]
        public OperationMessage Off()
        {
            return OperationMessage.LogicWrite(Id, LogicWriteValues.Clear, 0);
        }

        [OperationAttribute]
        public OperationMessage OnTime(byte seconds)
        {
            return OperationMessage.LogicWrite(Id, LogicWriteValues.Set, seconds);
        }

        [OperationAttribute]
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
