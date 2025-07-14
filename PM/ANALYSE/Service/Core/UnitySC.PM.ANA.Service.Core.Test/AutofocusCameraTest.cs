using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.ANA.Hardware.Manager;
using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Test.Helpers;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using static UnitySC.PM.ANA.Service.Core.Test.Helpers.CameraTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test
{
    [TestClass]
    public class AutofocusCameraTest
    {
        private AnaHardwareManager _hardwareManager;

        private Mock<IAxes> _simulatedAxes;
        private Mock<DummyIDSCamera> _simulatedCamera;

        private const string CameraId = "1002";
        private const string ObjectiveId = "objectiveId";

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            Bootstrapper.Register(container);

            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();

            // Objective
            var simulatedObjectiveSelector = new Mock<IObjectiveSelector>();
            var objectiveConfig = new ObjectiveConfig
            {
                DepthOfField = 1
            };
            simulatedObjectiveSelector.Setup(_ => _.GetObjectiveInUse()).Returns(objectiveConfig);
            _hardwareManager.ObjectivesSelectors[ObjectiveId] = simulatedObjectiveSelector.Object;

            // setup axes mock
            _simulatedAxes = new Mock<IAxes>();
            _simulatedAxes.Setup(_ => _.WaitMotionEnd(It.IsAny<int>()));
            _simulatedAxes.Setup(_ => _.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, It.IsAny<double>(), double.NaN), It.IsAny<AxisSpeed>()));
            _hardwareManager.Axes = _simulatedAxes.Object;

            // setup camera mock
            var globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            var logger = ClassLocator.Default.GetInstance<ILogger>();
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _simulatedCamera = new Mock<DummyIDSCamera>(new CameraConfigBase() { Name = "Camera Up" }, globalStatus, logger);
            _simulatedCamera.Setup(_ => _.IsAcquiring).Returns(true);
            _simulatedCamera.Object.Config.ObjectivesSelectorID = ObjectiveId;
            _hardwareManager.Cameras[CameraId] = _simulatedCamera.Object;
        }

        [TestMethod]
        public void Autofocus_succeeds_to_stabilize_on_default_images()
        {
            // Given :  The camera provides 2 blurry images, then 1 focused image, then 2 blurry images
            var img_at_focus = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8.tif"));
            var img_blured_with_median_kernel_3 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_3.tif"));
            var img_blured_with_median_kernel_5 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_5.tif"));
            var img_blured_with_median_kernel_7 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_7.tif"));
            var img_blured_with_median_kernel_9 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_9.tif"));

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_7)
                .Returns(img_at_focus)
                .Returns(img_blured_with_median_kernel_5)
                .Returns(img_blured_with_median_kernel_3);

            // When : Run autofocus between zMax = 15 and zMin = 11 with a step of 1
            var AFImageInput = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(11, 15, 1) };
            var AFImage = new AFImageFlow(AFImageInput);
            var result = AFImage.Execute();

            // Then : Autofocus succeeded to find autofocus position at the third step, so z = 13
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(13, result.ZPosition);
        }

        [TestMethod]
        public void Autofocus_succeeds_to_stabilize_on_interferometric_images()
        {
            // Given :  The camera provides 3 blurry interferometric images, then 1 focused, then 3 blurry
            var interfero_at_focus = CreateProcessingImageFromFile(CreateDefaultDataPath("interfero_at_focus.tif"));
            var interfero_blurx1_at_focus = CreateProcessingImageFromFile(CreateDefaultDataPath("interfero_blurx1_focus.tif"));
            var interfero_blurx2_at_focus = CreateProcessingImageFromFile(CreateDefaultDataPath("interfero_blurx2_focus.tif"));

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(interfero_blurx2_at_focus)
                .Returns(interfero_blurx2_at_focus)
                .Returns(interfero_blurx1_at_focus)
                .Returns(interfero_at_focus)
                .Returns(interfero_blurx1_at_focus)
                .Returns(interfero_blurx2_at_focus)
                .Returns(interfero_blurx1_at_focus);

            // When : Run autofocus between zMax = 19 and zMin = 13 with a step of 1
            var AFImageInput = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(13, 19, 1) };
            var AFImage = new AFImageFlow(AFImageInput);
            var result = AFImage.Execute();

            // Then : Autofocus succeeded to find autofocus position at the fourth step, so z = 16
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(16, result.ZPosition);
        }

        [TestMethod]
        public void Autofocus_succeeds_to_stabilize_when_focus_position_is_the_last_position()
        {
            // Given :  The camera provides 4 blurry images, then 1 focused image
            var img_at_focus = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8.tif"));
            var img_blured_with_median_kernel_9 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_9.tif"));

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_at_focus);

            // When : Run autofocus between zMax = 15 and zMin = 11 with a step of 1
            var AFImageInput = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(11, 15, 1) };
            var AFImage = new AFImageFlow(AFImageInput);
            var result = AFImage.Execute();

            // Then : Autofocus succeeded to find autofocus position at the last step, so z = 15
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(15, result.ZPosition);
        }

        [TestMethod]
        public void Autofocus_failed_to_stabilize_when_all_images_are_too_blured()
        {
            // Given :  The camera provides too blurry images
            var img_blured_with_median_kernel_7 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_7.tif"));
            var img_blured_with_median_kernel_9 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_9.tif"));

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9);

            // When : Run autofocus between zMax = 15 and zMin = 11 with a step of 1
            var AFImageInput = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(11, 15, 1) };
            var AFImage = new AFImageFlow(AFImageInput);
            var result = AFImage.Execute();

            // Then : Autofocus failed to find autofocus position
            Assert.AreEqual(FlowState.Error, result.Status.State);
            Assert.AreEqual(0, result.QualityScore);
        }

        [TestMethod]
        public void Autofocus_quality_score_is_smaller_when_stabilized_on_blured_images()
        {
            // Given :  The camera provides images
            var img_at_focus = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8.tif"));
            var img_blured_with_median_kernel_3 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_3.tif"));
            var img_blured_with_median_kernel_5 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_5.tif"));
            var img_blured_with_median_kernel_7 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_7.tif"));
            var img_blured_with_median_kernel_9 = CreateProcessingImageFromFile(CreateDefaultDataPath("cat_gray8_blured_with_median_kernel_9.tif"));

            // When : Run autofocus between zMax = 15 and zMin = 10 with a step of 1 and with provided images allowing to find a more or less blurred focus position

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_7)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9);

            var AFImageInput = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(10, 15, 1) };
            var AFImage = new AFImageFlow(AFImageInput);
            var result = AFImage.Execute();

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_5)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9);

            var AFImageInput2 = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(10, 15, 1) };
            var AFImage2 = new AFImageFlow(AFImageInput2);
            var result2 = AFImage2.Execute();

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_3)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9);

            var AFImageInput3 = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(10, 15, 1) };
            var AFImage3 = new AFImageFlow(AFImageInput3);
            var result3 = AFImage3.Execute();

            Bootstrapper.SimulatedCameraServer.Invocations.Clear();
            Bootstrapper.SimulatedCameraServer.SetupSequence(_ => _.GetNextCameraImage(It.IsAny<CameraBase>()))
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_at_focus)
                .Returns(img_blured_with_median_kernel_9)
                .Returns(img_blured_with_median_kernel_9);

            var AFImageInput4 = new AFImageInput(CameraId, ScanRangeType.Configured) { ScanRangeConfigured = new ScanRangeWithStep(10, 15, 1) };
            var AFImage4 = new AFImageFlow(AFImageInput4);
            var result4 = AFImage4.Execute();

            // Then : Autofocus quality score is grater when it stabilize on focus position than on a blured position
            Assert.IsTrue(result.QualityScore < result2.QualityScore && result2.QualityScore < result3.QualityScore && result3.QualityScore < result4.QualityScore);
        }
    }
}
