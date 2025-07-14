using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

using UnitySC.PM.EME.Hardware.Chamber;
using UnitySC.PM.EME.Hardware.Chamber.Dummy;
using UnitySC.PM.EME.Hardware.Chiller.Controller;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.Hardware.Wheel;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Hardware
{
    public class EmeHardwareManager : HardwareManager, IHardwareManager
    {
        private readonly ILogger _logger;
        private readonly IEMEServiceConfigurationManager _configManager;
        private readonly IReferentialManager _referentialManager;
        public const string HardwareConfigurationFileName = "EmeHardwareConfiguration.xml";

        public Dictionary<string, EMELightBase> EMELights { get; set; }
        public WheelBase Wheel { get; set; }
        public Chiller.Chiller Chiller { get; set; }

        public EmeHardwareManager(ILogger logger, IHardwareLoggerFactory hardwareLoggerFactory, IEMEServiceConfigurationManager configManager, IGlobalStatusServer globalStatus, IReferentialManager referentialManager) : base(logger, hardwareLoggerFactory)
        {
            _logger = logger;
            _configManager = configManager;
            _referentialManager = referentialManager;
            GlobalStatusServer = globalStatus;
        }

        public bool Init()
        {
            GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Initializing, new Message(MessageLevel.Information, "Loading configuration")));

            // Load XML configuration file            //............................

            string fullPath = Path.Combine(_configManager.ConfigurationFolderPath, HardwareConfigurationFileName);

            _logger?.Information("Loading EMERA hardware configuration from " + fullPath);
            var hardwareConfiguration = XML.Deserialize<EmeHardwareConfiguration>(fullPath);
            if (_configManager.HardwaresAreSimulated)
                hardwareConfiguration.SetAllHardwareInSimulation();

            if (EMELights == null)
            {
                EMELights = new Dictionary<string, EMELightBase>();
            }

            if (hardwareConfiguration.CameraConfigs.Exists(c => c.IsEnabled && !c.IsSimulated && c is MatroxCameraConfigBase))
                Mil.Instance.Allocate();

            // Initialize the lists, it is used when configuration items depend on other ones.
            // For example the probes depend on objectives. They are linked throw an ID in the xml file but we want to have a list of objectives instead of a list of objectives Ids
            bool initFatalError = InitializeDevices(hardwareConfiguration);
            var camera = Cameras.First().Value;
            camera.SetTriggerMode(CameraBase.TriggerMode.Off);
            camera.SetTriggerMode(CameraBase.TriggerMode.Software);
            camera.SetAOI(new Rect(0,0, camera.Width, camera.Height));
            camera.StartContinuousGrab();


            // No fatal error
            if (!initFatalError)
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Free, new Message(MessageLevel.Information, "Initialization Done")));
            else
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Information, "Initialization Failed")));

            return !initFatalError;
        }

        private bool InitializeDevices(EmeHardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;

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
                initFatalError = true;
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.ErrorHandling, new Message(MessageLevel.Fatal, "Initialization Aborted for SAFETY : " + ex.Message + " (Shared Controllers)")));
                return initFatalError;
            }
            catch (Exception ex)
            {
                initFatalError = true;
                _logger?.Fatal(ex, "Shared devices initialization Failed");
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "Shared controllers initialization Failed : " + ex.Message + " (Shared Controllers)")));
            }

            try
            {
                base.InitializeDevices(hardwareConfiguration);                
                InitializeWheel(hardwareConfiguration);
                InitializeLights(hardwareConfiguration);
                InitializeChiller(hardwareConfiguration);
            }
            catch (SafetyException ex)
            {
                initFatalError = true;
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.ErrorHandling, new Message(MessageLevel.Fatal, "Initialization Aborted for SAFETY :" + ex.Message, "Shared Devices")));
                return initFatalError;
            }
            catch (Exception ex)
            {
                initFatalError = true;
                _logger?.Fatal(ex, "Shared devices initialization Failed");
                GlobalStatusServer.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Error, new Message(MessageLevel.Fatal, "Shared devices initialization Failed :" + ex.Message, "Shared Devices")));
            }

            return initFatalError;
        }

        private void InitializeChiller(EmeHardwareConfiguration hardwareConfiguration)
        {
            _logger.Information("Emera chiller initialization");
            var chiller = new Chiller.Chiller(new DummyChillerController(), GlobalStatusServer, _logger);
            Chiller = chiller;
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
                var chuckLogger = new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Chuck.ToString(), config.Name);
                try
                {
                    Chuck = EMEChuck.Create(GlobalStatusServer, chuckLogger, config, Controllers);
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
        protected override void InitializeMotionAxes(HardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.MotionControllerConfigs.Exists(x => x.IsEnabled))
            {
                return;
            }

            foreach (var config in hardwareConfiguration.MotionAxesConfigs.Where(x => x.IsEnabled))
            {
                try
                {
                    if (!(config is PhotoLumAxesConfig emeraConfig))
                    {
                        continue;
                    }

                    _logger.Information(string.Format("{0} Creation: ", emeraConfig.Name));
                    var motionAxesLogger = new HardwareLogger(config.LogLevel.ToString(), nameof(DeviceFamily.Axes), emeraConfig.Name);
                    MotionAxes = new PhotoLumAxes(emeraConfig, MotionControllers, GlobalStatusServer, motionAxesLogger, _referentialManager);

                    var _initErrors = new List<Message>();
                    MotionAxes.Init(_initErrors);
                    if (_initErrors.Exists(message => message.Level == MessageLevel.Fatal))
                    {
                        throw new Exception("Fatal error occurred during Motion Axes Initialization.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, string.Format("{0} motion axes initialization error", config.Name));
                    if (MotionAxes != null) MotionAxes.State = new DeviceState(DeviceStatus.Error, ex.Message);
                }
            }
        }

        protected override void InitializeChambers(HardwareConfiguration hardwareConfiguration)
        {
            var config = hardwareConfiguration.ChamberConfigs.FirstOrDefault(x => x.IsEnabled);

            if (config == null)
                return;

            try
            {
                if (!Controllers.TryGetValue(config.ControllerID, out var controller))
                {
                    throw new Exception("Controller of the configuration was not found [deviceID = " + config.DeviceID +
                                        ", ControllerId = " + config.ControllerID + "]");
                } 
                
                if (config.IsSimulated)
                {
                    Chamber = new EmeDummyChamber(GlobalStatusServer, _logger, config,
                        (ChamberDummyController)controller);
                }
                else
                {
                    Chamber = new EMEChamber(GlobalStatusServer, _logger, config, (ChamberController)controller);
                }

                Chamber.Init();
            }
            catch (Exception ex)
            {
                if (Chamber != null)
                {
                    Chamber.State = new DeviceState(DeviceStatus.Error, ex.Message);
                }

                throw new Exception($"Error during chamber creation device {config.Name}", ex);
            }
        }

        private void InitializeWheel(EmeHardwareConfiguration hardwareConfiguration)
        {
            if (!hardwareConfiguration.WheelConfigs.Exists(x => x.IsEnabled))
            {
                return;
            }

            _logger.Information("Filter Wheel device initialization");
            foreach (var config in hardwareConfiguration.WheelConfigs.Where(x => x.IsEnabled))
            {
                try
                {
                    if (!(config is FilterWheelConfig filterWheelConfig))
                    {
                        continue;
                    }

                    _logger.Information($"Filter Wheel {config.Name} initialization");

                    if (!MotionControllers.TryGetValue(filterWheelConfig.ControllerID, out var motionController))
                    {
                        throw new Exception($"Motion controller {filterWheelConfig.ControllerID} not found " +
                                            $"for Filter Wheel {filterWheelConfig.Name}.");
                    }

                    var logger = new HardwareLogger(config.LogLevel.ToString(), nameof(DeviceFamily.Wheel),
                        config.Name);
                    Wheel = new FilterWheel.FilterWheel(filterWheelConfig, motionController, GlobalStatusServer,
                        logger);
                    Wheel.Init();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error during Wheel device {config.Name} initialization", ex);
                }
            }
        }

        protected void InitializeLights(EmeHardwareConfiguration hardwareConfiguration)
        {            
            _logger.Information("Emera light device initialization");
            foreach (var lightConfig in hardwareConfiguration.Lights.Where(x => x.IsEnabled))
            {
                var light = EMELightFactory.Create(lightConfig, Controllers, GlobalStatusServer, _logger);

                light.Init();
                EMELights.Add(light.DeviceID, light);
            }
        }
        
        public void Init(IHardwareConfiguation hardwareConfiguation, IGlobalStatusServer globalStatus)
        {
        }

        public void Reset()
        {
            UnitializeEmeDevices();
            UnitializeDevices();
            Wheel = null;
            if (!Init())
                throw new Exception("Initialisation devices failed");
        }

        public void Stop()
        {
            base.Shutdown();
            UnitializeEmeDevices();
            UnitializeDevices();
            Wheel = null;
        }

        private void UnitializeEmeDevices()
        {
            foreach(var l in EMELights) 
            {
                if (l.Value is EMELightBase)
                {
                    var light = l.Value as EMELightBase;
                    light.SwitchOn(false);
                }
            }

            EMELights.Clear();
        }
    }
}
