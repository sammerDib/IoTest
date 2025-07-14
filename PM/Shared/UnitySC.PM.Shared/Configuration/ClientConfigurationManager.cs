using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;

namespace UnitySC.PM.Shared
{
    public class ClientConfigurationManager : IClientConfigurationManager
    {
        private const string ClientConfigurationFileName = "ClientConfiguration.xml";
        private const string LogConfigurationFileName = "log.config";
        private StringBuilder _status;

        public bool UseLocalAddresses { get; private set; }

        public bool IsWaferLessMode { get; private set; }

        public ClientConfigurationManager(string[] args)
        {
            _status = new StringBuilder();

            var rootCommand = BuildRootCommand();

            int exitCode = rootCommand.Invoke(args);
            if (exitCode != 0)
            {
                throw new ArgumentException($"Command line invocation failed: {exitCode}");
            }
        }

        private RootCommand BuildRootCommand()
        {
            var rootCommand = new RootCommand();

            var configurationOption = new Option<string>("--config", "[Optional] Use a specific configuration. If option is not defined the computer name is used");
            configurationOption.AddAlias("-c");

            var useLocalAddressesOption = new Option<bool>("--useLocalAddreses", "[Optional] Use local addresses for all the wcf services");
            useLocalAddressesOption.AddAlias("-la");

            var isWaferLessMode = new Option<bool>("--waferLessMode", "[Optional] Wafer less mode");
            isWaferLessMode.AddAlias("-wl");

            rootCommand.AddOption(configurationOption);
            rootCommand.AddOption(useLocalAddressesOption);
            rootCommand.AddOption(isWaferLessMode);

            rootCommand.Description = "Use options to define startup configuration";

            rootCommand.SetHandler((configurationOptionValue, useLocalAddressesOptionValue, isWaferLessModeValue) =>
            {
                ApplyConfigOptions(configurationOptionValue, useLocalAddressesOptionValue, isWaferLessModeValue);
            },
            configurationOption, useLocalAddressesOption, isWaferLessMode);

            return rootCommand;
        }

        private void ApplyConfigOptions(string config, bool useLocalAddresses, bool isWaferLessMode)
        {
            string configToDisplay = string.IsNullOrEmpty(config) ? "Undefined" : config;
            _status.AppendLine($"Configuration args: config {configToDisplay}");
            ConfigurationName = string.IsNullOrEmpty(config) ? Environment.MachineName : config;
            SetFolderPath();
            UseLocalAddresses = useLocalAddresses;
            IsWaferLessMode = isWaferLessMode;
        }

        private void SetFolderPath()
        {
#if USE_ANYCPU
            ConfigurationFolderPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
#else
            ConfigurationFolderPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
#endif
            string rootDirWithConfig = Path.Combine(ConfigurationFolderPath, ConfigurationName);
            if (Directory.Exists(rootDirWithConfig))
            {
                ConfigurationFolderPath = rootDirWithConfig;
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

        public string ConfigurationFolderPath { get; private set; }

        public string ConfigurationName { get; private set; }

        public string ClientConfigurationFilePath => Path.Combine(ConfigurationFolderPath, ClientConfigurationFileName);
        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName);
    }
}
