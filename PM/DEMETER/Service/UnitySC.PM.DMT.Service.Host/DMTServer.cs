using System;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Implementation;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Chamber;
using UnitySC.PM.DMT.Service.Interface.Chuck;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using IDMTRecipeService = UnitySC.PM.DMT.Service.Interface.IDMTRecipeService;

namespace UnitySC.PM.DMT.Service.Host
{
    public class DMTServer : BaseServer
    {
        private readonly IDMTCameraService _cameraService;
        private readonly IDMTScreenService _screenService;
        private readonly IDMTAlgorithmsService _algorithmsService;
        private readonly IDMTRecipeService _recipeService;
        private readonly ICalibrationService _calibrationService;
        private readonly IPlcService _plcService;
        private readonly IDMTChamberService _chamberService;
        private readonly IDMTChuckService _chuckService;
        private readonly IMotionAxesService _motionAxesService;
        private readonly ILogger _logger;
        private readonly IGlobalDeviceService _globalDeviceService;
        private readonly IGlobalStatusService _globalStatusService;
        private readonly IGlobalStatusServer _globalStatusServer;
        private readonly IPMUserService _pmUserService;
        private readonly IUTOPMService _utopmService;
        private readonly IFDCService _fdcService;
        private readonly IFfuService _ffuService;

        private readonly FringeManager _fringeManager;
        private readonly ICalibrationManager _calibrationManager;
        private readonly AlgorithmManager _algorithmManager;
        private readonly DMTHardwareManager _hardwareManager;
        private readonly IPMTCManager _pmtcManager;

        public DMTServer(ILogger logger) : base(logger)
        {
            _logger = logger;

            _globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
            _globalDeviceService = ClassLocator.Default.GetInstance<IGlobalDeviceService>();
            _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            _plcService = ClassLocator.Default.GetInstance<IPlcService>();
            _chamberService = ClassLocator.Default.GetInstance<IDMTChamberService>();
            _chuckService = ClassLocator.Default.GetInstance<IDMTChuckService>();
            _cameraService = ClassLocator.Default.GetInstance<IDMTCameraService>();
            _screenService = ClassLocator.Default.GetInstance<IDMTScreenService>();
            _motionAxesService = ClassLocator.Default.GetInstance<IMotionAxesService>();

            _fringeManager = ClassLocator.Default.GetInstance<FringeManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<ICalibrationManager>();
            _algorithmManager = ClassLocator.Default.GetInstance<AlgorithmManager>();
            _hardwareManager = ClassLocator.Default.GetInstance<DMTHardwareManager>();
            _algorithmsService = ClassLocator.Default.GetInstance<IDMTAlgorithmsService>();
            _recipeService = ClassLocator.Default.GetInstance<IDMTRecipeService>();
            _calibrationService = ClassLocator.Default.GetInstance<ICalibrationService>();
            _pmUserService = ClassLocator.Default.GetInstance<IPMUserService>();
            _utopmService = ClassLocator.Default.GetInstance<IUTOPMService>();
            _fdcService = ClassLocator.Default.GetInstance<IFDCService>();
            _ffuService = ClassLocator.Default.GetInstance<IFfuService>();

            _pmtcManager = ClassLocator.Default.GetInstance<IPMTCManager>();

            _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Initializing,
                new Message(MessageLevel.Information, "Loading configuration")));
        }

        public override void Start()
        {
            try
            {
                StartServices();
                InitializeManagers();
                InitializePmtc();
            }
            catch (Exception ex)
            {
                var messageInitErr = "DmtServer initialization Fatal Error. Demeter Server will be stopped and must be closed and restarted";
                _logger.Fatal(ex, messageInitErr);
                _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, messageInitErr)));
                Stop();
                return;
            }

            var messageInitOk = "DmtServer initialization complete";
            _logger.Information(messageInitOk);
            _globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Free, new Message(MessageLevel.Information, messageInitOk)));
        }

        public override void Stop()
        {
            StopAllServiceHost();
            _hardwareManager.Stop();
            _fringeManager.Shutdown();
            _algorithmManager.Shutdown();
            _calibrationManager.Shutdown();
        }

        private void StartServices()
        {
            StartService((BaseService)_globalStatusService);
            StartService((BaseService)_globalDeviceService);
            StartService((BaseService)_plcService);
            StartService((BaseService)_chamberService);
            StartService((BaseService)_chuckService);
            StartService((BaseService)_cameraService);
            StartService((BaseService)_screenService);
            StartService((BaseService)_motionAxesService);
            StartService((BaseService)_fdcService);
            StartService((BaseService)_ffuService);

            StartService((BaseService)_algorithmsService);
            StartService((BaseService)_recipeService);
            StartService((BaseService)_calibrationService);

            StartService((BaseService)_pmUserService);
            StartService((BaseService)_utopmService);
        }

        private void InitializeManagers()
        {
            _logger.Information("Initializing hardware manager ...");
            _hardwareManager.Init();

            _logger.Information("Initializing fringes manager ...");
            _fringeManager.Init();

            _logger.Information("Initializing algorithms manager ...");
            _algorithmManager.Init();

            _logger.Information("Initializing calibrations manager ...");
            _calibrationManager.Init();
        }

        private void InitializePmtc()
        {
            _logger.Information("Initializing PMTC manager ...");
            _pmtcManager.Init_Status();
            _pmtcManager.Init_Services();
        }
    }
}
