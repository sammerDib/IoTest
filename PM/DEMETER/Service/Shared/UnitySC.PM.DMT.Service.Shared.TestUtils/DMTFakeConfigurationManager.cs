using System;
using System.IO;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Shared.TestUtils.Configuration
{
    public class DMTFakeConfigurationManager : IDMTServiceConfigurationManager
    {
        private string _currentDirectory;
        private bool _flowsAreSimulated;
        private bool _milIsSimulated;
        private bool _useLocalAddresses;
        private const string ConfigurationFolderName = "Configuration";
        private const string CalibrationFolderName = "Calibration";
        private const string CalibrationInputFolderName = "Input";
        private const string CalibrationOutputFolderName = "Output";
        private const string CalibrationOutputBackupFolderName = "OutputBackup";
        private const string CurvatureCalibrationFolderName = "CurvatureCalibrationData";
        private const string PMConfigurationFileName = "PMConfiguration.xml";
        private const string FlowsConfigurationFileName = "FlowsConfiguration.xml";
        private const string LogConfigurationFileName = "log.config";
        private const string DMTHardwareConfiguration = "DMTHardwareConfiguration.xml";

        public DMTFakeConfigurationManager(bool flowsAreSimulated = false, bool milIsSimulated = true, bool useLocalAddresses=false)
        {
            _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _flowsAreSimulated = flowsAreSimulated;
            _milIsSimulated = milIsSimulated;
            _useLocalAddresses= useLocalAddresses;
        }

        public string CalibrationFolderPath => Path.Combine(_currentDirectory, CalibrationFolderName);
        public string ConfigurationFolderPath => Path.Combine(_currentDirectory, ConfigurationFolderName);

        public string ConfigurationName => "Test";
        public string PMConfigurationFilePath => Path.Combine(ConfigurationFolderPath, PMConfigurationFileName);
        public string XYCalibrationRecipesFolderPath => throw new NotImplementedException();
        public string FlowsConfigurationFilePath => Path.Combine(ConfigurationFolderPath, FlowsConfigurationFileName);
        public string DMTHardwareConfigurationFilePath => Path.Combine(ConfigurationFolderPath, DMTHardwareConfiguration);

        public bool FlowsAreSimulated => _flowsAreSimulated;

        public bool HardwaresAreSimulated => false;
        public bool MilIsSimulated => _milIsSimulated;

        public bool UseLocalAddresses => _useLocalAddresses;
        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName);

        public string AlarmConfigurationFilePath { get; }
        public string CEConfigurationFilePath { get; }
        public string ECConfigurationFilePath { get; }
        public string SVConfigurationFilePath { get; }
        public FlowReportConfiguration AllFlowsReportMode { get; set; }

        public string CalibrationInputFolderPath => Path.Combine(CalibrationFolderPath, CalibrationInputFolderName);

        public string CalibrationOutputFolderPath => Path.Combine(CalibrationFolderPath, CalibrationOutputFolderName);

        public string CalibrationOutputBackupFolderPath => Path.Combine(CalibrationFolderPath, CalibrationOutputBackupFolderName);

        public string CurvatureCalibrationFolderPath => Path.Combine(CalibrationOutputFolderPath, CurvatureCalibrationFolderName);

        public string FDCsConfigurationFilePath { get; }
        public bool IsWaferlessMode { get; }

        public string GetStatus()
        {
            return "DEMETER Fake configuration manager";
        }
    }
}
