using UnitySC.Shared.Configuration;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.Shared
{
    public interface IPMServiceConfigurationManager : IServiceConfigurationManager
    {
        FlowReportConfiguration AllFlowsReportMode { get; }
        string CalibrationFolderPath { get; }
        bool FlowsAreSimulated { get; }
        
        bool HardwaresAreSimulated { get; }
        bool UseLocalAddresses { get; }
        bool IsWaferlessMode { get; }
        string PMConfigurationFilePath { get; }
        string FlowsConfigurationFilePath { get; }

        string FDCsConfigurationFilePath { get; }       
    }
}
