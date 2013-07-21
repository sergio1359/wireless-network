using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Comunications
{
    public enum OPCode : byte
    {
        Reset = 0x00,
        FirmwareVersionRead,
        FirmwareVersionReadResponse,
        ShieldModelRead,
        ShieldModelReadResponse,
        BaseModelRead,
        BaseModelReadResponse,
        ConfigWrite,
        ConfigWriteResponse,
        ConfigRead,
        ConfigReadResponse,
        ConfigReadConfirmation,
        ConfigChecksumRead,
        ConfigChecksumReadResponse,

        MacRead = 0x20,
        MacReadResponse,
        NextHopRead,
        NextHopReadResponse,
        RouteTableRead,
        RouteTableReadResponse,
        RouteTableReadConfirmation,

        DateTimeWrite = 0x30,
        DateTimeRead,
        DateTimeReadResponse,

        Warning = 0x3E,
        Error,

        LogicWrite = 0x40,
        LogicSwitch,
        LogicRead,
        LogicReadResponse,

        DimmerWrite = 0x46,
        DimmerRead,
        DimmerReadResponse,

        ColorWrite = 0x50,
        ColorWriteRandom,
        ColorRandomSecuenceWrite,
        ColorSortedSecuenceWrite,
        ColorRead,
        ColorReadResponse,

        PresenceRead = 0x57,
        PresenceReadResponse,

        TemperatureRead = 0x5A,
        TemperatureReadResponse,
        HumidityRead,
        HumidityReadResponse,

        PowerRead = 0x60,
        PowerReadResponse,

        LuminosityRead = 0x63,
        LuminosityReadResponse,

        PCOperation = 0xFE,
        Extension = 0xFF,
    }
}
