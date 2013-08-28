#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using SmartHome.Communications.Modules.Common;
using SmartHome.Communications;
using SmartHome.Communications.Messages;
using SmartHome.Communications.Modules;
using SmartHome.BusinessEntities;
using System.IO;
using System.Timers;
using DataLayer;
#endregion

namespace SmartHome.Communications.Modules.Config
{
    internal class ConfigTransaction
    {
        public ushort Checksum;
        public DateTime TimeFlag;
        public FragmentWriteTransaction FragmentWriteTransaction;
    }

    public class ConfigModule : ModuleBase
    {
        /// <summary>
        /// This dictionary contains the current config write transactions and the checksum valu for each of them.
        /// </summary>
        private Dictionary<ushort, ConfigTransaction> currentWriteTransactions;

        private List<ushort> waitingForChecksum;

        private Timer configUpdateTimer;

        private bool firstTime;

        /// <summary>
        /// List of the nodes that are receiving a configuration
        /// </summary>
        public IEnumerable<TransactionStatus> PendingWriteTransactions
        {
            get
            {
                return this.currentWriteTransactions.Select(wt => new TransactionStatus() { NodeAddress = wt.Key, Percentage = wt.Value.FragmentWriteTransaction.Percentage });
            }
        }

        public ConfigModule(CommunicationManager communicationManager)
            : base(communicationManager)
        {
            this.currentWriteTransactions = new Dictionary<ushort, ConfigTransaction>();

            this.waitingForChecksum = new List<ushort>();

            this.firstTime = true;

            this.configUpdateTimer = new Timer()
            {
                Interval = 1000 * 5,    // 30 seconds
                AutoReset = false,
            };
            this.configUpdateTimer.Elapsed += configUpdateTimer_Elapsed;

            this.configUpdateTimer.Start();
        }

        private async void configUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var nodes = repository.NodeRespository.GetAll();

                foreach (Node node in nodes)
                {
                    if (this.firstTime || !node.ConfigChecksum.HasValue)
                    {
                        ushort nodeAddress = (ushort)node.Address;

                        bool sent = await this.SendMessage(OperationMessage.ConfigChecksumRead(nodeAddress));

                        if (sent && !this.waitingForChecksum.Contains(nodeAddress))
                            this.waitingForChecksum.Add(nodeAddress);
                    }
                }
            }

            this.firstTime = false;
            //this.configUpdateTimer.Start();
        }

        #region Overridden Methods

        public override void ProcessReceivedMessage(Communications.Messages.IMessage inputMessage)
        {
            OperationMessage message = (OperationMessage)inputMessage;

            if (message.OpCode == OperationMessage.OPCodes.ConfigWriteResponse)
            {
                if (this.currentWriteTransactions.ContainsKey(message.SourceAddress))
                {
                    //TODO: Check other params to avoid exceptions
                    var currentTransactionVars = this.currentWriteTransactions[message.SourceAddress];
                    var nodeTransaction = currentTransactionVars.FragmentWriteTransaction;

                    if (nodeTransaction.IsCompleted)
                    {
                        using (UnitOfWork repository = new UnitOfWork())
                        {
                            Node updatedNode = repository.NodeRespository.GetByNetworkAddress(nodeTransaction.DestinationAddress);

                            if (updatedNode.LastChecksumUpdate == currentTransactionVars.TimeFlag)
                            {
                                updatedNode.ConfigChecksum = currentTransactionVars.Checksum;
                                updatedNode.LastChecksumUpdate = currentTransactionVars.TimeFlag;

                                repository.Commit();

                                PrintLog(false, string.Format("The node 0x{0:X4} has been updated successfully", updatedNode.Address));
                            }
                            else
                            {
                                this.SendConfiguration(updatedNode, repository.HomeRespository.GetHome());
                            }
                        }
                    }else if (!nodeTransaction.ProcessResponse(message).Result)
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

                    Node node;
                    Home home;

                    using (UnitOfWork repository = new UnitOfWork())
                    {
                        node = repository.NodeRespository.GetByNetworkAddressWithConnectedHomeDevices(message.SourceAddress);
                        home = repository.HomeRespository.GetHome();
                    }

                    if (node == null)
                    {
                        PrintLog(true, "Node not present in the DB!");
                    }
                    else if (!node.ConfigChecksum.HasValue || node.ConfigChecksum != checksum)
                    {
                        this.SendConfiguration(node, home);
                    }
                    else
                    {
                        PrintLog(false, string.Format("The node 0x{0:X4} is up to date", node.Address));
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

        public async void SendConfiguration(Node node, Home home)
        {
            if (!this.currentWriteTransactions.ContainsKey((ushort)node.Address))
            {
                var configuration = node.GetBinaryConfiguration(home);

                var newTransaction = new FragmentWriteTransaction(this, OperationMessage.OPCodes.ConfigWrite, typeof(ConfigWriteStatusCodes), (ushort)node.Address, configuration.Item2);
                this.currentWriteTransactions.Add((ushort)node.Address, new ConfigTransaction()
                {
                    Checksum = configuration.Item1,
                    FragmentWriteTransaction = newTransaction,
                    TimeFlag = (DateTime)node.LastChecksumUpdate,
                });

                if (!await newTransaction.StartTransaction())
                {
                    //TODO: Check the problem
                    PrintLog(true, string.Format("An error occurred on configuration update for node 0x{0:X4}", node.Address));
                }
            }
        }
    }
}
