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
using SmartHome.Comunications;
using SmartHome.Communications.Modules.Config;
using SmartHome.Communications.Modules;
using System.Threading.Tasks;
using SmartHome.Communications.Modules.Network;
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
        /// 3 == el homeDevice no es compatible con el conector
        /// 4 == el connector o el homeDevice no existe</returns>
        public int LinkHomeDevice(int idConnector, int idHomeDevice)
        {
            Connector connector = Repositories.ConnectorRepository.GetById(idConnector);
            HomeDevice homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            if (connector == null || homeDevice == null)
                return 4;

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

            if (homeDevice == null)
                return;

            homeDevice.Connector.UnlinkHomeDevice();
        }

        /// <summary>
        /// Return the MAC of the Pending Nodes
        /// </summary>
        /// <returns>Return string for the MACs</returns>
        public PendingNodeInfoDTO[] GetPendingNodes()
        {
            var pendingInfo = CommunicationManager.Instance.FindModule<NetworkJoin>().PendingNodes;
            return Mapper.Map<PendingNodeInfoDTO[]>(pendingInfo);
        }

        /// <summary>
        /// Allow a MAC in the system.
        /// </summary>
        /// <param name="MAC">The MAC.</param>
        /// <returns>
        /// True if the node was allowed successfuly. False otherwise
        /// </returns>
        public async Task<bool> AllowPendingNode(string MAC)
        {
            Home home = Repositories.HomeRespository.GetHome();

            Node node = Repositories.NodeRespository.GetByMacAddress(MAC);

            NetworkJoin joinMod = CommunicationManager.Instance.FindModule<NetworkJoin>();

            PendingNodeInfo info = joinMod.PendingNodes.FirstOrDefault(n => n.MacAddress == MAC);

            if (info != null)
            {
                if (node == null)
                {
                    node = BusinessNode.CreateNode((BaseTypes)info.BaseType, (ShieldTypes)info.ShieldType);
                    node.Mac = info.MacAddress;
                    node.Address = Repositories.NodeRespository.GetNewAddress();

                    node = Repositories.NodeRespository.Insert(node);
                }

                return await joinMod.AcceptNode(MAC, (ushort)node.Address, home.Security);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return the Connector of the Node
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Dicionario IDConnector, nombre, tipo, en uso</returns>
        public ConnectorDTO[] GetConnectors(int idNode)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if (node == null)
                return null;

            var connectors = node.Connectors;

            return Mapper.Map<ConnectorDTO[]>(connectors);
        }


        /// <summary>
        /// Devuelve los conectores que se pueden conectar con el homeDevice enviado por parametros
        /// </summary>
        /// <param name="HomeDeviceType"></param>
        /// <returns></returns>
        public ConnectorDTO[] GetConnectorsCapable(int idHomeDevice, int idNode)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);
            HomeDevice homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            if (node == null || homeDevice == null)
                return null;

            var connectors = node.Connectors;

            var connectorsResult = connectors.Where(c => c.IsCapable(homeDevice) && c.InUse == false);

            return Mapper.Map<ConnectorDTO[]>(connectorsResult);
        }

        public string GetNameNode(int idNode)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if (node == null)
                return null;

            return node.Name;
        }

        public void SetNameNode(int idNode, string newName)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if (node == null)
                return;

            node.Name = newName;

            Repositories.SaveChanges();
        }

        public int GetAddressNode(int idNode)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if (node == null)
                return -1;

            return node.Address;
        }

        public void SetAddressNode(int idNode, ushort newAddress)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if (node == null)
                return;

            node.Address = newAddress;

            Repositories.SaveChanges();
        }

        public LocationDTO GetNodePosition(int idNode)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if (node == null)
                return null;

            return Mapper.Map<LocationDTO>(node.Location);
        }

        public NodeDTO[] GetNodes()
        {
            var nodes = Repositories.NodeRespository.GetAll();
            return Mapper.Map<NodeDTO[]>(nodes);
        }

        public NodeDTO[] GetNodes(int idZone)
        {
            if (Repositories.ZoneRepository.GetById(idZone) == null)
                return null;

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
