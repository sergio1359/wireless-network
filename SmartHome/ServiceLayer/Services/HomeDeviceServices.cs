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

        /// <summary>
        /// Add a new HomeDevice to the system.
        /// </summary>
        /// <param name="NameHomeDevice">Name of the new HomeDevice</param>
        /// <param name="TypeHomeDevice">Type of HomeDevice</param>
        /// <returns>Return the ID for the new HomeDevice</returns>
        public int AddHomeDevice(string NameHomeDevice, string TypeHomeDevice)
        {
            HomeDevice homeDevice = (HomeDevice)Activator.CreateInstance(Type.GetType(TypeHomeDevice));

            homeDevice.Name = NameHomeDevice;

            NetworkManager.HomeDevices.Add(homeDevice);

            return NetworkManager.HomeDevices.Last().Id;
        }

        /// <summary>
        /// Return the HomeDevice's types of the system.
        /// </summary>
        /// <returns></returns>
        public string[] GetTypesHomeDevice()
        {
            return HomeDevice.HomeDeviceTypes;
        }

        /// <summary>
        /// Remove, and unlink if is necesary, a HomeDevice of the system.
        /// </summary>
        /// <param name="idHomeDevice">Identificator of the HomeDevice to be remove.</param>
        public void RemoveHomeDevice(int idHomeDevice)
        {
            HomeDevice homeDevice = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);
            if (homeDevice.Connector != null)
            {
                Services.NodeService.UnlinkHomeDevice(idHomeDevice);
            }

            NetworkManager.HomeDevices.Remove(homeDevice);
        }

        /// <summary>
        /// Change the name of a one HomeDevice
        /// </summary>
        /// <param name="idHomeDevice">Identificator of the HomeDevice</param>
        /// <param name="NewName">New Name</param>
        public void SetNameHomeDevice(int idHomeDevice, string NewName)
        {
            NetworkManager.HomeDevices.FirstOrDefault(hd => hd.Id == idHomeDevice).Name = NewName;
        }


        /// <summary>
        /// Update the position of a HomeDevice
        /// </summary>
        /// <param name="homeDevice">Identificator of the home device to be move</param>
        /// <param name="Zone">new zone</param>
        /// <param name="X">Relative position 0 to 1 of the X axis</param>
        /// <param name="Y">Relative position 0 to 1 of the X axis</param>
        public void UpdatePosition(int idHomeDevice, int idZone, float X, float Y)
        {
            HomeDevice home = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);
            Zone zone = NetworkManager.Home.Zones.First(z => z.Id == idZone);

            home.Position.Zone = zone;
            home.Position.ZoneCoordenates = new PointF(X, Y);
        }

        public void UpdateViewPosition(int idHomeDevice, int idView, float X, float Y)
        {
            throw new NotImplementedException();
        }

        public Position GetHomeDevicePosition(int idHomeDevice)
        {
            return NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice).Position;
        }

        /// <summary>
        /// Return all HomeDevices of the system (be or not be connected to a Node)
        /// </summary>
        /// <returns>Dictionary of Ids, Nombres, Tipos</returns>
        public Dictionary<int, Tuple<string, string>> GetHomeDevices()
        {
            return NetworkManager.HomeDevices.ToDictionary(h => (int)h.Id, h => new Tuple<string, string>(h.Name, h.HomeDeviceType));
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone and type
        /// </summary>
        /// <param name="zona">Identificator of the zone</param>
        /// <param name="homeDeviceType">Identificator of the type</param>
        /// <returns>Return Dictionary with ID, Name and type</returns>
        public Dictionary<int, Tuple<string, string>> GetHomeDevices(int idZona, string type)
        {
            return NetworkManager.HomeDevices.Where(hd => hd.Connector != null && hd.Position.Zone.Id == idZona && hd.HomeDeviceType == type).ToDictionary(h => (int)h.Id, h => new Tuple<string, string>(h.Name, h.HomeDeviceType));
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone, a list of types and if are connected or not.
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public Dictionary<int, Tuple<string, string>> GetHomeDevices(int idZona, List<string> homeDeviceTypes, bool connected)
        {
            return NetworkManager.HomeDevices.Where(hd => hd.Position.Zone.Id == idZona && homeDeviceTypes.Contains(hd.HomeDeviceType) && hd.InUse == connected).ToDictionary(h => (int)h.Id, h => new Tuple<string, string>(h.Name, h.HomeDeviceType));
        }

        /// <summary>
        /// Devuelve los conectores que se pueden conectar con el homeDevice enviado por parametros
        /// </summary>
        /// <param name="TypeHomeDevice"></param>
        /// <returns></returns>
        public Dictionary<int, Tuple<string, string, bool>> GetConnectorsCapable(int idHomeDevice, int idNode)
        {
            HomeDevice homeDev = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);

            return NetworkManager.Nodes.First(n => n.Id == idNode).Connectors.Where(c => c.ConnectorType == homeDev.ConnectorCapable).ToDictionary(c => c.Id, c => new Tuple<string, string, bool>(c.Name, Enum.GetName(typeof(ConnectorType), c.ConnectorType), c.InUse));
        }
    }
}
