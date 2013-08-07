#region Using Statements
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Timers; 
#endregion

namespace SmartHome.Communications.SerialManager
{
    public class SerialManager
    {
        private Timer checkTimer;

        private List<NodeConnection> nodes;

        public event EventHandler<NodeConnection> NodeConnectionAdded;
        public event EventHandler<NodeConnection> NodeConnectionRemoved;

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

        private void checkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var portNames = SerialPort.GetPortNames();

            //Remove the nodes that are not phisically connected
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (!portNames.Contains(nodes[i].PortName))
                {
                    nodes.RemoveAt(i);

                    OnNodeConnectionRemoved(nodes[i]);
                }
            }

            //Add the new nodes
            foreach (string portName in portNames.Where(p => !nodes.Exists(n => n.PortName == p)))
            {
                var newNode = new NodeConnection(portName);
                newNode.Identify();
                nodes.Add(newNode);

                OnNodeConnectionAdded(newNode);
            }
        }

        private void OnNodeConnectionAdded(NodeConnection node)
        {
            if (NodeConnectionAdded != null)
                NodeConnectionAdded(this, node);
        }

        private void OnNodeConnectionRemoved(NodeConnection node)
        {
            if (NodeConnectionRemoved != null)
                NodeConnectionRemoved(this, node);
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
