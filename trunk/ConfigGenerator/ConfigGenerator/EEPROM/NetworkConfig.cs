using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROM
{
    class NetworkConfig
    {
        UInt16 deviceAddress;
        Byte channel;
        UInt16 panId;
        Byte[] securityKey;

        public void GenerateKey(String key)
        {
        }


        public Byte[] ToBinary()
        {
            Byte[] result = new Byte[5 + 16];
            result[0] = (byte)deviceAddress;
            result[1] = (byte)(deviceAddress >> 8);
            result[2] = channel;
            result[3] = (byte)panId;
            result[4] = (byte)(panId >> 8);
            for (int i = 0; i < securityKey.Length; i++)
            {
                result[i + 5] = securityKey[i];
            }
            return result;
        }
    }
}
