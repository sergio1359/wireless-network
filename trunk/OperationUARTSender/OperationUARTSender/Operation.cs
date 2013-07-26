using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationUARTSender
{
    public class Operation
    {
        #region OPCodes
        public enum OPCodes : byte
        {
            Reset = 0x00,
            FirmwareVersionRead,
            FirmwareVersionReadResponse,
            ShieldModelRead,
            ShieldModelReadResponse,
            BaseModelRead,
            BaseModelReadResponse,
            ConfigWrite,
            ConfigWriteResponse,
            ConfigRead,
            ConfigReadResponse,
            ConfigReadConfirmation,
            ConfigChecksumRead,
            ConfigChecksumResponse,

            MacRead = 0x20,
            MacReadResponse,
            NextHopRead,
            NextHopReadResponse,
            RouteTableRead,
            RouteTableReadResponse,
            RouteTableReadConfirmation,

            DateTimeWrite = 0x30,
            DateTimeRead,
            DateTimeReadResponse,

            Warning = 0x3E,
            Error = 0x3F,

            LogicWrite = 0x40,
            LogicSwitch,
            LogicRead,
            LogicReadResponse,

            ColorWrite = 0x50,
            ColorWriteRandom = 0x51,
            ColorSecuenceWrite = 0x52,
            ColorSortedSecuenceWrite = 0x53,
            ColorRead = 0x54,
            ColorReadResponse = 0x55,

            PresenceRead = 0x57,
            PresenceReadResponse = 0x58,

            TemperatureRead = 0x5A,
            TemperatureReadResponse = 0x5B,
            HumidityRead = 0x5C,
            HumidityReadResponse = 0x5D,

            PowerRead = 0x60,
            PowerReadResponse = 0x61,

            LuminosityRead = 0x63,
            LuminosityReadResponse = 0x64,

            PCOperation = 0xFE,
            Extension = 0xFF,
        }
        #endregion

        public ushort SourceAddress { get; set; }
        public ushort DestinationAddress { get; set; }
        public OPCodes OpCode { get; set; }
        public byte[] Args { get; set; }

        public byte[] ToBinary()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(SourceAddress));
            result.AddRange(BitConverter.GetBytes(DestinationAddress));
            result.Add((byte)OpCode);
            if (Args != null)
                result.AddRange(Args);

            return result.ToArray();
        }

        public void FromBinary(byte[] buffer, int offset = 0)
        {
            SourceAddress = (ushort)((((ushort)buffer[1 + offset]) << 8) | (ushort)buffer[0 + offset]);
            DestinationAddress = (ushort)((((ushort)buffer[3 + offset]) << 8) | (ushort)buffer[2 + offset]);
            OpCode = (OPCodes)buffer[4 + offset];
            Args = new byte[buffer.Length - 5 - offset];
            Buffer.BlockCopy(buffer, 5 + offset, Args, 0, Args.Length);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("FROM: 0x{0:X4}   TO: 0x{1:X4}   OPCODE: 0x{2:X2}   ARGS: ",
                this.SourceAddress,
                this.DestinationAddress,
                this.OpCode);
            foreach (byte b in this.Args)
            {
                sb.AppendFormat("0x{0:X2} ", b);
            }

            return sb.ToString();
        }
    }
}
