
namespace UnitySC.PM.Shared.Flow.Interface
{
    public abstract class FlowConfigurationBase
    {
        public abstract FlowReportConfiguration WriteReportMode { get; set; }

        public bool IsAnyReportEnabled()
        {
            return WriteReportMode > FlowReportConfiguration.NeverWrite;
        }
    }
}
