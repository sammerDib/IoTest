using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.CD;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.CameraTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class CircleCriticalDimensionTest : TestWithMockedHardware<CircleCriticalDimensionTest>, ITestWithCamera
    {
        #region Interfaces properties

        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }

        #endregion Interfaces properties

        [TestMethod]
        public void Circle_Critical_dimension_flow_nominal_case()
        {
            // Given
            string directoryPath = "TestCriticalDimensionReport";
            Directory.CreateDirectory(directoryPath);

            ServiceImage img = new ServiceImage();
            img.LoadFromFile(GetDefaultDataPath("63_circles_of_70_pixels_in_diameter.png"));
            CenteredRegionOfInterest roi = null;

            Length expectedDiameter = 142.Micrometers();
            Length diameterTolerance = 5.Micrometers();

            var input = new CircleCriticalDimensionInput(img, ObjectiveUpId, roi, expectedDiameter, diameterTolerance);

            // When
            var flow = new CircleCriticalDimensionFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(expectedDiameter.Millimeters, result.Diameter.Millimeters, 0.5);

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public void Circle_Critical_dimension_flow_samsung_case()
        {
            // Given
            string directoryPath = "TestCriticalDimensionOnSamsungWafer";
            Directory.CreateDirectory(directoryPath);

            ServiceImage img = new ServiceImage();
            img.LoadFromFile(GetDefaultDataPath("CD_samsung_wafer_case.png"));
            CenteredRegionOfInterest roi = null;

            Length realPixelSizeX = 0.2167.Micrometers();
            Length realExpectedDiameter = 6.0.Micrometers();
            Length expectedDiameterForTestPurpose = ((realExpectedDiameter.Millimeters / realPixelSizeX.Millimeters) * PixelSizeX.Millimeters).Millimeters();
            Length diameterTolerance = (1.Micrometers() / realPixelSizeX.Millimeters) * PixelSizeX.Millimeters;

            var input = new CircleCriticalDimensionInput(img, ObjectiveUpId, roi, expectedDiameterForTestPurpose, diameterTolerance);

            // When
            var flow = new CircleCriticalDimensionFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(expectedDiameterForTestPurpose.Millimeters, result.Diameter.Millimeters, 0.5);

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public void Circle_Critical_dimension_flow_noisy_bottom_case()
        {
            // Given
            string directoryPath = "TestCriticalDimensionOnNoisyWafer";
            Directory.CreateDirectory(directoryPath);

            ServiceImage img = new ServiceImage();
            img.LoadFromFile(GetDefaultDataPath("CD_noisy_wafer_case.png"));
            var imgWidth = 1164 * 0.2167.Micrometers();
            var hauteur = 872 * 0.2167.Micrometers();
            CenteredRegionOfInterest roi = new CenteredRegionOfInterest(50.Micrometers(), 50.Micrometers(), (imgWidth.Micrometers - 25).Micrometers(), (hauteur.Micrometers - 25).Micrometers());

            Length realPixelSizeX = 0.2167.Micrometers();
            Length realExpectedDiameter = 4.7.Micrometers();
            Length expectedDiameterForTestPurpose = ((realExpectedDiameter.Millimeters / realPixelSizeX.Millimeters) * PixelSizeX.Millimeters).Millimeters();
            Length diameterTolerance = (1.Micrometers() / realPixelSizeX.Millimeters) * PixelSizeX.Millimeters;

            var input = new CircleCriticalDimensionInput(img, ObjectiveUpId, null, expectedDiameterForTestPurpose, diameterTolerance);

            // When
            var flow = new CircleCriticalDimensionFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(expectedDiameterForTestPurpose.Millimeters, result.Diameter.Millimeters, 0.5);

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestCircleCriticalDimensionReport";
            Directory.CreateDirectory(directoryPath);

            var input = SimulatedData.ValidCircleCriticalDimensionInput();

            var flow = new CircleCriticalDimensionFlow(input);
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
        public void Report_of_result_is_working()
        {
            //Given
            string directoryPath = "TestCircleCriticalDimensionReport";
            Directory.CreateDirectory(directoryPath);

            var input = SimulatedData.ValidCircleCriticalDimensionInput();

            var flow = new CircleCriticalDimensionFlow(input);
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
        public void Report_of_images_is_working()
        {
            //Given
            string directoryPath = "TestCircleCriticalDimensionReport";
            Directory.CreateDirectory(directoryPath);

            ServiceImage img = new ServiceImage();
            img.LoadFromFile(GetDefaultDataPath("63_circles_of_70_pixels_in_diameter.png"));
            CenteredRegionOfInterest roi = null;
            Length expectedDiameter = (70 * PixelSizeX.Millimeters).Millimeters();
            Length diameterTolerance = 10.Millimeters();
            var input = new CircleCriticalDimensionInput(img, ObjectiveUpId, roi, expectedDiameter, diameterTolerance);

            var flow = new CircleCriticalDimensionFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var filenameImg1 = Path.Combine(flow.ReportFolder, $"CD_initialImage_csharp.png");
            var filenameImg2 = Path.Combine(flow.ReportFolder, $"CD_controlImage_csharp.png");
            Assert.IsTrue(File.Exists(filenameImg1));
            Assert.IsTrue(File.Exists(filenameImg2));

            Directory.Delete(directoryPath, true);
        }
    }
}
