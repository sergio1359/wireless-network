#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Communications.SerialManager; 
#endregion

namespace SmartHome.Comunications
{
    public class CommunicationManager
    {
        private SerialManager serialManager;

        public CommunicationManager()
        {
            serialManager = new SerialManager();

        }
    }
}
