using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class Light: HomeDevice
    {
        public bool IsOn { get; set; }

        public void On()
        {
            LogicWrite(LogicWriteValues.Set, 0);
        }

        public void Off()
        {
            LogicWrite(LogicWriteValues.Clear, 0);
        }

        public void OnTime(byte seconds)
        {
            LogicWrite(LogicWriteValues.Set, seconds);
        }

        public void Switch()
        {
            LogicSwitch(0);
        }

        public Light(string name): base(name)
        {
            base.Operations = new List<Operation>();
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
