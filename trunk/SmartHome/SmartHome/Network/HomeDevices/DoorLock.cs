using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class DoorLock : HomeDevice
    {
        public const int DEFAULT_OPEN_TIME = 1; //1 segundo

        public int OpenTime { get; set; }

        public void OpenDoor()
        {

        }
    }
}
