using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerialPortManager.ConnectionManager;
using SmartHome.Comunications;
using SmartHome.Comunications.Messages;
using SmartHome.Comunications.Modules;
using SmartHome.Memory;

namespace SmartHome.Communications.Modules
{
    public class NetworkJoin : ModuleBase
    {
        /// <summary>
        /// Dictionary of the MACs, tempAddress and tempAESKey from the nodes that are waiting to be accepted
        /// </summary>
        private Dictionary<string, Tuple<ushort, byte[]>> pendingRequests;

        /// <summary>
        /// List of the MACs from the nodes that are pending to join
        /// </summary>
        public List<string> PendingNodes
        {
            get
            {
                return this.pendingRequests.Keys.ToList();
            }
        }

        public NetworkJoin(CommunicationManager manager)
            : base(manager, 0.9f, Endpoints.APPLICATION_EP)
        {
            this.pendingRequests = new Dictionary<string, Tuple<ushort, byte[]>>();

            this.Filter.Endpoint = Endpoints.APPLICATION_EP;
            this.Filter.FromMaster = true;
            this.Filter.OpCodeType = typeof(OperationMessage.OPCodes);
            this.Filter.OpCodes = new byte[] 
            { 
                (byte)OperationMessage.OPCodes.JoinRequest, 
                (byte)OperationMessage.OPCodes.JoinAccept, 
                (byte)OperationMessage.OPCodes.JoinAbort,
            };
        }

        public void AcceptNode(string macAddress, ushort newAddress, string securityKey)
        {
            this.SendJoinAcceptResponse(macAddress, newAddress, securityKey);
        }

        public override void ProccessReceivedMessage(IMessage message)
        {
            OperationMessage operation = (OperationMessage)message;

            if (operation.OpCode == OperationMessage.OPCodes.JoinRequest)
            {
                Debug.WriteLine(String.Format("JOIN REQUEST RECEIVED FROM 0x{0:X4}", operation.SourceAddress));
                this.SendJoinRequestResponse(operation.SourceAddress);
            }
            else if (operation.OpCode == OperationMessage.OPCodes.JoinAbort)
            {
                Debug.WriteLine(String.Format("JOIN ABORT RECEIVED FROM 0x{0:X4} -> NUMBER OF RESPONSES: {1}", operation.SourceAddress, operation.Args[0]));
            }
            else if (operation.OpCode == OperationMessage.OPCodes.JoinAccept)
            {
                byte[] macAddress = operation.Args.Take(6).ToArray();
                byte[] aesKey = operation.Args.Skip(6).ToArray();

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("JOIN ACCEPT RECEIVED FROM 0x{0:X4} -> MAC: ", operation.SourceAddress);

                foreach (byte b in macAddress)
                {
                    sb.AppendFormat("0x{0:X2} ", b);
                }

                sb.Append("  AES-KEY: ");

                foreach (byte b in aesKey)
                {
                    sb.AppendFormat("0x{0:X2} ", b);
                }

                this.pendingRequests.Add(macAddress.ToStrigMac(), new Tuple<ushort, byte[]>(operation.SourceAddress, aesKey));

                Debug.WriteLine(sb.ToString());
            }
        }

        #region Private Methods
        private void SendJoinRequestResponse(ushort destinationAddress)
        {
            byte[] RSAKey = new byte[16];
            Random r = new Random();
            r.NextBytes(RSAKey);

            this.SendMessage(new OperationMessage()
            {
                SourceAddress = 0x00,
                DestinationAddress = destinationAddress,
                OpCode = OperationMessage.OPCodes.JoinRequestResponse,
                Args = RSAKey
            });
        }

        private void SendJoinAcceptResponse(string macAddress, ushort newAddress, string securityKey)
        {
            List<byte> operationArgs = new List<byte>();

            if (!pendingRequests.ContainsKey(macAddress))
            {
                throw new InvalidOperationException();
            }

            byte[] AESKey = this.pendingRequests[macAddress].Item2;

            operationArgs.AddRange(BitConverter.GetBytes(newAddress));
            operationArgs.AddRange(Encoding.ASCII.GetBytes(securityKey));

            this.SendMessage(new OperationMessage() 
            { 
                SourceAddress = 0x00,
                DestinationAddress = this.pendingRequests[macAddress].Item1,
                OpCode = OperationMessage.OPCodes.JoinAcceptResponse,
                Args = operationArgs.ToArray() 
            });
            
        }
        #endregion
    }
}
