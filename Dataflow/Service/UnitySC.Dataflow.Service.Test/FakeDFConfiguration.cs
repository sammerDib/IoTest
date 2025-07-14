using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Service.Test
{
    public class FakeDFConfiguration : IServiceDFConfigurationManager
    {
        public int ToolKey;
        public double OrientationAngle; //angle orientation in degrees °.
        public DF_EndProcessBehavior EndProcessBehavior = DF_EndProcessBehavior.DFRecipCompleteAfterPMProcess;
        public ServiceAddress DataAccessAddress;
        public ServiceAddress DAPAddress;
        public string ADCADAPathFront;
        public string ADCADAPathBack;

        public FakeDFConfiguration()
        {
            LogConfigurationFilePath = "log.DataflowService.Test.config";
        }

        public string ConfigurationFolderPath { get; }
        public string InputConfigurationName { get; }
        public string DFServerConfigurationFilePath { get => "DFServerConfiguration.xml"; }
        public string LogConfigurationFilePath { get; }
        public bool UseLocalAddresses { get; }
        public string LastLogDataCollectionPathFile { get;}

        public string GetStatus()
        {
            return "Fake DF configuration";
        }
    }
}
