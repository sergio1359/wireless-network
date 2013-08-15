#region Using Statements
using DataLayer.Entities.Enums;
using ServiceLayer.DTO;
using System;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using DataLayer;
using SmartHome.BusinessEntities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using AutoMapper;
using System.Linq;
#endregion

namespace ServiceLayer
{
    public class NodeService
    {
        /// <summary>
        /// Conecta un conector con un home device
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="homeDevice"></param>
        /// <returns>0 == OK, 
        /// 1 == el conector estaba ya ocupado por un home Device
        /// 2 == el homeDevice ya estaba conectado en un conector diferente al nuevo
        /// 3 == el homeDevice no es compatible con el conector</returns>
        public int LinkHomeDevice(int idConnector, int idHomeDevice)
        {
            Connector connector = Repositories.ConnectorRepository.GetById(idConnector);
            HomeDevice homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            if (connector.InUse)
                return 1;
            
            if (homeDevice.InUse)
                return 2;

            connector.LinkHomeDevice(homeDevice);

            return 0;
        }


        /// <summary>
        /// Unlink a HomeDevice from the connector associated
        /// </summary>
        public void UnlinkHomeDevice(int idHomeDevice)
        {
            HomeDevice homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);
            homeDevice.Connector.UnlinkHomeDevice();
        }

        /// <summary>
        /// Return the MAC of the Pending Nodes
        /// </summary>
        /// <returns>Return string for the MACs</returns>
        public string[] GetPendingNodes()
        {
            throw new NotImplementedException();
            //VICTOR
        }

        /// <summary>
        /// Allow a MAC in the system.
        /// </summary>
        /// <param name="MAC"></param>
        public void AllowPendingNode(string MAC)
        {
            throw new NotImplementedException();
            //VICTOR
        }

        /// <summary>
        /// Deny a MAC in the system
        /// </summary>
        /// <param name="MAC"></param>
        public void DenyPendingNode(string MAC)
        {
            throw new NotImplementedException();
            //VICTOR
        }

        /// <summary>
        /// Return the Connector of the Node
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Dicionario IDConnector, nombre, tipo, en uso</returns>
        public ConnectorDTO[] GetConnectors(int idNode)
        {
            var connectors = Repositories.NodeRespository.GetById(idNode).Connectors;

            return Mapper.Map<ConnectorDTO[]>(connectors);
        }


        /// <summary>
        /// Devuelve los conectores que se pueden conectar con el homeDevice enviado por parametros
        /// </summary>
        /// <param name="HomeDeviceType"></param>
        /// <returns></returns>
        public ConnectorDTO[] GetConnectorsCapable(int idHomeDevice, int idNode)
        {
            var connectors = Repositories.NodeRespository.GetById(idNode).Connectors;
            var homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            var connectorsResult = connectors.Where(c => c.HomeDevices.Contains(homeDevice) && c.InUse == false);

            return Mapper.Map<ConnectorDTO[]>(connectors);
        }

        public string GetNameNode(int idNode)
        {
            return Repositories.NodeRespository.GetById(idNode).Name;            
        }

        public void SetNameNode(int idNode, string newName)
        {
            var node = Repositories.NodeRespository.GetById(idNode);
            node.Name = newName;

            Repositories.SaveChanges();
        }

        public int GetAddressNode(int idNode)
        {
            return Repositories.NodeRespository.GetById(idNode).Address;
        }

        public void SetAddressNode(int idNode, ushort newAddress)
        {
            var node = Repositories.NodeRespository.GetById(idNode);
            node.Address = newAddress;

            Repositories.SaveChanges();
        }

        public void UpdatePosition(int idLocation, float x, float y)
        {
            throw new NotImplementedException();
        }

        public LocationDTO GetNodePosition(int idNode)
        {
            var location = Repositories.NodeRespository.GetById(idNode).Location;
            return Mapper.Map<LocationDTO>(location);
        }

        /// <summary>
        /// Devuelve el id del conector, su nombre, tipo
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ConnectorDTO[] GetFreeConnectors(int idNode)
        {
            var connectors = Repositories.NodeRespository.GetById(idNode).Connectors.Where(c => c.InUse == false);

            return Mapper.Map<ConnectorDTO[]>(connectors);
        }

        public NodeDTO[] GetNodes()
        {
            var nodes = Repositories.NodeRespository.GetAll();
            return Mapper.Map<NodeDTO[]>(nodes);
        }

        public NodeDTO[] GetNodes(int idZone)
        {
            var nodes = Repositories.NodeRespository.GetAll().Where(n => n.Location.Id == idZone);
            return Mapper.Map<NodeDTO[]>(nodes);
        }

        /// <summary>
        /// Obtain the types of shields
        /// </summary>
        /// <returns></returns>
        public string[] GetTypesShields()
        {
            return Enum.GetNames(typeof(ShieldTypes));
        }

        /// <summary>
        /// Obtain the types of bases
        /// </summary>
        /// <returns></returns>
        public string[] GetTypesBases()
        {
            return Enum.GetNames(typeof(BaseTypes));
        }
    }
}
