namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbeMeasureParams
    {
        string Id { get; set; }
        double Gain { get; set; }
        double QualityThreshold { get; set; }
        double DetectionThreshold { get; set; }
        int NbMeasuresAverage { get; set; }
    }
}