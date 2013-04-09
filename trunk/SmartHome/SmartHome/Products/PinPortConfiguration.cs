using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Products
{
    class PinPortConfiguration
    {
        public const bool DEFAULT_OUTPUT = false; //Entrada
        public const bool DEFAULT_DIGITAL = true; //Digital


        public Boolean Output { get; set; }
        public Boolean Digital { get; set; }

        public enum Trigger : byte { None = 0x00, FallingEdge = 0x01, RisingEdge = 0x10, Both = 0x11 }

        //Digital-------------------------------------
        //output
        public Boolean DefaultValueD { get; set; }

        //input
        public Trigger changeTypeD { get; set; }


        //Analog-------------------------------------
        //output
        public Byte DefaultValueA { get; set; }

        //input
        public Byte Increment { get; set; }
        public Byte Threshold { get; set; }

        public PinPortConfiguration(Plugins.HomeDeviceType HomeDeviceType)
        {
            switch (Plugins.HomeDeviceType)
            {
                case Plugins.HomeDeviceType.Buttom:
                    break;
                case Plugins.HomeDeviceType.Dimmable:
                    break;
                case Plugins.HomeDeviceType.DoorLock:
                    break;
                case Plugins.HomeDeviceType.HumiditySensor:
                    break;
                case Plugins.HomeDeviceType.LightSensor:
                    break;
                case Plugins.HomeDeviceType.PowerSensor:
                    break;
                case Plugins.HomeDeviceType.PresenceSensor:
                    break;
                case Plugins.HomeDeviceType.RGBLight:
                    break;
                case Plugins.HomeDeviceType.Switch:
                    break;
                case Plugins.HomeDeviceType.TemperatureSensor:
                    break;
                case Plugins.HomeDeviceType.WallPlug:
                    break;
                default:
                    break;
            }
        }
    }
}
