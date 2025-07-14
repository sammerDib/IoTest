using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.OpticalMount;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.Shared.Hardware.Camera.CameraBase;

namespace UnitySC.PM.DMT.Hardware.Manager
{
    public class DMTHardwareManager : HardwareManager, IHardwareManager
    {
        public const string HardwareConfigurationFileName = "DMTHardwareConfiguration.xml";

        private readonly ILogger _logger;
        private readonly IDMTServiceConfigurationManager _configManager;
        private IGlobalStatusServer _globalStatus;

        public Dictionary<Side, CameraBase> CamerasBySide = new Dictionary<Side, CameraBase>();
        public Dictionary<Side, ScreenBase> ScreensBySide = new Dictionary<Side, ScreenBase>();
        public Dictionary<Side, OpticalMountShape> OpticalMounts = new Dictionary<Side, OpticalMountShape>();
        public Dictionary<string, ScreenBase> Screens { get; set; } = new Dictionary<string, ScreenBase>();

        public DMTHardwareManager(ILogger logger,IHardwareLoggerFactory hardwareLoggerFactory, IDMTServiceConfigurationManager configManager, IGlobalStatusServer globalStatus) : base(logger,hardwareLoggerFactory)
        {
            _logger = logger;
            _configManager = configManager;
            _globalStatus = globalStatus;
        }

        public void Init()
        {
            // Load XML configuration file
            //............................
            string fullPath = Path.Combine(_configManager.ConfigurationFolderPath, HardwareConfigurationFileName);

            _logger?.Information("Loading DEMETER hardware configuration from " + fullPath);
            var hardwareConfiguration = XML.Deserialize<DMTHardwareConfiguration>(fullPath);
            bool isHardwareConfigurationValid = CheckHardwareConfigurationValidity(hardwareConfiguration);
            if (!isHardwareConfigurationValid)
            {
                _logger.Error($"There was an error during {HardwareConfigurationFileName} validity check. Server will now exit.");
                throw new Exception($"There was an error during {HardwareConfigurationFileName}");
            }

            if (_configManager.HardwaresAreSimulated)
            {
                hardwareConfiguration.SetAllHardwareInSimulation();
            }

            if (Screens == null)
            {
                Screens = new Dictionary<string, ScreenBase>();
            }

            if (Mil.Instance.Application == null
                && (DoesAnyCameraRequireMil(hardwareConfiguration) || DoesAnyScreenRequireMil(hardwareConfiguration)))
            {
                Mil.Instance.Allocate();
            }

            InitializeDevices(hardwareConfiguration);
        }

        public Length GetCurrentChuckMaximumDiameter()
        {
            var position = (XTPosition)MotionAxes.GetPosition();
            // Rfid tag can only be read in loading position
            if (position.X.CompareTo(1.0) != 0)
            {
                // Chuck is not in loading position, moving
                MotionAxes.Move(new PMAxisMove("Linear", 1.Millimeters()));
                MotionAxes.WaitMotionEnd(5000);
            }
            var diameter = Rfids.Values.First().GetTag().Size;
            if (position.X.CompareTo(2.0) == 0)
            {
                // Chuck was in measurement position, return to it
                MotionAxes.Move(new PMAxisMove("Linear", 2.Millimeters()));
                MotionAxes.WaitMotionEnd(5000);    
            }
            return diameter;
        }

        private static bool DoesAnyCameraRequireMil(DMTHardwareConfiguration hardwareConfiguration)
        {
            return hardwareConfiguration.CameraConfigs.Exists(c => c.IsEnabled && !c.IsSimulated && c is MatroxCameraConfigBase);
        }

        private bool DoesAnyScreenRequireMil(DMTHardwareConfiguration hardwareConfiguration)
        {
            return hardwareConfiguration.DMTScreenConfigs.Exists(c => c.IsEnabled) && !_configManager.MilIsSimulated;
        }

        private void InitializeDevices(DMTHardwareConfiguration hardwareConfiguration)
        {
            try
            {
                // Note that InitializeAllControllers return true if there is any error
                if (InitializeAllControllers(hardwareConfiguration))
                {
                    _logger.Error("Controllers initialization failed");
                    throw new Exception("Controllers initialization failed");
                }
            }
            catch (SafetyException ex)
            {
                _logger.Error("Controllers initialization Aborted for SAFETY : " + ex.Message);
                throw new Exception("Controllers initialization Aborted for SAFETY : " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Controllers initialization failed : " + ex.Message);
                throw new Exception("Controllers initialization failed : " + ex.Message);
            }

            try
            {
                InitializeScreens(hardwareConfiguration);
                foreach (var screen in Screens)
                {
                    screen.Value.OnStatusChanged += DeviceStatusChanged;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Screens initialization Failed");
                throw new Exception("Screens initialization failed");
            }

            try
            {
                // Note that InitializeDevices return true if there is any error
                if (base.InitializeDevices(hardwareConfiguration))
                {
                    _logger.Error("Devices initialization failed");
                    throw new Exception("Devices initialization failed");
                }
                InitializeOpticalMounts(hardwareConfiguration);
                InitializeCameras();
            }
            catch (SafetyException ex)
            {
                _logger.Error("Devices initialization Aborted for SAFETY : " + ex.Message);
                throw new Exception("Devices initialization Aborted for SAFETY : " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Devices initialization Failed : " + ex.Message);
                throw new Exception("Devices initialization Failed : " + ex.Message);
            }
        }

        private void InitializeScreens(DMTHardwareConfiguration hardwareConfiguration)
        {
            Screens.Clear();

            if (hardwareConfiguration.DMTScreenConfigs.Exists(c => c.IsEnabled))
            {
                _logger.Information("Screens initialization");
                var screenman = new ScreenManager(_logger, _globalStatus);
                //TODO refacto and use PLCScreenController
                var listScreenDensitronDM430GNControllerConfig =
                    hardwareConfiguration.ControllerConfigs.OfType<ScreenDensitronDM430GNControllerConfig>().ToList();
                screenman.Init(hardwareConfiguration.DMTScreenConfigs, listScreenDensitronDM430GNControllerConfig, Controllers);
                Screens = screenman.Screens;
            }
        }

        private bool CheckHardwareConfigurationValidity(DMTHardwareConfiguration hardwareConfiguration)
        {
            if (hardwareConfiguration.OpticalMounts.IsEmpty())
            {
                _logger.Fatal(new Exception($"No optical mount defined in {HardwareConfigurationFileName}"), $"Optical mounts haven't been defined. Please define at least one in the {HardwareConfigurationFileName} file");
                return false;
            }
            if (hardwareConfiguration.DMTScreenConfigs.IsEmpty())
            {
                _logger.Fatal(new Exception($"No screen defined in {HardwareConfigurationFileName}"), $"No screens have been defined. Please define at least one in the {HardwareConfigurationFileName} file");
            }
            if (hardwareConfiguration.CameraConfigs.IsEmpty())
            {
                _logger.Fatal(new Exception($"No camera defined in {HardwareConfigurationFileName}"), $"No cameras have been defined. Please define at least one in the {HardwareConfigurationFileName} file");
            }
            return true;
        }

        private void InitializeOpticalMounts(DMTHardwareConfiguration hardwareConfiguration)
        {
            hardwareConfiguration.OpticalMounts?.ForEach(mount =>
            {
                OpticalMounts.Add(mount.Side, mount.MountShape);
                if (Cameras.TryGetValue(mount.CameraId, out CameraBase camera))
                {
                    CamerasBySide.Add(mount.Side, camera);
                }
                if (Screens.TryGetValue(mount.ScreenId, out ScreenBase screen))
                {
                    ScreensBySide.Add(mount.Side, screen);
                }
            });
        }

        private void InitializeCameras()
        {
            foreach (KeyValuePair<Side, CameraBase> cameraEntry in CamerasBySide)
            {
                CameraBase cam = cameraEntry.Value;
                cam.InitSettings();
                cam.SetTriggerMode(TriggerMode.Off);
                cam.SetTriggerMode(TriggerMode.Software);
                cam.StartContinuousGrab();
            }
        }

        public bool IsBackSideAvailable()
        {
            return ScreensBySide.ContainsKey(Side.Back) && CamerasBySide.ContainsKey(Side.Back);
        }

        public bool IsFrontSideAvailable()
        {
            return ScreensBySide.ContainsKey(Side.Front) && CamerasBySide.ContainsKey(Side.Front);
        }

        public void Init(IHardwareConfiguation hardwareConfiguration, IGlobalStatusServer globalStatus)
        {
        }

        public void Reset()
        {
            var messageResetStarted = "Hardware Reset started";
            _globalStatus.SetGlobalStatus(new GlobalStatus(PMGlobalStates.Initializing, new Message(MessageLevel.Information, messageResetStarted)));
            try
            {
                Stop();
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
            foreach (ScreenBase scr in Screens.Values)
            {
                scr.Shutdown();
            }
            Screens.Clear();
            ScreensBySide.Clear();
            CamerasBySide.Clear();
            OpticalMounts.Clear();
            UnitializeDevices();
        }

        protected override bool InitializeChuck(HardwareConfiguration hardwareConfiguration)
        {
            bool initFatalError = false;
            if (!hardwareConfiguration.USPChuckConfigs.Exists(x => x.IsEnabled))
                return false;

            _logger.Information("chucks device initialization started...");
            foreach (var config in hardwareConfiguration.USPChuckConfigs.Where(x => x.IsEnabled))
            {
                _logger.Information(string.Format("{0} chuck device creation", config.Name));
                var chuckLogger = new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Chuck.ToString(), config.Name);

                try
                {
                    Chuck = DMTChuck.Create(GlobalStatusServer, chuckLogger, config, Controllers);
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
                var ChamberLogger = new HardwareLogger(config.LogLevel.ToString(), DeviceFamily.Chamber.ToString(), config.Name);

                try
                {
                    Chamber = DMTChamber.Create(GlobalStatusServer, ChamberLogger, config, Controllers);
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
    }
}
