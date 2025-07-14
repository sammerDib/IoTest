using System;
using System.IO;
using System.Text;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Configuration
{
    public class AutomationConfiguration : IAutomationConfiguration
    {
        public const string ConfigurationFolderName = "Configuration";
        public const string AutomationSettintgsFolderName = "AutomationSettings";
        public const string ModuleFolderName = "Dataflow";

        private const string AlarmConfigurationFileName = "AlarmSettings.xml";
        private const string CEConfigurationFileName = "CESettings.xml";
        private const string ECConfigurationFileName = "ECSettings.xml";
        private const string SVConfigurationFileName = "SVSettings.xml";

        private StringBuilder _status;
        private ActorType _actor;

        public AutomationConfiguration(ActorType actor, string configurationName = null)
        {
            _status = new StringBuilder();
            InputConfigurationName = string.IsNullOrEmpty(configurationName) ? Environment.MachineName : configurationName;
            _actor = actor;
            SetFolderPath();
        }

        private void SetFolderPath()
        {
            _status.AppendLine($"ConfigurationName : {InputConfigurationName}");
#if USE_ANYCPU
            string baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
#else
            string baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
#endif
            string rootDir = baseDir;

            // If Input config is a full path
            if (Path.IsPathRooted(InputConfigurationName)) // Path defined to move configuration from developement folders            
                ConfigurationFolderPath = Path.Combine(InputConfigurationName, ConfigurationFolderName, AutomationSettintgsFolderName); // default folder                     
            else
            {
                string rootDirWithConfig = Path.Combine(rootDir, InputConfigurationName);
                if (Directory.Exists(rootDirWithConfig))
                {
                    rootDir = rootDirWithConfig;
                }
                ConfigurationFolderPath = Path.Combine(rootDir, ConfigurationFolderName);
                ConfigurationFolderPath = Path.Combine(ConfigurationFolderPath, AutomationSettintgsFolderName); // Add Module name
            }
            if (Directory.Exists(ConfigurationFolderPath))
            {
                _status.AppendLine($"Configuration folder: {ConfigurationFolderPath}");
            }
            else
            {
                _status.AppendLine($"[ERR] Configuration folder doesn't exist: {ConfigurationFolderPath}");
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
        public string AlarmConfigurationFilePath => Path.Combine(ConfigurationFolderPath, $"{_actor.ToString()}_{AlarmConfigurationFileName}");
        public string CEConfigurationFilePath => Path.Combine(ConfigurationFolderPath, $"{_actor.ToString()}_{CEConfigurationFileName}");
        public string ECConfigurationFilePath => Path.Combine(ConfigurationFolderPath, $"{_actor.ToString()}_{ECConfigurationFileName}");
        public string SVConfigurationFilePath => Path.Combine(ConfigurationFolderPath, $"{_actor.ToString()}_{SVConfigurationFileName}");

        
    }
}
