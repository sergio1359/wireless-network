#region Using Statements
using SmartHome.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text; 
#endregion

namespace SmartHome.Comunications.Messages
{
    public partial class OperationMessage
    {
        #region Private Methods
        private static OperationMessage BaseMessage(OPCodes opCode)
        {
            return BaseMessage(opCode, null);
        }

        private static OperationMessage BaseMessage(OPCodes opCode, ushort homeDeviceAddress)
        {
            return BaseMessage(opCode, homeDeviceAddress.UshortToByte());
        }

        private static OperationMessage BaseMessage(OPCodes opCode, ushort homeDeviceAddress, byte[] args)
        {
            return new OperationMessage()
            {
                OpCode = opCode,
                Args = homeDeviceAddress.UshortToByte().Concat(args).ToArray(),
            };

        } 

        private static OperationMessage BaseMessage(OPCodes opCode, byte[] args)
        {
            return new OperationMessage()
            {
                OpCode = opCode,
                Args = args
            };

        }
        #endregion

        #region Public Methods
        public static OperationMessage Reset()
        {
            return BaseMessage(OPCodes.Reset);
        }

        public static OperationMessage FirmwareVersionRead()
        {
            return BaseMessage(OPCodes.FirmwareVersionRead);
        }

        public static OperationMessage ShieldModelRead()
        {
            return BaseMessage(OPCodes.ShieldModelRead);
        }

        public static OperationMessage BaseModelRead()
        {
            return BaseMessage(OPCodes.BaseModelRead);
        }

        public static OperationMessage ConfigWrite(byte fragmentTotal, byte fragment, byte length, byte[] content)
        {
            List<byte> args = new List<byte>();
            args.Add((byte)(fragmentTotal << 4 | (fragment & 0xF)));
            args.Add(length);
            args.AddRange(content);
            return BaseMessage(OPCodes.ConfigWrite, args.ToArray());
        }

        public static OperationMessage ConfigRead()
        {
            return BaseMessage(OPCodes.ConfigRead);
        }

        public static OperationMessage ConfigReadConfirmation(byte fragmentTotal, byte fragment, ConfigWriteStatusCodes statusCode)
        {
            byte[] args = new byte[]
                {
                    (byte)(fragmentTotal << 4 | (fragment & 0xF)),
                    (byte)statusCode,
                };

            return BaseMessage(OPCodes.ConfigReadConfirmation, args);
        }

        public static OperationMessage ConfigChecksumRead() 
        {
            return BaseMessage(OPCodes.ConfigChecksumRead); 
        }

        public static OperationMessage MACRead() 
        {
            return BaseMessage(OPCodes.MacRead); 
        }

        public static OperationMessage NextHopRead(ushort nodeAddress)
        { 
            byte[] args = nodeAddress.UshortToByte();
            return BaseMessage(OPCodes.NextHopRead, args); 
        }

        public static OperationMessage RouteTableRead() 
        {
            return BaseMessage(OPCodes.RouteTableRead); 
        }

        public static OperationMessage RouteTableReadConfirmation(byte fragmentTotal, byte fragment, byte length, ConfigWriteStatusCodes statusCode)
        {
            byte[] args = new byte[]
                {
                    (byte)(fragmentTotal << 4 | (fragment & 0xF)),
                    (byte)statusCode,
                };

            return BaseMessage(OPCodes.RouteTableReadConfirmation, args);
        }

        public static OperationMessage PingRequest()
        {
            return BaseMessage(OPCodes.PingRequest);
        }

        public static OperationMessage JoinRequestResponse(byte[] RSAKey)
        {
            return BaseMessage(OPCodes.JoinRequestResponse, RSAKey);
        }

        public static OperationMessage JoinAcceptResponse(ushort newAddress, byte panId, byte channel, string securityKey)
        {
            List<byte> operationArgs = new List<byte>();
            operationArgs.AddRange(BitConverter.GetBytes(newAddress));
            operationArgs.Add(panId);
            operationArgs.Add(channel);
            operationArgs.AddRange(Encoding.ASCII.GetBytes(securityKey));

            return BaseMessage(OPCodes.JoinAcceptResponse, operationArgs.ToArray());
        }


        public static OperationMessage DateTimeWrite(DateTime dateTime)
        {
            var dow = (byte)Enum.Parse(typeof(WeekDays), dateTime.DayOfWeek.ToString());

            byte[] year = ((ushort)dateTime.Year).UshortToByte();

            byte[] args = new byte[]
                {
                    (byte)dow,
                    (byte)dateTime.Day,
                    (byte)dateTime.Month,
                    (byte)year[0],
                    (byte)year[1],
                    (byte)dateTime.Hour,
                    (byte)dateTime.Minute,
                    (byte)dateTime.Second,
                };

            return BaseMessage(OPCodes.DateTimeWrite, args);
        }

        public static OperationMessage DateTimeRead() 
        {
            return BaseMessage(OPCodes.DateTimeRead); 
        }



        public static OperationMessage LogicWrite(ushort homeDeviceAddress, LogicWriteValues value, byte seconds) 
        {
            byte[] args = new byte[]
                {
                    (byte)value,
                    seconds,
                };

            return BaseMessage(OPCodes.LogicWrite, homeDeviceAddress, args);
        }

        public static OperationMessage LogicSwitch(ushort homeDeviceAddress, byte seconds)
        {
            return BaseMessage(OPCodes.LogicSwitch, homeDeviceAddress, new byte[] { seconds });
        }

        public static OperationMessage LogicRead(ushort homeDeviceAddress) 
        {
            return BaseMessage(OPCodes.LogicRead, homeDeviceAddress);
        }



        public static OperationMessage DimmerWrite(ushort homeDeviceAddress, byte value, byte seconds) 
        {
            byte[] args = new byte[]
                {
                    (byte)value,
                    seconds,
                };

            return BaseMessage(OPCodes.DimmerWrite, homeDeviceAddress, args);
        }

        public static OperationMessage DimmerRead(ushort homeDeviceAddress) 
        {
            return BaseMessage(OPCodes.DimmerRead, homeDeviceAddress);
        }



        public static OperationMessage ColorWrite(ushort homeDeviceAddress, Color color, byte seconds)
        {
            byte[] args = new byte[]
                {
                    color.R,
                    color.G,
                    color.B,
                    seconds
                };

            return BaseMessage(OPCodes.ColorWrite, homeDeviceAddress, args);
        }

        public static OperationMessage ColorWriteRandom(ushort homeDeviceAddress, byte seconds) 
        {
            return BaseMessage(OPCodes.ColorWriteRandom, homeDeviceAddress, new byte[] { seconds });
        }

        public static OperationMessage ColorRandomSecuenceWrite(ushort homeDeviceAddress, byte seconds, Color[] colors)
        {
            List<byte> args = new List<byte>();
            args.Add(seconds);
            args.AddRange(colors.SelectMany(c => new byte[] { c.R, c.G, c.B }));

            return BaseMessage(OPCodes.ColorRandomSecuenceWrite, homeDeviceAddress, args.ToArray());
        }

        public static OperationMessage ColorSortedSecuenceWrite(ushort homeDeviceAddress, byte seconds, Color[] colors)
        {
            List<byte> args = new List<byte>();
            args.Add(seconds);
            args.AddRange(colors.SelectMany(c => new byte[] { c.R, c.G, c.B }));

            return BaseMessage(OPCodes.ColorSortedSecuenceWrite, homeDeviceAddress, args.ToArray());
        }

        public static OperationMessage ColorRead(ushort homeDeviceAddress) 
        {
            return BaseMessage(OPCodes.ColorRead, homeDeviceAddress); 
        }



        public static OperationMessage PresenceRead(ushort homeDeviceAddress) 
        {
            return BaseMessage(OPCodes.PresenceRead, homeDeviceAddress); 
        }

        public static OperationMessage TemperatureRead(ushort homeDeviceAddress) 
        {
            return BaseMessage(OPCodes.TemperatureRead, homeDeviceAddress);
        }

        public static OperationMessage HumidityRead(ushort homeDeviceAddress)
        {
            return BaseMessage(OPCodes.HumidityRead, homeDeviceAddress); 
        }

        public static OperationMessage PowerRead(ushort homeDeviceAddress) 
        {
            return BaseMessage(OPCodes.PowerRead, homeDeviceAddress); 
        }

        public static OperationMessage LuminosityRead(ushort homeDeviceAddress)
        {
            return BaseMessage(OPCodes.LuminosityRead, homeDeviceAddress); 
        }

        #endregion
    }
}
