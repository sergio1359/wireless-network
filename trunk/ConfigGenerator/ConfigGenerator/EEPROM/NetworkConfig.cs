using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROM
{
    class NetworkConfig
    {
        //aunque cada uno tiene una direccion asociada despues hay cosas que si que no cambian por ejemplo ¿todos los dispositivos trabajan en el mismo canal?
        //bajo la misma contraseña?
        //esto hay que sacarlo fuera
        public UInt16 DeviceAddress { set; get; }
        public Byte Channel { set; get; }
        public UInt16 PanId { set; get; }
        public Byte[] SecurityKey { set; get; }

        public void GenerateKey(String key)
        {
        }


        public Byte[] ToBinary()
        {
            Byte[] result = new Byte[5 + 16];
            result[0] = (byte)DeviceAddress;
            result[1] = (byte)(DeviceAddress >> 8);
            result[2] = Channel;
            result[3] = (byte)PanId;
            result[4] = (byte)(PanId >> 8);
            for (int i = 0; i < SecurityKey.Length; i++)
            {
                result[i + 5] = SecurityKey[i];
            }
            return result;
        }
    }
}
