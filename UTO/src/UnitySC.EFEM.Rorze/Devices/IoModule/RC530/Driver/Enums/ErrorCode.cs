using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Driver.Enums
{
    public enum ErrorCode
    {
        NoError = 0,

        [Description("Failure reading of setting data. Corruption of setting data.")]
        AbnormalROM = 23
    }
}
