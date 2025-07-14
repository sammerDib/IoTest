using System.Collections.Generic;
using System.IO;
using System.Windows;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.Distortion;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

namespace UnitySC.PM.EME.Service.Core.Test.Flow
{
    [TestClass]
    public class DistortionTest : TestWithMockedHardware<DistortionTest>, ITestWithPhotoLumAxes, ITestWithCamera
    {
        private Mock<AutoExposureFlow> _simulatedAutoExposureFlow { get; set; }
        public Mock<PhotoLumAxes> SimulatedMotionAxes { get; set; }
        public DummyIDSCamera SimulatedCamera { get; set; }
        public IEmeraCamera EmeraCamera { get; set; }

        protected override void PostGenericSetup()
        {

            TestWithCameraHelper.SetupCameraWithImagesForSingleScaledAcquisition(new List<DummyUSPImage>() { CreateDistortionImage("disto.png") });

            Bootstrapper.SimulatedEmeraCamera.Setup(x => x.GetCameraExposureTime()).Returns(100.0);
            _simulatedAutoExposureFlow = new Mock<AutoExposureFlow>(SimulatedData.ValidAutoExposureInput(), Bootstrapper.SimulatedEmeraCamera.Object) { CallBase = true };
            var autoExposureSuccess = new AutoExposureResult()
            {
                Brightness = 1.0,
                ExposureTime = 100,
                Status = new FlowStatus(FlowState.Success)
            };
            _simulatedAutoExposureFlow.Setup(_ => _.Execute()).Returns(autoExposureSuccess);

        }

        [TestMethod]
        public void Distortion_test()
        {

            List<DummyUSPImage> images = new List<DummyUSPImage>();

            SimulatedCamera.SetImageResolution(new Size(400, 400));
            MotorizedAxisConfig motorizedAxisConfig = new MotorizedAxisConfig();


            var distortionInput = new DistortionInput();
            var distortionFlow = new DistortionFlow(distortionInput, Bootstrapper.SimulatedEmeraCamera.Object, _simulatedAutoExposureFlow.Object);

            var result = distortionFlow.Execute();

            DistortionData cameraCorrectionMat = distortionFlow.Result.DistortionData;



            Assert.AreEqual(FlowState.Success, result.Status.State);

            distortionFlow.Result.DistortionData.CameraMat[0].Should().BeApproximately(687695.09556110145, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[1].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[2].Should().BeApproximately(249.9416846047213, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[3].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[4].Should().BeApproximately(687695.09556110145, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[5].Should().BeApproximately(249.9416846047213, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[6].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[7].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.CameraMat[8].Should().BeApproximately(1, 0.01);

            distortionFlow.Result.DistortionData.NewOptimalCameraMat[0].Should().BeApproximately(603010.022444149, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[1].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[2].Should().BeApproximately(250.04562391446581, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[3].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[4].Should().BeApproximately(603010.022444149, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[5].Should().BeApproximately(250.04562391446581, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[6].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[7].Should().BeApproximately(0, 0.01);
            distortionFlow.Result.DistortionData.NewOptimalCameraMat[8].Should().BeApproximately(1, 0.01);


        }

        private static DummyUSPImage CreateDistortionImage(string filename)
        {
            const string DistortionFolder = "distortion";
            return CameraTestUtils.CreateProcessingImageFromFile(CameraTestUtils.GetDefaultDataPath(Path.Combine(DistortionFolder, filename)));
        }
    }
}
