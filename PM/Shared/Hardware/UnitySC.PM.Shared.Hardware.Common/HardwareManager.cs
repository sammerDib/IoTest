using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Ffu;
using UnitySC.PM.Shared.Hardware.Ionizer;
using UnitySC.PM.Shared.Hardware.Laser;
using UnitySC.PM.Shared.Hardware.Led;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Mppc;
using UnitySC.PM.Shared.Hardware.OpticalPowermeter;
using UnitySC.PM.Shared.Hardware.Plc;
using UnitySC.PM.Shared.Hardware.PlcScreen;
using UnitySC.PM.Shared.Hardware.Rfid;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.Shutter;
using UnitySC.PM.Shared.Hardware.Spectrometer;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Common
{
    public class HardwareManager
    {
        private readonly ILogger _logger;

        protected IHardwareLoggerFactory HardwareLoggerFactory;

        private IGlobalStatusServer _globalStatusServer;

        public IGlobalStatusServer GlobalStatusServer
        {
            get
            {
                if (_globalStatusServer == null)
                {
                    _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
                }
                return _globalStatusServer;
            }
            protected set { _globalStatusServer = value; }
        }

        public Dictionary<string, ControllerBase> Controllers { get; set; }

        public Dictionary<string, MotionControllerBase> MotionControllers { get; set; }
        public PlcBase Plc { get; set; }
        public Dictionary<string, CameraBase> Cameras { get; set; }
        public IAxes Axes { get; set; }
        public MotionAxesBase MotionAxes { get; set; }
        public IUSPChuck Chuck { get; set; }

        public IChuckClamp ClampHandler
        {
            get
            {
                if ((Chuck is IChuckClamp handler))
                    return handler;
                else
                    throw new NotImplementedException("IClampHandler is not implemented");
            }
        }

        public IChuckPinLifts PinLiftHandler
        {
            get
            {
                if ((Chuck is IChuckPinLifts handler))
                    return handler;
                else
                    throw new NotImplementedException("IPinLiftsHandler is not implemented");
            }
        }

        public IChuckMaterialPresence MaterialPresenceHandler
        {
            get
            {
                if ((Chuck is IChuckMaterialPresence handler))
                    return handler;
                else
                    throw new NotImplementedException("IMaterialPresenceHandler is not implemented");
            }
        }

        public IRemovableChuck ChuckSizeDetectedHandler
        {
            get
            {
                if ((Chuck is IRemovableChuck handler))
                    return handler;
                else
                    throw new NotImplementedException("IRemovableChuckSizeSelection is not implemented");
            }
        }
        public IChuckAirBearing AirBearingHandler
        {
            get
            {
                if ((Chuck is IChuckAirBearing handler))
                    return handler;
                else
                    throw new NotImplementedException("IAirBearingHandler is not implemented");
            }
        }

        public IChuckLoadingPosition LoadingPositionHandler
        {
            get
            {
                if ((Chuck is IChuckLoadingPosition handler))
                    return handler;
                else
                    throw new NotImplementedException("IChuckLoadingPosition is not implemented");
            }
        }

        public IChuckInitialization InitializationHandler
        {
            get
            {
                if ((Chuck is IChuckInitialization handler))
                    return handler;
                else
                    throw new NotImplementedException("IInitializationHandler is not implemented");
            }
        }

        public DistanceSensorBase DistanceSensor { get; set; }
        public Dictionary<string, LaserBase> Lasers { get; set; }
        public Dictionary<string, MppcBase> Mppcs { get; set; }
        public Dictionary<string, OpticalPowermeterBase> OpticalPowermeters { get; set; }
        public Dictionary<string, ShutterBase> Shutters { get; set; }
        public Dictionary<string, SpectrometerBase> Spectrometers { get; set; }
        public ChamberBase Chamber { get; set; }
        public Dictionary<string, LightBase> Lights { get; set; }
        public Dictionary<string, FfuBase> Ffus { get; set; }
        public Dictionary<string, PlcScreenBase> PlcScreens { get; set; }
        public Dictionary<string, RfidBase> Rfids { get; set; }
        public IonizerBase Ionizer { get; set; }

        public HardwareManager(ILogger logger, IHardwareLoggerFactory hardwareLoggerFactory)
        {
            _logger = logger;
            this.HardwareLoggerFactory = hardwareLoggerFactory;

            Init();
        }

        private void Init()
        {
            if (Controllers == null)
            {
                Controllers = new Dictionary<string, ControllerBase>();
            }
            if (MotionControllers == null)
            {
                MotionControllers = new Dictionary<string, MotionControllerBase>();
            }
            if (Cameras == null)
            {
                Cameras = new Dictionary<string, CameraBase>();
            }
            if (Mppcs == null)
            {
                Mppcs = new Dictionary<string, MppcBase>();
            }
            if (OpticalPowermeters == null)
            {
                OpticalPowermeters = new Dictionary<string, OpticalPowermeterBase>();
            }
            if (Lasers == null)
            {
                Lasers = new Dictionary<string, LaserBase>();
            }
            if (Shutters == null)
            {
                Shutters = new Dictionary<string, ShutterBase>();
            }
            if (Spectrometers == null)
            {
                Spectrometers = new Dictionary<string, SpectrometerBase>();
            }
            if (Lights == null)
            {
                Lights = new Dictionary<string, LightBase>();
            }
            if (PlcScreens == null)
            {
                PlcScreens = new Dictionary<string, PlcScreenBase>();
            }
            if (Ffus == null)
            {
                Ffus = new Dictionary<string, FfuBase>();
            }
            if (Rfids == null)
            {
                Rfids = new Dictionary<string, RfidBase>();
            }
        }

        public virtual void Shutdown()
        {
            foreach (CameraBase cam in Cameras.Values)
            {
                cam.StopContinuousGrab();
                cam.Shutdown();
            }
            Cameras.Clear();

            Mil.Instance.Free();
        }

        protected bool InitializeAllControllers(HardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;
            initFatalError |= InitializeControllers(hardwareConfiguration);
            return initFatalError;
        }

        protected bool InitializeDevices(HardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;

            InitializeMotionControllers(hardwareConfiguration);
            InitializePlcs(hardwareConfiguration);
            InitializeChambers(hardwareConfiguration);
            initFatalError |= InitializeChuck(hardwareConfiguration);
            InitializeCameras(hardwareConfiguration);

            try
            {
                initFatalError |= InitializeAxes(hardwareConfiguration);
            }
            catch (SafetyException Ex)
            {
                throw Ex;
            }

            InitializeMotionAxes(hardwareConfiguration);
            InitializeDistanceSensor(hardwareConfiguration);
            InitializeLasers(hardwareConfiguration);
            InitializeMppc(hardwareConfiguration);
            InitializeOpticalPowermeters(hardwareConfiguration);
            InitializeShutters(hardwareConfiguration);
            InitializeSpectrometers(hardwareConfiguration);
            InitializeLights(hardwareConfiguration);
            InitializePlcScreens(hardwareConfiguration);
            InitializeFfus(hardwareConfiguration);
            InitializeRfids(hardwareConfiguration);
            InitializeIonizers(hardwareConfiguration);

            foreach (var camera in Cameras)
            {
                camera.Value.OnStatusChanged += DeviceStatusChanged;
            }
            return initFatalError;
        }

        protected void UnitializeDevices()
        {
            if (Controllers != null)
            {
                foreach (var c in Controllers)
                {
                    c.Value.Disconnect();
                }
            }
            Controllers.Clear();
            MotionControllers.Clear();
            if (Cameras != null)
            {
                foreach (var cam in Cameras.Values)
                {
                    cam.OnStatusChanged -= DeviceStatusChanged;
                    cam.StopContinuousGrab();
                    cam.Shutdown();
                }
            }
            Cameras.Clear();
            Lasers.Clear();
            Mppcs.Clear();
            OpticalPowermeters.Clear();
            Shutters.Clear();
            Spectrometers.Clear();
            Lights.Clear();
            PlcScreens.Clear();
            Plc = null;
            Chamber = null;
            Axes = null;
            Chuck = null;
            DistanceSensor = null;
            Ionizer = null;
        }

        protected virtual bool InitializeChuck(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.USPChuckConfigs.Exists(x => x.IsEnabled))
                return false;

            bool initFatlError = false;
            _logger.Information("chucks device initialization started...");
            foreach (var config in hardwareConfiguration.USPChuckConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"{config.Name} chuck device creation");
                var chuckLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Chuck.ToString(), config.Name);

                try
                {
                    if (config.IsSimulated)
                    {
                        Chuck = new USPDummyChuck(GlobalStatusServer, chuckLogger, config);
                    }
                    else
                    {
                        throw new Exception("Cannot initialize a non simulated chuck, use the specialized PM's InitializeChuck method");
                    }
                    try
                    {
                        _logger.Information($"{config.Name} chuck device initialization");
                        Chuck.Init();
                        _logger.Information($"{config.Name} chuck device Status: {Chuck.State.Status} Status message: {Chuck.State.StatusMessage}");
                    }
                    catch (Exception ex)
                    {
                        initFatlError = true;
                        _logger.Error(ex, $"{config.Name} chuck device init error", config.Name);
                        Chuck.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    initFatlError = true;
                    _logger.Error(ex, "Error during {0} chuck device creation", config.Name);
                    GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "chuck device initialization Failed", config.Name)));
                }
            }
            return initFatlError;
        }

        protected bool InitializeControllers(HardwareConfiguration hardwareConfiguration)
        {
            bool noEnabledController = !hardwareConfiguration.ControllerConfigs.Exists(x => x.IsEnabled);
            if (noEnabledController) return false;

            _logger.Information("Controllers initialization starting...");
            bool InitFatalError = false;

            // Create all controllers from config
            foreach (var config in hardwareConfiguration.ControllerConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"Create controller {config.Name}");
                var controllerLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Controller.ToString(), config.Name);

                var controller = ControllerFactory.CreateController(config, GlobalStatusServer, controllerLogger);
                if (!(controller is null))
                {
                    try
                    {
                        _logger.Information($"{config.Name} Controller initialization started");

                        var _initErrors = new List<Message>();
                        controller.Init(_initErrors);

                        bool atLeastOneFatalError = _initErrors.Exists(message => message.Level == MessageLevel.Fatal);
                        if (atLeastOneFatalError)
                            throw new Exception("Error during " + config.Name + " controller initialization");

                        Controllers.Add(config.DeviceID, controller);
                        _logger.Information($"{config.Name} Controller Status: {controller.State.Status} Status message: {controller.State.StatusMessage}");
                    }
                    catch (SafetyException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        InitFatalError = true;
                        _logger.Error(ex, $"{config.Name} Controller initialization error");
                        controller.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
            }

            if (hardwareConfiguration.AxesConfigs.Count > 0)
            {
                foreach (var controller in Controllers)
                {
                    if (controller.Value is IAxesController axesController)
                    {
                        // We take the first AxesConfigs because there is always only one
                        axesController.AxesConfiguration = hardwareConfiguration.AxesConfigs[0];
                    }
                }
            }
            return InitFatalError;
        }

        /// <summary>
        /// TODO GVA : Waiting to refactore the axes/motions
        /// </summary>
        /// <param name="hardwareConfiguration"></param>
        /// <returns></returns>
        protected void InitializeMotionControllers(HardwareConfiguration hardwareConfiguration)
        {
            bool noEnabledController = !hardwareConfiguration.MotionControllerConfigs.Exists(x => x.IsEnabled);
            if (noEnabledController) return;

            _logger.Information("Motion controllers initialization starting...");

            // Create all motion controllers from config
            foreach (var config in hardwareConfiguration.MotionControllerConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"Create controller {config.Name}");

                var motionControllerLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Controller.ToString(), config.Name);
                var motionController = ControllerFactory.CreateMotionController(config, GlobalStatusServer, motionControllerLogger);

                if (!(motionController is null))
                {
                    try
                    {
                        _logger.Information($"{config.Name} Motion controller initialization started");

                        var _initErrors = new List<Message>();
                        motionController.Init(_initErrors);

                        bool atLeastOneFatalError = _initErrors.Exists(message => message.Level == MessageLevel.Fatal);
                        if (atLeastOneFatalError)
                            throw new Exception("Error during " + config.Name + " motion controller initialization");

                        MotionControllers.Add(config.DeviceID, motionController);
                        _logger.Information($"{config.Name} Motion controller Status: {motionController.State.Status} Status message: {motionController.State.StatusMessage}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"{config.Name} Motion controller initialization error");
                        motionController.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
            }
        }

        protected void InitializePlcs(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("PLC device initialization");
            foreach (var config in hardwareConfiguration.PlcConfigs)
            {
                _logger.Information($"Plc {config.Name} create");

                try
                {
                    Plc = PlcFactory.Create(GlobalStatusServer, _logger, config, Controllers);
                    try
                    {
                        Plc.Init();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Plc device {config.Name} init error");
                        Plc.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create Plc device {config.Name}", ex);
                }
            }
        }

        protected virtual void InitializeChambers(HardwareConfiguration hardwareConfiguration)
        {
        }

        protected bool InitializeAxes(HardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;
            // Check Axes configuration enabled
            if ((hardwareConfiguration.AxesConfigs == null) || (!hardwareConfiguration.AxesConfigs.Exists(x => x.IsEnabled)))
                return initFatalError;

            var referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            foreach (var config in hardwareConfiguration.AxesConfigs.Where(x => x.IsEnabled))
            {
                try
                {
                    _logger.Information($"{config.Name} Axes device creation");
                    var axesLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Axes.ToString(), config.Name);
                    Axes = AxesFactory.CreateAxes(config, Controllers, GlobalStatusServer, axesLogger, referentialManager);
                    List<Message> _initErrors = new List<Message>();
                    Axes.Init(_initErrors);
                    if (_initErrors.Any())
                    {
                        bool atLeastOneFatalError = _initErrors.Exists(message => message.Level == MessageLevel.Fatal);
                        if (atLeastOneFatalError)
                            throw new Exception("Fatal error occurred during axes device initialization");
                    }
                }
                catch (Exception ex)
                {
                    initFatalError = true;
                    _logger.Error(ex, $"{config.Name} axes initialization error");
                    if (Axes != null) Axes.State = new DeviceState(DeviceStatus.Error, ex.Message);
                }

                if (!initFatalError)
                {
                    _logger.Information($"{config.Name} Axes device initialization complete");
                    if (Axes != null) Axes.State = new DeviceState(DeviceStatus.Ready, "Axes device initialization complete");
                }
            }
            return initFatalError;
        }

        protected virtual void InitializeMotionAxes(HardwareConfiguration hardwareConfiguration)
        {
            bool noEnabledMotionAxes = !hardwareConfiguration.MotionControllerConfigs.Exists(x => x.IsEnabled);
            if (noEnabledMotionAxes) return;

            var referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            foreach (var config in hardwareConfiguration.MotionAxesConfigs.Where(x => x.IsEnabled))
            {
                try
                {
                    _logger.Information($"{config.Name} Motion axes device creation");
                    var motionAxesLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Axes.ToString(), config.Name);

                    MotionAxes = AxesFactory.CreateMotionAxes(config, MotionControllers, GlobalStatusServer, motionAxesLogger, referentialManager);
                    List<Message> _initErrors = new List<Message>();
                    MotionAxes.Init(_initErrors);
                    if (_initErrors.Any())
                    {
                        bool atLeastOneFatalError = _initErrors.Exists(message => message.Level == MessageLevel.Fatal);
                        if (atLeastOneFatalError)
                            throw new Exception("Fatal error occurred during motion axes device initialization");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"{config.Name} motion axes initialization error");
                    if (MotionAxes != null) MotionAxes.State = new DeviceState(DeviceStatus.Error, ex.Message);
                }
            }
        }


        protected void InitializeCameras(HardwareConfiguration hardwareConfiguration)
        {
            // Matrox
            //.......
            var configs = hardwareConfiguration.CameraConfigs.OfType<MatroxCameraConfigBase>().Where(c => c.IsEnabled && !c.IsSimulated).ToList();
            if (configs.Any())
            {
                _logger.Information("Matrox Cameras initialization");
                var cameraManager = new MatroxCameraManager(_logger,HardwareLoggerFactory);
                bool haveError = cameraManager.Init(configs);
                if (haveError)
                {
                    _logger.Error("Matrox Camera initialization failed at least partially");
                    throw new Exception("Matrox Cameras initialization error");
                }
                Cameras.AddRange(cameraManager.Cameras);
            }

            // IDS
            //....
            foreach (var config in hardwareConfiguration.CameraConfigs.OfType<IDSCameraConfigBase>().Where(c => c.IsEnabled && !c.IsSimulated))
            {
                var camLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Camera.ToString(), config.Name);
                IDSCameraBase camera;

                switch (config)
                {
                    case UI524xCpNirIDSCameraConfig ui524xCpNirConfig:
                        {
                            camera = new UI524xCpNir(ui524xCpNirConfig, GlobalStatusServer, camLogger);
                            break;
                        }
                    case UI324xCpNirIDSCameraConfig ui324xCpNirConfig:
                        {
                            camera = new UI324xCpNir(ui324xCpNirConfig, GlobalStatusServer, camLogger);
                            break;
                        }
                    default: throw new Exception($"Camera of type '{config.GetType()}' not supported.");
                }

                camera.Init();
                Cameras.Add(config.DeviceID, camera);
            }

            // Simulée
            //........
            foreach (var cameraConfig in hardwareConfiguration.CameraConfigs.Where(c => c.IsEnabled && c.IsSimulated))
            {
                var camLogger = HardwareLoggerFactory.CreateHardwareLogger(cameraConfig.LogLevel.ToString(), DeviceFamily.Camera.ToString(), cameraConfig.Name);
                CameraBase cam;
                // we look for a DummyCameraConfig
                var dummyCameraConfig = hardwareConfiguration.CameraConfigs.Find(c => c is DummyCameraConfig && c.DeviceID == cameraConfig.DeviceID);

                if (!(dummyCameraConfig is null))
                {
                    cam = new DummyCamera((DummyCameraConfig)dummyCameraConfig, GlobalStatusServer, camLogger);
                }
                else
                {
                    cam = new DummyIDSCamera(cameraConfig, GlobalStatusServer, camLogger);
                }

                Cameras.Add(cameraConfig.DeviceID, cam);
            }
        }

        protected void InitializeDistanceSensor(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("distance sensor device initialization");
            foreach (var config in hardwareConfiguration.DistanceSensorConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"distance sensor {config.Name} create");
                var loggerDistanceSensor = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.DistanceSensor.ToString(), config.Name);
                try
                {
                    DistanceSensor = DistanceSensorFactory.Create(GlobalStatusServer, loggerDistanceSensor, config, Controllers);
                    try
                    {
                        DistanceSensor.Init();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"distance sensor device {config.Name} init error");
                        DistanceSensor.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create distance sensor device {config.Name}", ex);
                }
            }
        }

        protected void InitializeLasers(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("laser device initialization");
            foreach (var config in hardwareConfiguration.LaserConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"laser {config.Name} create");

                try
                {
                    var loggerLaser = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Laser.ToString(), config.Name);
                    var laser = LaserFactory.Create(GlobalStatusServer, loggerLaser, config, Controllers);
                    try
                    {
                        laser.Init();
                        Lasers.Add(config.DeviceID, laser);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"laser device {config.Name} init error");
                        laser.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create laser device {config.Name}", ex);
                }
            }
        }

        protected void InitializeMppc(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.MppcConfigs.Exists(x => x.IsEnabled))
                return;

            var configs = hardwareConfiguration.MppcConfigs.OfType<MppcConfig>().Where(c => c.IsEnabled).ToList();
            if (configs.Any())
            {
                _logger.Information("Mppc device initialization");
                foreach (MppcConfig config in configs)
                {
                    _logger.Information($"Mppc {config.Name} create");

                    try
                    {
                        var mppcLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Mppc.ToString(), config.Name);
                        var mppc = MppcFactory.Create(config, mppcLogger);
                        try
                        {
                            mppc.Init(config);
                            Mppcs.Add(mppc.DeviceID, mppc);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"Mppc device {config.Name} init error");
                            mppc.State = new DeviceState(DeviceStatus.Error, ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error during create Mppc device {config.Name}", ex);
                    }
                }
            }
        }

        protected void InitializeOpticalPowermeters(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.OpticalPowermeterConfigs.Exists(x => x.IsEnabled))
                return;

            var configs = hardwareConfiguration.OpticalPowermeterConfigs.OfType<OpticalPowermeterConfig>().Where(c => c.IsEnabled).ToList();
            if (configs.Any())
            {
                _logger.Information("OpticalPowermeter device initialization");
                foreach (OpticalPowermeterConfig config in configs)
                {
                    _logger.Information($"OpticalPowermeter {config.Name} create");

                    try
                    {
                        var powermeterLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.OpticalPowermeter.ToString(), config.Name);
                        var powermeter = OpticalPowermeterFactory.Create(config, powermeterLogger);
                        try
                        {
                            powermeter.Init(config);
                            OpticalPowermeters.Add(powermeter.DeviceID, powermeter);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"OpticalPowermeter device {config.Name} init error");
                            powermeter.State = new DeviceState(DeviceStatus.Error, ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error during create OpticalPowermeter device {config.Name}", ex);
                    }
                }
            }
        }

        protected void InitializeSpectrometers(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.SpectrometerConfigs.Exists(x => x.IsEnabled))
                return;

            _logger.Information("spectrometer device initialization");
            foreach (var config in hardwareConfiguration.SpectrometerConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"spectrometer {config.Name} create");

                try
                {
                    var spectroLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Spectrometer.ToString(), config.DeviceID);
                    var spectrometer = SpectrometerFactory.Create(GlobalStatusServer, _logger, config, Controllers);
                    try
                    {
                        spectrometer.Init(config);
                        Spectrometers.Add(config.DeviceID, spectrometer);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"spectrometer device {config.Name} init error");
                        spectrometer.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create spectrometer device {config.Name}", ex);
                }
            }
        }

        protected void InitializeShutters(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("shutter device initialization");
            foreach (var config in hardwareConfiguration.ShutterConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"shutter {config.Name} create");

                try
                {
                    var shutterLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Spectrometer.ToString(), config.DeviceID);
                    var shutter = ShutterFactory.Create(GlobalStatusServer, shutterLogger, config, Controllers);
                    try
                    {
                        shutter.Init();
                        Shutters.Add(config.DeviceID, shutter);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"shutter device {config.Name} init error");
                        shutter.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create shutter device {config.Name}", ex);
                }
            }
        }

        private void InitializeLights(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("light device initialization");
            foreach (var moduleConfig in hardwareConfiguration.LightModuleConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"Light {moduleConfig.Name} create");

                foreach (var lightConfig in moduleConfig.Lights)
                {
                    var lightLogger = HardwareLoggerFactory.CreateHardwareLogger(lightConfig.LogLevel.ToString(), DeviceFamily.Light.ToString(), lightConfig.Name);
                    var light = LightFactory.Create(lightConfig, Controllers, GlobalStatusServer, lightLogger);

                    light.Init();
                    Lights.Add(light.DeviceID, light);
                }
            }
        }

        protected void InitializeFfus(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("ffu device initialization");
            foreach (var config in hardwareConfiguration.FfuConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"ffu {config.Name} create");

                try
                {
                    var ffuLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Ffu.ToString(), config.Name);
                    var ffu = FfuFactory.Create(GlobalStatusServer, ffuLogger, config, Controllers);
                    try
                    {
                        ffu.Init();
                        Ffus.Add(config.DeviceID, ffu);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"ffu device {config.Name} init error");
                        ffu.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create ffu device {config.Name}", ex);
                }
            }
        }

        protected void InitializePlcScreens(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("plcScreen device initialization");
            foreach (var config in hardwareConfiguration.PlcScreenConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"plcScreen {config.Name} create");

                try
                {

                    var plcScreenLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Screen.ToString(), config.Name);
                    var plcScreen = PlcScreenFactory.Create(GlobalStatusServer, plcScreenLogger, config, Controllers);
                    try
                    {
                        plcScreen.Init();
                        PlcScreens.Add(config.DeviceID, plcScreen);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"plcScreen device {config.Name} init error");
                        plcScreen.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create plcScreen device {config.Name}", ex);
                }
            }
        }

        protected void InitializeRfids(HardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("Rfid device initialization");
            foreach (var config in hardwareConfiguration.RfidConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"Rfid {config.Name} create");

                try
                {
                    var rfidLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Rfid.ToString(), config.Name);
                    var rfid = RfidFactory.Create(GlobalStatusServer, rfidLogger, config, Controllers);
                    try
                    {
                        rfid.Init();
                        Rfids.Add(config.DeviceID, rfid);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Rfid device {config.Name} init error");
                        rfid.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during create rfid device {config.Name}", ex);
                }
            }
        }

        protected virtual void InitializeIonizers(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.IonizerConfigs.Exists(x => x.IsEnabled))
                return;

            _logger.Information("ionizer device initialization started...");
            foreach (var config in hardwareConfiguration.IonizerConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"{config.Name} ionizer device creation");
                var ionizerLogger = HardwareLoggerFactory.CreateHardwareLogger(config.LogLevel.ToString(), DeviceFamily.Ionizer.ToString(), config.Name);

                try
                {
                    Ionizer = IonizerFactory.Create(GlobalStatusServer, ionizerLogger, config, Controllers);
                    try
                    {
                        Ionizer.Init();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"{config.Name} Ionizer device init error");
                        Chamber.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error during {config.Name} Ionizer device creation");
                    GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "Ionizer device initialization Failed", config.Name)));
                }
            }
        }

        public LightBase GetMainLight()
        {
            var light = Lights.Values.FirstOrDefault(_ => _.IsMainLight);
            if (light is null)
            {
                throw new InvalidOperationException("The main light is not defined");
            }

            return light;
        }

        protected void DeviceStatusChanged(object sender, StatusChangedEventArgs deviceStatus)
        {
            // If one device is in error the PM is in error
            if (deviceStatus.NewStatus == DeviceStatus.Error)
            {
                GlobalStatusServer.SetGlobalState(PMGlobalStates.Error);
            }
        }

        public List<GlobalDevice> GetGlobalDevices()
        {
            var globalDevices = new List<GlobalDevice>();
            if (Plc != null) globalDevices.Add(new GlobalDevice() { Type = Plc.GetType().Name, Name = Plc.Name, State = Plc.State, Family = Plc.Family, DeviceID = Plc.DeviceID });
            globalDevices.AddRange(Cameras.Values.Select(x => new GlobalDevice() { Type = x.GetType().Name, Name = x.Name, State = x.State, Family = x.Family, DeviceID = x.DeviceID }));
            if (Axes != null) globalDevices.Add(new GlobalDevice() { Type = Axes.GetType().Name, Name = Axes.Name, State = Axes.State, Family = Axes.Family, DeviceID = Axes.DeviceID });
            if (DistanceSensor != null) globalDevices.Add(new GlobalDevice() { Type = DistanceSensor.GetType().Name, Name = DistanceSensor.Name, State = DistanceSensor.State, Family = DistanceSensor.Family, DeviceID = DistanceSensor.DeviceID });
            if (Lasers != null)
                globalDevices.AddRange(Lasers.Values.Select(x => new GlobalDevice() { Type = x.GetType().Name, Name = x.Name, State = x.State, Family = x.Family, DeviceID = x.DeviceID }));
            if (Mppcs != null)
                globalDevices.AddRange(Mppcs.Values.Select(x => new GlobalDevice() { Type = x.GetType().Name, Name = x.Name, State = x.State, Family = x.Family, DeviceID = x.DeviceID }));
            if (OpticalPowermeters != null)
                globalDevices.AddRange(OpticalPowermeters.Values.Select(x => new GlobalDevice() { Type = x.GetType().Name, Name = x.Name, State = x.State, Family = x.Family, DeviceID = x.DeviceID }));
            if (Shutters != null)
                globalDevices.AddRange(Shutters.Values.Select(x => new GlobalDevice() { Type = x.GetType().Name, Name = x.Name, State = x.State, Family = x.Family, DeviceID = x.DeviceID }));
            if (Chamber != null) globalDevices.Add(new GlobalDevice() { Type = Chamber.GetType().Name, Name = Chamber.Name, State = Chamber.State, Family = Chamber.Family, DeviceID = Chamber.DeviceID });
            if (Spectrometers != null)
                globalDevices.AddRange(Spectrometers.Values.Select(x => new GlobalDevice() { Type = x.GetType().Name, Name = x.Name, State = x.State, Family = x.Family, DeviceID = x.DeviceID }));

            return globalDevices;
        }

        public static Object GetDeviceConfigurationById(HardwareConfiguration hardwareConfiguration, IDeviceIdentification identification)
        {
            foreach (var config in hardwareConfiguration.CameraConfigs.Where(x => x.IsEnabled))
            {
                if ((config.Name == identification.Name) && (config.DeviceID == identification.DeviceID))
                    return config;
            }

            foreach (var config in hardwareConfiguration.ChamberConfigs.Where(x => x.IsEnabled))
            {
                if ((config.Name == identification.Name) && (config.DeviceID == identification.DeviceID))
                    return config;
            }

            foreach (var config in hardwareConfiguration.DistanceSensorConfigs.Where(x => x.IsEnabled))
            {
                if ((config.Name == identification.Name) && (config.DeviceID == identification.DeviceID))
                    return config;
            }

            foreach (var config in hardwareConfiguration.AxesConfigs.Where(x => x.IsEnabled))
            {
                if ((config.Name == identification.Name) && (config.DeviceID == identification.DeviceID))
                    return config;
            }
            return null;
        }
    }
}
