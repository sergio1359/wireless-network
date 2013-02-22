using System;

namespace DomoticNetwork.NetworkModel
{
    class HomeDevice
    {
        //Identificador
        public String Name { set; get; }
        //Type
        public enum HomeDeviceType { Light, RGBLight, DimmerLight, PresenceSensor, TemperatureHumidityAnalogSensor, TemperatureHumidityDigitalSensor, LightSensor, TouchSensor, WallPlugs, IR, Fan };
        public HomeDeviceType Type { set; get; }
        //Los conectores que son permitidos
        public Enums.ConnectorType[] Allows { set; get; } //los conectores a los que se le permite ser enganchado el HomeDevice

        public HomeDevice(String name, HomeDeviceType homeDevice)
        {
            Name = name;
            Type = homeDevice;

            switch (homeDevice)
            {
                case HomeDeviceType.Light:
                    break;
                case HomeDeviceType.RGBLight:
                    break;
                case HomeDeviceType.DimmerLight:
                    break;
                case HomeDeviceType.PresenceSensor:
                    break;
                case HomeDeviceType.TemperatureHumidityAnalogSensor:
                    break;
                case HomeDeviceType.TemperatureHumidityDigitalSensor:
                    break;
                case HomeDeviceType.LightSensor:
                    break;
                case HomeDeviceType.TouchSensor:
                    break;
                case HomeDeviceType.WallPlugs:
                    break;
                case HomeDeviceType.IR:
                    break;
                case HomeDeviceType.Fan:
                    break;
                default:
                    break;
            }
        }


    }
}
