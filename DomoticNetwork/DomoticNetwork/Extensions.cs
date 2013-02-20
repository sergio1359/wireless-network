﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class BinaryExtension
    {
        /// <summary>
        /// Transform a DateTime in a Byte[] sort in Hours, Minutes and Seconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Byte[] ToBinaryEEPROM(this DateTime time)
        {
            Byte[] result = new Byte[3];
            result[0] = (byte)time.Hour;
            result[1] = (byte)time.Minute;
            result[2] = (byte)time.Second;

            return result;
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
}
