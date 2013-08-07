using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Comunications
{
    public enum WriteSessionStatusCodes: byte
    {
        OK = 0,
        ErrorFragmentTotalNotExpected,
        ErrorFragmentOrder,
        ErrorWaitingFirstFragment,
        ErrorBusyReceivingState
    }

    public enum ConfigWriteStatusCodes : byte
    {
        ErrorConfigSizeTooBig = 5,
        ErrorConfigInvalidChecksum,
        ErrorConfigSizeNotExpected
    }
}
