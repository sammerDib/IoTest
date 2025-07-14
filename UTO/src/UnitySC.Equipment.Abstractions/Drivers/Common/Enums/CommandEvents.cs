using Agileo.Common.Communication;

namespace UnitySC.Equipment.Abstractions.Drivers.Common.Enums
{
    /// <summary>
    /// Events that commands can notify to the driver.
    /// (<see cref="IEquipmentFacade.SendEquipmentEvent"/>
    /// </summary>
    public enum CommandEvents
    {
        CmdCompleteWithError = 0,
        CmdNotRecognized,

        Last // Should be last value. Can be used as starting point by other command enums to not overlap with this
    }
}
