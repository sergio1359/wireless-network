#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Modules.Common;
using SmartHome.Communications;
using SmartHome.Communications.Messages;
using SmartHome.Communications.Modules;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using System.Diagnostics;
#endregion

namespace SmartHome.Communications.Modules
{
    class UserModule : ModuleBase
    {
        [RequiredModule]
        public StatusModule statusModule;

        private Dictionary<HomeDevice, TaskCompletionSource<bool>> pendingRequests;

        public UserModule(CommunicationManager communicationManager)
            : base(communicationManager)
        {
            this.pendingRequests = new Dictionary<HomeDevice, TaskCompletionSource<bool>>();
        }

        #region Overridden Methods
        public override void Initialize()
        {
            base.Initialize();

            statusModule.StateRefreshed += statusModule_StateRefreshed;
        }

        public override void ProcessReceivedMessage(Communications.Messages.IMessage inputMessage)
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

        #region Private Methods
        void statusModule_StateRefreshed(object sender, HomeDevice e)
        {
            if (this.pendingRequests.ContainsKey(e))
            {
                this.pendingRequests[e].SetResult(true);
            }
        }
        #endregion

        public async Task<bool> RefreshHomeDevice(HomeDevice homeDevice, TimeSpan timeout)
        {
            //TODO: To be implemented
            OperationMessage refreshMessage = homeDevice.RefreshState();

            //If the home device can't be updated. Return true.
            if (refreshMessage == null)
                return true;

            if (!this.pendingRequests.ContainsKey(homeDevice))
            {
                this.pendingRequests.Add(homeDevice, new TaskCompletionSource<bool>());

                bool sent = await this.SendMessage(refreshMessage);

                if (sent)
                {
                    var pendingConfirmationTask = this.pendingRequests[homeDevice];

                    //Await for a confirmation with timeout
                    Task delayTask = Task.Delay((int)timeout.TotalMilliseconds);
                    Task firstTask = await Task.WhenAny(pendingConfirmationTask.Task, delayTask);

                    bool result = false;

                    if (firstTask == delayTask)
                    {
                        //Timeout
                        this.PrintLog(true, "TIMEOUT during HD update");
                        pendingConfirmationTask.SetCanceled();
                        this.pendingRequests.Remove(homeDevice);
                    }
                    else
                    {
                        result = pendingConfirmationTask.Task.Result;
                        this.pendingRequests.Remove(homeDevice);
                    }

                    return result;
                }
            }
            else
            {
                return await this.pendingRequests[homeDevice].Task;
            }

            return false;
        }
    }
}
