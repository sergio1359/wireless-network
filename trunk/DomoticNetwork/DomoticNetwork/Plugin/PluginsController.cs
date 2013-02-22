using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork.Plugin
{
    /// <summary>
    /// Mantiene una referencia al plugin como así también información sobre el nombre del assembly que lo contiene y el AppDomain en donde se cargó. Esta clase también es la encargada de invocar al método Execute() del plugin con el mensaje adecuado y debería ser capaz (no lo es en este ejemplo) de controlar el tiempo de ejecución del mismo. Controla también que el Plugin no dispare una excepción y si lo hace simplemente descarga el AppDomain en el que se encuentra el Plugin.
    /// </summary>
    class PluginsController
    {
    }
}
