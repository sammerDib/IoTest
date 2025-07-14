namespace UnitySC.PM.ANA.Service.Interface.Compatibility.Capability
{
    public class ThicknessMeasure : CapabilityBase
    {
        public double MinThickness { get; set; }
        public double MaxThickness { get; set; }
        public double MinRefractiveIndex { get; set; }
        public double MaxRefractiveIndex { get; set; }
        public int MaxNumberOfLayersMeasured { get; set; }
    }
}