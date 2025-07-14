using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Modules.TestHardware.ViewModel;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Modules.TestHardware.Test
{
    [TestClass]
    public class CameraViewModelTest
    {
        [TestMethod]
        public void ShouldBeInitialized_WithGainAndExposureTime_FromCamera()
        {
            // Given
            var cameraSupervisor = new FakeCameraSupervisor(null) { Gain = 3.2, ExposureTime = 4.2 };
            var camera = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), null);

            // When
            var cameraViewModel = new TestCameraViewModel(camera, null);

            // Then
            Assert.AreEqual(3.2, cameraViewModel.Gain);
            Assert.AreEqual(100, cameraViewModel.ExposureTime);
        }

        [TestMethod]
        public void ApplyConfiguration_ShouldBeChangeCameraConfiguration()
        {
            // Given
            var messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var camera = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), messenger);
            var cameraViewModel = new TestCameraViewModel(camera, messenger);

            // When
            cameraViewModel.Gain = 3.2;
            cameraViewModel.ExposureTime = 4.2;
            cameraViewModel.ApplyConfiguration.ExecuteAsync(null).Wait();

            // Then
            Assert.AreEqual(3.2, cameraSupervisor.GetCameraGain().Result);
            Assert.AreEqual(4.2, cameraSupervisor.GetCameraExposureTime().Result);
        }

        [TestMethod]
        public void SingleAcquisition_ShouldProduceAnImage()
        {
            // Given
            var cameraSupervisor = new FakeCameraSupervisor(null);
            var camera = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), null);
            var cameraViewModel = new TestCameraViewModel(camera, null);

            // When
            cameraViewModel.SingleShotCommand.ExecuteAsync(null).Wait();

            // Then
            Assert.IsNotNull(cameraViewModel.Image);
        }

        [TestMethod]
        public void ContinuousAcquisition_ShouldProduceAnImage()
        {
            // Given
            var messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var camera = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), messenger);
            var cameraViewModel = new TestCameraViewModel(camera, messenger);

            // When
            cameraViewModel.StartStreamingCommand.ExecuteAsync(null).Wait();
            cameraSupervisor.WaitForOneAcquisition();
            cameraViewModel.StopStreamingCommand.ExecuteAsync(null).Wait();

            // Then
            Assert.IsNotNull(cameraViewModel.Image);
        }
    }
}
