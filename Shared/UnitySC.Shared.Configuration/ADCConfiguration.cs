using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Configuration
{
    public class ADCConfiguration : IADCConfiguration
    {
        public const string ConfigurationFolderName = "Configuration";
        public const string SettingsFolderName = "ADCsConfiguration";
        public const string ModuleFolderName = "Dataflow";
        private const string ADCConfigurationFileName = SettingsFolderName + ".xml";
        private const string LogConfigurationFileName = "log.ADCResults.config";

        private StringBuilder _status;

        public ADCConfiguration(string configurationName = null)
        {
            _status = new StringBuilder();
            InputConfigurationName = string.IsNullOrEmpty(configurationName) ? Environment.MachineName : configurationName;
            SetFolderPath();
        }

        private void SetFolderPath()
        {
#if USE_ANYCPU
            string baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
#else
            string baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
#endif
            string rootDir = baseDir;

            // If Input config is a full path
            if (Path.IsPathRooted(InputConfigurationName)) // Path defined to move configuration from developement folders            
                ConfigurationFolderPath = Path.Combine(InputConfigurationName, ConfigurationFolderName, SettingsFolderName); // default folder                     
            else
            {
                string rootDirWithConfig = Path.Combine(rootDir, InputConfigurationName);
                if (Directory.Exists(rootDirWithConfig))
                {
                    rootDir = rootDirWithConfig;
                }
                ConfigurationFolderPath = Path.Combine(rootDir, ConfigurationFolderName);
                ConfigurationFolderPath = Path.Combine(ConfigurationFolderPath, SettingsFolderName); // Add Module name
            }
            if (Directory.Exists(ConfigurationFolderPath))
            {
                _status.AppendLine($"ADCs Configurations folder: {ConfigurationFolderPath}");
            }
            else
            {
                _status.AppendLine($"No ADCs Configurations detected: {ConfigurationFolderPath}");
            }
        }

        public string GetStatus()
        {
            return _status.ToString();
        }

        public string InputConfigurationName { get; private set; }

        // paths
        public string ConfigurationFolderPath { get; private set; }
        //FileNames           
        public string ADCConfigurationFilePath => Path.Combine(ConfigurationFolderPath, $"{ADCConfigurationFileName}");
        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName);

    }
}
