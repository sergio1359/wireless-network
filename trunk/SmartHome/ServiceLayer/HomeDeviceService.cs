using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class HomeDeviceService
    {

        public void RemoveHomeDevice(HomeDevice homeDevice)
        {
            if (homeDevice.Connector != null)
            {
                //TODO UNLINK
            }

            NetworkManager.HomeDevices.Remove(homeDevice);
        }

        public void SetNameHomeDevice(HomeDevice homeDevice, string Name)
        {
            homeDevice.Name = Name;


        }

        public string[] GetTypesHomeDevices()
        {
            return Enum.GetNames(typeof(HomeDeviceType));
        }

        /// <summary>
        /// Acualiza la posicion de un HomeDevice
        /// </summary>
        /// <param name="homeDevice">El home device al que se quiere cambiar la posicion</param>
        /// <param name="Zone">Zona asignada</param>
        /// <param name="X">Posicion relativa en el eje X</param>
        /// <param name="Y">Posicion relativa en el eje Y</param>
        public void UpdatePosition(HomeDevice homeDevice, string Zone, int X, int Y)
        {
            homeDevice.Position.Zone = Zone;
            homeDevice.Position.X = X;
            homeDevice.Position.Y = Y;
        }

        /// <summary>
        /// Devuelve todos los homeDevices del sistema (estén o no conectados a un nodo)
        /// </summary>
        /// <returns></returns>
        public List<HomeDevice> GetHomeDevices()
        {
            return NetworkManager.HomeDevices;
        }

        /// <summary>
        /// Devuelve los home devices conectados de una zona particular con un tipo determinado
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceType"></param>
        /// <returns></returns>
        public HomeDevice[] GetHomeDevices(string zona, string homeDeviceType)
        {
            HomeDeviceType type = (HomeDeviceType)Enum.Parse(typeof(HomeDeviceType), homeDeviceType);

            return NetworkManager.HomeDevices.Where(hd => hd.Position.Zone == zona && hd.HomeDeviceType == type && hd.Connector != null).ToArray();
        }

        /// <summary>
        /// Devuelve los conectores que tengan tipo List<Tipo> que estén Connected connectados en una zona concreta
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public HomeDevice[] GetHomeDevices(string zona, List<string> homeDeviceTypes, bool connected)
        {
            List<HomeDeviceType> types = new List<HomeDeviceType>();
            foreach (var item in homeDeviceTypes)
            {
                types.Add((HomeDeviceType)Enum.Parse(typeof(HomeDeviceType), item));
            }

            return NetworkManager.HomeDevices.Where(hd => hd.Position.Zone == zona && types.Contains(hd.HomeDeviceType) && hd.InUse == connected).ToArray();
        }
    }
}
