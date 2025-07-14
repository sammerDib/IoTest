using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class AutoExposureTests : TestWithMockedHardware<AutoExposureTests>, ITestWithCamera
    {
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }

        [TestMethod]
        public void ShouldExtractBrightness()
        {
            // Given
            var cameraInfo = new CameraInfo() { MinExposureTimeMs = 10, MaxExposureTimeMs = 1000 };
            Bootstrapper.SimulatedEmeraCamera.SetupSequence(_ => _.GetCameraInfo()).Returns(cameraInfo);

            var input = new AutoExposureInput() { };
            var flow = new AutoExposureFlow(input, Bootstrapper.SimulatedEmeraCamera.Object);

            var blackImage = new DummyUSPImage(160, 80, 0, false);
            var image = new DummyUSPImage(160, 80, 191, false);
            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(new List<DummyUSPImage>() { blackImage, image, image });

            
            // When
            flow.Execute();

            // Then
            var result = flow.Result;
            Assert.IsTrue(result.Status.IsFinished);
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(0.75, result.Brightness, 0.05);
            Assert.AreNotEqual(0, result.ExposureTime);
        }
    }
}
