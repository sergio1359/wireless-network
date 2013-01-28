using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROM
{
    class ConfigDevice
    {
        //Device Info
        public Byte DeviceType { get; set; }
        public UInt16 DeviceID { get; set; }

        //NetworkConfig
        public NetworkConfig Network { get; set; }

        //PortConfig
        public List<Port> Ports { get; set; }

        //Events
        public List<PortEvent> PortEvents { get; set; }
        public List<TimeEvent> TimeEvents { get; set; }

        public ConfigDevice(Byte device)
        {
            //vamos a crear la EEPROM y todos los elementos por defecto
        }

        public String CheckConsistant()
        {

        }


        public bool IsConsistant()
        {

        }

    }
}
