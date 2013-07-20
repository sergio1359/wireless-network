using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Comunications
{
    public enum StatusCode: byte
    {
        OK = 0,
        ErrorFragmentTotalNotExpected,
        ErrorFragmentOrder,
        ErrorWaitingFirstFragment,
        ErrorConfigSizeTooBig,
        ErrorConfigInvalidChecksum,
        ErrorConfigSizeNotExpected,
        ErrorBusyReceivingState
    }
}
