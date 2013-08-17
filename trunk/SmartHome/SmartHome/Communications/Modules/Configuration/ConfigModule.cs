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
using System.IO;
using System.Timers;
using DataLayer;
#endregion

namespace SmartHome.Communications.Modules.Config
{
    public class ConfigModule : ModuleBase
    {
        private Dictionary<ushort, FragmentWriteTransaction> currentWriteTransactions;

        private List<ushort> waitingForChecksum;

        private Timer configUpdateTimer;

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

            this.waitingForChecksum = new List<ushort>();

            this.configUpdateTimer = new Timer()
            {
                Interval = 1000 * 1, // 10 seconds
                AutoReset = true,
            };
            this.configUpdateTimer.Elapsed += configUpdateTimer_Elapsed;

            //this.configUpdateTimer.Start();
        }

        private async void configUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var nodes = Repositories.NodeRespository.GetAll();

            foreach (Node node in nodes)
            {
                if (!node.ConfigChecksum.HasValue)
                {
                    ushort nodeAddress = (ushort)node.Address;

                    await this.SendMessage(OperationMessage.ConfigChecksumRead(nodeAddress));

                    if (!this.waitingForChecksum.Contains(nodeAddress))
                        this.waitingForChecksum.Add(nodeAddress);
                }
                else
                {
                    //ushort checksumDB =
                }
            }
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
            else if (message.OpCode == OperationMessage.OPCodes.ConfigChecksumResponse)
            {
                ushort checksum = (ushort)(((ushort)message.Args[1]) << 8 | (ushort)message.Args[0]);

                PrintLog(false, string.Format("CHECKSUM RECEIVED FROM 0x{0:X4}: 0x{1:X4}", message.SourceAddress, checksum));

                if (this.waitingForChecksum.Contains(message.SourceAddress))
                {
                    this.waitingForChecksum.Remove(message.SourceAddress);

                    Node node = Repositories.NodeRespository.GetById(message.SourceAddress);

                    if (node == null)
                    {
                        PrintLog(true, "Node not present in the DB!");
                    }
                    else
                    {
                        node.ConfigChecksum = checksum;
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
                    (byte)OperationMessage.OPCodes.ConfigChecksumResponse,
                    (byte)OperationMessage.OPCodes.ConfigWriteResponse,
                    (byte)OperationMessage.OPCodes.WakeUp,
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

        public async void SendConfiguration(Node node)
        {
            if (!this.currentWriteTransactions.ContainsKey((ushort)node.Address))
            {
                var newTransaction = new FragmentWriteTransaction(this, OperationMessage.OPCodes.ConfigWrite, typeof(ConfigWriteStatusCodes), (ushort)node.Address, node.GetBinaryConfiguration());
                this.currentWriteTransactions.Add((ushort)node.Address, newTransaction);

                if (!await newTransaction.StartTransaction())
                {
                    //TODO: Check the problem
                }
            }
        }
    }
}
