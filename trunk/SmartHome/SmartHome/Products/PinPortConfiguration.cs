using SmartHome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Products
{
    public class PinPortConfiguration
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

        public PinPortConfiguration(HomeDeviceType HomeDeviceType)
        {
            switch (HomeDeviceType)
            {
                case HomeDeviceType.User:
                    break;
                case HomeDeviceType.Central:
                    break;
                case HomeDeviceType.Buttom:
                    break;
                case HomeDeviceType.Switch:
                    break;
                case HomeDeviceType.WallPlug:
                    break;
                case HomeDeviceType.Light:
                    break;
                case HomeDeviceType.Dimmable:
                    break;
                case HomeDeviceType.PresenceSensor:
                    break;
                case HomeDeviceType.PowerSensor:
                    break;
                case HomeDeviceType.TemperatureSensor:
                    break;
                case HomeDeviceType.HumiditySensor:
                    break;
                case HomeDeviceType.LightSensor:
                    break;
                case HomeDeviceType.DoorLock:
                    break;
                case HomeDeviceType.RGBLight:
                    break;
                default:
                    break;
            }
        }
    }
}
