using System;

namespace DomoticNetwork.EEPROM
{
    public class Crc16
    {
        const ushort polynomial = 0xA001;
        ushort[] table = new ushort[256];

        public Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }        
        
        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes, bool littleEndian)
        {
            Byte[] result = new Byte[2];
            ushort crc = ComputeChecksum(bytes);
            if (littleEndian)
            {
                result[0] = (byte)crc;
                result[1] = (byte)(crc >> 8);
            }
            else
            {
                result[0] = (byte)(crc >> 8);
                result[1] = (byte)crc;
            }
            return result;
        }

        
    }
}
