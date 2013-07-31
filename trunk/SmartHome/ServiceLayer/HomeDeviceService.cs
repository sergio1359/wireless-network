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
        private ConnectorRepository _connectorRepository;

        public HomeDeviceService(ConnectorRepository connectorRepository)
        {
            _connectorRepository = connectorRepository;
        }

        /// <summary>
        /// Añade un HomeDevice
        /// </summary>
        /// <param name="homeDevice"></param>
        /// <returns></returns>
        public int AddHomeDevice(string NameHomeDevice, string TypeHomeDevice)
        {

            //NetworkManager.HomeDevices.Add(homeDevice);
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
        public List<HomeDevice> GetHomeDevices(int idZona, string type)
        {
            return NetworkManager.HomeDevices.Where(hd => hd.Connector != null && hd.Position.Zone.Id == idZona && hd.HomeDeviceType == type).ToList();
        }

        /// <summary>
        /// Devuelve los conectores que tengan tipo List<Tipo> que estén Connected connectados en una zona concreta
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public List<HomeDevice> GetHomeDevices(int idZona, List<string> homeDeviceTypes, bool connected)
        {
            return NetworkManager.HomeDevices.Where(hd => hd.Position.Zone.Id == idZona && homeDeviceTypes.Contains(hd.HomeDeviceType) && hd.InUse == connected).ToList();
        }

        public Connector[] GetConnectorsCapable(HomeDevice hd)
        {
            throw new NotImplementedException();
        }
    }
}
