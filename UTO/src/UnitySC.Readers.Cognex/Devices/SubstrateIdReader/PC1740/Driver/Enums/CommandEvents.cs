namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.Enums
{
    /// <summary>
    /// Events that commands can notify to the driver.
    /// (<see cref="IEquipmentFacade.SendEquipmentEvent"/>
    /// </summary>
    internal enum CommandEvents
    {
        // "inherit" from common (generic part of code handle the enum as int, so values should not overlap)
        CognexLoginRequestReceived = Equipment.Abstractions.Drivers.Common.Enums.CommandEvents.Last,
        CognexLoginCompleted,
        LoadJobCompleted,
        ReadSubstrateIdCompleted,
        GetFileListCompleted,
        GetImageCompleted
    }
}
