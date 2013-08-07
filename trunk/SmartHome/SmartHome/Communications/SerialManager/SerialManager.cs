using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Timers;

namespace SmartHome.Communications.SerialManager
{
    public class SerialManager
    {
        private Timer checkTimer;

        private List<NodeConnection> nodes;

        public event EventHandler NodeCollectionChanged;

        public SerialManager()
        {
            nodes = new List<NodeConnection>();

            checkTimer = new Timer()
            {
                Interval = 1000,
            };
            checkTimer.Elapsed += checkTimer_Elapsed;
            checkTimer.Start();
        }

        void checkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var portNames = SerialPort.GetPortNames();

            //Eliminamos los nodos que ya no están conectados fisicamente
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (!portNames.Contains(nodes[i].PortName))
                {
                    nodes[i].ConnectionStateChanged -= newNode_ConnectionStateChanged;
                    nodes.RemoveAt(i);
                }
            }

            //Añadimos los nuevos nodos
            foreach (string portName in portNames.Where(p => !nodes.Exists(n => n.PortName == p)))
            {
                var newNode = new NodeConnection(portName);
                newNode.Identify();
                newNode.ConnectionStateChanged += newNode_ConnectionStateChanged;
                nodes.Add(newNode);
            }
        }

        void newNode_ConnectionStateChanged(object sender, NodeConnection.ConnectionStates newState)
        {
            /*if (NodeCollectionChanged != null)
                EventHelper.RaiseEventOnUIThread(NodeCollectionChanged, new object[] { this, null });*/
        }

        public NodeConnection GetNodeConnection(ushort nodeAddress)
        {
            return nodes.First(n => n.NodeAddress == nodeAddress);
        }

        public NodeConnection GetMasterConnection()
        {
            return nodes.First(n => n.NodeType == NodeConnection.NodeTypes.Master);
        }

        public IEnumerable<NodeConnection> GetSlaveConections()
        {
            return nodes.Where(n => n.NodeType == NodeConnection.NodeTypes.Slave);
        }

        public IEnumerable<NodeConnection> GetAllConections()
        {
            return nodes.Where(n => n.NodeType != NodeConnection.NodeTypes.NotANode);
        }
    }
}
