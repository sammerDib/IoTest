namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class PatternRecConfiguration : AxesMovementConfiguration
    {
        public double AngleTolerance { get; set; } = 30.0;
        public double ScaleTolerance { get; set; } = 0.02;
        public int DilationMaskSize { get; set; } = 7;
        public double SimilarityThreshold { get; set; } = 0.5;
    }
}
