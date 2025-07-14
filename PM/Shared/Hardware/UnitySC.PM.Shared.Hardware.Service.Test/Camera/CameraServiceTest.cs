using System;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Service.Test.Camera
{
    [TestClass]
    public class CameraServiceTest
    {
        private CameraService _cameraService;
        private ICameraManager _cameraManager;
        private IMessenger _messenger;
        private CancellationTokenSource _cancellationTokenSource;
        private int _exposureTimeMs;
        private int _frameRate;

        [TestInitialize]
        public void SetUp()
        {
            Bootstrapper.Register(new Container());
            var logger = new Mock<ILogger<CameraService>>();
            _cameraService = new CameraService(logger.Object);
            _cameraService.Init();

            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();

            _cancellationTokenSource = new CancellationTokenSource();
            _exposureTimeMs = 20;
            _frameRate = 24;
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                _cameraService.Shutdown();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Cannot shutdown camera service: " + e.Message);
            }

            _cancellationTokenSource.Cancel();
        }

        [TestMethod]
        public void GetNextImage_SkipsFirstImageAcquiredBeforeCall()
        {
            // Set long exposure time to make sure threading/sleeps aren't messed up.
            _exposureTimeMs = 200;
            _frameRate = 3;

            // Given
            var camera = BuildAcquiringCamera();
            var imageWhoseAcquisitionStartedBeforeCall = BuildImage();
            var actualNextImage = BuildImage();
            Assert.AreNotEqual(imageWhoseAcquisitionStartedBeforeCall, actualNextImage);

            // When
            var task = Task.Run(() => _cameraManager.GetNextCameraImage(camera), _cancellationTokenSource.Token);
            Thread.Sleep(_exposureTimeMs / 10); // First image, acquired before call (sent before ExposureTime elapsed)
            _messenger.Send(new CameraMessage { Camera = camera, Image = imageWhoseAcquisitionStartedBeforeCall });
            Thread.Sleep(_exposureTimeMs); // Second image acquired after call (first real image whose acquisition started after call)
            _messenger.Send(new CameraMessage { Camera = camera, Image = actualNextImage });
            bool taskFinished = task.Wait(TimeSpan.FromSeconds(1));
            Assert.IsTrue(taskFinished, "method not finished as expected");

            // Then
            var resultImage = task.Result;
            Assert.AreEqual(actualNextImage, resultImage, $"Result image is not the expected next image. Is it actually the first image that is expected to be skipped? {resultImage == imageWhoseAcquisitionStartedBeforeCall}");
        }

        [TestMethod]
        public void GetNextImage_ReturnsFirstImageAcquiredAfterCall()
        {
            // Given
            var camera = BuildAcquiringCamera();
            var firstNextImage = BuildImage();
            var secondNextImage = BuildImage();
            Assert.AreNotEqual(firstNextImage, secondNextImage);

            // When
            var task = Task.Run(() => _cameraManager.GetNextCameraImage(camera), _cancellationTokenSource.Token);
            Thread.Sleep(_exposureTimeMs * 2); // First image acquired after call
            _messenger.Send(new CameraMessage { Camera = camera, Image = firstNextImage });
            Thread.Sleep(_exposureTimeMs); // Second image acquired after call
            _messenger.Send(new CameraMessage { Camera = camera, Image = secondNextImage });
            bool taskFinished = task.Wait(TimeSpan.FromSeconds(1));
            Assert.IsTrue(taskFinished, "method not finished as expected");

            // Then
            var actualImage = task.Result;
            Assert.AreEqual(firstNextImage, actualImage);
        }

        [TestMethod]
        public void GetNextImage_does_not_return_image_from_another_camera()
        {
            // Given
            var camera = BuildAcquiringCamera();

            // When
            var task = Task.Run(() => _cameraManager.GetNextCameraImage(camera));
            Thread.Sleep(CameraService.DefaultNextImageTimeout.Milliseconds / 2); // This makes sure we will listen for next message

            var anotherCamera = BuildAcquiringCamera();
            var anotherImage = BuildImage();
            // Send 2 images because the firest will always be skipped
            _messenger.Send(new CameraMessage { Camera = anotherCamera, Image = anotherImage });
            _messenger.Send(new CameraMessage { Camera = anotherCamera, Image = anotherImage });

            var image = BuildImage();
            // Send 2 images because the firest will always be skipped
            _messenger.Send(new CameraMessage { Camera = camera, Image = image });
            _messenger.Send(new CameraMessage { Camera = camera, Image = image });

            bool taskFinished = task.Wait(TimeSpan.FromSeconds(1));
            Assert.IsTrue(taskFinished, "method not finished as expected");

            // Then
            Assert.AreNotEqual(anotherImage, task.Result);
            Assert.AreEqual(image, task.Result);
        }

        [TestMethod]
        public void GetNextImage_throws_when_camera_is_not_started()
        {
            // Given
            var camera = BuildNotAcquiringCamera();

            // When
            Assert.ThrowsException<Exception>(() => _cameraManager.GetNextCameraImage(camera));
        }

        [TestMethod]
        public void GetNextImage_throws_when_no_image_received_after_timeout()
        {
            // Given
            var camera = BuildNotAcquiringCamera();
            double fps = 20;
            camera.SetFrameRate(fps);

            // When
            var task = Task.Run(() => _cameraManager.GetNextCameraImage(camera));
            int delayBetweenFramesInMs = 1000 / 20;
            Thread.Sleep(2 * delayBetweenFramesInMs + 100);

            // Then
            Assert.IsTrue(task.IsCompleted);
            var aggregateException = task.Exception;
            Assert.IsNotNull(aggregateException);
            Assert.AreEqual(1, aggregateException.InnerExceptions.Count);
        }

        private Mock<DummyIDSCamera> BuildCameraMock()
        {
            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var camLogger = new Mock<ILogger<DummyIDSCamera>>();
            var mock = new Mock<DummyIDSCamera>(new CameraConfigBase { Name = "Test camera" }, globalStatusServer,
                camLogger.Object);
            return mock;
        }

        private CameraBase BuildAcquiringCamera()
        {
            var mock = BuildCameraMock();
            mock.Setup(c => c.IsAcquiring).Returns(true);
            mock.Setup(c => c.GetExposureTimeMs()).Returns(_exposureTimeMs);
            mock.Setup(c => c.GetFrameRate()).Returns(_frameRate);
            return mock.Object;
        }

        private CameraBase BuildNotAcquiringCamera()
        {
            var mock = BuildCameraMock();
            mock.Setup(c => c.IsAcquiring).Returns(false);
            return mock.Object;
        }

        private USPImage BuildImage()
        {
            return new USPImage();
        }
    }
}
