using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Hardware.Manager.Test
{
    [TestClass]
    public class AnaHardwareManagerTest
    {
        private AnaHardwareManager _anaHardwareManager;
        private ILogger _hardwareLogger;

        [TestInitialize]
        public void SetUp()
        {
            _anaHardwareManager = new AnaHardwareManager(Mock.Of<ILogger>(), Mock.Of<IHardwareLoggerFactory>(), new FakeConfigurationManager());
            _hardwareLogger=Mock.Of<IHardwareLogger>();
        }

        [Ignore("Mandatory logs configuration is missing, when running in Release mode")]
        [TestMethod]
        public void GetMainCameraObjective()
        {
            // Given
            var mainObjective = new ObjectiveConfig { IsMainObjective = true };
            var objectiveSelector = BuildObjectiveSelector(_ =>
                _.Config.Objectives = new List<ObjectiveConfig> { mainObjective });
            _anaHardwareManager.ObjectivesSelectors.Add(objectiveSelector.Config.DeviceID, objectiveSelector);

            var camera = BuildCameraBase(_ =>
            {
                _.Config.ObjectivesSelectorID = objectiveSelector.DeviceID;
                _.Config.IsMainCamera = true;
            });
            _anaHardwareManager.Cameras.Add(camera.DeviceID, camera);

            // When
            var mainObjectiveOfMainCamera = _anaHardwareManager.GetMainObjectiveOfMainCamera();

            // Then
            Assert.AreEqual(mainObjective, mainObjectiveOfMainCamera);
        }

        [TestMethod]
        public void GetMainCameraObjective_throws_when_no_main_camera()
        {
            // Given
            var camera = BuildCameraBase(_ => _.Config.IsMainCamera = false);
            _anaHardwareManager.Cameras.Add(camera.DeviceID, camera);

            // When & Then
            Assert.ThrowsException<InvalidOperationException>(() => _anaHardwareManager.GetMainObjectiveOfMainCamera());
        }

        [TestMethod]
        public void GetMainCameraObjective_throws_when_no_objective_selector()
        {
            // Given
            _anaHardwareManager.ObjectivesSelectors = new Dictionary<string, IObjectiveSelector>();

            var camera = BuildCameraBase(_ =>
            {
                _.Config.ObjectivesSelectorID = "selector01";
                _.Config.IsMainCamera = true;
            });
            _anaHardwareManager.Cameras.Add(camera.DeviceID, camera);

            // When & Then
            Assert.ThrowsException<InvalidOperationException>(() => _anaHardwareManager.GetMainObjectiveOfMainCamera());
        }

        [Ignore("Mandatory logs configuration is missing, when running in Release mode")]
        [TestMethod]
        public void GetMainCameraObjective_throws_when_no_main_objective()
        {
            // Given
            var objectiveSelector = BuildObjectiveSelector(_ => _.Config.Objectives = new List<ObjectiveConfig>());
            _anaHardwareManager.ObjectivesSelectors.Add(objectiveSelector.Config.DeviceID, objectiveSelector);

            var camera = BuildCameraBase(_ =>
            {
                _.Config.ObjectivesSelectorID = objectiveSelector.DeviceID;
                _.Config.IsMainCamera = true;
            });
            _anaHardwareManager.Cameras.Add(camera.DeviceID, camera);

            // When & Then
            Assert.ThrowsException<InvalidOperationException>(() => _anaHardwareManager.GetMainObjectiveOfMainCamera());
        }

        private CameraBase BuildCameraBase(Action<CameraBase> customize = null)
        {
            var cameraMock = new Mock<CameraBase>(
                Mock.Of<CameraConfigBase>(),
                Mock.Of<IGlobalStatusServer>(),
                Mock.Of<ILogger>()
            );
            cameraMock.SetupAllProperties();
            var camera = cameraMock.Object;
            camera.Config = new CameraConfigBase { DeviceID = "camera01" };
            camera.DeviceID = camera.Config.DeviceID;
            customize?.Invoke(camera);
            return camera;
        }

        private IObjectiveSelector BuildObjectiveSelector(Action<IObjectiveSelector> customize = null)
        {
            var objectiveSelector = new LinMotUdp(new LineMotObjectivesSelectorConfig
            {
                DeviceID = "selector01",
                Objectives = new List<ObjectiveConfig>
                {
                    new ObjectiveConfig { IsMainObjective = true },
                    new ObjectiveConfig { IsMainObjective = false }
                }
            },_hardwareLogger);
            customize?.Invoke(objectiveSelector);
            return objectiveSelector;
        }
    }
}
