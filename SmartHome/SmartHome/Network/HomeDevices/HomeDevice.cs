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

        public bool InUse
        {
            get
            {
                if (Connector != null)
                    return true;
                else
                    return false;
            }
        }

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

        public virtual void RefreshState()
        {

        }
    }
}
