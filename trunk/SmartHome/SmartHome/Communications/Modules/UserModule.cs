#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications;
using SmartHome.Comunications.Messages;
using SmartHome.Comunications.Modules; 
#endregion

namespace SmartHome.Communications.Modules
{
    class UserModule : ModuleBase
    {
        public UserModule(CommunicationManager communicationManager)
            : base(communicationManager)
        {

        }

        #region Overridden Methods
        public override void ProcessReceivedMessage(Comunications.Messages.IMessage inputMessage)
        {
            //Do Nothing
        }

        protected override Filter ConfigureInputFilter()
        {
            return new Filter()
            {
                Endpoint = Endpoints.APPLICATION_EP,
                OpCodeType = typeof(OperationMessage.OPCodes),
                Secured = true,
                Routed = true,
                OpCodes = new byte[0],
            };
        }

        protected override OutputParameters ConfigureOutputParameters()
        {
            return new OutputParameters(
                priority: 1f,
                endpoint: Endpoints.APPLICATION_EP,
                securityEnabled: true,
                routingEnabled: true);
        } 
        #endregion
    }
}
