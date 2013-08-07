using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages;

namespace SmartHome.Comunications.Interfaces
{
    public class Filter<TOpCode> where TOpCode : struct
    {
        /// <summary>
        /// Null is the default value. It means that this variable will not be checked for filtering. 
        /// Otherwise, only message with this property will be received.
        /// </summary>

        public bool? Secured = null;
        public bool? Routed = null;
        public bool? FromMaster = null;
        public int? Endpoint = null;
        public TOpCode[] OpCodes;
    }

    public abstract class ModuleBase<TOpCode> where TOpCode : struct
    {
        /// <summary>
        /// Indicate the filter used for incoming messages.
        /// </summary>
        public Filter<TOpCode> Filter;

        /// <summary>
        /// 0 is the lowest, and 1 is the higher.
        /// </summary>
        public float Priority { get; set; }

        public abstract void ProccessReceivedMessage<TMessage>(TMessage inputMessage) where TMessage : IMessage;
    }
}
