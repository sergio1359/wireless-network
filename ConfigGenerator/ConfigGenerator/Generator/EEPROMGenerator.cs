using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.EEPROM
{
    class Generator
    {
        public Device Device { get; set; }
        public Byte FirmVersion { get; set; }
        

        public Generator(Device device)
        {
            Device = device;
        }


        public Byte[] GenerateEEPROM() 
        {
            throw new NotImplementedException();






            //Generar toda la memoria (la memoria se genera con CRC16 a "00 00"





            //Generar el CRC




            //devolvemos el valor con el CRC sustituido

        }

    }



}
