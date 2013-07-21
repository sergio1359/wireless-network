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
        /// 2 == el homeDevice ya estaba conectado en un conector diferente al nuevo</returns>
        public int LinkHomeDevice(int idConnector, int idHomeDevice)
        {
            Connector connector = NetworkManager.Nodes.SelectMany(n => n.Connectors).First(con => con.Id == idConnector);
            HomeDevice homeDevice = NetworkManager.HomeDevices.FirstOrDefault(h => h.Id == idHomeDevice);

            if (connector.HomeDevice != null)
                return 1;

            if (homeDevice.InUse)
                return 2;


            connector.HomeDevice = homeDevice;
            homeDevice.Connector = connector;
            return 0;
        }

        /// <summary>
        /// Desrelaciona un HomeDevice de su conector asociado
        /// </summary>
        public void Unlink(int idHomeDevice)
        {
            HomeDevice homeDevice = NetworkManager.HomeDevices.FirstOrDefault(h => h.Id == idHomeDevice);
            homeDevice.Connector.HomeDevice = null;
            homeDevice.Connector = null;
        }

        public Connector[] GetConnectors(Node node)
        {
            throw new NotImplementedException();
        }

        public string GetNameNode(int idNode)
        {
            return NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode).Name;
        }

        public ushort GetAddressNode(int idNode)
        {
            return NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode).Address;
        }

        public void UpdatePosition(int idNode, int idZone, float X, float Y)
        {
            Node node = NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode);
            node.Position.Zone = NetworkManager.Home.Zones.FirstOrDefault(z => z.Id == idNode);
            node.Position.ZoneCoordenates = new PointF(X, Y);
        }

        public Connector[] GetFreeConnectors(Node node)
        {
            throw new NotImplementedException();
        }

        public Node[] GetNodes()
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, Tuple<string, Position>> GetNode(int idZone)
        {
            Dictionary<int, Tuple<string, Position>> res = new Dictionary<int,Tuple<string,Position>>();
            NetworkManager.Nodes.Where(n => n.Position.Id == idZone).ToList().ForEach(n => { res.Add(n.Id, new Tuple<string, Position>(n.Name, n.Position)); });
            return res;
        }
    }
}
