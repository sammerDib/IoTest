using System;
using System.IO;

using UnitySC.PM.HLS.Service.Interface;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;

//using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.HLS.Hardware.Manager
{
    public class HlsHardwareManager : HardwareManager, IHardwareManager
    {
        private ILogger _logger;

        private IGlobalStatusServer _globalStatus;

        public const string HardwareConfigurationFileName = "HlsHardwareConfiguration.xml";

        //private const string ReformulationFileName = "Reformulation.xml";
        private IServiceConfigurationManager _configManager;

        public HlsHardwareManager(ILogger logger, IServiceConfigurationManager configManager) : base(logger)
        {
            _logger = logger;
            _configManager = configManager;
            //ReformulationMessageManager.Init(Path.Combine(_configManager.ConfigurationFolderPath, ReformulationFileName));
        }

        public new bool Init()
        {
            _globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Initializing, new Message(MessageLevel.Information, "Loading configuration")));

            // Load XML configuration file            //............................

            string fullPath = Path.Combine(_configManager.ConfigurationFolderPath, HardwareConfigurationFileName);

            _logger?.Information("Loading HeLioS hardware configuration from " + fullPath);
            var hardwareConfiguration = XML.Deserialize<HlsHardwareConfiguration>(fullPath);
            if (_configManager.HardwaresAreSimulated)
            {
                hardwareConfiguration.SetAllHardwareInSimulation();
            }

            // Initialize the lists, it is used when configuration items depend on other ones.
            // For example the probes depend on objectives. They are linked throw an ID in the xml file but we want to have a list of objectives instead of a list of objectives Ids
            var initFatalError = InitializeDevices(hardwareConfiguration);

            // No fatal error
            if (!initFatalError)
                _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Free, new Message(MessageLevel.Information, "Initialization Done")));
            else
                _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Information, "Initialization Failed")));

            return !initFatalError;
        }

        private bool InitializeDevices(HlsHardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;

            hardwareConfiguration.InitializeObjectsFromIDs();

            // Shared Devices
            try
            {
                base.InitializeDevices(hardwareConfiguration);
            }
            catch (Exception ex)
            {
                initFatalError = true;
                _logger.Fatal(ex, "Shared devices initialization Failed");
                _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "Shared devices initialization Failed :" + ex.Message, "Shared Devices")));
            }

            return initFatalError;
        }

        public void Init(IHardwareConfiguation hardwareConfiguation, IGlobalStatusServer globalStatus)
        {
        }

        public void Reset()
        {
            UnitializeHlsDevices();
            if (!Init())
            {
                throw new Exception("Initialisation devices failed");
            }
        }

        public void Stop()
        {
            //TODO Implement me
            throw new NotImplementedException();
        }

        private void UnitializeHlsDevices()
        {
            _globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            base.UnitializeDevices();
        }

        public void ResetAxis()
        {
            //TODO Implement me
            throw new NotImplementedException();
        }
    }
}
