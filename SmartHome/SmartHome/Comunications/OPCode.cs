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
        ConfigWrite,
        ConfigWriteResponse,
        ConfigRead,
        ConfigReadResponse,
        ConfigReadConfirmation,
        ConfigChecksum,
        ConfigChecksumResponse,

        MacRead = 0x20,
        MacReadResponse,
        NextHopRead,
        NextHopReadResponse,
        RouteTableRead,
        RouteTableReadResponse,
        RouteTableReadConfirmation,

        TimeWrite = 0x30,
        TimeRead,
        TimeReadResponse,
        DateWrite,
        DateRead,
        DateReadResponse,
        DateTimeRequest,
        DateTimeRequestResponse,

        Warning = 0x3E,
        Error,

        DigitalWrite = 0x40,
        DigitalSwitch,
        DigitalRead,
        DigitalReadResponse,

        AnalogWrite = 0x4A,
        AnalogRead,
        AnalogReadResponse,

        ColorWrite = 0x50,
        ColorWriteRandom,
        ColorSecuenceWrite,
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
