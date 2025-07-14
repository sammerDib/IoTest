using System;
using System.Threading.Tasks;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.PM.EME.Service.Interface.Chamber;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.PM.EME.Service.Interface.Chuck;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using UnitySCSharedAlgosOpenCVWrapper;

using EventForwarder = UnitySC.PM.EME.Service.Core.Shared.EventForwarder;

namespace UnitySC.PM.EME.Service.Host
{
    public class EmeServer : BaseServer
    {
        private readonly IEMEChamberService _chamberService;
        private readonly IEMEChuckService _chuckService;
        private readonly IGlobalStatusService _globalStatusService;
        private readonly ICameraServiceEx _cameraService;
        private readonly ICalibrationService _calibrationService;
        private readonly IPMUserService _pmUserService;        
        private readonly IEmeraMotionAxesService _motionAxesService;
        private readonly IFilterWheelService _filterWheelService;
        private readonly IAlgoService _algoService;
        private readonly IReferentialService _referentialService;
        private readonly IEMERecipeService _emeRecipeService;        
        private readonly IEMELightService _lightService;
        private readonly IDistanceSensorService _distanceSensorService;
        private readonly IPlcService _plcService;
        private readonly IChillerService _chillerService;
        private readonly ILogger _logger;
        private readonly IUTOPMService _utopmService;

        private readonly EmeHardwareManager _hardwareManager;
        private readonly IGlobalStatusServer _globalStatusServer;
        private readonly FDCManager _fdcManager;


        // Event forwarding from C++ algorithm library to Host application
        private ManagedEventQueue _algorithmLoggerService;
        private EventForwarder _eventForwarder;

        public EmeServer(ILogger logger) : base(logger)
        {
            _logger = logger;
            _globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
            _cameraService = ClassLocator.Default.GetInstance<ICameraServiceEx>();
            _calibrationService = ClassLocator.Default.GetInstance<ICalibrationService>();
            _pmUserService = ClassLocator.Default.GetInstance<IPMUserService>();
            _chamberService = ClassLocator.Default.GetInstance<IEMEChamberService>();
            _chuckService = ClassLocator.Default.GetInstance<IEMEChuckService>();
            _motionAxesService = ClassLocator.Default.GetInstance<IEmeraMotionAxesService>();
            _filterWheelService = ClassLocator.Default.GetInstance<IFilterWheelService>();
            _algoService = ClassLocator.Default.GetInstance<IAlgoService>();
            _referentialService = ClassLocator.Default.GetInstance<IReferentialService>();
            _lightService = ClassLocator.Default.GetInstance<IEMELightService>();             
            _emeRecipeService = ClassLocator.Default.GetInstance<IEMERecipeService>();
            _distanceSensorService = ClassLocator.Default.GetInstance<IDistanceSensorService>();
            _plcService = ClassLocator.Default.GetInstance<IPlcService>();
            _chillerService = ClassLocator.Default.GetInstance<IChillerService>();

            _hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _utopmService = ClassLocator.Default.GetInstance<IUTOPMService>();
            _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
        }
        private void InitializeGlobalServices()
        {
            StartService((BaseService)_globalStatusService);
            StartService((BaseService)_pmUserService);
            var messageInitStart = "EmeServer initialization ...";
            _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Initializing, new Message(MessageLevel.Information, messageInitStart)));
        }
        public override void Start()
        {

            Task initGlobalServices = new Task(InitializeGlobalServices);
            Task initHardware = new Task(InitializeEmeHardware);
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
                //initFdc.Start(); FDCs not used for the moment
                //initFdc.Wait();

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
                    var messageInitErr = "EmeServer initialization Fatal Error. Emera Server will be stopped and must be closed and restarted";
                    _logger.Fatal(ex, messageInitErr);
                    _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, messageInitErr)));
                    Stop();
                    return;
                }
            }

            var messageInitOk = "EmeServer initialization complete";
            _logger.Information(messageInitOk);
            _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Free, new Message(MessageLevel.Information, messageInitOk)));

        }
        private void StartServices()
        {
            StartService((BaseService)_cameraService);
            StartService((BaseService)_calibrationService);
            StartService((BaseService)_chamberService);
            StartService((BaseService)_chuckService);
            StartService((BaseService)_motionAxesService);
            StartService((BaseService)_filterWheelService);
            StartService((BaseService)_algoService);
            StartService((BaseService)_referentialService);            
            StartService((BaseService)_lightService);
            StartService((BaseService)_emeRecipeService);
            StartService((BaseService)_distanceSensorService);
            StartService((BaseService)_plcService);
            StartService((BaseService)_chillerService);
            StartService((BaseService)_utopmService);

            _algorithmLoggerService = new ManagedEventQueue();
            _eventForwarder = new EventForwarder(_logger, "[ALGOS]"); // note de RTI : Pour logger dans la dll algo opencv
            _algorithmLoggerService.AddMessageEventHandler(_eventForwarder.ForwardEvent);
        }

        public override void Stop()
        {
            try
            {
                _algorithmLoggerService?.RemoveMessageEventHandler(_eventForwarder.ForwardEvent);
                _logger?.Information("RemoveMessageEventHandler successfully removed");
            }
            catch
            {
                _logger?.Information("MessageEventHandler remove failure");
            }          

            try
            {
                StopAllServiceHost();
                _logger?.Information("Emera services successfully stopped");
            }
            catch
            {
                _logger?.Information("Emera services stop failure");
            }

            try
            {
                _hardwareManager?.Stop();
                _logger?.Information("Emera hardware manager successfully stopped");
            }
            catch
            {
                _logger?.Information("Emera hardware manager stop failure");
            }
        }

        private void InitializeEmeHardware()
        {
            _logger?.Information("Hardware Initialisation ...");
            bool isHWHasInitErrors = !_hardwareManager.Init();

            if (isHWHasInitErrors)
            {
                _logger.Warning("Hardware initialisation incomplete.");
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
            // Get Instance of all FDCs providers
        }
    }
}
