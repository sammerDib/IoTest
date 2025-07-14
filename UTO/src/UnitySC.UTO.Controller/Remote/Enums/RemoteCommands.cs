namespace UnitySC.UTO.Controller.Remote.Enums
{
    public enum RemoteCommands
    {
        None,
        LOCAL,
        PROCEEDWITHCARRIER,
        CANCELCARRIER,
        SETUPJOB,
        START,
        STOP,
        PAUSE,
        RESUME,
        ABORT,
        PROCEEDWITHSUBSTRATE,
        CANCELSUBSTRATE,
        InvalidParameterRcmd,
        InappropriateTimeRcmd
    }

    public enum RemoteCommandsParamters
    {
        CarrierId,
        PortId,
        SlotMap,
        ContentMap,
        JobId,
        RecipeId,
        Carriers,
        AutoStart,
        OcrProfileName,
        StopConfig,
        SubstId,
        SubstLocId
    }
}
