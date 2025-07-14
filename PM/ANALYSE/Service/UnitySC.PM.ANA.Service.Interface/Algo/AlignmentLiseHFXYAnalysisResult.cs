using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AlignmentLiseHFXYAnalysisResult : IFlowResult
    {
        public List<double> TSV { get; set; } = new List<double>(new double[21]);
        public double Average { get; set; }
        public double Grad_45_deg { get; set; }
        public double Grad_135_deg { get; set; }
        public double ModulusOfGrad { get; set; }
        public double Defocus { get; set; }
        public FlowStatus Status { get; set; }
    }
}
