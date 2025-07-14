using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Modules.TestApps.Camera;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Modules.TestApps.Test
{
    [TestClass]
    public class CameraViewModelTest
    {
        [TestInitialize]
        public void SetupTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
        public void ImageShouldBeDisplayed_whenGrabbedImageIsSent()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var cameraBench = new CameraBench(new FakeCameraSupervisor(null), new FakeCalibrationSupervisor(), null);
            var cameraViewModel = new CameraViewModel(cameraBench, messenger);

            // When
            var image = new ServiceImageWithStatistics() { Data = new byte[] { 1 }, DataHeight = 1, DataWidth = 1 };
            messenger.Send(image);

            // Then
            Assert.IsNotNull(cameraViewModel.Image);
        }

        [TestMethod]
        public void IntensityProfileShouldBeComputed_whenGrabbedImageIsSent()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var cameraBench = new CameraBench(new FakeCameraSupervisor(null), new FakeCalibrationSupervisor(), null);
            var cameraViewModel = new CameraViewModel(cameraBench, messenger)
            {
                RulerStartPoint = new Point(0, 0),
                RulerEndPoint = new Point(cameraBench.Width, 0),
                ImageReferentialOrigin = new Point(0, 0),
                IsRulerActivated = true,
                Zoom = 1.0
            };

            // When
            var image = new ServiceImageWithStatistics() { Scale = 1.0 };
            image.CreateFromBitmap(new DummyImage(cameraBench.Width, cameraBench.Height, 42, PixelFormats.Gray8).Src);
            messenger.Send(image);

            // Then
            Assert.IsNotNull(cameraViewModel.IntensityPoints);
            Assert.AreEqual(cameraBench.Width, cameraViewModel.IntensityPoints.Length);
            Assert.AreEqual(42, cameraViewModel.IntensityPoints[0].Y);
        }


        [TestMethod]
        public void ScaleTextValue_ShouldDependOnPixelSize()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var calibrationSupervisor = new FakeCalibrationSupervisor { PixelSize = 0.1.Micrometers() };
            var cameraBench = new CameraBench(new FakeCameraSupervisor(null), calibrationSupervisor, null);

            // When
            var image = new ServiceImageWithStatistics() { Scale = 1.0 };
            image.CreateFromBitmap(new DummyImage(cameraBench.Width, cameraBench.Height, 1, PixelFormats.Gray8).Src);
            var cameraViewModel = new CameraViewModel(cameraBench, messenger) { Image = image, Zoom = 1 };

            // Then
            Assert.AreEqual(200, cameraViewModel.ScaleLengthInPixel);
            Assert.AreEqual("20 µm", cameraViewModel.ScaleTextValue);
        }

        [TestMethod]
        public void IncreaseZoomBy2_ShouldReduceTheImageScaleByTwo()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var cameraBench = new CameraBench(new FakeCameraSupervisor(null), new FakeCalibrationSupervisor(), null);

            // When
            var image = new ServiceImageWithStatistics() { Scale = 1.0 };
            image.CreateFromBitmap(new DummyImage(cameraBench.Width, cameraBench.Height, 1, PixelFormats.Gray8).Src);
            var cameraViewModel = new CameraViewModel(cameraBench, messenger) { Image = image, Zoom = 2 };

            // Then
            Assert.AreEqual(200, cameraViewModel.ScaleLengthInPixel);
            Assert.AreEqual("100 µm", cameraViewModel.ScaleTextValue);
        }

        [TestMethod]
        public void DistanceTextValue_ShouldDependOnPixelSizeAndPointPosition()
        {
            // Given
            var calibrationSupervisor = new FakeCalibrationSupervisor { PixelSize = 0.1.Micrometers() };
            var cameraBench = new CameraBench(new FakeCameraSupervisor(null), calibrationSupervisor, null);

            // When
            var image = new ServiceImageWithStatistics() { Scale = 1.0 };
            image.CreateFromBitmap(new DummyImage(cameraBench.Width, cameraBench.Height, 1, PixelFormats.Gray8).Src);
            var cameraViewModel = new CameraViewModel(cameraBench, null)
            {
                Image = image,
                RulerStartPoint = new Point(2, 2),
                RulerEndPoint = new Point(6, 5),
                Zoom = 1
            };

            // Then
            Assert.AreEqual("500 nm", cameraViewModel.DistanceValueText);
        }

        [TestMethod]
        public void WhenTheImageIsZoomed_TheDistanceTextValueIsReduced()
        {
            // Given
            var calibrationSupervisor = new FakeCalibrationSupervisor { PixelSize = 0.1.Micrometers() };
            var cameraBench = new CameraBench(new FakeCameraSupervisor(null), calibrationSupervisor, null);

            // When
            var image = new ServiceImageWithStatistics() { Scale = 1.0 };
            image.CreateFromBitmap(new DummyImage(cameraBench.Width, cameraBench.Height, 1, PixelFormats.Gray8).Src);
            var cameraViewModel = new CameraViewModel(cameraBench, null)
            {
                Image = image,
                RulerStartPoint = new Point(2, 2),
                RulerEndPoint = new Point(6, 5),
                Zoom = 2
            };

            // Then
            Assert.AreEqual("250 nm", cameraViewModel.DistanceValueText);
        }

        [TestMethod]
        public void WhenAnImagePortionIsDisplayed_TheStreamedImageIsCroppedToTheImagePortion()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var cameraBench = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), messenger);
            var cameraViewModel = new CameraViewModel(cameraBench, messenger);

            // When
            cameraSupervisor.StartAcquisition();
            cameraSupervisor.WaitForOneAcquisition();
            cameraViewModel.ImagePortion = new Int32Rect { Height = 50, Width = 40, X = 15, Y = 25 };
            cameraSupervisor.WaitForOneAcquisition();
            cameraSupervisor.StopAcquisition();

            // Then
            var lastStreamedImage = cameraViewModel.Image;
            Assert.IsNotNull(lastStreamedImage);
            Assert.AreEqual(50, lastStreamedImage.DataHeight);
            Assert.AreEqual(40, lastStreamedImage.DataWidth);
            Assert.AreEqual(15, cameraViewModel.ImageCropArea.X);
            Assert.AreEqual(25, cameraViewModel.ImageCropArea.Y);
        }

        [TestMethod]
        public void WhenTheImagePortionIsLargerThanTheTargetResolution_TheDisplayedImageResolutionIsReduced()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var cameraBench = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), messenger);
            var cameraViewModel = new CameraViewModel(cameraBench, messenger);
            cameraViewModel.TargetResolution = 80 * 45;

            // When
            cameraSupervisor.StartAcquisition();
            cameraSupervisor.WaitForOneAcquisition();
            cameraViewModel.ImagePortion = new Int32Rect { Height = 90, Width = 160, X = 0, Y = 0 };
            cameraSupervisor.WaitForOneAcquisition();
            cameraSupervisor.StopAcquisition();

            // Then
            var lastStreamedImage = cameraViewModel.Image;
            Assert.IsNotNull(lastStreamedImage);
            Assert.AreEqual(45, lastStreamedImage.DataHeight);
            Assert.AreEqual(80, lastStreamedImage.DataWidth);
        }

    }
}
