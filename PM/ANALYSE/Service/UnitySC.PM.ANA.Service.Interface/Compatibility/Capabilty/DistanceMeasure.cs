namespace UnitySC.PM.ANA.Service.Interface.Compatibility.Capability
{
    public class DistanceMeasure : CapabilityBase
    {
        public double MinMeasureRange { get; set; }
        public double MaxMeasureRange { get; set; }
        public bool MultiRefractiveIndexCompatibility { get; set; }
    }
}