using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Serilog.Core;
using Serilog.Events;

using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Hardware.Probe;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.Configuration.ProbeLiseHF;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Shared.Interface;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Laser;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Shutter;
using UnitySC.PM.Shared.Hardware.Spectrometer;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

using IProbeConfig = UnitySC.PM.ANA.Service.Interface.IProbeConfig;

namespace UnitySC.PM.ANA.Hardware
{
    public class AnaHardwareManager : HardwareManager, IHardwareManager
    {
        private ILogger _logger;

        public Dictionary<string, IProbe> Probes { get; set; }
        public Dictionary<string, IObjectiveSelector> ObjectivesSelectors { get; set; }
        private IGlobalStatusServer _globalStatus;

        public const string HardwareConfigurationFileName = "AnaHardwareConfiguration.xml";
        private const string ReformulationFileName = "Reformulation.xml";
        private IPMServiceConfigurationManager _configManager;

        public AnaHardwareManager(ILogger logger, IHardwareLoggerFactory hardwareLoggerFactory, IPMServiceConfigurationManager configManager) : base(logger, hardwareLoggerFactory)
        {
            _logger = logger;
            _configManager = configManager;
            var loggerReformulation = hardwareLoggerFactory.CreateHardwareLogger($"{LogEventLevel.Debug}", "Reformulation", "Debug"); ;
            ReformulationMessageManager.Init(Path.Combine(_configManager.ConfigurationFolderPath, ReformulationFileName), loggerReformulation);
            Probes = new Dictionary<string, IProbe>();
            ObjectivesSelectors = new Dictionary<string, IObjectiveSelector>();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ShutdownProbes();
            UnitializeAnaDevices();
        }

        public void Init()
        {
            _globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            // Load XML configuration file            //............................

            string fullPath = Path.Combine(_configManager.ConfigurationFolderPath, HardwareConfigurationFileName);

            _logger?.Information("Loading Analyse hardware configuration from " + fullPath);
            var hardwareConfiguration = XML.Deserialize<AnaHardwareConfiguration>(fullPath);
            if (_configManager.HardwaresAreSimulated)
            {
                hardwareConfiguration.SetAllHardwareInSimulation();
            }

            // Initialize the lists, it is used when configuration items depend on other ones.
            // For example the probes depend on objectives. They are linked throw an ID in the xml file but we want to have a list of objectives instead of a list of objectives Ids
            InitializeDevices(hardwareConfiguration);

            InitializeAnaLightFDCProviderDico();
        }

        private void InitializeDevices(AnaHardwareConfiguration hardwareConfiguration)
        {
            var errorMessage = string.Empty;
            if (Probes == null)
            {
                Probes = new Dictionary<string, IProbe>();
            }
            if (ObjectivesSelectors == null)
            {
                ObjectivesSelectors = new Dictionary<string, IObjectiveSelector>();
            }

            //It is necessary to initialise the controllers and run the ACS controller monitoring
            //task before initialising the selector targets.The other devices(including the axes)
            //must be initialised afterwards.
            //If there is a problem on one of the axes making it impossible to move the stage, the tool will be put
            //into error and a hardware reset will be necessary.
            try
            {
                // Note that InitializeAllControllers return true if there is any error
                if (base.InitializeAllControllers(hardwareConfiguration))
                {
                    throw new Exception("InitializeAllControllers failed");
                }
            }
            catch (SafetyException ex)
            {
                errorMessage += "Initialization Aborted for SAFETY : " + ex.Message + " (Shared Controllers)";
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage += "Shared controllers initialization Failed : " + ex.Message + " (Shared Controllers)";
            }

            // Objectives selectors
            _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(MessageLevel.Information, " Initializing Objectives Selectors")));
            try
            {
                hardwareConfiguration.InitializeObjectsFromIDs();
                InitObjectiveSelector(hardwareConfiguration);
            }
            catch (Exception ex)
            {
                errorMessage += "Objectives selectors initialization Failed : " + ex.Message + " (Objectives Selectors)";
            }

            // Shared Devices
            try
            {
                // Note that InitializeDevices return true if there is any error
                if (base.InitializeDevices(hardwareConfiguration))
                {
                    throw new Exception("InitializeDevices failed");
                }
            }
            catch (SafetyException ex)
            {
                errorMessage += "Initialization Aborted for SAFETY : " + ex.Message + " (Shared Devices)";
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage += "Shared devices initialization Failed : " + ex.Message + " (Shared Devices)";
            }

            // Probes
            _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(MessageLevel.Information, "Initializing Probes")));
            try
            {
                InitProbes(hardwareConfiguration);
            }
            catch (Exception ex)
            {
                errorMessage += "Probes initialization Failed : " + ex.Message + " (Probes)";
            }

            if (errorMessage != string.Empty)
            {
                throw new Exception(errorMessage);
            }
        }

        protected override bool InitializeChuck(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.USPChuckConfigs.Exists(x => x.IsEnabled))
                return false;

            bool initFatalError = false;
            _logger.Information("chucks device initialization started...");
            foreach (var config in hardwareConfiguration.USPChuckConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information(string.Format("{0} chuck device creation", config.Name));
                var chuckLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Chuck.ToString(), config.Name);
                try
                {
                    Chuck = ANAChuck.Create(GlobalStatusServer, chuckLogger, config, Controllers);
                    try
                    {
                        _logger.Information(string.Format("{0} chuck device initialization", config.Name));
                        Chuck.Init();
                        _logger.Information(string.Format("{0} chuck device Status: {1} Status message: {2}", config.Name, Chuck.State.Status, Chuck.State.StatusMessage));
                    }
                    catch (Exception ex)
                    {
                        initFatalError = true;
                        _logger.Error(ex, string.Format("{0} chuck device init error", config.Name));
                        Chuck.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    initFatalError = true;
                    _logger.Error(ex, "Error during {0} chuck device creation", config.Name);
                    GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "chuck device initialization Failed", config.Name)));
                }
            }
            return initFatalError;
        }
        protected override void InitializeChambers(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.ChamberConfigs.Exists(x => x.IsEnabled))
                return;

            _logger.Information("chamber device initialization started...");
            foreach (var config in hardwareConfiguration.ChamberConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information(string.Format("{0} chamber device creation", config.Name));
                var ChamberLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Chamber.ToString(), config.Name);

                try
                {
                    Chamber = ANAChamber.Create(GlobalStatusServer, ChamberLogger, config, Controllers);
                    try
                    {
                        _logger.Information(string.Format("{0} Chamber device initialization", config.Name));
                        Chamber.Init();
                        _logger.Information(string.Format("{0} Chamber device Status: {1} Status message: {2}", config.Name, Chamber.State.Status, Chamber.State.StatusMessage));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, string.Format("{0} Chamber device init error", config.Name));
                        Chamber.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during {0} Chamber device creation", config.Name);
                    GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "Chamber device initialization Failed", config.Name)));
                }
            }
        }
        private void InitObjectiveSelector(AnaHardwareConfiguration hardwareConfiguration)
        {
            foreach (var objectiveSelectorConfig in hardwareConfiguration.ObjectivesSelectorConfigs.Where(x => x.IsEnabled))
            {
                var objectifLogger = HardwareLoggerFactory.CreateHardwareLogger(objectiveSelectorConfig.LogLevel.ToString(), "ObjectivesSelector", objectiveSelectorConfig.Name);

                IObjectiveSelector objectivesSelector = ObjectiveSelectorFactory.CreateObjectiveSelector(objectiveSelectorConfig, objectifLogger);
                if (objectivesSelector == null)
                    break;
                objectivesSelector.Init();
                ObjectivesSelectors.Add(objectivesSelector.DeviceID, objectivesSelector);
            }
            foreach (var cameraConfig in hardwareConfiguration.CameraConfigs)
            {
                if (!ObjectivesSelectors.ContainsKey(cameraConfig.ObjectivesSelectorID))
                    throw new Exception($"Unknown objective selector \"{cameraConfig.ObjectivesSelectorID}\" for camera \"{cameraConfig.Name}\"");
                ObjectivesSelectors[cameraConfig.ObjectivesSelectorID].Position = cameraConfig.ModulePosition;
            }
        }

        private void InitProbes(AnaHardwareConfiguration hardwareConfiguration)
        {

            // We retrieve the simple probes and the simulated or non-simulated mode of the module.
            var simpleProbes = hardwareConfiguration.ProbeModulesConfigs
                .SelectMany(module => module.Probes
                    .Where(probe => !(probe is ProbeDualLiseConfig))
                    .Select(probe => (Probe: probe, IsSimulated: module.IsSimulated)) // Tuple with Probe and IsSimulated state
                )
                .ToList();
            InitializeSimpleProbes(simpleProbes);

            //We retrieve the dual probes and the simulated or non-simulated mode of the module.
            var dualProbes = hardwareConfiguration.ProbeModulesConfigs
                .SelectMany(module => module.Probes.OfType<ProbeDualLiseConfig>()
                .Select(probe => (Probe: probe, IsSimulated: module.IsSimulated)) // Tuple with Probe and IsSimulated state
                )
                .ToList();


            InitializeDualProbes(dualProbes, hardwareConfiguration);

            ClassLocator.Default.GetInstance<IProbeCalibrationManagerInit>().InitializeCalibrationManagers(Probes.Values.ToList());
        }

        private void InitializeSimpleProbes(List<(ProbeConfigBase Probe, bool IsSimulated)> simpleProbes)
        {
            foreach (var probeConfig in simpleProbes)
            {
                //Changes the probe to simulation mode according to its module
                probeConfig.Probe.IsSimulated = probeConfig.IsSimulated;
                switch (probeConfig.Probe)
                {
                    case ProbeLiseConfig liseConfig:
                        AddProbe(liseConfig);
                        break;
                    case ProbeLiseHFConfig liseHFConfig:
                        if (liseHFConfig.Filters == null || liseHFConfig.Filters.IsEmpty())
                        {
                            _logger?.Error($"Probe {liseHFConfig.Name} Filters definitions are missing! Skipping initialization.");
                            throw new Exception($"<{liseHFConfig.Name}> Failed due to missing Filters definitions");
                        }
                        var probeDevices = CreateProbeLiseHFDevices(liseHFConfig);
                        AddProbe(liseHFConfig, probeDevices);
                        break;
                    default:
                        _logger.Warning($"Unknow simple probe of type {probeConfig.Probe?.GetType()} and named {probeConfig.Probe?.Name}/{probeConfig.Probe?.DeviceID}");
                        break;
                }
            }
        }

        private void InitializeDualProbes(List<(ProbeDualLiseConfig Probe, bool IsSimulated)> dualProbes, AnaHardwareConfiguration hardwareConfiguration)
        {
            foreach (var dualLiseConfig in dualProbes)
            {
                dualLiseConfig.Probe.IsSimulated = dualLiseConfig.IsSimulated;
                var configUp = Probes[dualLiseConfig.Probe.ProbeUpID].Configuration as ProbeLiseConfig;
                var configDown = Probes[dualLiseConfig.Probe.ProbeDownID].Configuration as ProbeLiseConfig;
                AddProbe(dualLiseConfig.Probe, configUp: configUp, configDown: configDown);
            }
        }

        private void InitializeAnaLightFDCProviderDico()
        {
            foreach (var lightentry in Lights)
            {
                var lightBase = lightentry.Value;
                if (lightBase != null && lightBase.Config.IsEnabled)
                {
                    string shortLightName;
                    switch (lightentry.Key.ToLowerInvariant())
                    {
                        case "vis_white_led": //TOP WHITE
                            shortLightName = "White";
                            break;
                        case "vis_red_led": //TOP RED
                            shortLightName = "Red";
                            break;
                        case "halogen_uoh": //TOP NIR
                            shortLightName = "NIR";
                            break;
                        case "halogen_loh": // BOTTOM NIR
                            shortLightName = "BottomNIR";
                            break;
                        default:
                            _logger.Warning($"Unknow light configuration DeviceId <{lightentry.Key}> - Skip FDC Dico Init");
                            continue;
                    }

                    // Light UsageDuration
                    var dico = new Dictionary<LightBaseFDCType, string>();
                    string fdcNameLightUsageDuration = $"ANA_LightUsage_{shortLightName}";
                    dico.Add(LightBaseFDCType.UsageDuration, fdcNameLightUsageDuration);

                    lightBase.SetFDCTypeToNameDico(dico);
                }
            }

        }

        public void FdcProvidersRegistering()
        {
            foreach (var lightBase in Lights.Values)
            {
                if (lightBase != null && lightBase.Config.IsEnabled)
                {
                    lightBase.Register();
                }
            }
        }

        private ProbeLiseHFDevices CreateProbeLiseHFDevices(ProbeLiseHFConfig config)
        {
            return new ProbeLiseHFDevices
            {
                Shutter = ShutterSelector(config.ShutterID),
                Laser = LaserSelector(config.LaserID),
                OpticalRackAxes = MotionAxes as LiseHfAxes,
                Spectrometer = SpectrometerSelector(config.SpectrometerID)
            };
        }

        private void AddProbe(ProbeConfigBase config, ProbeLiseHFDevices devices = null, ProbeLiseConfig configUp = null, ProbeLiseConfig configDown = null)
        {
            IProbe probe = null;
            var probeLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Probe.ToString(), config.DeviceID);

            switch (config)
            {
                case ProbeLiseConfig liseConfig:
                    probe = ProbeFactory.CreateProbe(liseConfig, probeLogger);
                    break;

                case ProbeLiseHFConfig liseHFConfig:
                    probe = ProbeFactory.CreateProbe(liseHFConfig, devices, probeLogger);
                    break;

                case ProbeDualLiseConfig dualLiseConfig:
                    probe = ProbeFactory.CreateProbe(dualLiseConfig, configUp, configDown, probeLogger);
                    break;
                default:
                    _logger.Warning($"Unknow probe of type {config?.GetType()} and named {config?.Name}/{config?.DeviceID}");
                    return;
            }

            if (probe != null)
            {
                Probes.Add(probe.DeviceID, probe);
                _logger?.Information($"Probe added: {probe.DeviceID}");
                probe.Init();
            }
        }

        private SpectrometerBase SpectrometerSelector(string spectrometerID)
        {
            SpectrometerBase spectrometer = Spectrometers.FirstOrDefault(s => s.Key == spectrometerID).Value;
            if (spectrometer is null)
                throw new InvalidOperationException($"Spectrometer selector \"{spectrometerID}\" not found.");
            return spectrometer;
        }

        private ShutterBase ShutterSelector(string shutterID)
        {
            ShutterBase shutter = Shutters.FirstOrDefault(s => s.Key == shutterID).Value;
            if (shutter is null)
                throw new InvalidOperationException($"Shutter selector \"{shutterID}\" not found.");
            return shutter;
        }

        public List<ObjectiveConfig> GetObjectiveConfigs()
        {
            return ObjectivesSelectors.SelectMany(x => x.Value.Config.Objectives).ToList();
        }

        private LaserBase LaserSelector(string laserId)
        {
            LaserBase laser = Lasers.FirstOrDefault(s => s.Key == laserId).Value;
            if (laser is null)
                throw new InvalidOperationException($"Laser selector \"{laserId}\" not found.");
            return laser;
        }

        public IObjectiveSelector GetObjectiveSelectorOfObjective(string objectiveId)
        {
            IObjectiveSelector objSelector = ObjectivesSelectors.FirstOrDefault(s => s.Value.Config.FindObjective(objectiveId) != null).Value;
            if (objSelector is null)
                throw new InvalidOperationException($"Objective selector of objective \"{objectiveId}\" not found.");
            return objSelector;
        }

        public ObjectiveConfig GetObjectiveInUseById(string objectiveSelectorID)
        {
            var currentObjective = ObjectivesSelectors[objectiveSelectorID].GetObjectiveInUse();
            return currentObjective;
        }

        public ObjectiveConfig GetObjectiveInUseByProbe(string probeID)
        {
            IProbe probe;
            if (!Probes.TryGetValue(probeID, out probe))
                throw new InvalidOperationException($"Probe with ID={probeID} not found.");

            var probePosition = probe.Configuration.ModulePosition;
            var camera = Cameras.Values.FirstOrDefault(x => x.Config.ModulePosition == probePosition);
            if (camera is null)
                throw new InvalidOperationException($"No camera defined in {probePosition}");

            return GetObjectiveInUseByCamera(camera.DeviceID);
        }

        public ObjectiveConfig GetObjectiveInUseByCamera(string cameraID)
        {
            CameraBase camera;
            if (!Cameras.TryGetValue(cameraID, out camera))
                throw new InvalidOperationException("Invalid camera ID");

            var selelectorId = camera.Config.ObjectivesSelectorID;
            var currentObjective = ObjectivesSelectors[selelectorId].GetObjectiveInUse();
            return currentObjective;
        }

        public virtual ObjectiveConfig GetObjectiveInUseByPosition(ModulePositions modulePositions)
        {
            var camera = Cameras.Values.FirstOrDefault(x => x.Config.ModulePosition == modulePositions);
            if (camera is null)
                throw new InvalidOperationException($"No camera defined in {modulePositions}");

            return GetObjectiveInUseByCamera(camera.DeviceID);
        }

        public List<ObjectiveConfig> GetObjectiveConfigsByPosition(ModulePositions modulePositions)
        {
            var objectiveSelector = ObjectivesSelectors.Values.FirstOrDefault(selector => selector.Position == modulePositions);
            if (objectiveSelector is null)
                throw new InvalidOperationException($"No objective selector defined in {modulePositions}");

            return objectiveSelector.Config.Objectives;
        }

        public CameraBase GetMainCamera()
        {
            var mainCamera = Cameras.Values.FirstOrDefault(x => x.Config.IsMainCamera);
            if (mainCamera is null)
                throw new InvalidOperationException("The main camera is not defined");
            return mainCamera;
        }

        public IProbeLise GetProbeLiseUp()
        {
            var probeLiseUp = Probes.Values
                .OfType<ProbeLise>()
                .FirstOrDefault(_ => _.Configuration.ModulePosition == ModulePositions.Up);
            if (probeLiseUp is null)
            {
                throw new InvalidOperationException("No probe lise up defined");
            }

            return probeLiseUp;
        }

        public ObjectiveConfig GetMainObjectiveOfMainCamera()
        {
            IObjectiveSelector objectiveSelector;
            try
            {
                objectiveSelector = ObjectivesSelectors.Values
                        .First(_ => _.DeviceID == GetMainCamera().Config.ObjectivesSelectorID);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Objective selector of main camera not found");
            }

            var objective = objectiveSelector.Config.MainObjective;
            if (objective == null)
            {
                throw new InvalidOperationException($"No main objective found for objective selector '{objectiveSelector.DeviceID}'");
            }
            return objective;
        }

        public List<IProbeConfig> GetProbesConfigsByPosition(ModulePositions modulePosition)
        {
            var probes = Probes.Values.Where(x => x.Configuration.ModulePosition == modulePosition).Select(p => p.Configuration);
            if (probes.Count() == 0)
                throw new InvalidOperationException($"Probe on {modulePosition} not found.");

            return probes.ToList();
        }

        public PiezoController GetPiezoController(string objectiveID)
        {
            var piezoAxisID = GetPiezoAxisID(objectiveID);
            PiezoController piezoController = null;

            foreach (var controller in Controllers.Values)
            {
                if (controller.ControllerConfiguration is PiezoControllerConfig piezoControllerConfig
                    && piezoControllerConfig.PiezoAxisIDs.Any(x => x == piezoAxisID))
                {
                    piezoController = controller as PiezoController;
                    break;
                }
            }

            if (piezoController == null)
                throw new Exception($"No piezo controller found for objective {objectiveID} (this objective is configured with piezo axis {piezoAxisID})");

            return piezoController;
        }

        public string GetPiezoAxisID(string objectiveID)
        {
            var objectiveConfiguration = GetObjectiveConfigs().FirstOrDefault(x => x.DeviceID == objectiveID);
            string piezoAxisID = objectiveConfiguration?.PiezoAxisID;
            if (string.IsNullOrEmpty(piezoAxisID))
                throw new Exception($"Piezo axis is not defined for {objectiveID}");
            return piezoAxisID;
        }

        public void Init(IHardwareConfiguation hardwareConfiguation, IGlobalStatusServer globalStatusServer)
        {
        }

        public void Reset()
        {
            var messageResetStart = "Hardware Reset start ...";
            _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Initializing, new Message(MessageLevel.Information, messageResetStart)));
            UnitializeAnaDevices();
            try
            {
                Init();
            }
            catch (Exception ex)
            {
                var messageResetError = "Hardware Reset error : " + ex.Message;
                _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, messageResetError)));
                return;
            }

            var messageResetOk = "Hardware Reset done";
            _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Free, new Message(MessageLevel.Information, messageResetOk)));
        }

        public void Stop()
        {
            //TODO Implement me
            throw new NotImplementedException();
        }

        private void UnitializeAnaDevices()
        {
            _globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            base.UnitializeDevices();

            if (ObjectivesSelectors != null)
            {
                foreach (var o in ObjectivesSelectors)
                {
                    if (o.Value is LinMotUdp)
                    {
                        var linMotUp = o.Value as LinMotUdp;
                        linMotUp.Disconnect();
                    }
                }
            }
            ObjectivesSelectors.Clear();

            Probes.Clear();
        }

        private void ShutdownProbes()
        {
            foreach (IProbe probe in Probes.Values)
            {
                probe.Shutdown();
            }
        }
    }
}
