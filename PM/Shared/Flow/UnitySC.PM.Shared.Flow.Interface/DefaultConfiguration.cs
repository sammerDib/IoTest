namespace UnitySC.PM.Shared.Flow.Interface
{
    public class DefaultConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; } = FlowReportConfiguration.NeverWrite;
    }
}
