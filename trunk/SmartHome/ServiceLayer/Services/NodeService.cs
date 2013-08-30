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
using SmartHome.Communications;
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
            using (UnitOfWork repository = new UnitOfWork())
            {
                var connector = repository.ConnectorRepository.GetById(idConnector);
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (connector == null || homeDevice == null)
                    return 4;

                if (connector.InUse)
                    return 1;

                if (homeDevice.InUse)
                    return 2;

                //UPDATE CHECKSUM
                connector.Node.UpdateChecksum(null);

                connector.LinkHomeDevice(homeDevice);

                repository.Commit();
            }

            return 0;
        }


        /// <summary>
        /// Unlink a HomeDevice from the connector associated
        /// </summary>
        public void UnlinkHomeDevice(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice != null)
                {
                    //UPDATE CHECKSUM
                    homeDevice.Connector.Node.UpdateChecksum(null);

                    homeDevice.Connector.UnlinkHomeDevice();
                    repository.Commit();
                }
            }
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
            using (UnitOfWork repository = new UnitOfWork())
            {
                Home home = repository.HomeRespository.GetHome();

                Node node = repository.NodeRespository.GetByMacAddress(MAC);

                NetworkJoin joinMod = CommunicationManager.Instance.FindModule<NetworkJoin>();

                PendingNodeInfo info = joinMod.PendingNodes.FirstOrDefault(n => n.MacAddress == MAC);

                if (info != null)
                {
                    if (node == null)
                    {
                        node = BusinessNode.CreateNode((BaseTypes)info.BaseType, (ShieldTypes)info.ShieldType);
                        node.Mac = info.MacAddress;
                        node.Address = repository.NodeRespository.GetNewAddress();

                        node = repository.NodeRespository.Insert(node);
                        repository.Commit();
                    }

                    return await joinMod.AcceptNode(MAC, (ushort)node.Address, home.Security);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Return the Connector of the Node
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Dicionario IDConnector, nombre, tipo, en uso</returns>
        public ConnectorDTO[] GetConnectors(int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var node = repository.NodeRespository.GetById(idNode);

                if (node == null)
                    return null;

                return Mapper.Map<ConnectorDTO[]>(node.Connectors);
            }
        }


        /// <summary>
        /// Devuelve los conectores que se pueden conectar con el homeDevice enviado por parametros
        /// </summary>
        /// <param name="HomeDeviceType"></param>
        /// <returns></returns>
        public ConnectorDTO[] GetConnectorsCapable(int idHomeDevice, int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var node = repository.NodeRespository.GetById(idNode);
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (node == null || homeDevice == null)
                    return null;

                var connectorsResult = node.Connectors.Where(c => c.IsCapable(homeDevice) && c.InUse == false);

                return Mapper.Map<ConnectorDTO[]>(connectorsResult);
            }
        }

        public string GetNameNode(int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var node = repository.NodeRespository.GetById(idNode);

                if (node == null)
                    return null;

                return node.Name;
            }
        }

        public void SetNameNode(int idNode, string newName)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Node node = repository.NodeRespository.GetById(idNode);

                if (node != null)
                {
                    node.Name = newName;
                    repository.Commit();
                }
            }
        }

        public int GetAddressNode(int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var node = repository.NodeRespository.GetById(idNode);

                if (node == null)
                    return -1;

                return node.Address;
            }
        }

        public void SetAddressNode(int idNode, ushort newAddress)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Node node = repository.NodeRespository.GetById(idNode);

                if (node != null)
                {
                    //UPDATE CHECKSUM
                    node.UpdateChecksum(null);

                    node.Address = newAddress;
                    repository.Commit();
                }
            }
        }

        public LocationDTO GetNodePosition(int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var node = repository.NodeRespository.GetById(idNode);

                if (node == null)
                    return null;

                return Mapper.Map<LocationDTO>(node.Location);
            }
        }

        public NodeDTO[] GetNodes()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var nodes = repository.NodeRespository.GetAll();

                return Mapper.Map<NodeDTO[]>(nodes);
            }
        }

        public NodeDTO[] GetNodes(int idZone)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                if (repository.ZoneRepository.GetById(idZone) == null)
                    return null;

                var nodes = repository.NodeRespository.GetAll().Where(n => n.Location.Id == idZone);

                return Mapper.Map<NodeDTO[]>(nodes);
            }
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

        public bool LinkProduct(int idConnector, string typeProduct)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Connector connector = repository.ConnectorRepository.GetById(idConnector);

                if (connector == null)
                    return false;

                if (connector.InUse)
                    return false;

                Type type = Type.GetType(typeProduct);

                connector.LinkHomeDevice(type);
                return true;
            }
        }

        public bool UnlinkProduct(int idConnector)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Connector connector = repository.ConnectorRepository.GetById(idConnector);

                if (connector == null)
                    return false;

                connector.UnlinkHomeDevice();
                return true;
            }
        }

        public ConnectorDTO[] GetConnectorProductsConnected()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var connectors = repository.NodeRespository.GetAll().SelectMany(n => n.Connectors).Where(c => c.Product != null);

                return Mapper.Map<ConnectorDTO[]>(connectors);
            }
        }

        //public ConnectorDTO[] GetConnectorCapableProducts(int idNode, string type)
        //{

        //}
    }
}
