namespace UnitySC.PM.ANA.Service.Interface
{
    public interface ISingleLiseInputParams : ILiseInputParams
    {
        double Gain { get; set; }
        double QualityThreshold { get; set; }
        double DetectionThreshold { get; set; }
    }
}
