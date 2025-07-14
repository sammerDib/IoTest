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
    public class EllipseCriticalDimensionTest : TestWithMockedHardware<EllipseCriticalDimensionTest>, ITestWithCamera
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
        public void Ellipse_Critical_dimension_flow_nominal_case()
        {
            // Given
            ServiceImage img = new ServiceImage();
            img.LoadFromFile(GetDefaultDataPath("63_circles_of_70_pixels_in_diameter.png"));
            CenteredRegionOfInterest roi = null;

            Length expectedDiameter = (70 * PixelSizeX.Millimeters).Millimeters();
            Length lengthTolerance = 10.Millimeters();
            Length widthTolerance = 10.Millimeters();

            var input = new EllipseCriticalDimensionInput(img, ObjectiveUpId, roi, expectedDiameter, expectedDiameter, lengthTolerance, widthTolerance);

            // When
            var flow = new EllipseCriticalDimensionFlow(input);
            var result = flow.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(expectedDiameter.Millimeters, result.Length.Millimeters, 0.5);
            Assert.AreEqual(expectedDiameter.Millimeters, result.Width.Millimeters, 0.5);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestCriticalDimensionReport";
            Directory.CreateDirectory(directoryPath);

            var input = SimulatedData.ValidEllipseCriticalDimensionInput();

            var flow = new EllipseCriticalDimensionFlow(input);
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
            string directoryPath = "TestCriticalDimensionReport";
            Directory.CreateDirectory(directoryPath);

            var input = SimulatedData.ValidEllipseCriticalDimensionInput();

            var flow = new EllipseCriticalDimensionFlow(input);
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
            string directoryPath = "TestCriticalDimensionReport";
            Directory.CreateDirectory(directoryPath);

            ServiceImage img = new ServiceImage();
            img.LoadFromFile(GetDefaultDataPath("63_circles_of_70_pixels_in_diameter.png"));
            CenteredRegionOfInterest roi = null;
            Length expectedDiameter = (70 * PixelSizeX.Millimeters).Millimeters();
            Length lengthTolerance = 10.Millimeters();
            Length widthTolerance = 10.Millimeters();
            var input = new EllipseCriticalDimensionInput(img, ObjectiveUpId, roi, expectedDiameter, expectedDiameter, lengthTolerance, widthTolerance);

            var flow = new EllipseCriticalDimensionFlow(input);
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
