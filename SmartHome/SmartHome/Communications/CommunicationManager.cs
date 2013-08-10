#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerialPortManager.ConnectionManager;
using SmartHome.Communications.SerialManager;
using SmartHome.Comunications.Messages;
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

        private SerialManager serialManager;

        private NodeConnection masterConnection;

        private Dictionary<ushort, NodeConnection> connectionsInUse;

        private Dictionary<ushort, List<PendingMessage>> pendingMessagesQueue;

        private Dictionary<ushort, PendingMessage> currentMessages;

        public CommunicationManager()
        {
            this.connectionsInUse = new Dictionary<ushort, NodeConnection>();
            this.pendingMessagesQueue = new Dictionary<ushort, List<PendingMessage>>();
            this.currentMessages = new Dictionary<ushort, PendingMessage>();

            this.serialManager = new SerialManager();
            this.serialManager.NodeConnectionAdded += this.serialManager_NodeConnectionAdded;
            this.serialManager.NodeConnectionRemoved += this.serialManager_NodeConnectionRemoved;
        }

        void serialManager_NodeConnectionAdded(object sender, NodeConnection e)
        {
            if (e.NodeAddress == MASTER_ADDRESS)
            {
                masterConnection = e;
                connectionsInUse.Add(e.NodeAddress, e);
            }
            Debug.WriteLine(string.Format("DONGLE NODE 0x{0:X4} ({1}) Connected!", e.NodeAddress, e.NodeAddress == MASTER_ADDRESS ? "Master" : "Slave"));
        }

        void serialManager_NodeConnectionRemoved(object sender, NodeConnection e)
        {
            if (masterConnection == e)
            {
                masterConnection = null;
            }

            if (connectionsInUse.ContainsKey(e.NodeAddress))
                connectionsInUse.Remove(e.NodeAddress);

            //TODO: What to do with the other dictionaries

            Debug.WriteLine(string.Format("DONGLE NODE 0x{0:X4} ({1}) Disconnected!", e.NodeAddress, e.NodeAddress == MASTER_ADDRESS ? "Master" : "Slave"));
        }

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

        private Task<bool> EnqueueMessage(OutputHeader message, NodeConnection connection)
        {
            ushort connectionAddress = connection.NodeAddress;

            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            lock (this)
            {
                if (!this.pendingMessagesQueue.ContainsKey(connectionAddress))
                    this.pendingMessagesQueue.Add(connectionAddress, new List<PendingMessage>());

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
                if (!this.currentMessages.ContainsKey(connectionAddress))
                    this.currentMessages.Add(connectionAddress, null);

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
    }
}
