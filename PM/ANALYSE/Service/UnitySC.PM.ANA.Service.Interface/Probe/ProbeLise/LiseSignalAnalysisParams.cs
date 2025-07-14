namespace UnitySC.PM.ANA.Service.Interface.ProbeLise
{
    public class LiseSignalAnalysisParams
    {
        public LiseSignalAnalysisParams(int lag, double coef, double influence)
        {
            Lag = lag;
            DetectionCoef = coef;
            Influence = influence;
        }

        public int Lag;
        public double DetectionCoef;
        public double Influence;
    }
}
