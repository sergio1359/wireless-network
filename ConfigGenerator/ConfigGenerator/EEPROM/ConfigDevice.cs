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
        Byte DeviceType;
        Byte FirmVersion;
        UInt16 DeviceID;

        //NetworkConfig
        NetworkConfig Network;

        //PortConfig
        List<Port> Ports;

        //Events
        List<PortEvent> PortEvents;
        List<TimeEvent> TimeEvents;

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
