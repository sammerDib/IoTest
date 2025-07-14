using System;
using System.Linq;
using System.Threading.Tasks;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Shared;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.AutofocusV2;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Alignment;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.PM.ANA.Service.Interface.Compatibility;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Host
{
    public class AnaServer : BaseServer
    {
        private readonly IProbeService _probeService;
        private readonly IAxesService _axesService;
        private readonly ICameraServiceEx _cameraService;
        private readonly IGlobalStatusService _globalStatusService;
        private readonly IPMUserService _pmUserService;
        private readonly ICalibrationService _calibrationService;
        private readonly IProbeAlignmentService _probeAlignmentService;
        private readonly IAlgoService _algoService;
        private readonly IChamberService _chamberService;
        private readonly IFDCService _fdcService;
        private readonly IANARecipeService _recipeService;
        private readonly ICompatibilityService _compatibilityService;
        private readonly ILightService _lightService;
        private readonly IChuckService _chuckService;
        private readonly IReferentialService _referentialService;
        private readonly IMeasureService _measureService;
        private readonly IContextService _contextService;
        private readonly IControllerService _controllerService;
        private readonly ILaserService _laserService;
        private readonly IShutterService _shutterService;
        private readonly IPlcService _plcService;
        private readonly IMotionAxesService _motionAxesService;
        private readonly IUTOPMService _utopmService;
        private readonly ISpectroService _spectroService;
        private readonly IClientFDCsService _clientFDCsService;
        private readonly IFfuService _ffuService;


        private readonly IGlobalStatusServer _globalStatusServer;
        private readonly AnaHardwareManager _hardwareManager;
        private readonly FDCManager _fdcManager;
        private readonly ILogger _logger;
        private readonly ManagedEventQueue _algorithmLoggerService;
        private readonly Core.Shared.EventForwarder _eventForwarder; // Event forwarding from C++ algorithm library to Host application

        private ServiceAddress _anaServiceAddress = null;

        public AnaServer(ILogger logger) : base(logger)
        {
            _probeService = ClassLocator.Default.GetInstance<IProbeService>();
            _axesService = ClassLocator.Default.GetInstance<IAxesService>();
            _chuckService = ClassLocator.Default.GetInstance<IChuckService>();
            _globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
            _cameraService = ClassLocator.Default.GetInstance<ICameraServiceEx>();
            _pmUserService = ClassLocator.Default.GetInstance<IPMUserService>();
            _calibrationService = ClassLocator.Default.GetInstance<ICalibrationService>();
            _probeAlignmentService = ClassLocator.Default.GetInstance<IProbeAlignmentService>();
            _algoService = ClassLocator.Default.GetInstance<IAlgoService>();
            _chamberService = ClassLocator.Default.GetInstance<IChamberService>();
            _fdcService = ClassLocator.Default.GetInstance<IFDCService>();
            _recipeService = ClassLocator.Default.GetInstance<IANARecipeService>();
            _compatibilityService = ClassLocator.Default.GetInstance<ICompatibilityService>();
            _lightService = ClassLocator.Default.GetInstance<ILightService>();
            _referentialService = ClassLocator.Default.GetInstance<IReferentialService>();
            _measureService = ClassLocator.Default.GetInstance<IMeasureService>();
            _contextService = ClassLocator.Default.GetInstance<IContextService>();
            _controllerService = ClassLocator.Default.GetInstance<IControllerService>();
            _laserService = ClassLocator.Default.GetInstance<ILaserService>();
            _shutterService = ClassLocator.Default.GetInstance<IShutterService>();
            _motionAxesService = ClassLocator.Default.GetInstance<IMotionAxesService>();
            _utopmService = ClassLocator.Default.GetInstance<IUTOPMService>();
            _spectroService = ClassLocator.Default.GetInstance<ISpectroService>();
            _plcService = ClassLocator.Default.GetInstance<IPlcService>();
            _clientFDCsService = ClassLocator.Default.GetInstance<IClientFDCsService>();
            _ffuService = ClassLocator.Default.GetInstance<IFfuService>();

            _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
            _logger = logger;
            _algorithmLoggerService = new ManagedEventQueue();
            _eventForwarder = new Core.Shared.EventForwarder(_logger, "[ALGOS]"); // note de RTI : Pour logger dans la dll algo opencv
            _algorithmLoggerService.AddMessageEventHandler(_eventForwarder.ForwardEvent);
        }

        public override void Start()
        {
            var isWaferLessMode = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().IsWaferlessMode;
            if (isWaferLessMode)
            {
                _anaServiceAddress = ClassLocator.Default.GetInstance<ServiceConfigurationWaferLess>().AnalyseServiceAddress;
            }

            Task initGlobalServices = new Task(InitializeGlobalServices);
            Task initHardware = new Task(InitializeAnaHardware);
            Task initPmtc = new Task(InitializePmtc);
            Task initServices = new Task(StartServices);
            Task initFdc = new Task(InitializeFdc);

            try
            {
                initGlobalServices.Start();
                initGlobalServices.Wait();

                initHardware.Start();
                initHardware.Wait();
                initPmtc.Start();
                initPmtc.Wait();
                initServices.Start();
                initServices.Wait();
                initFdc.Start();
                initFdc.Wait();
            }
            catch (Exception ex)
            {
                if (!initGlobalServices.IsFaulted && !initPmtc.IsFaulted && !initServices.IsFaulted && !initFdc.IsFaulted
                    && initHardware.IsFaulted)
                {
                    var messageInitErr = "Hardware initialization error : " + ex.InnerException.Message;
                    _logger.Fatal(ex, messageInitErr);

                    _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, messageInitErr)));
                    return;
                }
                else
                {
                    var messageInitErr = "AnaServer initialization Fatal Error. Analyse Server will be stopped and must be closed and restarted";
                    _logger.Fatal(ex, messageInitErr);
                    _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, messageInitErr)));
                    Stop();
                    return;
                }
            }

            var messageInitOk = "AnaServer initialization complete";
            _logger.Information(messageInitOk);
            _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Free, new Message(MessageLevel.Information, messageInitOk)));
        }

        public override void Stop()
        {
            try
            {
                _algorithmLoggerService?.RemoveMessageEventHandler(_eventForwarder.ForwardEvent);
                _logger.Information("MessageEventHandler successfully removed");
            }
            catch
            {
                _logger.Information("MessageEventHandler remove failure");
            }

            try
            {
                _fdcManager.ForceSavingPersistantData();
                _logger.Information("Persistant FDCs successfully saved");
            }
            catch
            {
                _logger.Information("Persistant FDCs save failure");
            }

            try
            {
                StopAllServiceHost();
                _logger.Information("Analyse services successfully stopped");
            }
            catch
            {
                _logger.Information("Analyse services stop failure");
            }

            try
            {
                _hardwareManager?.Shutdown();
                _logger.Information("Analyse hardware manager successfully stopped");
            }
            catch
            {
                _logger.Information("Analyse hardware manager stop failure");
            }
        }

        private void InitializeGlobalServices()
        {
            StartService((BaseService)_globalStatusService, _anaServiceAddress);
            StartService((BaseService)_pmUserService, _anaServiceAddress);
            var messageInitStart = "AnaServer initialization ...";
            _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Initializing, new Message(MessageLevel.Information, messageInitStart)));
        }

        private void InitializeAnaHardware()
        {
            _logger.Information("Hardware Initialization ...");
            _hardwareManager.Init();
            AddMessageIfCalibrationsDoesntExist();
        }

        private void StartServices()
        {
            StartService((BaseService)_probeService, _anaServiceAddress);
            StartService((BaseService)_axesService, _anaServiceAddress);
            StartService((BaseService)_chuckService, _anaServiceAddress);
            StartService((BaseService)_cameraService, _anaServiceAddress);
            StartService((BaseService)_calibrationService, _anaServiceAddress);
            StartService((BaseService)_probeAlignmentService, _anaServiceAddress);
            StartService((BaseService)_algoService, _anaServiceAddress);
            StartService((BaseService)_chamberService, _anaServiceAddress);
            StartService((BaseService)_fdcService, _anaServiceAddress);
            StartService((BaseService)_recipeService, _anaServiceAddress);
            StartService((BaseService)_compatibilityService, _anaServiceAddress);
            StartService((BaseService)_lightService, _anaServiceAddress);
            StartService((BaseService)_referentialService, _anaServiceAddress);
            StartService((BaseService)_measureService, _anaServiceAddress);
            StartService((BaseService)_contextService, _anaServiceAddress);
            StartService((BaseService)_controllerService, _anaServiceAddress);
            StartService((BaseService)_laserService, _anaServiceAddress);
            StartService((BaseService)_shutterService, _anaServiceAddress);
            StartService((BaseService)_plcService, _anaServiceAddress);
            StartService((BaseService)_motionAxesService, _anaServiceAddress);
            StartService((BaseService)_utopmService, _anaServiceAddress);
            StartService((BaseService)_spectroService, _anaServiceAddress);
            StartService((BaseService)_clientFDCsService, _anaServiceAddress);
            StartService((BaseService)_ffuService, _anaServiceAddress);


            bool? mountainsIsHostedByPM = ClassLocator.Default.GetInstance<ExternalProcessingConfiguration>().Mountains?.IsHostedByPM;
            if (mountainsIsHostedByPM.HasValue && mountainsIsHostedByPM.Value)
            {
                var mountainsGatewayService = ClassLocator.Default.GetInstance<IMountainsGatewayService>();
                StartService((BaseService)mountainsGatewayService, _anaServiceAddress);
            }
        }

        private void InitializePmtc()
        {
            _logger.Information("PMTC Manager Initialization ...");
            var pmtc = ClassLocator.Default.GetInstance<IPMTCManager>();
            pmtc.Init_Status();
            pmtc.Init_Services();
        }

        private void InitializeFdc()
        {
            // Start monitoring FDC
            _fdcManager.StartMonitoringFDC();
            RegisterFdcProviders();
            _fdcManager.RequestAllFDCsUpdate();
        }

        private void RegisterFdcProviders()
        {
            ClassLocator.Default.GetInstance<ChamberFDCs>().Register();
            ClassLocator.Default.GetInstance<ClientFDCs>().Register();
            ClassLocator.Default.GetInstance<BareWaferAlignmentFlowFDCProvider>().Register();
            ClassLocator.Default.GetInstance<PatternRecFlowFDCProvider>().Register();
            ClassLocator.Default.GetInstance<AFLiseFlowFDCProvider>().Register();
            ClassLocator.Default.GetInstance<AFCameraFlowFDCProvider>().Register();
            ClassLocator.Default.GetInstance<ANARecipeExecutionManagerFDCProvider>().Register();
            ClassLocator.Default.GetInstance<CalibrationManagerFDCProvider>().Register();

            // registering fdc providers direclty link to harware devices
            _hardwareManager.FdcProvidersRegistering();

            bool HasProbeLiseHF = _hardwareManager.Probes.ContainsKey("ProbeLiseHF");
            if (HasProbeLiseHF)
            {
                ClassLocator.Default.GetInstance<ProbeLiseHFFDCProvider>().Register();
            }
        }

        private void AddMessageIfCalibrationsDoesntExist()
        {
            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            var xyCalibration = calibrationManager.Calibrations.OfType<XYCalibrationData>().FirstOrDefault();
            if (xyCalibration is null)
            {
                var message = new Message(MessageLevel.Warning, "XY calibration is missing");
                _globalStatusServer.AddMessage(message);
            }

            var objectiveCalibration = calibrationManager.Calibrations.OfType<ObjectivesCalibrationData>().FirstOrDefault();
            if (objectiveCalibration?.Calibrations is null)
            {
                var message = new Message(MessageLevel.Warning, "Objectives calibration is missing");
                _globalStatusServer.AddMessage(message);
            }
            else
            {
                foreach (var obj in _hardwareManager.GetObjectiveConfigs())
                {
                    if (!objectiveCalibration.Calibrations.Exists(x => x.DeviceId == obj.DeviceID && x.Image != null && x.AutoFocus != null))
                    {
                        var message = new Message(MessageLevel.Warning, $"Objective calibration is missing for {obj.DeviceID}");
                        _globalStatusServer.AddMessage(message);
                    }
                }
            }
        }
    }
}
