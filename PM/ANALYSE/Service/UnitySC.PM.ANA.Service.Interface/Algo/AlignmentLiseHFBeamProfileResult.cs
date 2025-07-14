using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AlignmentLiseHFBeamProfileResult : IFlowResult
    {
        public double Ampl { get; set; }
        public double XGauss { get; set; }
        public double YGauss { get; set; }
        public double XGauss45 { get; set; }
        public double YGauss135 { get; set; }
        public double Radius { get; set; }
        public double Norm { get; set; }
        public double Background { get; set; }
        public double WeightedNorm { get; set; }
        public double RatioOfAxisOfEllipse { get; set; }
        public double AngleOfEllipse { get; set; }
        public FlowStatus Status { get; set; }
    }
}
