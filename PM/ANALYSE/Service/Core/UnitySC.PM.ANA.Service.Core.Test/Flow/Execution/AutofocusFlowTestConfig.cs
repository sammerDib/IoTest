using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Execution
{
    public class AutofocusFlowTestConfig : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }
        public bool TestError { get; set; } = true;
        public string ReportFolder { get; set; } = "";
    }
}
