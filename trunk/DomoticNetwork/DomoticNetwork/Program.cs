using DomoticNetwork.NetworkModel;
using DomoticNetwork.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork
{
    class Program
    {
        static void Main(string[] args)
        {





            //MODULE
            PluginLoader ml = new PluginLoader();

            

            //Llamo al método "method" de cada módulo
            foreach (IPlugin m in ml.Modules)
            {
                m.method();
            }

            //END MODULE

            Console.WriteLine("End");
            Console.ReadLine();

        }
    }
}
