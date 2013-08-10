#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SerialPortManager.ConnectionManager;
using SmartHome.Communications.Modules;
using SmartHome.Communications.SerialManager;
using SmartHome.Comunications.Modules;
#endregion

namespace SmartHome.Comunications
{
    public class CommunicationManager
    {
        private class PendingMessage
        {
            public TaskCompletionSource<bool> TaskSource;

            public OutputHeader Message;

            public PendingMessage(TaskCompletionSource<bool> taskSource, OutputHeader message)
            {
                this.TaskSource = taskSource;
                this.Message = message;
            }
        }

        const ushort MASTER_ADDRESS = 0x4003;

        #region Private Vars
        private SerialManager serialManager;

        private NodeConnection masterConnection;

        private Dictionary<ushort, NodeConnection> connectionsInUse;

        private Dictionary<ushort, List<PendingMessage>> pendingMessagesQueue;

        private Dictionary<ushort, PendingMessage> currentMessages;

        private List<ModuleBase> modulesList;

        #endregion

        /// <summary>
        /// Raise when a new node was physically plugged to the system
        /// </summary>
        public event EventHandler<ushort> NodeConnectionDetected;

        /// <summary>
        /// Raise when a existing node was physically unplugged from the system
        /// </summary>
        public event EventHandler<ushort> NodeConnectionRemoved;

        public CommunicationManager()
        {
            this.connectionsInUse = new Dictionary<ushort, NodeConnection>();
            this.pendingMessagesQueue = new Dictionary<ushort, List<PendingMessage>>();
            this.currentMessages = new Dictionary<ushort, PendingMessage>();

            this.modulesList = new List<ModuleBase>();
            this.modulesList.Add(new NetworkJoin(this));
            //Sort by priority descendent
            this.modulesList.Sort((c, l) => c.Priority.CompareTo(l.Priority));

            this.serialManager = new SerialManager();
            this.serialManager.NodeConnectionDetected += this.serialManager_NodeConnectionAdded;
            this.serialManager.NodeConnectionRemoved += this.serialManager_NodeConnectionRemoved;
        }

        #region Private Methods
        private void serialManager_NodeConnectionAdded(object sender, NodeConnection e)
        {
            if (e.NodeAddress == MASTER_ADDRESS)
            {
                masterConnection = e;
                StoreConnection(e);
            }

            if (NodeConnectionDetected != null)
                NodeConnectionDetected(this, e.NodeAddress);

            Debug.WriteLine(string.Format("DONGLE NODE 0x{0:X4} ({1}) Connected!", e.NodeAddress, e.NodeAddress == MASTER_ADDRESS ? "Master" : "Slave"));
        }

        private void serialManager_NodeConnectionRemoved(object sender, NodeConnection e)
        {
            if (masterConnection == e)
            {
                masterConnection = null;
            }

            RemoveConnection(e);

            if (NodeConnectionRemoved != null)
                NodeConnectionRemoved(this, e.NodeAddress);

            //TODO: What to do with the other dictionaries

            Debug.WriteLine(string.Format("DONGLE NODE 0x{0:X4} ({1}) Disconnected!", e.NodeAddress, e.NodeAddress == MASTER_ADDRESS ? "Master" : "Slave"));
        }

        void connection_OperationReceived(object sender, InputHeader e)
        {
            //Send Message to the placeHolders
            foreach (var module in this.modulesList.Where(m => m.Filter.CheckMessage(e)))
            {
                module.ProccessReceivedMessage(e.Content);
            }
        }

        private Task<bool> EnqueueMessage(OutputHeader message, NodeConnection connection)
        {
            ushort connectionAddress = connection.NodeAddress;

            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            lock (this)
            {
                var currentQueue = this.pendingMessagesQueue[connectionAddress];
                PendingMessage newMsg = new PendingMessage(result, message);
                int index = 0;
                foreach (PendingMessage pendingMsg in currentQueue)
                {
                    if (newMsg.Message.Priority > pendingMsg.Message.Priority)
                    {
                        currentQueue.Insert(index, newMsg);
                        break;
                    }
                    index++;
                }
                if (index == currentQueue.Count)
                    currentQueue.Add(newMsg);

                this.CheckForSend(connectionAddress);
            }

            return result.Task;
        }

        private void CheckForSend(ushort connectionAddress)
        {
            lock (this)
            {
                var pendingMsg = this.currentMessages[connectionAddress];

                //Not busy
                if (pendingMsg == null)
                {
                    if (this.pendingMessagesQueue[connectionAddress].Count > 0) //SomethingToSend
                    {
                        Debug.WriteLine("Sending... " + DateTime.Now.Ticks);
                        pendingMsg = this.currentMessages[connectionAddress] = this.pendingMessagesQueue[connectionAddress][0];
                        this.pendingMessagesQueue[connectionAddress].Remove(pendingMsg);

                        Task.Factory.StartNew(async () =>
                        {
                            NodeConnection connection = masterConnection.NodeAddress == connectionAddress ? masterConnection : this.connectionsInUse[connectionAddress];

                            var val = await connection.SendMessage(pendingMsg.Message);

                            this.currentMessages[connectionAddress] = null;
                            this.CheckForSend(connectionAddress);

                            pendingMsg.TaskSource.SetResult(val);
                        });
                    }
                }
            }
        }

        private void StoreConnection(NodeConnection connection)
        {
            ushort connectionAddress = connection.NodeAddress;

            if (!this.connectionsInUse.ContainsKey(connectionAddress))
            {
                this.connectionsInUse.Add(connectionAddress, connection);
                this.pendingMessagesQueue.Add(connectionAddress, new List<PendingMessage>());
                this.currentMessages.Add(connectionAddress, null);

                connection.OperationReceived += connection_OperationReceived;
            }
        }

        private void RemoveConnection(NodeConnection connection)
        {
            ushort connectionAddress = connection.NodeAddress;

            if (this.connectionsInUse.ContainsKey(connectionAddress))
            {
                this.connectionsInUse.Remove(connectionAddress);
                this.pendingMessagesQueue.Remove(connectionAddress);
                this.currentMessages.Remove(connectionAddress);

                connection.OperationReceived -= connection_OperationReceived;
            }
        }
        #endregion

        #region Public Methods
        public async Task<bool> SendMessage(OutputHeader message)
        {
            ushort destinationAddress = message.Content.DestinationAddress;
            NodeConnection connection;

            lock (this)
            {
                // Try to use a cached direct connection to the destination address.
                if (connectionsInUse.ContainsKey(destinationAddress))
                {
                    connection = connectionsInUse[destinationAddress];
                    message.Content.DestinationAddress = 0;
                }
                else if ((connection = this.serialManager.GetNodeConnection(destinationAddress)) != null)
                {
                    connectionsInUse.Add(destinationAddress, connection);
                    message.Content.DestinationAddress = 0;
                }
                else if (masterConnection != null)
                {
                    // If not posible send to the master
                    connection = masterConnection;
                }
                else
                {
                    // If the master is not connected return false...
                    return false;
                }
            }

            //Enqueue the message
            return await EnqueueMessage(message, connection);
        }

        public T FindModule<T>() where T : ModuleBase
        {
            return this.modulesList.FirstOrDefault(m => m is T) as T;
        }
        #endregion
    }
}
