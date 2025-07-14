namespace UnitySC.PM.ANA.Service.Interface
{

    public enum CalibrationFrequency
    {
        Wafer,
        Foup
    }

    // Interface NOT USED anywhere to delete if YES
    public interface IProbeLiseHFMeasureParams
    {
        string Id { get; set; }
        double Gain { get; set; }
        double QualityThreshold { get; set; }
        double DetectionThreshold { get; set; }
        int NbMeasuresAverage { get; set; }
    }
}
