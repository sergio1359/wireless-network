using SerialPortManager.ConnectionManager;
using SmartHome.Communications.SerialManager;
using SmartHome.Comunications.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Comunications.Modules
{
    public class Filter
    {
        /// <summary>
        /// Null is the default value. It means that this variable will not be checked for filtering. 
        /// Otherwise, only message with this property will be received.
        /// </summary>

        public bool? Secured = null;

        public bool? Routed = null;

        public bool? FromMaster = null;

        public int? Endpoint = null;

        public Type OpCodeType;

        public byte[] OpCodes;

        public bool CheckMessage(InputHeader message)
        {
            bool result = true;

            result &= this.Secured == null ? true : this.Secured == message.SecurityEnabled;

            result &= this.Routed == null ? true : this.Routed == message.RoutingEnabled;

            //result &= this.FromMaster == null ? true : this.FromMaster == ;

            result &= this.Endpoint == null ? true : this.Endpoint == message.EndPoint;

            result &= this.OpCodeType == message.Content.OpCode.GetType();

            result &= this.OpCodes.Length == 0 ? true : this.OpCodes.Contains((byte)message.Content.OpCode);

            return result;
        }
    }

    public abstract class ModuleBase
    {
        /// <summary>
        /// Indicate the filter used for incoming messages.
        /// </summary>
        public Filter Filter { get; private set; }

        /// <summary>
        /// 0 is the lowest, and 1 is the higher.
        /// </summary>
        public float Priority { get; private set; }

        /// <summary>
        /// Endpoint used for sended messages.
        /// </summary>
        public int OutputEndpoint { get; private set; }

        private CommunicationManager communicationManager;

        public ModuleBase(CommunicationManager communicationManager, float priotity, int outputEndpoint)
        {
            this.Filter = new Filter();
            this.communicationManager = communicationManager;
            this.Priority = priotity;
            this.OutputEndpoint = outputEndpoint;
        }

        public async Task<bool> SendMessage(IMessage message)
        {
            OutputHeader outputMessage = new OutputHeader(this.Priority)
            {
                EndPoint = OutputEndpoint,
                Retries = 3,
                Content = message,
            };

            return await this.communicationManager.SendMessage(outputMessage);
        }

        public abstract void ProccessReceivedMessage(IMessage inputMessage);
    }
}
