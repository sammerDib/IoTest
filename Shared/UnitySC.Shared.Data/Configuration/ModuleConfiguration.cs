using System;
using System.IO;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Data
{
    /// <summary>
    ///  Définition du fichier XML de configuration des process modules
    /// </summary>
    [Serializable]
    public class ModuleConfiguration
    {
        static public ModuleConfiguration Init(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"ModuleConfiguration file is missing <{path}>");
            var PMConfig = XML.Deserialize<ModuleConfiguration>(path);
            return PMConfig;
        }

        // Identity
        public ActorType Actor;
        [Obsolete("Use ChamberKey And ToolKey instead of ChamberId")]
        public int ChamberId;
        public int ToolKey = -1;
        // The ADC is also a "chamber" so all the modules have a ChamberKey
        public int ChamberKey = -1;

        // Output
        public string OutputAcqServer;
        public string OutputAcqPathTemplate;
        public string OutputAcqFileNameTemplate;
        public string OutputAdaFolder;
        public string OutputAdaFileNameTemplate;
        public string OutputResultFolder;
        public string LocalCacheResultFolderPath;
        public string ResultPathRootTemplate;
        public string OutputThroughputDataFullFilePathName;

        // Defaults
        public string DefaultLotName;
        public string DefaultJobName;
        public int DefaultSlotId;
        public string DefaultWaferName;
        public string DefaultLoadPort;

        // Uto user access
        public string UtoAccessRightsFilePath;
        public string UtoAccessRightsXSDFilePath;
        public string UtoUserProfilesFilePath;
        public double PrealignementAngle;

        // Services addresses
        public ServiceAddress DataAccessAddress;
        public ServiceAddress DataFlowAddress;
        public ServiceAddress UTOAddress;

        public bool UseMatroxImagingLibrary;
        public bool MonitorTaskTimerIsEnable;
        public bool AutoExtractResultCSVIsEnable;
        
        // TODO: Chamber need to be update with door slit presence in hardware, here for the moment
        public bool SlitDoorEnabled=false;
    }
}
