namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    /// <summary>
    /// Defines potential E84 errors.
    /// </summary>
    public enum E84Errors
    {
        // First implementation contains all errors that can be notified to Host
        None,
        Tp1Timeout,
        Tp2Timeout,
        Tp3Timeout,
        Tp4Timeout,
        Tp5Timeout,
        SignalError
    }
}
