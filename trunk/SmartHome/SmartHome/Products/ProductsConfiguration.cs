#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace SmartHome.Products
{
    public static class ProductConfiguration
    {
        public static Dictionary<string, Tuple<ConnectorTypes, PinPort[]>> GetShieldDictionary(ShieldTypes shieldtype)
        {
            Dictionary<string, Tuple<ConnectorTypes, PinPort[]>> pinPorts = new Dictionary<string, Tuple<ConnectorTypes, PinPort[]>>();
            switch (shieldtype)
            {
                case ShieldTypes.Debug:
                    pinPorts.Add("Button", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.LogicInput, new PinPort[] { new PinPort("D7") }));
                    pinPorts.Add("DimmerZero", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.DimmerPassZero, new PinPort[] { new PinPort("D0") }));
                    pinPorts.Add("DimmerLight", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.Dimmer, new PinPort[] { new PinPort("D1") }));
                    pinPorts.Add("Light", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.SwitchLOW, new PinPort[] { new PinPort("D6") }));
                    pinPorts.Add("Temperature", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.LogicInput, new PinPort[] { new PinPort("E2") }));
                    pinPorts.Add("Humidity", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.LogicInput, new PinPort[] { new PinPort("E2") }));
                    pinPorts.Add("Presence", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.LogicInput, new PinPort[] { new PinPort("B6") }));
                    pinPorts.Add("SensorConnector", new Tuple<ConnectorTypes, PinPort[]>(ConnectorTypes.ConnectorSensorBoard, new PinPort[] { new PinPort("A0"), new PinPort("A1"), new PinPort("A2"), new PinPort("A3"), new PinPort("A4"), new PinPort("A5") }));
                    break;
                default:
                    throw new NotImplementedException();
            }

            return pinPorts;
        }

        public static Base GetBaseConfiguration(BaseTypes controller)
        {
            Base result = new Base();
            switch (controller)
            {
                case BaseTypes.ATMega128RFA1_V1:
                case BaseTypes.ATMega128RFA1_V2:
                    result.UController = controller;
                    result.DeviceSignature = 128;
                    result.NumPorts = 7;
                    result.NumPins = 8;

                    result.AnalogPorts = new string[] { "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7" };
                    result.PWMPorts = new string[] { "B4", "B5", "B6", "B7", "E3", "E4", "E5", "G5" };  //VERSION MINOLO:{ "B4", "B7", "G5" } el B7 y el G5 estan compartidos con el mismo timer
                    result.UnavailablePorts = new string[] { "G3", "G4" };  //TODO: de momento estos pero hay que chequear

                    result.LittleEndian = true;
                    break;
                default:
                    throw new Exception();
            }

            return result;
        }

        public static PinPortConfiguration GetPinPortConfiguration(HomeDevice homeDevice)
        {
            PinPortConfiguration configuration = DefaultPinPortConfiguration();

            if (homeDevice is Button)
            {
                configuration.Output = false;
                configuration.Digital = true;
                configuration.ChangeTypeD = PinPortConfiguration.Trigger.FallingEdge;
            }
            else if (homeDevice is SwitchButton)
            {
                configuration.Output = false;
                configuration.Digital = true;
                configuration.ChangeTypeD = PinPortConfiguration.Trigger.Both;
            }
            else if (homeDevice is Light || homeDevice is WallPlug)
            {
                configuration.Output = true;
                configuration.Digital = true;
                configuration.DefaultValueD = false;
            }
            else if (homeDevice is Dimmable)
            {
                configuration.Output = true;
                configuration.Digital = false;
                configuration.DefaultValueA = 0x00;
            }
            else if (homeDevice is PresenceSensor)
            {
                configuration.Output = false;
                configuration.Digital = true;
                configuration.ChangeTypeD = PinPortConfiguration.Trigger.RisingEdge;
            }
            else if (homeDevice is DoorLock)
            {
                configuration.Output = true;
                configuration.Digital = true;
            }

            return configuration;
        }

        public static PinPortConfiguration DefaultPinPortConfiguration()
        {
            PinPortConfiguration configuration = new PinPortConfiguration
            {
                Digital = PinPortConfiguration.DEFAULT_DIGITAL,
                Output = PinPortConfiguration.DEFAULT_OUTPUT,

                ChangeTypeD = PinPortConfiguration.Trigger.None,
                DefaultValueD = false,

                DefaultValueA = 0x00,
                Increment = 0x00,
                Threshold = 0x00,
            };

            return configuration;
        }

        /// <summary>
        /// Return the PinPort of a shield, if not exits return null
        /// </summary>
        /// <param name="shield"></param>
        /// <returns></returns>
        public static PinPort GetDimmerPassZeroPinPort(ShieldTypes shield)
        {
            var dictionary = GetShieldDictionary(shield);

            if (dictionary.Values.Any(elem => elem.Item1 == ConnectorTypes.DimmerPassZero))
            {
                return dictionary.Values.First(elem => elem.Item1 == ConnectorTypes.DimmerPassZero).Item2[0];
            }

            return null;
        }
    }
}
