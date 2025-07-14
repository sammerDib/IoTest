using System;
using System.IO;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Configuration
{
    /// <summary>
	///  Définition du fichier XML de configuration 
	/// </summary>
	[Serializable]
    public class DFServerConfiguration
    {
        public static DFServerConfiguration Init(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("DFServerConfiguration file is missing");
            var DFConfig = XML.Deserialize<DFServerConfiguration>(path);
            var UseLocalAddresses = ClassLocator.Default.GetInstance<IServiceDFConfigurationManager>().UseLocalAddresses;
            if (UseLocalAddresses)
            {
                DFConfig.DataAccessAddress.Host = "localhost";
                DFConfig.DAPAddress.Host = "localhost";
            }
            return DFConfig;
        }

        public int ToolKey;
        public double OrientationAngle; //angle orientation in degrees °.
        public DF_EndProcessBehavior EndProcessBehavior = DF_EndProcessBehavior.DFRecipCompleteAfterPMProcess;
        public ServiceAddress DataAccessAddress;
        public ServiceAddress DAPAddress;
        public string ADCADAPathFront;
        public string ADCADAPathBack;
        public string LogDataCollectionPathFile;
    }
}
