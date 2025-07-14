using System.IO;
using System.Xml.Serialization;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.Shared;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.EP.Shared
{
    public class ExternalProcessingConfiguration
    {
        [XmlAttribute]
        public string Version { get; set; } = "1.0.0";

        public MountainsConfiguration Mountains { get; set; }

        private const string EPConfigurationsPathFileName = "ExternalProcessingConfiguration.xml";
        static public ExternalProcessingConfiguration Init(string configurationFolderPath)
        {
            string configurationFilePath = Path.Combine(configurationFolderPath, EPConfigurationsPathFileName);
            ExternalProcessingConfiguration ePConfiguration = null;
            if (File.Exists(configurationFilePath))
            {
                ePConfiguration = XML.Deserialize<ExternalProcessingConfiguration>(configurationFilePath);
            }
            else
            {
                ePConfiguration = new ExternalProcessingConfiguration();
            }

            return ePConfiguration;
        }
    }
}
