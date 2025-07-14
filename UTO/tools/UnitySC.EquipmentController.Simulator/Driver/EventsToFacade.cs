namespace UnitySC.EquipmentController.Simulator.Driver
{
    /// <summary>
    /// Defines events that can be notified from commands to the driver's facade.
    /// </summary>
    public enum EventsToFacade
    {
        CmdCompleteWithError,
        GeneralStatusReceived,
        SystemStatusReceived,
        CarrierPresenceReceived,
        MappingReceived,
        ArmHistoryAndWaferPresenceReceived,
        WaferSizeReceived,
        CarrierIdReceived,
        WaferIdReceived,
        EfemPressureReceived,
        OcrRecipesReceived,
        FfuSpeedReceived,
        CarrierTypeReceived
    }
}
