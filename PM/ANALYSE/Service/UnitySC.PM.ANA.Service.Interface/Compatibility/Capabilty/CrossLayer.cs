namespace UnitySC.PM.ANA.Service.Interface.Compatibility.Capability
{
    /// <summary>
    /// Capability to cross layers
    /// </summary>
    public class CrossLayer : CapabilityBase
    {
        public int NumbersOfLayersToCross { get; set; }
        public double MaxRefractiveIndexOfLayerToCross { get; set; }
        public double MaxThicknessOfLayersToCross { get; set; }
    }
}