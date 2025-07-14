namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public enum Referential
    {
        // Coordinates linked to this referential will not be 
        // corrected in any way. 
        // Used to send order to IAxes implementations.
        Stage,

        // Coordinates linked to this referential will 
        // be corrected using BWA result (if any)
        Wafer,
    }
}
