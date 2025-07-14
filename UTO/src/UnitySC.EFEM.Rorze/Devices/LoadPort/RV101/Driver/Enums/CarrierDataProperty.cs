using System;
using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums
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

        [Description("Height of 1sf slot (pulse)\n"
                     + "Format = +-nnnnnnn")]
        HeightOf1stSlot = 7,

        [Description("Front bow detecting pulse (pulse)\n"
                     + "Format = +-nnnnnnn")]
        FrontBowDetectingPulse = 8,

        [Description("Z-axis moving quantity for mapping (pulse)\n"
                     + "Format = +-nnnnnnn")]
        ZAxisMovingQuantityForMapping = 9,

        [Description("Maximum amount to offset the thickness\n"
                     + "Format = +-nnnnnnn")]
        MaximumAmountOffsetThickness = 10,

        [Description("Sensor pattern\n"
                     + "Format = nnn")]
        SensorPattern = 11,

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
        IP_A     = 1 << 3,
        IP_B     = 1 << 4,
        IP_C     = 1 << 5,
        IP_D     = 1 << 6
    }
}
