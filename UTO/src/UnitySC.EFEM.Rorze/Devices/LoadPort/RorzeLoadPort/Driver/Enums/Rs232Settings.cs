using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums
{
    public enum Rs232Settings
    {
        [Description("RS-232C is not used")]
        NotUsed = 0,

        [Description("RS-232C is used for Host communication (9600bps, data 8bit, no parity, stop bit 1)")]
        Host = 1,

        [Description("RS-232C is used for KEYENCE barcode reader communication (9600bps, data 7bit, even parity, stop bit 1)")]
        Keyence = 2,

        [Description("RS-232C is used for OMRON RF ID Reader/Writer communication (9600bps, data 8bit, even parity, stop bit 1)")]
        Omron = 3,

        [Description("RS-232C is used for Heart RF ID Reader/Writer communication (9600bps, data 8bit, even parity, stop bit 1)")]
        Heart = 4,

        [Description("RS-232C is used for Unison RF ID Reader/Writer communication (9600bps, data 8bit, no parity, stop bit 1)")]
        Unison = 5,

        [Description("RS-232C is used for ASYST RF ID Reader/Writer communication (9600bps, data 8bit, no parity, stop bit1)")]
        Asyst = 6,

        [Description("RS-232C is used for Brooks RF ID Reader/Writer communication (19200bps, data 8bit, even parity, stop bit1)")]
        Brooks = 7,
    }
}
