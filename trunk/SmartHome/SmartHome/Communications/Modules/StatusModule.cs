#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Entities.HomeDevices;
using SmartHome.Comunications;
using SmartHome.Comunications.Messages;
using SmartHome.Comunications.Modules;
#endregion

namespace SmartHome.Communications.Modules
{
    class StatusModule : ModuleBase
    {
        public StatusModule(CommunicationManager communicationManager)
            : base(communicationManager)
        {

        }

        #region Overridden Methods
        public override void ProcessReceivedMessage(Comunications.Messages.IMessage inputMessage)
        {
            OperationMessage message = (OperationMessage)inputMessage;

            if (message.OpCode == OperationMessage.OPCodes.ColorReadResponse)
            {
                ushort deviceAddress = (ushort)(((ushort)message.Args[1]) << 8 | message.Args[0]);
                Color color = Color.FromArgb(message.Args[2], message.Args[3], message.Args[4]);
                byte mode = message.Args[5];

                this.ProcessRGBLightReading(message.SourceAddress, deviceAddress, color, mode);
            }
            else if (message.OpCode == OperationMessage.OPCodes.DimmerReadResponse)
            {
                this.CallProcessMethod(message, this.ProcessDimmerReading);
            }
            else if (message.OpCode == OperationMessage.OPCodes.TemperatureReadResponse)
            {
                this.CallProcessMethod(message, this.ProcessTemperatureReading);
            }
            else if (message.OpCode == OperationMessage.OPCodes.HumidityReadResponse)
            {
                this.CallProcessMethod(message, this.ProcessHumidityReading);
            }
            else if (message.OpCode == OperationMessage.OPCodes.PresenceReadResponse)
            {
                this.CallProcessMethod(message, this.ProcessPresenceReading);
            }
            else if (message.OpCode == OperationMessage.OPCodes.LogicReadResponse)
            {
                this.CallProcessMethod(message, this.ProcessLogicReading);
            }
            else if (message.OpCode == OperationMessage.OPCodes.LuminosityReadResponse)
            {
                this.CallProcessMethod(message, this.ProcessLuminosityReading);
            }
            else if (message.OpCode == OperationMessage.OPCodes.PowerReadResponse)
            {
                this.CallProcessMethod(message, this.ProcessPowerReading);
            }

        }

        protected override Filter ConfigureInputFilter()
        {
            return new Filter()
            {
                Endpoint = Endpoints.APPLICATION_EP,
                OpCodeType = typeof(OperationMessage.OPCodes),
                Secured = true,
                Routed = true,
                OpCodes = new byte[] 
                { 
                    (byte)OperationMessage.OPCodes.ColorReadResponse, 
                    (byte)OperationMessage.OPCodes.DimmerReadResponse,
                    (byte)OperationMessage.OPCodes.HumidityReadResponse, 
                    (byte)OperationMessage.OPCodes.LogicReadResponse,
                    (byte)OperationMessage.OPCodes.LuminosityReadResponse,
                    (byte)OperationMessage.OPCodes.PowerReadResponse,
                    (byte)OperationMessage.OPCodes.PresenceReadResponse, 
                    (byte)OperationMessage.OPCodes.TemperatureReadResponse, 
                },
            };
        }

        protected override OutputParameters ConfigureOutputParameters()
        {
            return new OutputParameters(
                priority: 0.5f,
                endpoint: Endpoints.APPLICATION_EP,
                securityEnabled: true,
                routingEnabled: true);
        }
        #endregion

        #region Private Common Methods
        private void CallProcessMethod(OperationMessage message, Action<ushort, ushort, int> processMethod)
        {
            ushort deviceAddress = (ushort)(((ushort)message.Args[1]) << 8 | message.Args[0]);
            int value = message.Args[2];

            processMethod(message.SourceAddress, deviceAddress, value);
        }
        
        private T CheckHomeDevice<T>(ushort nodeAddress, ushort deviceAddress) where T : HomeDevice
        {
            lock(this){
            var homeDev = Repositories.HomeDeviceRespository.GetById(deviceAddress);


            if (homeDev == null)
            {
                Debug.WriteLine("HD not present in the DB!");
            }
            else
            {
                var nodeAddressRep = homeDev.Connector.Node.Address;

                if (nodeAddressRep != nodeAddress)
                {
                    Debug.WriteLine("NodeAddress different from the DB!");
                }
                else if (!(homeDev is T))
                {
                    Debug.WriteLine("HD of different type from the DB!");
                }
                else
                {
                    return (T)homeDev;
                }
            }
            }
            return null;
        } 
        #endregion

        #region Private Process Methods

        private void ProcessRGBLightReading(ushort nodeAddress, ushort deviceAddress, Color color, byte mode)
        {
            if (mode == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("RGB LIGHT READING FROM 0x{0:X4} DEVICE {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                ModeRGBLight modeRGB = (ModeRGBLight)mode;

                Debug.WriteLine(string.Format("RGB LIGHT READING FROM 0x{0:X4} DEVICE {1}: 0x{2:X2}{3:X2}{4:X2} {5}",
                    nodeAddress,
                    deviceAddress,
                    color.R,
                    color.G,
                    color.B, 
                    modeRGB.ToString()));

                var rgbHD = CheckHomeDevice<RGBLight>(nodeAddress, deviceAddress);

                if (rgbHD != null)
                {
                    rgbHD.Color = color;
                    rgbHD.Mode = modeRGB;
                    rgbHD.LastStatusUpdate = DateTime.Now;
                }
            }
        }

        private void ProcessDimmerReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("DIMMER READING FROM 0x{0:X4} DEVICE {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                Debug.WriteLine(string.Format("DIMMER READING FROM 0x{0:X4} DEVICE {1}: {2}", nodeAddress, deviceAddress, value));

                var dimmerHD = CheckHomeDevice<Dimmable>(nodeAddress, deviceAddress);

                if (dimmerHD != null)
                {
                    dimmerHD.Value = value;
                    dimmerHD.LastStatusUpdate = DateTime.Now;
                }
            }
        }

        private void ProcessTemperatureReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("TEMPERATURE READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                Debug.WriteLine(string.Format("TEMPERATURE READING FROM 0x{0:X4} SENSOR {1}: {2}ºC", nodeAddress, deviceAddress, value));

                var tempHD = CheckHomeDevice<TemperatureSensor>(nodeAddress, deviceAddress);

                if (tempHD != null)
                {
                    tempHD.CelciusTemperature = value;
                    tempHD.LastStatusUpdate = DateTime.Now;
                }
            }
        }

        private void ProcessHumidityReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("HUMIDITY READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                Debug.WriteLine(string.Format("HUMIDITY READING FROM 0x{0:X4} SENSOR {1}: {2}%", nodeAddress, deviceAddress, value));

                var humHD = CheckHomeDevice<HumiditySensor>(nodeAddress, deviceAddress);

                if (humHD != null)
                {
                    humHD.Humidity = value;
                    humHD.LastStatusUpdate = DateTime.Now;
                }
            }
        }

        private void ProcessPresenceReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("PRESENCE READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                Debug.WriteLine(string.Format("PRESENCE READING FROM 0x{0:X4} SENSOR {1}: {2}", nodeAddress, deviceAddress, (value != 0) ? "DETECTED" : "NOT DETECTED"));

                var presenceHD = CheckHomeDevice<PresenceSensor>(nodeAddress, deviceAddress);

                if (presenceHD != null)
                {
                    //TODO: Raise Presence Event
                    presenceHD.LastStatusUpdate = DateTime.Now;
                }
            }
        }

        private void ProcessLogicReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("LOGIC READING FROM 0x{0:X4} DEVICE {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                bool status = (value != 0);

                Debug.WriteLine(string.Format("LOGIC READING FROM 0x{0:X4} DEVICE {1}: {2}", nodeAddress, deviceAddress, status ? "HIGH" : "LOW"));

                HomeDevice logicHD;

                if ((logicHD = CheckHomeDevice<WallPlug>(nodeAddress, deviceAddress)) != null)
                {
                    (logicHD as WallPlug).IsOn = status;
                }
                else if ((logicHD = CheckHomeDevice<Light>(nodeAddress, deviceAddress)) != null)
                {
                    (logicHD as Light).IsOn = status;
                }
                else if ((logicHD = CheckHomeDevice<Button>(nodeAddress, deviceAddress)) != null)
                {
                    //TODO: Raise Button Pressed Event
                }
                else if ((logicHD = CheckHomeDevice<SwitchButton>(nodeAddress, deviceAddress)) != null)
                {
                    (logicHD as SwitchButton).Open = status;
                }

                if(logicHD != null)
                    logicHD.LastStatusUpdate = DateTime.Now;
            }
        }

        private void ProcessPowerReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("POWER READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                Debug.WriteLine(string.Format("POWER READING FROM 0x{0:X4} SENSOR {1}: {2}", nodeAddress, deviceAddress, value));

                var powerHD = CheckHomeDevice<PowerSensor>(nodeAddress, deviceAddress);

                if (powerHD != null)
                {
                    powerHD.Consumption = value;
                    powerHD.LastStatusUpdate = DateTime.Now;
                }
            }
        }

        private void ProcessLuminosityReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                Debug.WriteLine(string.Format("LUMINOSITY READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                Debug.WriteLine(string.Format("LUMINOSITY READING FROM 0x{0:X4} SENSOR {1}: {2}", nodeAddress, deviceAddress, value));

                var luminHD = CheckHomeDevice<LuminositySensor>(nodeAddress, deviceAddress);

                if (luminHD != null)
                {
                    luminHD.Luminosity = value;
                    luminHD.LastStatusUpdate = DateTime.Now;
                }
            }
        }
        #endregion
    }
}
