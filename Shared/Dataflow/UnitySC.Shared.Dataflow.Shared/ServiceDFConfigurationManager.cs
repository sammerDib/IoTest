using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;

namespace UnitySC.PM.Shared
{
    public class ServiceDFConfigurationManager : IServiceDFConfigurationManager
    {
        // TODO: Make this private. It requires to refactor this class to make it more usable (by
        // UnitySC.PM.ANA.Configuration.Test for instance), and to make it more testable by
        // providing temporary folder, instead of looking at current assembly folder.
        // Note: *Path properties are not tested (because not testable for same reason) for now.
        public const string ConfigurationFolderName = "Configuration";
        public bool UseLocalAddresses { get; private set; }

        public const string ModuleFolderName = "Dataflow";
        private const string DFServerConfigurationFileName = "DFServerConfiguration.xml";
        private const string LogConfigurationFileName = "log.DataflowService.config";
        private const string FDCsConfigurationFileName = "FDCsConfiguration.xml";
        private const string FDCsPersistentDataFileName = "FDCsPersistentData.fpd";


        private StringBuilder _status;

        public ServiceDFConfigurationManager(string[] args)
        {
            _status = new StringBuilder();
            var rootCommand = new RootCommand();

            var configurationOption = new Option<string>("--config", "[Optional] Use a specific configuration. If option is not defined the computer name is used");
            configurationOption.AddAlias("-c");

            var useLocalAddressesOption = new Option<bool>("--useLocalAddreses", "[Optional] Use local addresses for all the wcf services");
            useLocalAddressesOption.AddAlias("-la");

            rootCommand.AddOption(configurationOption);
            rootCommand.AddOption(useLocalAddressesOption);

            rootCommand.Description = "Use options to defined the startup configuration";

            rootCommand.SetHandler((configurationOptionValue, useLocalAddressesOptionValue) =>
            {
                ApplyConfigOptions(configurationOptionValue, useLocalAddressesOptionValue);
            },
            configurationOption, useLocalAddressesOption);

            int exitCode = rootCommand.Invoke(args);
            if (exitCode != 0)
            {
                throw new Exception($"Command line invocation failed: {exitCode}");
            }
        }

        private void ApplyConfigOptions(string config, bool useLocalAddresses)
        {
            string configToDisplay = string.IsNullOrEmpty(config) ? "Undefined" : config;
            _status.AppendLine($"Configuration args: config {configToDisplay}");

            InputConfigurationName = string.IsNullOrEmpty(config) ? Environment.MachineName : config;
            SetFolderPath();
            UseLocalAddresses = useLocalAddresses;
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
            if (Path.IsPathRooted(InputConfigurationName))
                ConfigurationFolderPath = Path.Combine(InputConfigurationName, ConfigurationFolderName); // Add Module name
            else
            {
                string rootDirWithConfig = Path.Combine(rootDir, InputConfigurationName);
                if (Directory.Exists(rootDirWithConfig))
                {
                    rootDir = rootDirWithConfig;
                }
                ConfigurationFolderPath = Path.Combine(rootDir, ConfigurationFolderName);
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
        public string DFServerConfigurationFilePath => Path.Combine(ConfigurationFolderPath, DFServerConfigurationFileName);

        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName);

        public string FDCsConfigurationFilePath => Path.Combine(ConfigurationFolderPath, FDCsConfigurationFileName);
        public string FDCsPersistentDataFilePath => Path.Combine(ConfigurationFolderPath, FDCsPersistentDataFileName);
    }
}
