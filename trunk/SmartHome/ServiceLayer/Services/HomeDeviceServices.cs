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
using ServiceLayer.DTO;
using AutoMapper;
using System.Reflection;

namespace ServiceLayer
{
    public class HomeDeviceService
    {
        /// <summary>
        /// Add a new HomeDevice to the system.
        /// </summary>
        /// <param name="nameHomeDevice">Name of the new HomeDevice</param>
        /// <param name="homeDeviceType">Type of HomeDevice</param>
        /// <returns>Return the ID for the new HomeDevice</returns>
        public int AddHomeDevice(string nameHomeDevice, string homeDeviceType)
        {
            Type deviceType = typeof(HomeDevice).Assembly.GetTypes().First(t => t.Name == homeDeviceType);

            HomeDevice homeDevice = (HomeDevice)Activator.CreateInstance(deviceType);

            homeDevice.Name = nameHomeDevice;

            NetworkManager.HomeDevices.Add(homeDevice);

            return NetworkManager.HomeDevices.Last().Id;
        }

        /// <summary>
        /// Return the HomeDevice's types of the system.
        /// </summary>
        /// <returns>Array with the types names</returns>
        public string[] GetHomeDeviceTypes()
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
        /// <param name="newName">New Name</param>
        public void SetNameHomeDevice(int idHomeDevice, string newName)
        {
            NetworkManager.HomeDevices.FirstOrDefault(hd => hd.Id == idHomeDevice).Name = newName;
        }


        /// <summary>
        /// Update the position of a HomeDevice in a Zone
        /// </summary>
        /// <param name="homeDevice">Identificator of the home device to be move</param>
        /// <param name="Zone">new zone</param>
        /// <param name="x">Relative position 0 to 1 of the X axis</param>
        /// <param name="y">Relative position 0 to 1 of the X axis</param>
        public void UpdateZonePosition(int idHomeDevice, int idZone, float x, float y)
        {
            HomeDevice home = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);
            Zone zone = NetworkManager.Home.Zones.First(z => z.Id == idZone);

            home.Position.Zone = zone;
            home.Position.ZoneCoordenates = new PointF(x, y);
        }

        /// <summary>
        /// Update the position of a HomeDevice in a View
        /// </summary>
        /// <param name="homeDevice">Identificator of the home device to be move</param>
        /// <param name="Zone">new zone</param>
        /// <param name="x">Relative position 0 to 1 of the X axis</param>
        /// <param name="y">Relative position 0 to 1 of the X axis</param>
        public void UpdateViewPosition(int idHomeDevice, int idView, float x, float y)
        {
            throw new NotImplementedException();
        }

        public PositionDTO GetHomeDevicePosition(int idHomeDevice)
        {
            return Mapper.Map<PositionDTO>(NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice).Position);
        }

        /// <summary>
        /// Return all HomeDevices of the system (be or not be connected to a Node)
        /// </summary>
        /// <returns>Return a HomeDeviceDTO</returns>
        public HomeDeviceDTO[] GetHomeDevices()
        {
            return Mapper.Map<List<HomeDeviceDTO>>(NetworkManager.HomeDevices).ToArray();
        }

        /// <summary>
        /// Return all HomeDevices of the system (be or not be connected to a Node)
        /// </summary>
        /// <returns>Return a HomeDeviceDTO</returns>
        public HomeDeviceDTO[] GetHomeDevices(bool isInUse)
        {
            var homeDevices = NetworkManager.HomeDevices.Where(hd => hd.InUse == isInUse);

            return Mapper.Map<List<HomeDeviceDTO>>(homeDevices).ToArray();
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone and type
        /// </summary>
        /// <param name="zona">Identificator of the zone</param>
        /// <param name="homeDeviceType">Identificator of the type</param>
        /// <returns>Return Dictionary with ID, Name and type</returns>
        public HomeDeviceDTO[] GetHomeDevices(int idZona, string type)
        {
            var homeDevices = NetworkManager.HomeDevices.Where(hd => hd.Connector != null && hd.Position.Zone.Id == idZona && hd.HomeDeviceType == type);

            return Mapper.Map<List<HomeDeviceDTO>>(homeDevices).ToArray();
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone, a list of types and if are connected or not.
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public HomeDeviceDTO[] GetHomeDevices(int idZona, List<string> homeDeviceTypes, bool connected)
        {
            var homeDevices = NetworkManager.HomeDevices.Where(hd => hd.Position.Zone.Id == idZona && homeDeviceTypes.Contains(hd.HomeDeviceType) && hd.InUse == connected);

            return Mapper.Map<List<HomeDeviceDTO>>(homeDevices).ToArray();
        }

        /// <summary>
        /// Devuelve los conectores que se pueden conectar con el homeDevice enviado por parametros
        /// </summary>
        /// <param name="HomeDeviceType"></param>
        /// <returns></returns>
        public ConnectorDTO[] GetConnectorsCapable(int idHomeDevice, int idNode)
        {
            HomeDevice homeDev = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);

            var connectors = NetworkManager.Nodes.First(n => n.Id == idNode).Connectors.Where(c => c.ConnectorType == homeDev.ConnectorCapable);

            return Mapper.Map<List<ConnectorDTO>>(connectors).ToArray();
        }
    }
}
