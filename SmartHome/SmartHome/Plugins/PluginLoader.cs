using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SmartHome.Plugins
{
    /// <summary>
    /// Un singleton encargado de realizar la primera carga de los assemblies (plugins) y de vigilar los eventos sobre la carpeta "\Plugins" y cuando detecta un cambio descargar los plugins y los cargalos de nuevo. Esta clase mantiene una instancia de PluginsContainer.
    /// </summary>
    class PluginLoader
    {

        public List<IConfiguration> Modules { get; private set; }
        public const String DIRECTORY_PLUGIN = "//Plugins";
        public const String EXTENSION = ".dll";


        public PluginLoader()
        {
            Modules = new List<IConfiguration>();

            //recorro cada fichero del directorio en el que se encuentra esta aplicacion e intento cargarlo como módulo.
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory() + DIRECTORY_PLUGIN))
            {
                LoadPlugin(file);
            }
        }

        public void LoadPlugin(string file)
        {
            Assembly moduleAssembly;

            if (IsPluginExtension(file))
            {
                try
                {
                    moduleAssembly = Assembly.LoadFrom(file);

                    foreach (Type moduleType in moduleAssembly.GetTypes())
                    {
                        //Se trata de una clase publica, no abstacta.
                        if (moduleType.IsPublic && !moduleType.IsAbstract && moduleType.IsClass)
                        {
                            //Implementa la interfaz IModule.
                            if (moduleType.GetInterface(typeof(IConfiguration).FullName, true) != null)
                            {
                                IConfiguration i = (IConfiguration)Activator.CreateInstance(moduleAssembly.GetType(moduleType.ToString()));
                                Modules.Add(i);
                                //break;
                            }
                        }
                    }

                }
                catch (Exception)
                { }
                finally
                { moduleAssembly = null; }
            }
        }

        public bool IsPluginExtension(string file)
        {
            return file.EndsWith(EXTENSION);
        }
    }
}
