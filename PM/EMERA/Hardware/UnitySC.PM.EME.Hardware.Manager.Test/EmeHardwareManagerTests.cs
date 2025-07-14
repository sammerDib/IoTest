using System;
using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Implementation.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using static UnitySC.PM.Shared.Hardware.Camera.CameraBase;

namespace UnitySC.PM.EME.Hardware.Manager.Test
{
    [TestClass()]
    public class EmeHardwareManagerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register(typeof(IGlobalStatusServer), typeof(StubGlobalStatus), singleton: true);
            ClassLocator.Default.Register(typeof(IReferentialManager), typeof(StubReferentialManager), true);
            ClassLocator.Default.Register(typeof(IMotionAxesServiceCallbackProxy), typeof(StubMotionAxesServiceCallbackProxy), true);
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
            SerilogInit.Init(Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "DEMO/Configuration/log.config"));
        }

        [TestMethod]
        public void InitializeHardware_FromDummyConfiguration()
        {
            // Given
            var globalStatus = new StubGlobalStatus();
            var configManager = new FakeConfigurationManager("DEMO");
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), configManager, globalStatus);

            // When
            bool isInitialInSuccess = hardwareManager.Init();

            // Then
            Assert.AreEqual(PMGlobalStates.Free, globalStatus.GetGlobalState());
            Assert.AreEqual(true, isInitialInSuccess);
            Assert.AreEqual(1, hardwareManager.Cameras.Count);
        }

        [TestMethod]
        public void InitializeHardware_FromAlphaConfiguration()
        {
            // Given
            var globalStatus = new StubGlobalStatus();
            var configManager = new FakeConfigurationManager("ALPHA");
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), configManager, globalStatus);

            // When
            bool isInitialInSuccess = hardwareManager.Init();

            // Then
            Assert.AreEqual(PMGlobalStates.Free, globalStatus.GetGlobalState());
            Assert.AreEqual(true, isInitialInSuccess);
            Assert.AreEqual(1, hardwareManager.Cameras.Count);
        }

        [TestMethod]
        public void InitializeHardware_FromDemoConfiguration()
        {
            // Given
            var globalStatus = new StubGlobalStatus();
            var configManager = new FakeConfigurationManager("DEMO");
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), configManager, globalStatus);

            // When
            bool isInitialInSuccess = hardwareManager.Init();

            // Then
            Assert.AreEqual(PMGlobalStates.Free, globalStatus.GetGlobalState());
            Assert.AreEqual(true, isInitialInSuccess);
            Assert.AreEqual(1, hardwareManager.Cameras.Count);
        }

        [TestMethod]
        public void GetNextImage_FromDummyCamera_AfterContinuousGrab()
        {
            // Given
            var globalStatus = new StubGlobalStatus();
            var configManager = new FakeConfigurationManager("DEMO");
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), configManager, globalStatus);

            // When
            hardwareManager.Init();
            var camera = hardwareManager.Cameras["1"];
            camera.SetTriggerMode(TriggerMode.Off);
            camera.StartContinuousGrab();

            // Then
            var cameraManger = new USPCameraManager();
            var image = cameraManger.GetNextCameraImage(camera);
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void GetLastImage_FromDummyCamera_AfterContinuousGrab()
        {
            // Given
            var globalStatus = new StubGlobalStatus();
            var configManager = new FakeConfigurationManager("DEMO");
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), configManager, globalStatus);
            var cameraManger = new USPCameraManager();

            // When
            hardwareManager.Init();
            var camera = hardwareManager.Cameras["1"];
            camera.SetTriggerMode(TriggerMode.Off);
            camera.StartContinuousGrab();
            camera.WaitForSoftwareTriggerGrabbed();

            // Then
            var image = cameraManger.GetLastCameraImage(camera);
            Assert.IsNotNull(image);
        }

        private class StubGlobalStatus : IGlobalStatusServer
        {
            private PMGlobalStates _globalState;

            public event GlobalStatusChangedEventHandler GlobalStatusChanged;

            public event ToolModeChangedEventHandler ToolModeChanged;

            public void AddMessage(Message message)
            {
            }

            public PMGlobalStates GetGlobalState()
            {
                return _globalState;
            }

            public bool ReleaseLocalHardware()
            {
                return true;
            }

            public bool ReserveLocalHardware()
            {
                return true;
            }

            public void SetGlobalState(PMGlobalStates globalState)
            {
                _globalState = globalState;
            }

            public void SetGlobalStatus(GlobalStatus globalStatus)
            {
                _globalState = (PMGlobalStates)globalStatus.CurrentState;
            }

            public void SetToolModeStatus(ToolMode toolMode)
            {
                throw new NotImplementedException();
            }
        }

        private class StubMotionAxesServiceCallbackProxy : IMotionAxesServiceCallbackProxy
        {
            public void PositionChanged(PositionBase position)
            {
            }

            public void StateChanged(AxesState state)
            {
            }

            public void EndMove(bool targetReached)
            {
            }

        }

        private class StubReferentialManager : IReferentialManager
        {
            private PositionBase _position;
            private ReferentialSettingsBase _referentialSettings;

            public PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
            {
                return _position;
            }

            public ReferentialSettingsBase GetSettings(ReferentialTag referentialTag)
            {
                return _referentialSettings;
            }

            public void SetSettings(ReferentialSettingsBase settings)
            {
            }

            public void DeleteSettings(ReferentialTag referentialTag)
            {
            }

            public void DisableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
            {
            }

            public void EnableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
            {
            }
        }

        private class FakeConfigurationManager : IServiceConfigurationManager
        {
            private readonly string _configName;

            public FakeConfigurationManager(string configName)
            {
                _configName = configName;
            }

            public FlowReportConfiguration AllFlowsReportMode => FlowReportConfiguration.NeverWrite;

            public string CalibrationFolderPath => null;

            public string ConfigurationFolderPath => Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, $"{_configName}/Configuration");

            public string ConfigurationName => null;

            public bool FlowsAreSimulated => true;

            public bool HardwaresAreSimulated => true;

            public string PMConfigurationFilePath => null;

            public string FlowsConfigurationFilePath => null;

            public string LogConfigurationFilePath => null;

            public string FDCsConfigurationFilePath { get; }

            public string GetStatus()
            {
                return "";
            }
        }
    }
}
