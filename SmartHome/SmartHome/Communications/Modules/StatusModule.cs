#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications;
using SmartHome.Communications.Messages;
using SmartHome.Communications.Modules;
using SmartHome.BusinessEntities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
#endregion

namespace SmartHome.Communications.Modules
{
    public class StatusModule : ModuleBase
    {
        public event EventHandler<HomeDevice> StateRefreshed;

        private Timer statusUpdateTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusModule"/> class.
        /// </summary>
        /// <param name="communicationManager">The communication manager.</param>
        public StatusModule(CommunicationManager communicationManager)
            : base(communicationManager)
        {
            this.statusUpdateTimer = new Timer()
            {
                Interval = 1000 * 10, // 10 seconds
                AutoReset = false,
            };
            this.statusUpdateTimer.Elapsed += statusUpdateTimer_Elapsed;

            this.statusUpdateTimer.Start();
        }

        private async void statusUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<HomeDevice> homeDevices;

            using (UnitOfWork repository = new UnitOfWork())
            {
                homeDevices = repository.HomeDeviceRespository.GetConnectedHomeDevices().ToList();
            }

            foreach (var hd in homeDevices)
            {
                if (hd.LastStatusUpdate == null)
                {
                    //TODO: Call to RefreshState Method with the priority of this module
                    OperationMessage refreshMessage = hd.RefreshState();
                    await this.SendMessage(refreshMessage);
                }

                // TESTING
                //if (hd is Light)
                //{
                //    var message = hd.GetAddressableOperation(((Light)hd).Switch());
                //    await this.SendMessage(message);
                //}
            }

            this.statusUpdateTimer.Start();
        }

        #region Overridden Methods
        public override void ProcessReceivedMessage(Communications.Messages.IMessage inputMessage)
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
        private void OnStateRefresh(HomeDevice hd)
        {
            if (StateRefreshed != null)
                StateRefreshed(hd, hd);
        }

        private void CallProcessMethod(OperationMessage message, Action<ushort, ushort, int> processMethod)
        {
            ushort deviceAddress = (ushort)(((ushort)message.Args[1]) << 8 | message.Args[0]);
            int value = message.Args[2];

            processMethod(message.SourceAddress, deviceAddress, value);
        }

        /// <summary>
        /// Checks the home device. If the type is different an error message will be logged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeAddress">The node address.</param>
        /// <param name="deviceAddress">The device address.</param>
        /// <returns></returns>
        private T CheckHomeDevice<T>(ushort nodeAddress, ushort deviceAddress) where T : HomeDevice
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDev = repository.HomeDeviceRespository.GetById(deviceAddress);


                if (homeDev == null)
                {
                    PrintLog(true, "HD not present in the DB!");
                }
                else
                {
                    if (homeDev.Connector == null)
                    {
                        PrintLog(true, "HomeDevice not connected!");
                    }
                    else if (homeDev.Connector.Node.Address != nodeAddress)
                    {
                        PrintLog(true, "NodeAddress different from the DB!");
                    }
                    else if (!(homeDev is T))
                    {
                        PrintLog(true, "HD of different type from the DB!");
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
                PrintLog(true, string.Format("RGB LIGHT READING FROM 0x{0:X4} DEVICE {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                ModeRGBLight modeRGB = (ModeRGBLight)mode;

                PrintLog(false, string.Format("RGB LIGHT READING FROM 0x{0:X4} DEVICE {1}: 0x{2:X2}{3:X2}{4:X2} {5}",
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

                    OnStateRefresh(rgbHD);
                }
            }
        }

        private void ProcessDimmerReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                PrintLog(true, string.Format("DIMMER READING FROM 0x{0:X4} DEVICE {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                PrintLog(false, string.Format("DIMMER READING FROM 0x{0:X4} DEVICE {1}: {2}", nodeAddress, deviceAddress, value));

                var dimmerHD = CheckHomeDevice<Dimmable>(nodeAddress, deviceAddress);

                if (dimmerHD != null)
                {
                    dimmerHD.Value = value;
                    dimmerHD.LastStatusUpdate = DateTime.Now;

                    OnStateRefresh(dimmerHD);
                }
            }
        }

        private void ProcessTemperatureReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                PrintLog(true, string.Format("TEMPERATURE READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                PrintLog(false, string.Format("TEMPERATURE READING FROM 0x{0:X4} SENSOR {1}: {2}ºC", nodeAddress, deviceAddress, value));

                var tempHD = CheckHomeDevice<TemperatureSensor>(nodeAddress, deviceAddress);

                if (tempHD != null)
                {
                    tempHD.CelciusTemperature = value;
                    tempHD.LastStatusUpdate = DateTime.Now;

                    OnStateRefresh(tempHD);
                }
            }
        }

        private void ProcessHumidityReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                PrintLog(true, string.Format("HUMIDITY READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                PrintLog(false, string.Format("HUMIDITY READING FROM 0x{0:X4} SENSOR {1}: {2}%", nodeAddress, deviceAddress, value));

                var humHD = CheckHomeDevice<HumiditySensor>(nodeAddress, deviceAddress);

                if (humHD != null)
                {
                    humHD.Humidity = value;
                    humHD.LastStatusUpdate = DateTime.Now;

                    OnStateRefresh(humHD);
                }
            }
        }

        private void ProcessPresenceReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                PrintLog(true, string.Format("PRESENCE READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                PrintLog(false, string.Format("PRESENCE READING FROM 0x{0:X4} SENSOR {1}: {2}", nodeAddress, deviceAddress, (value != 0) ? "DETECTED" : "NOT DETECTED"));

                var presenceHD = CheckHomeDevice<PresenceSensor>(nodeAddress, deviceAddress);

                if (presenceHD != null)
                {
                    //TODO: Raise Presence Event
                    presenceHD.LastStatusUpdate = DateTime.Now;

                    OnStateRefresh(presenceHD);
                }
            }
        }

        private void ProcessLogicReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                PrintLog(true, string.Format("LOGIC READING FROM 0x{0:X4} DEVICE {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                bool status = (value != 0);

                PrintLog(false, string.Format("LOGIC READING FROM 0x{0:X4} DEVICE {1}: {2}", nodeAddress, deviceAddress, status ? "HIGH" : "LOW"));

                HomeDevice logicHD = CheckHomeDevice<HomeDevice>(nodeAddress, deviceAddress);

                if (logicHD is WallPlug)
                {
                    (logicHD as WallPlug).IsOn = status;
                }
                else if (logicHD is Light)
                {
                    (logicHD as Light).IsOn = status;
                }
                else if (logicHD is Button)
                {
                    //TODO: Raise Button Pressed Event
                }
                else if (logicHD is SwitchButton)
                {
                    (logicHD as SwitchButton).Open = status;
                }
                else
                {
                    PrintLog(true, "HD type not spected!");
                }

                if (logicHD != null)
                {
                    logicHD.LastStatusUpdate = DateTime.Now;

                    OnStateRefresh(logicHD);
                }
            }
        }

        private void ProcessPowerReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                PrintLog(true, string.Format("POWER READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                PrintLog(false, string.Format("POWER READING FROM 0x{0:X4} SENSOR {1}: {2}", nodeAddress, deviceAddress, value));

                var powerHD = CheckHomeDevice<PowerSensor>(nodeAddress, deviceAddress);

                if (powerHD != null)
                {
                    powerHD.Consumption = value;
                    powerHD.LastStatusUpdate = DateTime.Now;

                    OnStateRefresh(powerHD);
                }
            }
        }

        private void ProcessLuminosityReading(ushort nodeAddress, ushort deviceAddress, int value)
        {
            if (value == 0xFF) //Invalid Reading
            {
                PrintLog(true, string.Format("LUMINOSITY READING FROM 0x{0:X4} SENSOR {1}: UNKNOWN", nodeAddress, deviceAddress));
            }
            else
            {
                PrintLog(false, string.Format("LUMINOSITY READING FROM 0x{0:X4} SENSOR {1}: {2}", nodeAddress, deviceAddress, value));

                var luminHD = CheckHomeDevice<LuminositySensor>(nodeAddress, deviceAddress);

                if (luminHD != null)
                {
                    luminHD.Luminosity = value;
                    luminHD.LastStatusUpdate = DateTime.Now;

                    OnStateRefresh(luminHD);
                }
            }
        }
        #endregion
    }
}
