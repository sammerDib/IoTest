using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Client.Proxy.Test
{
    [TestClass]
    public class CameraSupervisorTests
    {
        [TestMethod]
        public void CameraModel_ShouldBeDummy()
        {
            // Given
            var camera = new CameraSupervisorEx(new SerilogLogger<ICameraServiceEx>(), null);

            // When
            var cameraInfo = camera.GetCameraInfo().Result;

            // Then
            Assert.IsNotNull(cameraInfo);
            Assert.AreEqual("DummyIDSCamera", cameraInfo.Model);
        }

        [TestMethod]
        public void SetExposureTime_ShouldChangeCameraExposureTime()
        {
            // Given
            var camera = new CameraSupervisorEx(new SerilogLogger<ICameraServiceEx>(), null);

            // When
            camera.SetCameraExposureTime(3.14);
            double actualExposureTime = camera.GetCameraExposureTime().Result;

            // Then
            Assert.AreEqual(3.14, actualExposureTime);
        }

        [TestMethod]
        public void SetGain_ShouldChangeCameraGain()
        {
            // Given
            var camera = new CameraSupervisorEx(new SerilogLogger<ICameraServiceEx>(), null);

            // When
            camera.SetCameraGain(42);
            double actualGain = camera.GetCameraGain().Result;

            // Then
            Assert.AreEqual(42, actualGain);
        }

        [TestMethod]
        public void SingleCameraAcquisition_ShouldProduceAnNonNullImage()
        {
            // Given
            var camera = new CameraSupervisorEx(new SerilogLogger<ICameraServiceEx>(), null);

            // When
            var image = camera.SingleAcquisition().Result;

            // Then
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void ContinuousCameraAcquisition_ShouldProduceNonNullImage()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var camera = new CameraSupervisorEx(new SerilogLogger<ICameraServiceEx>(), messenger);

            ServiceImageWithStatistics image = null;
            messenger.Register<ServiceImageWithStatistics>(this, (_, i) => image = i);

            // When
            camera.Subscribe(Int32Rect.Empty, 1);
            camera.StartAcquisition();
            Task.Delay(500).Wait();
            camera.StopAcquisition();
            camera.Unsubscribe();

            // Then
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void AcquisitionWithReducedScale_ShouldProduceAnImageWithSmallerSize()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var camera = new CameraSupervisorEx(new SerilogLogger<ICameraServiceEx>(), messenger);
            camera.Subscribe(Int32Rect.Empty, 0.5);

            ServiceImageWithStatistics image = null;
            messenger.Register<ServiceImageWithStatistics>(this, (_, i) => image = i);

            // When
            camera.StartAcquisition();
            Task.Delay(500).Wait();
            camera.StopAcquisition();
            camera.Unsubscribe();

            // Then
            Assert.AreEqual(image.OriginalWidth / 2, image.DataWidth);
            Assert.AreEqual(image.OriginalHeight / 2, image.DataHeight);
            Assert.AreEqual(0.5, image.Scale);
        }
    }
}
