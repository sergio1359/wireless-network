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
using DataLayer.Entities;

namespace SmartHome.Communications.Modules.Network
{
    public class NetworkJoin : ModuleBase
    {
        /// <summary>
        /// List of the MACs from the nodes that are pending to join
        /// </summary>
        public List<PendingNodeInfo> PendingNodes
        {
            get;
            private set;
        }

        public event EventHandler<string> NetworkJoinReceived;

        public event EventHandler<string> NodeJoined;

        public NetworkJoin(CommunicationManager manager)
            : base(manager)
        {
            this.PendingNodes = new List<PendingNodeInfo>();
        }

        public async Task<bool> AcceptNode(string macAddress, ushort newAddress, Security security)
        {
            //TODO: check if macAddress exists
            return await this.SendJoinAcceptResponse(macAddress, newAddress, security);
        }

        #region Overridden Methods
        public override void ProcessReceivedMessage(IMessage message)
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
                byte baseModel = operation.Args[6];
                byte shieldModel = operation.Args[7];
                byte[] aesKey = operation.Args.Skip(8).ToArray();

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("JOIN ACCEPT RECEIVED FROM 0x{0:X4} -> MAC: ", operation.SourceAddress);

                foreach (byte b in macAddress)
                {
                    sb.AppendFormat("0x{0:X2} ", b);
                }

                sb.AppendFormat("  BASE-MODEL: {0}  SHIELD-MODEL: {1}", baseModel, shieldModel);

                sb.Append("  AES-KEY: ");

                foreach (byte b in aesKey)
                {
                    sb.AppendFormat("0x{0:X2} ", b);
                }

                Debug.WriteLine(sb.ToString());

                string macString = macAddress.ToStrigMac();

                PendingNodeInfo info = this.PendingNodes.FirstOrDefault(n => n.MacAddress == macString);

                if (info == null)
                {
                    this.PendingNodes.Add(new PendingNodeInfo()
                        {
                            MacAddress = macString,
                            BaseType = baseModel,
                            ShieldType = shieldModel,
                            TemporalAddress = operation.SourceAddress,
                            TemporalAESKey = aesKey,
                        });

                    if (NetworkJoinReceived != null)
                        NetworkJoinReceived(this, macString);
                }else{
                    //Refresh params
                    info.BaseType = baseModel;
                    info.ShieldType = shieldModel;
                    info.TemporalAddress = operation.SourceAddress;
                    info.TemporalAESKey = aesKey;
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

            OperationMessage joinResponse = OperationMessage.JoinRequestResponse(RSAKey, destinationAddress);

            return await this.SendMessage(joinResponse);
        }

        private async Task<bool> SendJoinAcceptResponse(string macAddress, ushort newAddress, Security security)
        {
            PendingNodeInfo info = this.PendingNodes.FirstOrDefault(n => n.MacAddress == macAddress);
            if (info == null)
            {
                throw new InvalidOperationException("There is no pending node with the specified MAC Address");
            }

            OperationMessage joinAcceptResponse = OperationMessage.JoinAcceptResponse(newAddress, (ushort)security.PanId, (byte)security.Channel, security.SecurityKey, info.TemporalAddress);

            bool result = await this.SendMessage(joinAcceptResponse);

            if (result)
            {
                this.PendingNodes.Remove(info);

                Debug.WriteLine(string.Format("JOIN ACCEPTED {0} -> NEW ADDRESS: 0x{1:X2}", macAddress, newAddress));

                if (this.NodeJoined != null)
                    NodeJoined(this, macAddress);
            }
            else
            {
                Debug.WriteLine(string.Format("THE NODE {0} DOESN'T RECEIVE THE JOIN ACCEPT RESPONSE", macAddress));
            }

            return result;
        }
        #endregion
    }
}
