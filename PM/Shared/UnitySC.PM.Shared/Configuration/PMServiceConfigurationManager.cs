using System;
using System.CommandLine;
using System.IO;
using System.Text;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.Shared
{
    public class PMServiceConfigurationManager : IPMServiceConfigurationManager
    {
        // TODO: Make this private. It requires to refactor this class to make it more usable (by
        // UnitySC.PM.ANA.Configuration.Test for instance), and to make it more testable by
        // providing temporary folder, instead of looking at current assembly folder.
        // Note: *Path properties are not tested (because not testable for same reason) for now.
        public const string ConfigurationFolderName = "Configuration";

        private const string CalibrationFolderName = "Calibration";

        private const string PMConfigurationFileName = "PMConfiguration.xml";
        private const string FlowsConfigurationFileName = "FlowsConfiguration.xml";
        private const string LogConfigurationFileName = "log.config";
        private const string FDCsConfigurationFileName = "FDCsConfiguration.xml";
        private const string FDCsPersistentDataFileName = "FDCsPersistentData.fpd";

        private StringBuilder _status;

        public PMServiceConfigurationManager(string[] args)
        {
            _status = new StringBuilder();
            var rootCommand = new RootCommand();

            var configurationOption = new Option<string>("--config", "[Optional] Use a specific configuration. If option is not defined the computer name is used");
            configurationOption.AddAlias("-c");

            var hardwareSimulationOption = new Option<bool>("--simulateHardware", "[Optional] Simulate all hardwares. If option is not defined IsSimulated defined in HardwareConfiguration are used");
            hardwareSimulationOption.AddAlias("-sh");

            var flowSimulationOption = new Option<bool>("--simulateFlow", "[Optional] Simulate all flows. If option is not defined the flows are not simulate");
            flowSimulationOption.AddAlias("-sf");

            var milSimulationOption = new Option<bool>("--simulateMil", "[Optional] Simulate MIL library. If option is not defined the MIL library is not simulate");
            milSimulationOption.AddAlias("-sm");

            var useLocalAddressesOption = new Option<bool>("--useLocalAddreses", "[Optional] Use local addresses for all the wcf services");
            useLocalAddressesOption.AddAlias("-la");

            var flowReportOption = new Option<bool>("--reportAllFlow", "[Optional] Enable all flows report. If option is not defined WriteReport defined in FlowsConfiguration are used");
            flowReportOption.AddAlias("-rf");

            var flowReportOnErrorOption = new Option<bool>("--reportOnlyError", "[Optional] Enable report for flows in error. If option is not defined WriteReport defined in FlowsConfiguration are used");
            flowReportOnErrorOption.AddAlias("-re");

            var isWaferLessMode = new Option<bool>("--waferLessMode", "[Optional] Wafer less mode");
            isWaferLessMode.AddAlias("-wl");

            rootCommand.AddOption(configurationOption);
            rootCommand.AddOption(hardwareSimulationOption);
            rootCommand.AddOption(flowSimulationOption);
            rootCommand.AddOption(milSimulationOption);
            rootCommand.AddOption(useLocalAddressesOption);
            rootCommand.AddOption(flowReportOption);
            rootCommand.AddOption(flowReportOnErrorOption);
            rootCommand.AddOption(isWaferLessMode);

            rootCommand.Description = "Use options to defined the startup configuration";

            rootCommand.SetHandler((configurationOptionValue, hardwareSimulationOptionValue, flowSimulationOptionValue, milSimulationOptionValue, useLocalAddressesOptionValue, flowReportOptionValue, flowReportOnErrorOptionValue, isWaferLessModeValue) =>
            {
                ApplyConfigOptions(configurationOptionValue, hardwareSimulationOptionValue, flowSimulationOptionValue, milSimulationOptionValue, useLocalAddressesOptionValue,flowReportOptionValue, flowReportOnErrorOptionValue, isWaferLessModeValue);
            },
            configurationOption, hardwareSimulationOption, flowSimulationOption, milSimulationOption, useLocalAddressesOption, flowReportOption, flowReportOnErrorOption, isWaferLessMode);

            int exitCode = rootCommand.Invoke(args);
            if (exitCode != 0)
            {
                throw new Exception($"Command line invocation failed: {exitCode}");
            }
        }

        private void ApplyConfigOptions(string config, bool simulateHardware, bool simulateFlow, bool simulateMil, bool useLocalAddresses, bool reportAllFlow, bool reportOnlyError, bool isWaferlessMode)
        {
            string configToDisplay = string.IsNullOrEmpty(config) ? "Undefined" : config;
            _status.AppendLine($"Configuration args: config {configToDisplay}  simulateHardware {simulateHardware} simulateFlow {simulateFlow} simulateMil {simulateMil} reportAllFlow {reportAllFlow}  reportOnlyError { reportOnlyError} isWaferlessMode {isWaferlessMode}");
            HardwaresAreSimulated = simulateHardware;
            FlowsAreSimulated = simulateFlow;
            MilIsSimulated = simulateMil;
            UseLocalAddresses = useLocalAddresses;
            IsWaferlessMode = isWaferlessMode;
            if (reportAllFlow)
            {
                AllFlowsReportMode = FlowReportConfiguration.AlwaysWrite;
            }
            else if (reportOnlyError)
            {
                AllFlowsReportMode = FlowReportConfiguration.WriteOnError;
            }
            else
            {
                AllFlowsReportMode = FlowReportConfiguration.NeverWrite;
            }
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

            CalibrationFolderPath = Path.Combine(rootDir, CalibrationFolderName);
            if (Directory.Exists(CalibrationFolderPath))
            {
                _status.AppendLine($"Calibration folder: {CalibrationFolderPath}");
            }
            else
            {
                _status.AppendLine($"[ERR] Calibration folder doesn't exist: {CalibrationFolderPath}");
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

        public bool HardwaresAreSimulated { get; private set; }
        public bool FlowsAreSimulated { get; private set; }
        public bool MilIsSimulated { get; private set; }
        public bool UseLocalAddresses { get; private set; }
        public bool IsWaferlessMode { get; private set; }

        public FlowReportConfiguration AllFlowsReportMode { get; private set; }
        public string ConfigurationName { get; private set; }

        // paths
        public string CalibrationFolderPath { get; private set; }

        public string ConfigurationFolderPath { get; private set; }

        //FileNames
        public string PMConfigurationFilePath => Path.Combine(ConfigurationFolderPath, PMConfigurationFileName);

        public string FlowsConfigurationFilePath => Path.Combine(ConfigurationFolderPath, FlowsConfigurationFileName);
        public string LogConfigurationFilePath => Path.Combine(ConfigurationFolderPath, LogConfigurationFileName);
        public string FDCsConfigurationFilePath => Path.Combine(ConfigurationFolderPath, FDCsConfigurationFileName);

        public string FDCsPersistentDataFilePath => Path.Combine(ConfigurationFolderPath, FDCsPersistentDataFileName);
    }
}
