using System;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class CorrectorConfiguration : FlowConfigurationBase
    {
        public bool AreApplied;

        public double NotchDetectionDeviationFactor;

        public double NotchDetectionSimilarityThreshold;

        public int NotchDetectionWidthFactor;

        public Length NotchWidth;

        public double WaferDiameterTolerancePercent;
        public override FlowReportConfiguration WriteReportMode { get; set; }
    }
}
