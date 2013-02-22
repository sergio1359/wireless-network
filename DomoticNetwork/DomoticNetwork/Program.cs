using DomoticNetwork.EEPROM;
using DomoticNetwork.NetworkModel;
using System;
using System.IO;

namespace DomoticNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            //EEPROMEXAMPLE
            Network network = new Network();
            network.AddNode();
            var add = network.Nodes[0].NodeAddress;
            Generator generatorEEPROM = new Generator(network, add);
            byte[] EEPROM = generatorEEPROM.GenerateEEPROM();
            //guardamos el bin
            File.WriteAllBytes("ex.bin", EEPROM);
            //guardamos el hex
            Binary.SaveBin2Hex(EEPROM);
            Console.WriteLine("DONE");
            Console.ReadLine();
            


            ////MODULE EXAMPLE
            //PluginLoader ml = new PluginLoader();
            ////Llamo al método "method" de cada módulo
            //foreach (IPlugin m in ml.Modules)
            //{
            //    m.method();
            //}
            ////END MODULE
            //Console.WriteLine("End");
            //Console.ReadLine();
        }
    }
}
