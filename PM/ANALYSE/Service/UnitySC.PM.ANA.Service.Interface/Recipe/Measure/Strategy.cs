namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    /// <summary>
    /// Measure strategy
    /// </summary>
    public enum AcquisitionStrategy
    {
        // First good measure on point position
        Standard,

        // Average on several measures on point position
        HighPrecision,

        // Flight (No stop for on measure)
        Fast,

        // Search good measure with cirucular move around point position(Snail)
        RoughSample
    }
}
