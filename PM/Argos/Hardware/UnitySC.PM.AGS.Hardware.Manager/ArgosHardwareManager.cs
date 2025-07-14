using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.PM.AGS.Hardware.Axes;
using UnitySC.PM.AGS.Service.Interface.Axes;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Chuck;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.Shared.LibMIL;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Data.Configuration;

namespace UnitySC.PM.AGS.Hardware.Manager
{
    public class ArgosHardwareManager
    {
        public Dictionary<string, MotionControllerBase> MotionControllers { get; set; } =
            new Dictionary<string, MotionControllerBase>();

        public Dictionary<string, ControllerBase> Controllers { get; set; } = new Dictionary<string, ControllerBase>();

        public ArgosAxesBase Axes { get; set; }
        public Dictionary<string, CameraBase> Cameras { get; set; } = new Dictionary<string, CameraBase>();
        public ChamberBase Chamber { get; set; }
        public IChuck Chuck { get; set; }
        public PlcBase Plc { get; set; }

        private readonly ILogger _logger;

        public ArgosHardwareManager(ILogger logger)
        {
            _logger = logger;
        }

        [Obsolete]
        public void Init()
        {
            IServiceConfigurationManager _configManager = (IServiceConfigurationManager)ClassLocator.Default.GetInstance(typeof(IServiceConfigurationManager));
            //Load XML configuration file
            string path = Path.Combine(_configManager.ConfigurationFolderPath, "HardwareConfiguration.xml");
            _logger.Information("Loading hardware configuration from " + path);

            bool fileDoesNotExist = ListExtension.IsNullOrEmpty<String>(path) && !File.Exists(path);
            if (fileDoesNotExist)
            {
                throw new ArgumentException("No HardwareConfiguration.xml file found next to the executable");
            }

            HardwareConfiguration hardwareConfiguration = null;
            hardwareConfiguration = XML.Deserialize<HardwareConfiguration>(path);

            if (hardwareConfiguration.CameraConfigs.Any(c => c.IsEnabled) ||
                hardwareConfiguration.ScreenConfigs.Any(c => c.IsEnabled))
            {
                Mil.Instance.Allocate();
            }

            InitializeDevices(hardwareConfiguration);
            _logger.Information("Hardware devices initialized");
        }

        protected bool InitializeDevices(HardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;

            initFatalError |= InitializeMotionControllers(hardwareConfiguration);

            InitializeChambers(hardwareConfiguration);
            InitializeCameras(hardwareConfiguration);
            initFatalError |= InitializeAxes(hardwareConfiguration);

            foreach (var camera in Cameras)
            {
                camera.Value.OnStatusChanged += DeviceStatusChanged;
            }

            return initFatalError;
        }

        protected void DeviceStatusChanged(object sender, StatusChangedEventArgs deviceStatus)
        {
            // If one device is in error the PM is in error
            if (deviceStatus.NewStatus == DeviceStatus.Error)
            {
                var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
                globalStatusServer.SetGlobalState(PMGlobalStates.Error);
            }
        }

        protected bool InitializeAxes(HardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;
            // Check Axes configuration enabled
            if ((hardwareConfiguration.AxesConfigs == null) ||
                (!hardwareConfiguration.AxesConfigs.Any(x => x.IsEnabled)))
                return initFatalError;

            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            // Use only the first axes configuration enabled
            var config = hardwareConfiguration.AxesConfigs.First(x => x.IsEnabled);
            try
            {
                _logger.Information(string.Format("{0} Axes device creation", config.Name));
                var axesLogger =
                    new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Axes.ToString(), config.Name);
                //Replace with a ArgosAxesFactory when a new tool is added
                Axes = ArgosAxesFactory.Create(config, MotionControllers, globalStatusServer, axesLogger,
                    referentialManager);
                // PMAxes = new ArgosAxesBase(config, Controllers, globalStatusServer, axesLogger, referentialManager);
                List<Message> _initErrors = new List<Message>();
                Axes.Init(_initErrors);
                if (_initErrors.Any())
                {
                    bool atLeastOneFatalError = _initErrors.Any(message => message.Level == MessageLevel.Fatal);
                    if (atLeastOneFatalError)
                        throw new Exception("Fatal error occured during axes device initialization");
                }
            }
            catch (Exception ex)
            {
                initFatalError = true;
                _logger.Error(ex, string.Format("{0} axes initialization error", config.Name));
                if (Axes != null) Axes.State = new DeviceState(DeviceStatus.Error, ex.Message);
            }

            if (!initFatalError)
            {
                _logger.Information(string.Format("{0} Axes device initialization complete", config.Name));
                if (Axes != null)
                    Axes.State = new DeviceState(DeviceStatus.Ready, "Axes device initialization complete");
            }

            return initFatalError;
        }

        protected bool InitializeMotionControllers(HardwareConfiguration hardwareConfiguration)
        {
            bool noEnabledController = !hardwareConfiguration.ControllerConfigs.Any(x => x.IsEnabled);
            if (noEnabledController) { return false; }

            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            _logger.Information("Controllers initialization starting...");
            bool initFatalError = false;

            // Create all controllers from config
            foreach (var config in hardwareConfiguration.ControllerConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information($"Create controller {config.Name}");

                var controllerLogger = new HardwareLogger(config.LogLevel.ToString(),
                    DeviceFamily.Controller.ToString(), config.Name);

                //If Motion controller
                if (config is SMC100ControllerConfig || config is AerotechControllerConfig)
                {
                    var controller = MotionControllerFactory.CreateController(config, globalStatusServer, controllerLogger);
                    initFatalError = InitController(config, controller, true);
                }
                else
                {
                    var controller = ControllerFactory.CreateController(config, globalStatusServer, controllerLogger);
                    initFatalError = InitController(config, controller, false);
                }
            }
            return initFatalError;
        }

        private bool InitController(ControllerConfig config, ControllerBase controller, bool isMotionController)
        {
            bool initFatalError = false;
            if (!(controller is null))
            {
                try
                {
                    _logger.Information(string.Format("{0} Controller initialization started", config.Name));

                    var _initErrors = new List<Message>();
                    controller.Init(_initErrors);

                    bool atLeastOneFatalError = _initErrors.Any(message => message.Level == MessageLevel.Fatal);
                    if (atLeastOneFatalError)
                        throw new Exception("Error during " + config.Name + " controller initialization");

                    if (isMotionController)
                    {
                        MotionControllers.Add(config.DeviceID, (MotionControllerBase)controller);
                    }
                    else
                    {
                        Controllers.Add(config.DeviceID, controller);
                    }
                    _logger.Information(
                        $"{config.Name} Controller Status: {controller.State.Status} Status message: {controller.State.StatusMessage}");
                }
                catch (Exception ex)
                {
                    initFatalError = true;
                    _logger.Error(ex, $"{config.Name} Controller initialization error");
                    controller.State = new DeviceState(DeviceStatus.Error, ex.Message);
                }
            }

            return initFatalError;
        }

        protected void InitializeChambers(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.ChamberConfigs.Any(x => x.IsEnabled))
                return;

            _logger.Information("Chamber communications device initialization starting...");
            foreach (var config in hardwareConfiguration.ChamberConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information(string.Format("Chamber communication device {0} create", config.Name));

                try
                {
                    Chamber = ChamberFactory.Create(config, Controllers);
                    try
                    {
                        _logger.Information(string.Format("Chamber communication device {0} init...", config.Name));
                        Chamber.Init(config);
                        _logger.Information(string.Format("Chamber communication device {0}", config.Name));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, string.Format("Chamber communication device {0} init error", config.Name));
                        Chamber.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        string.Format("Error during create chamber communication device {0}", config.Name), ex);
                }
            }
        }

        protected void InitializeCameras(HardwareConfiguration hardwareConfiguration)
        {
            Cameras.Clear();

            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            // Matrox
            //.......
            var configs = hardwareConfiguration.CameraConfigs.OfType<MatroxCameraConfigBase>()
                .Where(c => c.IsEnabled && !c.IsSimulated).ToList();
            if (configs.Any())
            {
                _logger.Information("Matrox Cameras initialization");
                var cameraManager = new MatroxCameraManager(_logger);
                bool haveError = cameraManager.Init(configs);
                if (haveError)
                    _logger.Warning("Matrox Camera initialization failed at least partially");
                Cameras.AddRange(cameraManager.Cameras);
            }

            // Simulée
            //........
            foreach (var config in hardwareConfiguration.CameraConfigs.Where(c => c.IsEnabled && c.IsSimulated))
            {
                var logger = new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Camera.ToString(),
                    config.Name);
                CameraBase cam;
                if (config is DummyCameraConfig)
                    cam = new DummyCamera((DummyCameraConfig)config, globalStatusServer, logger);
                else
                    cam = new DummyIDSCamera(config, globalStatusServer, logger);
                Cameras.Add(config.DeviceID, cam);
            }
        }

        protected bool InitializeChuck(HardwareConfiguration hardwareConfiguration)
        {
            bool initFatlError = false;
            if (!hardwareConfiguration.ChuckConfigs.Any(x => x.IsEnabled))
                return false;

            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            _logger.Information("chucks device initialization started...");
            foreach (var config in hardwareConfiguration.ChuckConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information(string.Format("{0} chuck device creation", config.Name));
                var chuckLogger = new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Chuck.ToString(), config.Name);

                try
                {
                    //TODO new factory ? different kind of controllers ?
                    Chuck = ChuckFactory.Create(config, Controllers, globalStatusServer, chuckLogger);
                    try
                    {
                        _logger.Information(string.Format("{0} chuck device initialization", config.Name));
                        List<Message> _initErrors = new List<Message>();
                        Chuck.Init(_initErrors);
                        _logger.Information(string.Format("{0} chuck device Status: {1} Status message: {2}", config.Name, Chuck.State.Status, Chuck.State.StatusMessage));
                    }
                    catch (Exception ex)
                    {
                        initFatlError = true;
                        _logger.Error(ex, string.Format("{0} chuck device init error", config.Name));
                        Plc.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    initFatlError = true;
                    _logger.Error(ex, "Error during {0} chuck device creation", config.Name);
                    globalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "chuck device initialization Failed", config.Name)));
                }
            }
            return initFatlError;
        }

        protected void InitializePlcs(HardwareConfiguration hardwareConfiguration)
        {
            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            _logger.Information("PLC device initialization");
            foreach (var config in hardwareConfiguration.PlcConfigs)
            {
                _logger.Information(string.Format("Plc {0} create", config.Name));

                try
                {
                    Plc = PlcFactory.Create(globalStatusServer, _logger, config, Controllers);
                    try
                    {
                        Plc.Init();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, string.Format("Plc device {0} init error", config.Name));
                        Plc.State = new DeviceState(DeviceStatus.Error, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error during create Plc device {0}", config.Name), ex);
                }
            }
        }
    }
}
