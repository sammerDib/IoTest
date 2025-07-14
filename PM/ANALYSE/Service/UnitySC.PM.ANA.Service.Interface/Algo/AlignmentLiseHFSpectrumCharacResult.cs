using System.Collections.Generic;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AlignmentLiseHFSpectrumCharacResult : IFlowResult
    {
        public double AverageWavelength { get; set; }
        public double RelativeSpectralBroadness { get; set; }
        public double AverageWavelengthEquidistSampl { get; set; }
        public double RelativeSpecBroadnessEquidistSampl { get; set; }
        public List<string> AdditionnalMessages { get; set; }
        public FlowStatus Status { get; set; }
    }
}
