using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Enums
{
    public enum HomeDeviceType : byte
    {
        User = 0x00,
        Central = 0x01,
        Buttom = 0x02,
        Switch = 0x03,
        WallPlug = 0x04,
        Light = 0x05,
        Dimmable = 0x06,

        PresenceSensor = 0x10,
        PowerSensor = 0x11,
        TemperatureSensor = 0x12,
        HumiditySensor = 0x13,
        LightSensor = 0x14,

        DoorLock = 0x20,
        RGBLight = 0x21,
    }
}
