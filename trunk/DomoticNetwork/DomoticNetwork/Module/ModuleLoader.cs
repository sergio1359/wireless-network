using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork.Module
{
    class ModuleLoader
    {

        public List<IModule> Modules { get; private set; }

        public ModuleLoader()
        {
            Modules = new List<IModule>();
        }

        public void LoadModule(string file)
        {
            Assembly moduleAssembly;

            if (IsModule(file))
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
                            if (moduleType.GetInterface(typeof(IModule).FullName, true) != null)
                            {
                                IModule i = (IModule)Activator.CreateInstance(moduleAssembly.GetType(moduleType.ToString()));
                                Modules.Add(i);
                                break;
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

        public bool IsModule(string file)
        {
            return file.EndsWith(".dll");
        }
    }
}
