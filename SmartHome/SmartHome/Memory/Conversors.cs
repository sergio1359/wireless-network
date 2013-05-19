using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Memory
{
    public static class BinaryExtension
    {
        /// <summary>
        /// Transform a DateTime in a Byte[] sort in Hours, Minutes and Seconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Byte[] ToBinaryTime(this DateTime time)
        {
            Byte[] result = new Byte[3];
            result[0] = (byte)time.Hour;
            result[1] = (byte)time.Minute;
            result[2] = (byte)time.Second;

            return result;
        }

        public static Byte[] ToBinaryDate(this DateTime time, bool littleEndian)
        {

            List<Byte> result = new List<Byte>();

            result.Add((byte)time.Day);
            result.Add((byte)time.Month);
            result.AddRange(time.Year.Uint16ToByte(littleEndian));

            return result.ToArray();
        }

        public static Byte[] Uint16ToByte(this UInt16 b, bool litteEndian)
        {
            Byte[] result = new Byte[2];

            if (litteEndian)
            {
                result[0] = (byte)b;
                result[1] = (byte)(b >> 8);
            }
            else
            {
                result[0] = (byte)(b >> 8);
                result[1] = (byte)b;
            }

            return result;
        }

        public static Byte[] Uint16ToByte(this int b, bool litteEndian)
        {
            Byte[] result = new Byte[2];

            if (litteEndian)
            {
                result[0] = (byte)b;
                result[1] = (byte)(b >> 8);
            }
            else
            {
                result[0] = (byte)(b >> 8);
                result[1] = (byte)b;
            }

            return result;
        }
    }

    public class FirmwareUnoExtension
    {
        public static Byte[] PinIOConfig(Byte port)
        {
            //Si no esta definido suponemos Entrada digital
            Byte[] result = new Byte[5];
            PinPort p = null;
            //Input:0 - Output:1, default=0
            result[0] = 0x00;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null && p.Output == true)
                {
                    result[0] = (byte)(result[0] | (0x01 << i));
                }
            }

            //Analog:0 - Digital:1 default: 1
            result[1] = 0xFF;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null && p.Digital == false)
                {
                    result[1] = (byte)(result[1] & ~(0x01 << i));
                }
            }

            //Input:0 - Output:1, default=0
            result[2] = 0x00;
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null && p.DefaultValueD == true)
                {
                    result[2] = (byte)(result[2] | (0x01 << i));
                }
            }

            //ChangetypeD None:00 Rising:10, Fall:01, Both:11
            UInt16 ctd = 0x00; //change type digital
            for (byte i = 0; i < node.GetBaseConfiguration().NumPins; i++)
            {
                p = ShieldNode.GetPinPort(port, i);
                if (p != null) ctd = (byte)(ctd | ((byte)p.changeTypeD) << (i * 2));
            }

            result[3] = ctd.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[0];
            result[4] = ctd.Uint16ToByte(node.GetBaseConfiguration().LittleEndian)[1];

            return result;
        }

        private Byte[] ToBinaryEvent(Event e, bool littleEndian)
        {
            List<Byte> result = new List<byte>();

            result.AddRange(node.Address.Uint16ToByte(littleEndian));

            result.Add(e.OPCode);

            result.AddRange(e.Args);

            return result.ToArray();
        }

        private UInt16 SizePinEvents(PinPort pin)
        {
            UInt16 size = 0;
            foreach (BasicEvent pe in pin.PinEvents)
            {
                size += (UInt16)ToBinaryEvent(pe.Event, true).Length;
            }
            return size;
        }


        //private Byte[] ToBinaryTimeEventRestriction(TimeRestriction tr)
        //{
        //    List<Byte> result = new List<Byte>();
        //    result.AddRange(tr.Start.ToBinaryTime());
        //    result.AddRange(tr.End.ToBinaryTime());
        //    return result.ToArray();
        //}
    }
}
