using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.ProbeLise
{
    public class LiseSignalAnalysisAccordingSampleParams
    {
        public LiseSignalAnalysisAccordingSampleParams(LiseSignalAnalysisParams analysisParams, Length acceptanceTreeshold)
        {
            AnalysisParams = analysisParams;
            AcceptanceTreeshold = acceptanceTreeshold;
        }

        public LiseSignalAnalysisParams AnalysisParams;
        public Length AcceptanceTreeshold;
    }
}
