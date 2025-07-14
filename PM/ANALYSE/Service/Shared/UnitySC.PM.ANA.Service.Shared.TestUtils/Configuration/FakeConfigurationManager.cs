using System;
using System.IO;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration
{
    public class FakeConfigurationManager : IPMServiceConfigurationManager
    {
        private string _currentDirectory;
        private bool _flowsAreSimulated;
        private const string ConfigurationFolderName = "Configuration";
        private const string CalibrationFolderName = "Calibration";
        private const string PMConfigurationFileName = "PMConfiguration.xml";
        private const string FlowsConfigurationFileName = "FlowsConfiguration.xml";
        private const string LogConfigurationFileName = "log.config";

        public FakeConfigurationManager(bool flowsAreSimulated = false)
        {
            _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _flowsAreSimulated = flowsAreSimulated;
        }

        public FlowReportConfiguration AllFlowsReportMode => FlowReportConfiguration.NeverWrite;

        public string CalibrationFolderPath => Path.Combine(_currentDirectory, CalibrationFolderName);
        public string ConfigurationFolderPath => Path.Combine(_currentDirectory, ConfigurationFolderName);

        public string ConfigurationName => "Test";
        public string PMConfigurationFilePath => Path.Combine(ConfigurationFolderPath, PMConfigurationFileName);
        public string FlowsConfigurationFilePath => Path.Combine(ConfigurationFolderPath, FlowsConfigurationFileName);

        public bool FlowsAreSimulated => _flowsAreSimulated;

        public bool MilIsSimulated => false;

        public bool HardwaresAreSimulated => false;

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
