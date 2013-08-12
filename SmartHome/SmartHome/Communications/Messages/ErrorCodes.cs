
namespace SmartHome.Comunications.Messages
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
