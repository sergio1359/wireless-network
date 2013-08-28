#region Using Statements
using SmartHome.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text; 
#endregion

namespace SmartHome.Communications.Messages
{
    public partial class OperationMessage
    {
        #region Private Methods
        private static OperationMessage BaseMessage(ushort nodeAddress, OPCodes opCode)
        {
            return BaseMessage(nodeAddress, opCode, null);
        }

        private static OperationMessage BaseMessage(ushort nodeAddress, OPCodes opCode, ushort homeDeviceAddress)
        {
            return BaseMessage(nodeAddress, opCode, homeDeviceAddress.UshortToByte());
        }

        private static OperationMessage BaseMessage(ushort nodeAddress, OPCodes opCode, ushort homeDeviceAddress, byte[] args)
        {
            return BaseMessage(nodeAddress, opCode, homeDeviceAddress.UshortToByte().Concat(args).ToArray());
        }

        private static OperationMessage BaseMessage(ushort nodeAddress, OPCodes opCode, byte[] args)
        {
            return new OperationMessage()
            {
                DestinationAddress = nodeAddress,
                OpCode = opCode,
                Args = args
            };

        }
        #endregion

        #region Public Methods
        public static OperationMessage Reset(ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.Reset);
        }

        public static OperationMessage FirmwareVersionRead(ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.FirmwareVersionRead);
        }

        public static OperationMessage ShieldModelRead(ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.ShieldModelRead);
        }

        public static OperationMessage BaseModelRead(ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.BaseModelRead);
        }

        public static OperationMessage ConfigWrite(byte fragmentTotal, byte fragment, byte length, byte[] content, ushort destinationAddress = 0)
        {
            List<byte> args = new List<byte>();
            args.Add((byte)(fragmentTotal << 4 | (fragment & 0xF)));
            args.Add(length);
            args.AddRange(content);
            return BaseMessage(destinationAddress, OPCodes.ConfigWrite, args.ToArray());
        }

        public static OperationMessage ConfigRead(ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.ConfigRead);
        }

        public static OperationMessage ConfigReadConfirmation(byte fragmentTotal, byte fragment, ConfigWriteStatusCodes statusCode, ushort destinationAddress = 0)
        {
            byte[] args = new byte[]
                {
                    (byte)(fragmentTotal << 4 | (fragment & 0xF)),
                    (byte)statusCode,
                };

            return BaseMessage(destinationAddress, OPCodes.ConfigReadConfirmation, args);
        }

        public static OperationMessage ConfigChecksumRead(ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.ConfigChecksumRead); 
        }

        public static OperationMessage MACRead(ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.MacRead); 
        }

        public static OperationMessage NextHopRead(ushort nodeAddress, ushort destinationAddress = 0)
        { 
            byte[] args = nodeAddress.UshortToByte();
            return BaseMessage(destinationAddress, OPCodes.NextHopRead, args); 
        }

        public static OperationMessage RouteTableRead(ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.RouteTableRead); 
        }

        public static OperationMessage RouteTableReadConfirmation(byte fragmentTotal, byte fragment, byte length, ConfigWriteStatusCodes statusCode, ushort destinationAddress = 0)
        {
            byte[] args = new byte[]
                {
                    (byte)(fragmentTotal << 4 | (fragment & 0xF)),
                    (byte)statusCode,
                };

            return BaseMessage(destinationAddress, OPCodes.RouteTableReadConfirmation, args);
        }

        public static OperationMessage PingRequest(ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.PingRequest);
        }

        public static OperationMessage JoinRequestResponse(byte[] RSAKey, ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.JoinRequestResponse, RSAKey);
        }

        public static OperationMessage JoinAcceptResponse(ushort newAddress, ushort panId, byte channel, string securityKey, ushort destinationAddress = 0)
        {
            List<byte> operationArgs = new List<byte>();
            operationArgs.AddRange(BitConverter.GetBytes(newAddress));
            operationArgs.AddRange(BitConverter.GetBytes(panId));
            operationArgs.Add(channel);
            operationArgs.AddRange(Encoding.ASCII.GetBytes(securityKey));

            return BaseMessage(destinationAddress, OPCodes.JoinAcceptResponse, operationArgs.ToArray());
        }


        public static OperationMessage DateTimeWrite(DateTime dateTime, ushort destinationAddress = 0)
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

            return BaseMessage(destinationAddress, OPCodes.DateTimeWrite, args);
        }

        public static OperationMessage DateTimeRead(ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.DateTimeRead); 
        }



        public static OperationMessage LogicWrite(ushort homeDeviceAddress, LogicWriteValues value, byte seconds, ushort destinationAddress = 0) 
        {
            byte[] args = new byte[]
                {
                    (byte)value,
                    seconds,
                };

            return BaseMessage(destinationAddress, OPCodes.LogicWrite, homeDeviceAddress, args);
        }

        public static OperationMessage LogicSwitch(ushort homeDeviceAddress, byte seconds, ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.LogicSwitch, homeDeviceAddress, new byte[] { seconds });
        }

        public static OperationMessage LogicRead(ushort homeDeviceAddress, ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.LogicRead, homeDeviceAddress);
        }



        public static OperationMessage DimmerWrite(ushort homeDeviceAddress, byte value, byte seconds, ushort destinationAddress = 0) 
        {
            byte[] args = new byte[]
                {
                    (byte)value,
                    seconds,
                };

            return BaseMessage(destinationAddress, OPCodes.DimmerWrite, homeDeviceAddress, args);
        }

        public static OperationMessage DimmerRead(ushort homeDeviceAddress, ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.DimmerRead, homeDeviceAddress);
        }



        public static OperationMessage ColorWrite(ushort homeDeviceAddress, Color color, byte seconds, ushort destinationAddress = 0)
        {
            byte[] args = new byte[]
                {
                    color.R,
                    color.G,
                    color.B,
                    seconds
                };

            return BaseMessage(destinationAddress, OPCodes.ColorWrite, homeDeviceAddress, args);
        }

        public static OperationMessage ColorWriteRandom(ushort homeDeviceAddress, byte seconds, ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.ColorWriteRandom, homeDeviceAddress, new byte[] { seconds });
        }

        public static OperationMessage ColorRandomSecuenceWrite(ushort homeDeviceAddress, byte seconds, Color[] colors, ushort destinationAddress = 0)
        {
            List<byte> args = new List<byte>();
            args.Add(seconds);
            args.AddRange(colors.SelectMany(c => new byte[] { c.R, c.G, c.B }));

            return BaseMessage(destinationAddress, OPCodes.ColorRandomSecuenceWrite, homeDeviceAddress, args.ToArray());
        }

        public static OperationMessage ColorSortedSecuenceWrite(ushort homeDeviceAddress, byte seconds, Color[] colors, ushort destinationAddress = 0)
        {
            List<byte> args = new List<byte>();
            args.Add(seconds);
            args.AddRange(colors.SelectMany(c => new byte[] { c.R, c.G, c.B }));

            return BaseMessage(destinationAddress, OPCodes.ColorSortedSecuenceWrite, homeDeviceAddress, args.ToArray());
        }

        public static OperationMessage ColorRead(ushort homeDeviceAddress, ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.ColorRead, homeDeviceAddress); 
        }



        public static OperationMessage PresenceRead(ushort homeDeviceAddress, ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.PresenceRead, homeDeviceAddress); 
        }

        public static OperationMessage TemperatureRead(ushort homeDeviceAddress, ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.TemperatureRead, homeDeviceAddress);
        }

        public static OperationMessage HumidityRead(ushort homeDeviceAddress, ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.HumidityRead, homeDeviceAddress); 
        }

        public static OperationMessage PowerRead(ushort homeDeviceAddress, ushort destinationAddress = 0) 
        {
            return BaseMessage(destinationAddress, OPCodes.PowerRead, homeDeviceAddress); 
        }

        public static OperationMessage LuminosityRead(ushort homeDeviceAddress, ushort destinationAddress = 0)
        {
            return BaseMessage(destinationAddress, OPCodes.LuminosityRead, homeDeviceAddress); 
        }

        #endregion
    }
}
