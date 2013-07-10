using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;

namespace SmartHome.Network.HomeDevices
{
    public abstract class HomeDevice
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public Connector Connector { get; set; }

        public ConnectorType ConnectorType { get; set; }
        public HomeDeviceType HomeDeviceType { get; set; }
        public Position Position { get; set; }
        public List<Operation> Operations { get; set; }

        public static ushort incrementID = 0;

        public HomeDevice()
        {
            ID = incrementID;
            incrementID++;
        }

        public void Link(Connector Connector)
        {
            if (Connector.HomeDevice != null)
            {
                throw new Exception("Connector it's available for other HomeDevice, please unlink firts the other homedevice :D");
            }

            this.Connector.HomeDevice = this;
            this.Connector = Connector;
        }

        public void Unlink()
        {
            this.Connector.HomeDevice = null;
            this.Connector = null;
        }


        public virtual void RefreshState()
        {

        }
    }
}
