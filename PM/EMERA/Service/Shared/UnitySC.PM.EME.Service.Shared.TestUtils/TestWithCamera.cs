using System.Collections.Generic;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Implementation.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public interface ITestWithCamera
    {
        DummyIDSCamera SimulatedCamera { get; set; }

        IEmeraCamera EmeraCamera { get; set; }

        EmeHardwareManager HardwareManager { get; set; }
    }

    public static class TestWithCameraHelper
    {
        public static void Setup(ITestWithCamera test)
        {
            var globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var logger = ClassLocator.Default.GetInstance<ILogger>();
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();

            var cameraConfig = new CameraConfigBase();
            cameraConfig.DeviceID = "cameraId";
            test.SimulatedCamera = new DummyIDSCamera(cameraConfig, globalStatus, logger);
            test.SimulatedCamera.SetTriggerMode(CameraBase.TriggerMode.Off);
            test.HardwareManager.Cameras["cameraId"] = test.SimulatedCamera;

            var cameraManager = new USPCameraManager();
            test.EmeraCamera = new EmeraCamera(test.HardwareManager, cameraManager, messenger);

            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.SetAOI(It.IsAny<Rect>())).Callback<Rect>(rect => { test.SimulatedCamera.SetAOI(rect); });
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.SetImageResolution(It.IsAny<Size>())).Callback<Size>(size => { test.SimulatedCamera.SetImageResolution(size); });
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.SetFrameRate(It.IsAny<double>())).Callback<double>(framerate => { test.SimulatedCamera.SetFrameRate(framerate); });
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.SetCameraExposureTime(It.IsAny<double>())).Callback<double>(expTime => { test.SimulatedCamera.SetExposureTimeMs(expTime); });
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.StartAcquisition()).Callback(() => { test.SimulatedCamera.StartContinuousGrab(); });
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.StopAcquisition()).Callback(() => { test.SimulatedCamera.StopContinuousGrab(); });
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetFrameRate()).Returns(test.SimulatedCamera.GetFrameRate());
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetCameraExposureTime()).Returns(test.SimulatedCamera.GetExposureTimeMs());

            var cameraInfo = new MatroxCameraInfo()
            {
                Width = test.SimulatedCamera.Width,
                Height = test.SimulatedCamera.Height,
                MinFrameRate = test.SimulatedCamera.MinFrameRate,
                MaxFrameRate = test.SimulatedCamera.MaxFrameRate,
                MinExposureTimeMs = test.SimulatedCamera.MinExposureTimeMs,
                MaxExposureTimeMs = test.SimulatedCamera.MaxExposureTimeMs,
                MinGain = test.SimulatedCamera.MinGain,
                MaxGain = test.SimulatedCamera.MaxGain,
                Model = test.SimulatedCamera.Model,
            };

            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetMatroxCameraInfo()).Returns(cameraInfo);
        }

        public static void SetupCameraWithImages(List<DummyUSPImage> images)
        {
            var sequentialImages = new List<DummyUSPImage>();
            foreach (var img in images)
            {
                sequentialImages.Add(img);
            }

            Bootstrapper.SimulatedEmeraCamera.Invocations.Clear();
            var sequentialResult = Bootstrapper.SimulatedEmeraCamera.SetupSequence(_ => _.GetScaledCameraImage(It.IsAny<Int32Rect>(), It.IsAny<double>()));
            foreach (var img in sequentialImages)
            {
                sequentialResult.Returns(img.ToServiceImage());
            }
        }

        public static void SetupCameraWithImagesForSingleScaledAcquisition(List<DummyUSPImage> images)
        {
            var sequentialImages = new List<DummyUSPImage>();
            foreach (var img in images)
            {
                sequentialImages.Add(img);
            }

            Bootstrapper.SimulatedEmeraCamera.Invocations.Clear();
            var sequentialResult = Bootstrapper.SimulatedEmeraCamera.SetupSequence(_ => _.SingleScaledAcquisition(It.IsAny<Int32Rect>(), It.IsAny<double>()));
            foreach (var img in sequentialImages)
            {
                sequentialResult.Returns(img.ToServiceImage());
            }
        }

        public static void SetupCameraWithSameImageContinuously(DummyUSPImage image)
        {
            Bootstrapper.SimulatedEmeraCamera.Invocations.Clear();
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.GetScaledCameraImage(It.IsAny<Int32Rect>(), It.IsAny<double>())).Returns(image.ToServiceImage());
        }

        public static void SetupCameraWithSameImageContinuouslyForSingleAcquisition(DummyUSPImage image)
        {
            Bootstrapper.SimulatedEmeraCamera.Invocations.Clear();
            Bootstrapper.SimulatedEmeraCamera.Setup(_ => _.SingleScaledAcquisition(It.IsAny<Int32Rect>(), It.IsAny<double>())).Returns(image.ToServiceImage());
        }

    }
}
