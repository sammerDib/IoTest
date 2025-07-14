using System;
using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    /// <summary>
    /// Define carrier data properties.
    /// Format definition:
    ///     - n for one-digit numeral (from '0' to '9')
    ///     - +- for '+' or '-' ('+' if omitted)
    ///     - + for '+' ('+' if omitted)
    /// </summary>
    public enum CarrierDataProperty
    {
        [Description("The first parameter of \"CLMP\" command (0/7/8/9/10/11).\n"
                     + "Format = nnn")]
        FirstParameterOfCLMP = 0,

        [Description("bit0: When docking    0: Enable / 1: Disable\n"
                     + "bit1: When undocking 0: Enable / 1: Disable\n"
                     + "Format = n")]
        MappingFlag = 1,

        [Description("0: Enable / 1: Disable / 2: No error check\n"
                     + "Format = n")]
        ClampFlag = 2,

        [Description("1 to 50\n"
                     + "Format = nn")]
        NumberOfSlots = 3,

        [Description("Slot pitch (pulse)\n"
                     + "Format = nnnnnnnn")]
        SlotPitch = 4,

        [Description("Minimum thickness (pulse)\n"
                     + "Format = nnnnnnnn")]
        MinimumThickness = 5,

        [Description("Maximum thickness (pulse)\n"
                     + "Format = nnnnnnnn")]
        MaximumThickness = 6,

        [Description("Height offset (pulse)\n"
                     + "Format = +-nnnnnnn")]
        HeightOffset = 7,

        [Description("Front bow detecting pulse (pulse)\n"
                     + "Format = +-nnnnnnn")]
        FrontBowDetectingPulse = 8,

        [Description("Y-axis offset for mapping (pulse)\n"
                     + "Format = +-nnnnnnn")]
        YAxisOffsetForMapping = 9,

        [Description("Speed of short dock - 0: High speed / 1: Low speed\n"
                     + "Format = n")]
        SpeedOfShortDock = 10,

        [Description("OS when undocking\n"
                     + "Format = +-nnnnnnn")]
        OSWhenUndocking = 11,

        [Description("Sensor pattern\n"
                     + "Format = +-nnnnnnn")]
        SensorPattern = 12,

        [Description("Carrier identification characters (effective only the first four characters)\n"
                     + "Format = Within 31-character string")]
        CarrierIdentificationCharacters = 16
    }

    /// <summary>
    /// Defines the Sensor Pattern flags.
    /// It is the value contained in <see cref="CarrierDataProperty.SensorPattern"/>.
    /// </summary>
    [Flags]
    public enum SensorPattern
    {
        PRS_L    = 1 << 0,
        PRS_R    = 1 << 1,
        PRS_M    = 1 << 2,
        FOSB     = 1 << 3,
        PFA_L    = 1 << 4,
        PFA_R    = 1 << 5,
        OC       = 1 << 6,
        IP_A     = 1 << 7,
        IP_B     = 1 << 8,
        IP_C     = 1 << 9,
        IP_D     = 1 << 10,
        DSC300mm = 1 << 11,
        DSC200mm = 1 << 12,
        DSC150mm = 1 << 13,
        Common   = 1 << 14,
        W200mm   = 1 << 15,
        W150mm   = 1 << 16,
        Adapter  = 1 << 17
    }
}
