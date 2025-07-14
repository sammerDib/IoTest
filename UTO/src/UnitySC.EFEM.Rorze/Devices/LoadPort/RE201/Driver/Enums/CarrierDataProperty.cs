using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums
{
    /// <summary>
    ///     Define carrier data properties.
    ///     Format definition:
    ///     - n for one-digit numeral (from '0' to '9')
    ///     - +- for '+' or '-' ('+' if omitted)
    ///     - + for '+' ('+' if omitted)
    /// </summary>
    public enum CarrierDataProperty
    {
        [Description("Z-axis position after mapping operation completed.\n"
                     + "0: Mapping completed position\n"
                     + "1 to 50: Teaching position"
                     + "Format = nnn")]
        ZAxisPosition = 0,

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

        [Description("First slot height (pulse)\n"
                     + "Format = +-nnnnnnn")]
        FirstSlotHeight = 7,

        [Description("Front bow detecting pulse (pulse)\n"
                     + "Format = +-nnnnnnn")]
        FrontBowDetectingPulse = 8,

        [Description("Z-axis moving quantity for mapping operation (pulse)\n"
                     + "Format = +-nnnnnnn")]
        ZAxisMovingQuantityForMappingOperation = 9,

        [Description("Maximum intermediate omission quantity\n"
                     + "Format = +-nnnnnnn")]
        MaximumIntermediateOmissionQuantity = 10,

        [Description("HOME reference position (pulse)\n"
                     + "Format = +-nnnnnnn")]
        HomeReferencePosition = 11,

        [Description("Carrier identification characters (effective only the first four characters)\n"
                     + "Format = Within 31-character string")]
        CarrierIdentificationCharacters = 16
    }
}
