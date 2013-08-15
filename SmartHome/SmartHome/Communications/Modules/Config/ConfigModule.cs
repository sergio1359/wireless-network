#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using SmartHome.Communications.Modules.Common;
using SmartHome.Comunications;
using SmartHome.Comunications.Messages;
using SmartHome.Comunications.Modules;
using SmartHome.BusinessEntities;
#endregion

namespace SmartHome.Communications.Modules.Config
{
    public class ConfigModule : ModuleBase
    {
        private Dictionary<ushort, FragmentWriteTransaction> currentWriteTransactions;

        /// <summary>
        /// List of the nodes that are receiving a configuration
        /// </summary>
        public IEnumerable<TransactionStatus> PendingWriteTransactions
        {
            get
            {
                return this.currentWriteTransactions.Select(wt => new TransactionStatus() { NodeAddress = wt.Key, Percentage = wt.Value.Percentage });
            }
        }

        public ConfigModule(CommunicationManager communicationManager)
            :base(communicationManager)
        {
            this.currentWriteTransactions = new Dictionary<ushort, FragmentWriteTransaction>();
        }

        #region Overridden Methods
        public override void ProcessReceivedMessage(Comunications.Messages.IMessage inputMessage)
        {
            OperationMessage message = (OperationMessage)inputMessage;

            if (message.OpCode == OperationMessage.OPCodes.ConfigWriteResponse)
            {
                if (this.currentWriteTransactions.ContainsKey(message.SourceAddress))
                {
                    //TODO: Check other params to avoid exceptions
                    var nodeTransaction = this.currentWriteTransactions[message.SourceAddress];
                    if (!nodeTransaction.ProcessResponse(message).Result)
                    {
                        //TODO: Check the problem
                    }
                }
            }
        }

        protected override Filter ConfigureInputFilter()
        {
            return new Filter()
            {
                Endpoint = Endpoints.APPLICATION_EP,
                OpCodeType = typeof(OperationMessage.OPCodes),
                Secured = true,
                Routed = true,
                OpCodes = new byte[] 
                { 
                    (byte)OperationMessage.OPCodes.ConfigWriteResponse, 
                    (byte)OperationMessage.OPCodes.ConfigReadConfirmation,
                },
            };
        }

        protected override OutputParameters ConfigureOutputParameters()
        {
            return new OutputParameters(
                priority: 0.4f,
                endpoint: Endpoints.APPLICATION_EP,
                securityEnabled: true,
                routingEnabled: true);
        } 
        #endregion

        public void SendConfiguration(Node node)
        {
            if (!this.currentWriteTransactions.ContainsKey(node.Address))
            {
                var newTransaction = new FragmentWriteTransaction(this, OperationMessage.OPCodes.ConfigWrite, typeof(ConfigWriteStatusCodes), node.Address, node.GetBinaryConfiguration());
                this.currentWriteTransactions.Add(node.Address, newTransaction);
            }
        }
    }
}
