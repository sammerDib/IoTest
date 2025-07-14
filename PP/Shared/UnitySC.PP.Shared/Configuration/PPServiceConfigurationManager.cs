using System;
using System.Collections.Generic;

using System.CommandLine;
using System.CommandLine.Invocation;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PP.Shared.Configuration
{
    public class PPServiceConfigurationManager : IPPServiceConfigurationManager
    {
        public const string ConfigurationFolderName = "Configuration";

        private const string PPConfigurationFileName = "PPConfiguration.xml";
        private const string LogConfigurationFileName = "log.config";

        private StringBuilder _status;

        //Paths
        public string ConfigurationFolderPath { get; private set; }
        public string ConfigurationName { get; private set; }

        public bool MilIsSimulated { get; private set; }


        //FileNames
        public string PPConfigurationFilePath => Path.Combine(ConfigurationFolderPath, PPConfigurationFileName);
        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName);

        //methods

        public PPServiceConfigurationManager(string[] args)
        {
            _status = new StringBuilder();
            var rootCommand = new RootCommand();

            var configurationOption = new Option<string>("--config", "[Optional] Use a specific configuration. If option is not defined the computer name is used");
            configurationOption.AddAlias("-c");

            var flagOption = new Option<bool>("--flag", "[Optional] Demo Flag boolean ");
            flagOption.AddAlias("-f");

            rootCommand.AddOption(configurationOption);
            rootCommand.AddOption(flagOption);
         
            rootCommand.Description = "Use options to defined the startup configuration";

            rootCommand.SetHandler(  (configurationOptionValue, flagOptionValue) =>
            {
                ApplyConfigOptions(configurationOptionValue, flagOptionValue);
            },
            configurationOption, flagOption);

            int exitCode = rootCommand.Invoke(args);
            if (exitCode != 0)
            {
                throw new Exception($"Command line invocation failed: {exitCode}");
            }
        }

        private void ApplyConfigOptions(string config, bool flag)
        {
           string configToDisplay = string.IsNullOrEmpty(config) ? "Undefined" : config;
           _status.AppendLine($"Configuration args: config {configToDisplay}  flag {flag}");

            //flag --> option flag for exemple for the moment

            ConfigurationName = string.IsNullOrEmpty(config) ? Environment.MachineName : config;
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
            string rootDirWithConfig = Path.Combine(rootDir, ConfigurationName);
            if (Directory.Exists(rootDirWithConfig))
            {
                rootDir = rootDirWithConfig;
            }

            ConfigurationFolderPath = Path.Combine(rootDir, ConfigurationFolderName);
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
    }
}
