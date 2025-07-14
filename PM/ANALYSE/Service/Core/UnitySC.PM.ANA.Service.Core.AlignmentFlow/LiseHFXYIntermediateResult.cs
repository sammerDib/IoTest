using System.Collections.Generic;

namespace UnitySC.PM.ANA.Service.Core.AlignmentFlow
{
    public class LiseHFXYIntermediateResult
    {
        public List<double> TSV { get; set; } = new List<double>(new double[21]);
        public double StepSize { get; set; }
        public double StandardStepSize { get; set; } = 0.6;
    }
}
