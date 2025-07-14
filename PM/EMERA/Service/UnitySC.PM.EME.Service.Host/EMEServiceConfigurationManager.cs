using System.IO;

using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.Shared;

namespace UnitySC.PM.EME.Service.Host
{
    public class EMEServiceConfigurationManager : PMServiceConfigurationManager, IEMEServiceConfigurationManager
    {
        private const string RecipeConfigurationFileName = "RecipeConfiguration.xml";

        public EMEServiceConfigurationManager(string[] args) : base(args)
        {
            // EMERA-specific configuration
            RecipeConfigurationFilePath = Path.Combine(ConfigurationFolderPath, RecipeConfigurationFileName);

        }

        public string RecipeConfigurationFilePath { get; private set; }
    }
}

