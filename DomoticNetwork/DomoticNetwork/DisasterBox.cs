using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork
{
    class DisasterBox
    {
        //private Byte[] TableEnableEventsFlags()
        //{
        //    List<Byte> result = new List<Byte>();
        //    byte pointer = 0x00;
        //    Connector c = null;

        //    result.Add(pointer); //el primero siempre es 00

        //    for (byte i = 0; i < ShieldNode.ShieldBase.NumPorts; i++)
        //    {
        //        for (byte j = 0; j < ShieldNode.ShieldBase.NumPins; j++)
        //        {
        //            c = ShieldNode.GetConector(i, j);
        //            if (c != null)
        //            {
        //                pointer += (byte)(c.ConnectorEvent.Count / 8);
        //            }
        //            else
        //            {
        //                pointer++;
        //                result.Add(pointer);
        //            }
        //        }
        //    }

        //    //Add free region
        //    pointer += (byte)(ShieldNode.TimeEvents.Count / 8);
        //    result.Add(pointer);

        //    return result.ToArray();
        //}

        ////OJO REVISAR
        //private Byte[] TableEventsFlags()
        //{
        //    List<Byte> result = new List<Byte>();
        //    Connector c = null;
        //    for (byte i = 0; i < ShieldNode.ShieldBase.NumPorts; i++)
        //    {
        //        for (byte j = 0; j < ShieldNode.ShieldBase.NumPins; j++)
        //        {
        //            c = ShieldNode.GetConector(i, j);
        //            if (c != null)
        //            {
        //                byte toResult = 0x00; //el byte que añadiremos a la memoria

        //                for (int k = 0; k < c.ConnectorEvent.Count; k++)
        //                {

        //                    if (c.ConnectorEvent[k].Enable == true)
        //                    {
        //                        toResult = (byte)(toResult | (0x01 << Math.Abs((k % 8) - 8)));  //NOTA MENTAL: Math.Abs((k % 8) - 8)) es la posicion que ocupa el enable en ese momento
        //                    }

        //                    if (Math.Abs((k % 8) - 8) == 0 || k + 1 == c.ConnectorEvent.Count) // Si hemos agotado la capacidad del byte o estamos en la ultima iteracion del for
        //                    {
        //                        result.Add(toResult);
        //                        toResult = 0x00;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                result.Add(0x00);
        //            }
        //        }
        //    }

        //    for (int i = 0; i < ShieldNode.TimeEvents.Count; i++)
        //    {
        //        byte toResult = 0x00; //el byte que añadiremos a la memoria

        //        if (ShieldNode.TimeEvents[i].Enable == true)
        //        {
        //            toResult = (byte)(toResult | (0x01 << Math.Abs((i % 8) - 8)));  //NOTA MENTAL: Math.Abs((i % 8) - 8)) es la posicion que ocupa el enable en ese momento
        //        }

        //        if (Math.Abs((i % 8) - 8) == 0 || i + 1 == ShieldNode.TimeEvents.Count) // Si hemos agotado la capacidad del byte o estamos en la ultima iteracion del for
        //        {
        //            result.Add(toResult);
        //            toResult = 0x00;
        //        }
        //    }
        //    return result.ToArray();
        //}
    }
}
