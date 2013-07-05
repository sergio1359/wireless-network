using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class Light: HomeDevice
    {
        public bool TurnOn { get; set; }

        public void Off() { }
        public void On() { }
        public void OnTime(DateTime time) { }
        public void Switch() { }

        public Light(string Name)
        {
            base.ConnectorType = Network.ConnectorType.SwitchLOW;
            base.HomeDeviceType = Network.HomeDeviceType.Light;
            base.Name = Name;
            base.Operations = new List<Operation>();
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
