using System;
using System.IO;
using System.Text;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.UTO.Controller.Components.DataFlow
{
    public class DataFlowClientConfigurationManager : IClientConfigurationManager
    {
        #region Fields

        private const string ClientConfigurationFileName = "ClientConfiguration.xml";
        private const string LogConfigurationFileName = "log.config";
        private readonly StringBuilder _status = new();

        #endregion

        #region Properties

        public ClientConfiguration Configuration { get; private set; }

        public string ConfigurationFolderPath { get; private set; }

        public string ConfigurationName { get; private set; }

        #endregion

        #region Implementation of IClientConfigurationManager

        public string ClientConfigurationFilePath => Path.Combine(ConfigurationFolderPath, ClientConfigurationFileName);

        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName);

        public bool UseLocalAddresses => false;

        public bool IsWaferLessMode { get; }

        public string GetStatus() => _status.ToString();

        #endregion

        public void Setup()
        {
            if (ApplyConfigOptions())
            {
                var currentConfiguration = new ClientConfigurationManager(null);
                ClassLocator.Default.Register<IClientConfigurationManager>(() => currentConfiguration, true);
                ClassLocator.Default.Register(() => ClientConfiguration.Init(ClientConfigurationFilePath), true);
            }
            else
            {
                throw new DirectoryNotFoundException("DataFlow configuration not found.");
            }
        }

        private bool ApplyConfigOptions()
        {
            var dataFlowFolder = App.UtoInstance.ControllerConfig.DataFlowFolderName;
            var configToDisplay = string.IsNullOrEmpty(dataFlowFolder) ? "Undefined" : dataFlowFolder;
            _status.AppendLine($"DataFlow configuration: {configToDisplay}");
            ConfigurationName = string.IsNullOrEmpty(dataFlowFolder) ? Environment.MachineName : dataFlowFolder;
            return SetFolderPath();
        }

        private bool SetFolderPath()
        {
            ConfigurationFolderPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
            var rootDirWithConfig = Path.Combine(ConfigurationFolderPath, ConfigurationName);

            if (Directory.Exists(rootDirWithConfig))
            {
                ConfigurationFolderPath = rootDirWithConfig;
                _status.AppendLine($"Configuration folder: {ConfigurationFolderPath}");
                return true;
            }

            _status.AppendLine($"[ERR] Configuration folder doesn't exist: {ConfigurationFolderPath}");
            return false;
        }
    }
}
