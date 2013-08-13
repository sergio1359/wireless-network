using SmartHome.Comunications;
using SmartHome.Comunications.Messages;
using SmartHome.Comunications.Modules;
using SmartHome.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public event EventHandler<string> NetworkJoinReceived;

        public event EventHandler<string> NodeJoined;

        public NetworkJoin(CommunicationManager manager)
            : base(manager)
        {
            this.pendingRequests = new Dictionary<string, Tuple<ushort, byte[]>>();
        }

        public async Task<bool> AcceptNode(string macAddress, ushort newAddress, string securityKey)
        {
            return await this.SendJoinAcceptResponse(macAddress, newAddress, securityKey);
        }

        #region Overridden Methods
        public override void ProccessReceivedMessage(IMessage message)
        {
            OperationMessage operation = (OperationMessage)message;

            if (operation.OpCode == OperationMessage.OPCodes.JoinRequest)
            {
                Debug.WriteLine(String.Format("JOIN REQUEST RECEIVED FROM 0x{0:X4}", operation.SourceAddress));
                this.SendJoinRequestResponse(operation.SourceAddress).Wait();
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

                Debug.WriteLine(sb.ToString());

                string macString = macAddress.ToStrigMac();

                if (!this.pendingRequests.ContainsKey(macString))
                {
                    this.pendingRequests.Add(macString, new Tuple<ushort, byte[]>(operation.SourceAddress, aesKey));

                    if (NetworkJoinReceived != null)
                        NetworkJoinReceived(this, macString);
                }
            }
        }

        protected override Filter ConfigureInputFilter()
        {
            return new Filter()
            {
                Endpoint = Endpoints.APPLICATION_EP,
                FromMaster = true,
                OpCodeType = typeof(OperationMessage.OPCodes),
                OpCodes = new byte[] 
                { 
                    (byte)OperationMessage.OPCodes.JoinRequest, 
                    (byte)OperationMessage.OPCodes.JoinAccept, 
                    (byte)OperationMessage.OPCodes.JoinAbort,
                },
            };
        }

        protected override OutputParameters ConfigureOutputParameters()
        {
            return new OutputParameters(
                priority: 0.9f,
                endpoint: Endpoints.APPLICATION_EP,
                securityEnabled: false,
                routingEnabled: false);
        } 
        #endregion

        #region Private Methods
        private async Task<bool> SendJoinRequestResponse(ushort destinationAddress)
        {
            byte[] RSAKey = new byte[16];
            Random r = new Random();
            r.NextBytes(RSAKey);

            return await this.SendMessage(new OperationMessage()
            {
                SourceAddress = 0x00,
                DestinationAddress = destinationAddress,
                OpCode = OperationMessage.OPCodes.JoinRequestResponse,
                Args = RSAKey
            });
        }

        private async Task<bool> SendJoinAcceptResponse(string macAddress, ushort newAddress, string securityKey)
        {
            List<byte> operationArgs = new List<byte>();

            if (!pendingRequests.ContainsKey(macAddress))
            {
                throw new InvalidOperationException();
            }

            byte[] AESKey = this.pendingRequests[macAddress].Item2;

            operationArgs.AddRange(BitConverter.GetBytes(newAddress));
            operationArgs.AddRange(Encoding.ASCII.GetBytes(securityKey));

            bool result = await this.SendMessage(new OperationMessage() 
            { 
                SourceAddress = 0x00,
                DestinationAddress = this.pendingRequests[macAddress].Item1,
                OpCode = OperationMessage.OPCodes.JoinAcceptResponse,
                Args = operationArgs.ToArray() 
            });

            if (result)
            {
                this.pendingRequests.Remove(macAddress);

                Debug.WriteLine(String.Format("JOIN ACCEPTED {0} -> NEW ADDRESS: 0x{1:X2}", macAddress, newAddress));

                if (this.NodeJoined != null)
                    NodeJoined(this, macAddress);
            }

            return result;
        }
        #endregion
    }
}
