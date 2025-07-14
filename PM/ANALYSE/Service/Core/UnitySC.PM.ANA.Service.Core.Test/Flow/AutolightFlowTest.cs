using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Service.Core.Autolight;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.TestUtils.Context;
using UnitySC.PM.ANA.Service.Interface.TestUtils.ObjectiveSelector.Configuration;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Camera.TestUtils;
using UnitySC.PM.Shared.Hardware.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    internal static class AutolightInputFactory
    {
        public static AutolightInput Build()
        {
            return new AutolightInput(
                    cameraId: "camera#1",
                    lightId: "light#1",
                    exposure: 1.8,
                    lightPower: new ScanRangeWithStep(0, 1, 0.5)
                );
        }
    }

    [TestClass]
    public class AutolightFlowTest : TestWithMockedHardware<AutolightFlowTest>, ITestWithCamera, ITestWithLight
    {
        private Mock<Shared.ImageOperators> _simulatedImageOperatorsLib;

        private AutolightInput _defaultAutolightInput;

        #region Interfaces properties

        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public string LightId { get; set; }
        public Mock<LightBase> SimulatedLight { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            _defaultAutolightInput = new AutolightInput(
                cameraId: CameraUpId,
                lightId: LightId,
                exposure: 10,
                lightPower: new ScanRangeWithStep(40, 50, 1));

            _simulatedImageOperatorsLib = new Mock<Shared.ImageOperators>(FocusMeasureMethod.TenenbaumGradient);

            TestWithCameraHelper.SetupCameraWithSameImageContinuously(new DummyUSPImage(10, 10, 255));
        }

        private Dictionary<string, IObjectiveSelector> BuildDefaultObjectiveSelectors()
        {
            var objectiveContext = ObjectiveContextFactory.Build();
            var objectiveSelectors = new Dictionary<string, IObjectiveSelector>();
            var objectiveSelector = Hardware.ObjectiveSelector.TestUtils.ObjectiveSelectorFactory.Build(ObjectiveConfigFactory.Build(conf => conf.DeviceID = objectiveContext.ObjectiveId));
            objectiveSelectors.Add("selector", objectiveSelector);
            return objectiveSelectors;
        }

        [TestMethod]
        public void Autolight_flow_nominal_case()
        {
            // Given  The camera provides more or less lighted images
            var img_light_0 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(-1.5).tif"));
            var img_light_1 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(-1.0).tif"));
            var img_light_2 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(-0.5).tif"));
            var img_light_3 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8.tif"));
            var img_light_4 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(+1.0).tif"));
            var img_light_5 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(+1.0).tif"));
            var img_light_6 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_overexposed.tif"));

            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() {
                img_light_0,
                img_light_1,
                img_light_2,
                img_light_3,
                img_light_4,
                img_light_5,
                img_light_6 });

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(40, 46, 1);

            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);

            var autolight = new AutolightFlow(input);

            // When
            var result = autolight.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(43, result.LightPower);
        }

        [TestMethod]
        public void Autolight_fail_when_light_power_coud_not_be_reached_before_timeout()
        {
            // Given: All images are overexposed
            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>()))
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0.3)
                .Returns(0);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(0.1);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(46, 50, 1);

            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => (currentIntensity > 47.0) ? 47.5 : currentIntensity );

            var autolight = new AutolightFlow(input, _simulatedImageOperatorsLib.Object);

            // When
            var result = autolight.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Autolight_failed_when_using_top_camera_without_ZTop_position()
        {
            // Given
            var input = AutolightInputFactory.Build();
            var flow = new AutolightFlow(input);

            var camera = CameraBaseFactory.Build(cam => cam.Config.ModulePosition = ModulePositions.Up);
            HardwareManager.Cameras = new Dictionary<string, CameraBase> { { input.CameraId, camera } };
            HardwareManager.Lights = new Dictionary<string, LightBase> { { input.LightId, new Mock<LightBase>().Object } };
            HardwareManager.ObjectivesSelectors = BuildDefaultObjectiveSelectors();

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Autolight_failed_when_using_down_camera_without_ZBottom_position()
        {
            // Given
            var input = AutolightInputFactory.Build();
            var flow = new AutolightFlow(input);

            var camera = CameraBaseFactory.Build(cam => cam.Config.ModulePosition = ModulePositions.Down);
            HardwareManager.Cameras = new Dictionary<string, CameraBase> { { input.CameraId, camera } };
            HardwareManager.Lights = new Dictionary<string, LightBase> { { input.LightId, new Mock<LightBase>().Object } };
            HardwareManager.ObjectivesSelectors = BuildDefaultObjectiveSelectors();

            // When
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Autolight_failed_when_all_images_are_underexposed()
        {
            // Given: All images are overexposed
            double maxSaturation = 0.9;

            _simulatedImageOperatorsLib.Setup(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>())).Returns(1);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(0);

            var autolight = new AutolightFlow(_defaultAutolightInput, _simulatedImageOperatorsLib.Object);
            autolight.Configuration.SaturationMax = maxSaturation;

            // When
            var result = autolight.Execute();

            // Then: Autolight failed to find optimal light power
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Autolight_failed_when_all_images_are_overexposed()
        {
            // Given: All images are overexposed
            double maxSaturation = 0.9;

            _simulatedImageOperatorsLib.Setup(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>())).Returns(1);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(maxSaturation + 0.01);

            var autolight = new AutolightFlow(_defaultAutolightInput, _simulatedImageOperatorsLib.Object);
            autolight.Configuration.SaturationMax = maxSaturation;

            // When
            var result = autolight.Execute();

            // Then: Autolight failed to find optimal light power
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Autolight_success_at_correct_light_power_when_best_light_is_at_the_first_position_of_scan_range()
        {
            // Given: All images are overexposed
            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>()))
                .Returns(0.3)
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(0.1);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(46, 50, 1);

            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);

            var autolight = new AutolightFlow(input, _simulatedImageOperatorsLib.Object);

            // When
            var result = autolight.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(46, result.LightPower);
        }

        [TestMethod]
        public void Autolight_success_at_correct_light_power_when_best_light_is_at_the_middle_position_of_scan_range()
        {
            // Given: All images are overexposed
            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>()))
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0.3)
                .Returns(0);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(0.1);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(46, 50, 1);

            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);

            var autolight = new AutolightFlow(input, _simulatedImageOperatorsLib.Object);

            // When
            var result = autolight.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(49, result.LightPower);
        }

        [TestMethod]
        public void Autolight_success_at_correct_light_power_when_best_light_is_at_the_last_position_of_scan_range()
        {
            // Given: All images are overexposed
            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>()))
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0.3);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(0.1);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(46, 50, 1);

            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);

            var autolight = new AutolightFlow(input, _simulatedImageOperatorsLib.Object);

            // When
            var result = autolight.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(50, result.LightPower);
        }

        [TestMethod]
        public void Autolight_stop_before_image_became_overexposed()
        {
            // Given
            double maxSaturation = 0.9;

            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>()))
                .Returns(0)
                .Returns(0.29)
                .Returns(0)
                .Returns(5);
            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>()))
                .Returns(maxSaturation - 0.1)
                .Returns(maxSaturation - 0.1)
                .Returns(maxSaturation - 0.1)
                .Returns(maxSaturation + 0.01);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(46, 50, 1);

            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);

            var autolight = new AutolightFlow(input, _simulatedImageOperatorsLib.Object);
            autolight.Configuration.SaturationMax = maxSaturation;

            // When
            var result = autolight.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(47, result.LightPower);
        }

        [TestMethod]
        public void Report_of_input_is_working_with_AlwaysWrite_mode()
        {
            //Given
            string directoryPath = "TestAutoLightReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAutolightInput;

            var flow = new AutolightFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var filename = Path.Combine(flow.ReportFolder, $"input.txt");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_input_be_present_when_WriteOnError_and_flow_state_is_error()
        {
            //Given
            string directoryPath = "TestAutoLightReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAutolightInput;

            var flow = new AutolightFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.WriteOnError;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var filename = Path.Combine(flow.ReportFolder, $"input.txt");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_of_result_is_working()
        {
            //Given
            string directoryPath = "TestAutoLightReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAutolightInput;

            var flow = new AutolightFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var status = (flow.Result == null) ? "null" : ((flow.Result.Status == null) ? "ukn" : flow.Result.Status.State.ToString());
            var filename = Path.Combine(flow.ReportFolder, $"result_{status}.txt");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(flow.ReportFolder, true);
        }

        [TestMethod]
        public void Report_should_be_deleted_when_flow_configuration_has_WriteOnError_and_flow_state_is_success()
        {
            // Given  The camera provides more or less lighted images
            var img_light_0 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(-1.5).tif"));
            var img_light_1 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(-1.0).tif"));
            var img_light_2 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(-0.5).tif"));
            var img_light_3 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8.tif"));
            var img_light_4 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(+1.0).tif"));
            var img_light_5 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_light_(+1.0).tif"));
            var img_light_6 = CreateProcessingImageFromFile(GetDefaultDataPath("cat_gray8_overexposed.tif"));

            TestWithCameraHelper.SetupCameraWithImages(new List<DummyUSPImage>() {
                img_light_0,
                img_light_1,
                img_light_2,
                img_light_3,
                img_light_4,
                img_light_5,
                img_light_6 });

            string directoryPath = "TestAutoLightReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(40, 46, 1);
            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);

            var autolight = new AutolightFlow(input);
            autolight.Configuration.WriteReportMode = FlowReportConfiguration.WriteOnError;
            autolight.ReportFolder = directoryPath;

            // When
            autolight.Execute();

            // Then
            Assert.IsFalse(Directory.Exists(autolight.ReportFolder));
        }

        [TestMethod]
        public void Report_of_image_is_working()
        {
            // Given
            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>()))
                .Returns(1)
                .Returns(0.99)
                .Returns(0.9)
                .Returns(0.8)
                .Returns(0.7);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(0.1);

            string directoryPath = "TestAutoLightReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(46, 50, 1);
            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);
            var autolight = new AutolightFlow(input, _simulatedImageOperatorsLib.Object);
            autolight.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            autolight.ReportFolder = directoryPath;

            // When
            autolight.Execute();

            // Then
            Assert.IsTrue(File.Exists(Path.Combine(autolight.ReportFolder, $"image_at_optimal_light_power_{46}_csharp.png")));

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public void Report_of_signal_is_working()
        {
            // Given
            _simulatedImageOperatorsLib.SetupSequence(_ => _.ComputeContrastMeasure(It.IsAny<ServiceImage>()))
                .Returns(1)
                .Returns(0.99)
                .Returns(0.9)
                .Returns(0.8)
                .Returns(0.7);
            _simulatedImageOperatorsLib.Setup(_ => _.ComputeSaturationMeasure(It.IsAny<ServiceImage>())).Returns(0.1);

            string directoryPath = "TestAutoLightReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAutolightInput;
            input.LightPower = new ScanRangeWithStep(46, 50, 1);
            double currentIntensity = 0.0;
            SimulatedLight.Setup(x => x.SetIntensity(It.IsAny<double>())).Callback<double>(x => currentIntensity = x);
            SimulatedLight.Setup(x => x.GetIntensity()).Returns(() => currentIntensity);

            var autolight = new AutolightFlow(input, _simulatedImageOperatorsLib.Object);
            autolight.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            autolight.ReportFolder = directoryPath;

            // When
            autolight.Execute();

            // Then
            var csvFilename = Path.Combine(autolight.ReportFolder, $"contrast_function_of_light_power_csharp.csv");
            Assert.IsTrue(File.Exists(csvFilename));

            using (var reader = new StreamReader(csvFilename))
            {
                string separator = CSVStringBuilder.GetCSVSeparator();
                var cSep = new char[] { separator[0] };

                var line = reader.ReadLine();
                var values = line.Split(cSep);

                Assert.AreEqual("Light power", values[0]);
                Assert.AreEqual("Contrast", values[1]);

                List<string> lightPower = new List<string>();
                List<string> contrast = new List<string>();

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    values = line.Split(cSep);

                    lightPower.Add(values[0]);
                    contrast.Add(values[1]);
                }

                Assert.AreEqual(5, lightPower.Count);
                Assert.AreEqual(5, contrast.Count);

                Assert.AreEqual(46, Convert.ToDouble(lightPower[0], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(47, Convert.ToDouble(lightPower[1], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(48, Convert.ToDouble(lightPower[2], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(49, Convert.ToDouble(lightPower[3], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(50, Convert.ToDouble(lightPower[4], System.Globalization.CultureInfo.InvariantCulture), 1e-10);

                Assert.AreEqual(1, Convert.ToDouble(contrast[0], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(0.99, Convert.ToDouble(contrast[1], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(0.9, Convert.ToDouble(contrast[2], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(0.8, Convert.ToDouble(contrast[3], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
                Assert.AreEqual(0.7, Convert.ToDouble(contrast[4], System.Globalization.CultureInfo.InvariantCulture), 1e-10);
            }

            Directory.Delete(directoryPath, true);
        }
    }
}
