using ServiceLayer.DTO;
using SmartHome.HomeModel;
using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Connector connector = NetworkManager.Nodes.SelectMany(n => n.Connectors).First(con => con.Id == idConnector);
            HomeDevice homeDevice = NetworkManager.HomeDevices.FirstOrDefault(h => h.Id == idHomeDevice);

            if (connector.InUse)
                return 1;
            
            if (homeDevice.InUse)
                return 2;

            connector.LinkHomeDevice(homeDevice);
            homeDevice.LinkConnector(connector);

            return 0;
        }

        /// <summary>
        /// Desrelaciona un HomeDevice de su conector asociado
        /// </summary>
        public void UnlinkHomeDevice(int idHomeDevice)
        {
            HomeDevice homeDevice = NetworkManager.HomeDevices.FirstOrDefault(h => h.Id == idHomeDevice);
            homeDevice.Connector.UnlinkHomeDevice();
            homeDevice.UnlinkConnector();
        }

        /// <summary>
        /// Devuelve los conectores que pertenecen a un NODO
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Dicionario IDConnector, nombre, tipo, en uso</returns>
        public ConnectorDTO[] GetConnectors(int idNode)
        {
            return NetworkManager.Nodes.First(n => n.Id == idNode).Connectors.Select(c => new ConnectorDTO() { Id = c.Id, Name = c.Name, ConnectorType = Enum.GetName(typeof(ConnectorType), c.ConnectorType), InUse = c.InUse }).ToArray();
        }

        public string GetNameNode(int idNode)
        {
            return NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode).Name;
        }

        public int GetAddressNode(int idNode)
        {
            return NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode).Address;
        }

        public void UpdatePosition(int idNode, int idZone, float X, float Y)
        {
            Node node = NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode);
            node.Position.Zone = NetworkManager.Home.Zones.FirstOrDefault(z => z.Id == idNode);
            node.Position.ZoneCoordenates = new PointF(X, Y);
        }

        public Position GetNodePosition(int idNode)
        {
            return NetworkManager.Nodes.First(n => n.Id == idNode).Position;
        }

        /// <summary>
        /// Devuelve el id del conector, su nombre, tipo
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ConnectorDTO[] GetFreeConnectors(int idNode)
        {
            return NetworkManager.Nodes.First(n => n.Id == idNode).Connectors.Where(c => c.InUse == false).Select(c => new ConnectorDTO() { Id = c.Id, Name = c.Name, ConnectorType = Enum.GetName(typeof(ConnectorType), c.ConnectorType), InUse = c.InUse }).ToArray();
        }

        public NodeDTO[] GetNodes()
        {
            return NetworkManager.Nodes.Select(n => new NodeDTO() { Id = n.Id, Name = n.Name, Base = Enum.GetName(typeof(BaseType), n.Base), Shield = Enum.GetName(typeof(ShieldType), n.Shield) }).ToArray();
        }

        public NodeDTO[] GetNode(int idZone)
        {
            return NetworkManager.Nodes.Where(n => n.Position.Id == idZone).Select(n => new NodeDTO() { Id = n.Id, Name = n.Name, Base = Enum.GetName(typeof(BaseType), n.Base), Shield = Enum.GetName(typeof(ShieldType), n.Shield) }).ToArray();
        }

        public string[] GetTypeShields()
        {
            return Enum.GetNames(typeof(ShieldType));
        }

        public string[] GetTypeBases()
        {
            return Enum.GetNames(typeof(BaseType));
        }
    }
}
