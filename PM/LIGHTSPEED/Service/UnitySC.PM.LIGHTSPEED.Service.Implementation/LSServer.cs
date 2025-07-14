using System.Collections.Generic;
using System.ServiceModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.AttenuationFilter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.FiberSwitch;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Mppc;
using UnitySC.PM.Shared.Hardware.Service.Interface.OpticalPowermeter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.PolarisationFilter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.FastAttenuation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Implementation
{
    public class LSServer : BaseServer
    {
        //public static LightspeedState State { get; private set; }
        private Dictionary<BaseService, ServiceHost> _hosts = new Dictionary<BaseService, ServiceHost>();

        private IGlobalDeviceService _globalDeviceService;
        private IGlobalStatusService _globalStatusService;
        private IGlobalStatusServer _globalStatusServiceCallback;
        private ILogger _logger;

        private IPlcService _plcService;
        private IAttenuationFilterService _attenuationFilterService;
        private IPolarisationFilterService _polarisationFilterService;
        private IDistanceSensorService _distanceSensorService;
        private IFiberSwitchService _fiberSwitchService;
        private ILaserService _laserService;
        private IMppcService _mppcService;
        private IOpticalPowermeterService _opticalPowermeterService;
        private IShutterService _shutterService;
        private IChamberService _chamberService;
        private IFastAttenuationService _fastAttenuationService;

        private LSAcquisitionService _acquisitionService;
        private LSRotatorsKitCalibrationService _rotatorsKitCalibrationService;
        private LSFeedbackLoopService _feedbackLoopService;
        private NSTLiseHFService _liseHFService;

        public LSServer(ILogger logger) : base(logger)
        {
            _logger = logger;
            _globalDeviceService = ClassLocator.Default.GetInstance<IGlobalDeviceService>();
            _globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
            _globalStatusServiceCallback = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            _acquisitionService = ClassLocator.Default.GetInstance<LSAcquisitionService>();
            _rotatorsKitCalibrationService = ClassLocator.Default.GetInstance<LSRotatorsKitCalibrationService>();
            _feedbackLoopService = ClassLocator.Default.GetInstance<LSFeedbackLoopService>();
            _liseHFService = ClassLocator.Default.GetInstance<NSTLiseHFService>();

            _plcService = ClassLocator.Default.GetInstance<IPlcService>();
            _attenuationFilterService = ClassLocator.Default.GetInstance<IAttenuationFilterService>();
            _polarisationFilterService = ClassLocator.Default.GetInstance<IPolarisationFilterService>();
            _distanceSensorService = ClassLocator.Default.GetInstance<IDistanceSensorService>();
            _fiberSwitchService = ClassLocator.Default.GetInstance<IFiberSwitchService>();
            _laserService = ClassLocator.Default.GetInstance<ILaserService>();
            _mppcService = ClassLocator.Default.GetInstance<IMppcService>();
            _opticalPowermeterService = ClassLocator.Default.GetInstance<IOpticalPowermeterService>();
            _shutterService = ClassLocator.Default.GetInstance<IShutterService>();
            _chamberService = ClassLocator.Default.GetInstance<IChamberService>();
            _fastAttenuationService = ClassLocator.Default.GetInstance<IFastAttenuationService>();
        }

        public override void Start()
        {
            _globalStatusServiceCallback.SetGlobalState(PMGlobalStates.Initializing);

            ClassLocator.Default.GetInstance<HardwareManager>().Init();

            StartService((BaseService)_plcService);
            StartService((BaseService)_attenuationFilterService);
            StartService((BaseService)_polarisationFilterService);
            StartService((BaseService)_distanceSensorService);
            StartService((BaseService)_fiberSwitchService);
            StartService((BaseService)_laserService);
            StartService((BaseService)_mppcService);
            StartService((BaseService)_opticalPowermeterService);
            StartService((BaseService)_shutterService);
            StartService((BaseService)_chamberService);
            StartService((BaseService)_fastAttenuationService);

            StartService((BaseService)_globalDeviceService);
            StartService((BaseService)_globalStatusService);

            StartService(_acquisitionService);
            StartService(_rotatorsKitCalibrationService);
            StartService(_liseHFService);
            StartService(_feedbackLoopService);

            HardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();

            _plcService.Connect();
            _attenuationFilterService.Connect();
            _polarisationFilterService.Connect();
            _distanceSensorService.Connect();
            _fiberSwitchService.Connect();
            _laserService.Connect();

            foreach (var mppc in HardwareManager.Mppcs)
            {
                MppcCollector collector = (mppc.Key == MppcCollector.WIDE.ToString()) ? MppcCollector.WIDE : MppcCollector.NARROW;
                _mppcService.Connect(collector);
            }
            foreach (var powermeter in HardwareManager.OpticalPowermeters)
            {
                PowerIlluminationFlow flow = (powermeter.Key == PowerIlluminationFlow.HS.ToString()) ? PowerIlluminationFlow.HS : PowerIlluminationFlow.HT;
                _opticalPowermeterService.Connect(flow);
            }

            _shutterService.Connect();
            _chamberService.Connect();
            _fastAttenuationService.Connect();

            _logger.Information("Procees module initialized");
        }

        public override void Stop()
        {
            foreach (var kvp in _hosts)
                StopService(service: kvp.Key, host: kvp.Value);
        }

        private HardwareManager _hardwareManager;

        public HardwareManager HardwareManager
        {
            get
            {
                if (_hardwareManager == null)
                    _hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();

                return _hardwareManager;
            }
            set { _hardwareManager = value; }
        }
    }
}
