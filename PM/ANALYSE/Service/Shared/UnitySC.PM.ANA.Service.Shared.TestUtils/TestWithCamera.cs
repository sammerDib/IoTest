using Moq;
using System.Collections.Generic;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public interface ITestWithCamera : ITestWithObjective
    {
        string CameraUpId { get; set; }
        string CameraBottomId { get; set; }

        Mock<CameraBase> SimulatedCameraUp { get; set; }
        Mock<CameraBase> SimulatedCameraBottom { get; set; }
    }

    public static class TestWithCameraHelper
    {
        public static void Setup(ITestWithCamera test)
        {
            test.CameraUpId = "cameraUp";
            test.CameraBottomId = "cameraBottom";

            var globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var logger = ClassLocator.Default.GetInstance<ILogger>();

            // Setup mocked camera & associated objectives & calibrations

            test.SimulatedCameraUp = new Mock<CameraBase>(new CameraConfigBase() { Name = test.CameraUpId, DeviceID = test.CameraUpId, ModulePosition = ModulePositions.Up, IsMainCamera = true }, globalStatusServer, logger);
            test.SimulatedCameraUp.Setup(_ => _.IsAcquiring).Returns(true);
            test.SimulatedCameraUp.Setup(_ => _.GetFrameRate()).Returns(20);
            test.SimulatedCameraUp.Setup(_ => _.GetExposureTimeMs()).Returns(40);
            test.SimulatedCameraUp.Object.Config.ObjectivesSelectorID = test.ObjectiveUpId;
            test.HardwareManager.Cameras[test.CameraUpId] = test.SimulatedCameraUp.Object;

            test.SimulatedCameraBottom = new Mock<CameraBase>(new CameraConfigBase() { Name = test.CameraBottomId, DeviceID = test.CameraBottomId, ModulePosition = ModulePositions.Down }, globalStatusServer, logger);
            test.SimulatedCameraBottom.Setup(_ => _.IsAcquiring).Returns(true);
            test.SimulatedCameraBottom.Setup(_ => _.GetFrameRate()).Returns(20);
            test.SimulatedCameraBottom.Setup(_ => _.GetExposureTimeMs()).Returns(40);
            test.SimulatedCameraBottom.Object.Config.ObjectivesSelectorID = test.ObjectiveBottomId;
            test.HardwareManager.Cameras[test.CameraBottomId] = test.SimulatedCameraBottom.Object;
        }

        public static void SetupCameraWithImages(List<DummyUSPImage> images)
        {
            var sequentialImages = new List<DummyUSPImage>();
            foreach (var img in images)
            {
                sequentialImages.Add(img);
            }

            Bootstrapper.SimulatedCameraManager.Invocations.Clear();
            var sequentialResult = Bootstrapper.SimulatedCameraManager.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()));
            foreach (var img in sequentialImages)
            {
                sequentialResult.Returns(img);
            }
        }

        public static void SetupCameraWithSameImageContinuously(DummyUSPImage image)
        {
            Bootstrapper.SimulatedCameraManager.Invocations.Clear();
            Bootstrapper.SimulatedCameraManager.Setup(_ => _.GetNextCameraImage(It.IsAny<CameraBase>())).Returns(image);
        }
    }
}
