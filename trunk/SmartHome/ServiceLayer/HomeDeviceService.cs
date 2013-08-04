using SmartHome.HomeModel;
using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DataLayer.Repositories;

namespace ServiceLayer
{
    public class HomeDeviceService
    {
        //private ConnectorRepository _connectorRepository;

        //public HomeDeviceService(ConnectorRepository connectorRepository)
        //{
        //    _connectorRepository = connectorRepository;
        //}

        /// <summary>
        /// Añade un HomeDevice
        /// </summary>
        /// <param name="homeDevice"></param>
        /// <returns></returns>
        public int AddHomeDevice(string NameHomeDevice, string TypeHomeDevice)
        {
            HomeDevice homeDevice = (HomeDevice)Activator.CreateInstance(Type.GetType(TypeHomeDevice));

            homeDevice.Name = NameHomeDevice;

            NetworkManager.HomeDevices.Add(homeDevice);

            return NetworkManager.HomeDevices.Last().Id;
        }

        public string[] GetTypesHomeDevice()
        {
            return HomeDevice.HomeDeviceTypes;
        }

        public void RemoveHomeDevice(int idHomeDevice)
        {
            HomeDevice homeDevice = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);
            if (homeDevice.Connector != null)
            { //TODO: MEJORAR ESTO!
                NodeService ns = new NodeService();
                ns.UnlinkHomeDevice(idHomeDevice);
            }

            NetworkManager.HomeDevices.Remove(homeDevice);
        }

        public void SetNameHomeDevice(int idHomeDevice, string NewName)
        {
            NetworkManager.HomeDevices.FirstOrDefault(hd => hd.Id == idHomeDevice).Name = NewName;
        }


        /// <summary>
        /// Acualiza la posicion de un HomeDevice
        /// </summary>
        /// <param name="homeDevice">El home device al que se quiere cambiar la posicion</param>
        /// <param name="Zone">Zona asignada</param>
        /// <param name="X">Posicion relativa en el eje X</param>
        /// <param name="Y">Posicion relativa en el eje Y</param>
        public void UpdatePosition(int idHomeDevice, int idZone, int X, int Y)
        {
            HomeDevice home = NetworkManager.HomeDevices.FirstOrDefault(hd => hd.Id == idHomeDevice);
            Zone zone = NetworkManager.Home.Zones.FirstOrDefault(z => z.Id == idZone);

            home.Position.Zone = zone;
            home.Position.ZoneCoordenates = new PointF(X, Y);
        }

        //HAY QUE VOLVER A PROGRAMARLOS PENSANDO QUE ME VOY A LLEVAR
        /// <summary>
        /// Devuelve todos los homeDevices del sistema (estén o no conectados a un nodo)
        /// </summary>
        /// <returns>Diccionario de Ids, Nombres, Tipos</returns>
        public Dictionary<ushort, Tuple<string, string>> GetHomeDevices()
        {
            return NetworkManager.HomeDevices.ToDictionary(h => h.Id, h => new Tuple<string, string>(h.Name, h.HomeDeviceType));
        }

        /// <summary>
        /// Devuelve los home devices conectados de una zona particular con un tipo determinado
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceType"></param>
        /// <returns></returns>
        public Dictionary<ushort, Tuple<string, string>> GetHomeDevices(int idZona, string type)
        {
            return NetworkManager.HomeDevices.Where(hd => hd.Connector != null && hd.Position.Zone.Id == idZona && hd.HomeDeviceType == type).ToDictionary(h => h.Id, h => new Tuple<string, string>(h.Name, h.HomeDeviceType));
        }

        /// <summary>
        /// Devuelve los conectores que tengan tipo List<Tipo> que estén Connected connectados en una zona concreta
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public Dictionary<ushort, Tuple<string, string>> GetHomeDevices(int idZona, List<string> homeDeviceTypes, bool connected)
        {
            return NetworkManager.HomeDevices.Where(hd => hd.Position.Zone.Id == idZona && homeDeviceTypes.Contains(hd.HomeDeviceType) && hd.InUse == connected).ToDictionary(h => h.Id, h => new Tuple<string, string>(h.Name, h.HomeDeviceType));
        }

        /// <summary>
        /// Devuelve los conectores que se pueden conectar con el homeDevice enviado por parametros
        /// </summary>
        /// <param name="TypeHomeDevice"></param>
        /// <returns></returns>
        public Dictionary<int, Tuple<string, string, bool>> GetConnectorsCapable(ushort idHomeDevice, ushort idNode)
        {
            HomeDevice homeDev = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);

            return NetworkManager.Nodes.First(n => n.Id == idNode).Connectors.Where(c => c.ConnectorType == homeDev.ConnectorCapable).ToDictionary(c => c.Id, c => new Tuple<string, string, bool>(c.Name, Enum.GetName(typeof(ConnectorType), c.ConnectorType), c.InUse));
        }
    }
}
