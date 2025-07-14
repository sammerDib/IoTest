using System;
using System.IO;

using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Shared.TestUtils.Configuration
{
    public class FakeConfigurationManager : IEMEServiceConfigurationManager
    {
        private string _currentDirectory;
        private bool _flowsAreSimulated;
        private string _calibrationFolderPath;
        private readonly string _configName;
        private const string ConfigurationFolderName = "Configuration";
        private const string CalibrationFolderName = "Calibration";
        private const string PMConfigurationFileName = "PMConfiguration.xml";
        private const string FlowsConfigurationFileName = "FlowsConfiguration.xml";
        private const string RecipeConfigurationFileName = "RecipeConfiguration.xml";
        private const string LogConfigurationFileName = "log.config";

        public FakeConfigurationManager(bool flowsAreSimulated = false)
        {
            _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _flowsAreSimulated = flowsAreSimulated;
            _calibrationFolderPath = Path.Combine(_currentDirectory, CalibrationFolderName);           
        }
        public FakeConfigurationManager(string calibrationFolderPath, bool flowsAreSimulated = false)
        {
            _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _flowsAreSimulated = flowsAreSimulated;
            _calibrationFolderPath = calibrationFolderPath;
        }
        public FakeConfigurationManager(string configName, string calibrationFolderPath = null, bool flowsAreSimulated = false)
        {
            _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var baseDirPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            _flowsAreSimulated = flowsAreSimulated;
            _configName = configName;
            _calibrationFolderPath = calibrationFolderPath ?? Path.Combine(baseDirPath, $"{_configName}\\{CalibrationFolderName}");
        }

        public FlowReportConfiguration AllFlowsReportMode => FlowReportConfiguration.NeverWrite;
        public string CalibrationFolderPath => _calibrationFolderPath;       
        public string ConfigurationFolderPath => Path.Combine(new DirectoryInfo(CalibrationFolderPath).Parent.FullName, $"Configuration");
        public string ConfigurationName => "Test";
        public string PMConfigurationFilePath => Path.Combine(ConfigurationFolderPath, PMConfigurationFileName);
        public string FlowsConfigurationFilePath => Path.Combine(ConfigurationFolderPath, FlowsConfigurationFileName);
        public string RecipeConfigurationFilePath => Path.Combine(ConfigurationFolderPath, RecipeConfigurationFileName);

        public bool FlowsAreSimulated => _flowsAreSimulated;

        public bool HardwaresAreSimulated => true;

        public bool MilIsSimulated => false;

        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName); 

        public string AlarmConfigurationFilePath { get; }
        public string CEConfigurationFilePath { get; }
        public string ECConfigurationFilePath { get; }
        public string SVConfigurationFilePath { get; }
        public string FDCsConfigurationFilePath { get; }
        public bool UseLocalAddresses { get; }
        public bool IsWaferlessMode { get; }

        public string GetStatus()
        {
            return "Fake configuration manager";
        }
    }
}
