using SerialPortManager.ConnectionManager;
using SmartHome.Communications.SerialManager;
using SmartHome.Comunications.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

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

        public bool CheckMessage(InputHeader message, bool fromMaster)
        {
            bool result = true;

            //result &= this.Secured == null ? true : this.Secured == message.SecurityEnabled;

            //result &= this.Routed == null ? true : this.Routed == message.RoutingEnabled;

            result &= this.FromMaster == null ? true : this.FromMaster == fromMaster;

            result &= this.Endpoint == null ? true : this.Endpoint == message.EndPoint;

            result &= this.OpCodeType == message.Content.OpCode.GetType();

            result &= this.OpCodes.Length == 0 ? true : this.OpCodes.Contains((byte)message.Content.OpCode);

            return result;
        }
    }

    public class OutputParameters
    {
        /// <summary>
        /// 0 is the lowest, and 1 is the higher.
        /// </summary>
        public float Priority { get; private set; }

        /// <summary>
        /// Endpoint used for sended messages.
        /// </summary>
        public int Endpoint { get; private set; }

        /// <summary>
        /// Indicates if the output message will be secured or not
        /// </summary>
        public bool SecurityEnabled { get; private set; }

        /// <summary>
        /// Indicates if the output message can be routed through the network
        /// </summary>
        public bool RoutingEnabled { get; private set; }

        /// <summary>
        /// Maximum number of retries if the sending procces fails
        /// </summary>
        /// <remarks>
        /// This value must be between 0 and 30. If that value is greater than that imposed by the node, the maximum will be taken by the node.
        /// </remarks>
        public int MaxRetries { get; private set; }

        public OutputParameters(float priority, int endpoint, bool securityEnabled = true, bool routingEnabled = true, int maxRetries = 3)
        {
            this.Priority = Math.Max(Math.Min(priority, 1.0f), 0f);
            this.Endpoint = endpoint;
            this.SecurityEnabled = securityEnabled;
            this.RoutingEnabled = routingEnabled;
            this.MaxRetries = Math.Min(maxRetries, 30);
        }
    }

    public abstract class ModuleBase
    {
        /// <summary>
        /// Indicate the filter used for incoming messages.
        /// </summary>
        public Filter Filter { get; private set; }

        public OutputParameters OutputParameters { get; private set; }

        private CommunicationManager communicationManager;

        public ModuleBase(CommunicationManager communicationManager)
        {
            this.communicationManager = communicationManager;

            this.Filter = this.ConfigureInputFilter();

            this.OutputParameters = this.ConfigureOutputParameters();
        }

        public async Task<bool> SendMessage(IMessage message)
        {
            OutputHeader outputMessage = new OutputHeader(this.OutputParameters.Priority)
            {
                EndPoint = this.OutputParameters.Endpoint,
                Retries = this.OutputParameters.MaxRetries,
                Content = message,
            };

            return await this.communicationManager.SendMessage(outputMessage);
        }

        public abstract void ProcessReceivedMessage(IMessage inputMessage);

        protected abstract Filter ConfigureInputFilter();

        protected abstract OutputParameters ConfigureOutputParameters();

        protected void PrintLog(bool error, string message)
        {
            Debug.WriteLine(string.Format("[{0}] {1} {2}: {3}", DateTime.Now.ToLongTimeString(), this.GetType(), error ? "ERROR" : "INFO", message));
        }
    }
}
