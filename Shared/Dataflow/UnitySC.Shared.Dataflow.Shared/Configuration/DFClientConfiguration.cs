using System;
using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.Dataflow.Shared.Configuration
{

    /// <summary>
    /// Configuration file for Dataflow UI
    /// </summary>
    [Serializable]
    public class DFClientConfiguration : IDFClientConfiguration
    {
        static public DFClientConfiguration Init(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("DFClientConfiguration file is missing");
            var TCConfig = XML.Deserialize<DFClientConfiguration>(path);
            return TCConfig;
        }

        public string ExternalUserControlsDir { get; set; }
        public int ToolKey { get; set; }
        public List<ActorType> AvailableModules { get; set; }

    }
}
