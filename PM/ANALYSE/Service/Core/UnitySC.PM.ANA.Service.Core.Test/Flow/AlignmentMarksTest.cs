using System.IO;
using System.IO.Packaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.AlignmentMarks;
using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class AlignmentMarksTest : TestWithMockedHardware<AlignmentMarksTest>, ITestWithAxes
    {
        private Mock<PatternRecFlow> _simulatedPatternRecFlow;

        private AlignmentMarksInput _defaultAlignmentMarksInput;

        #region Interfaces properties

        public Mock<IAxes> SimulatedAxes { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            _simulatedPatternRecFlow = new Mock<PatternRecFlow>(new PatternRecInput(null), null, null, null);

            _defaultAlignmentMarksInput = SimulatedData.ValidAlignmentMarksInput();

            // Use consistent values for an alignment marks
            _defaultAlignmentMarksInput.Site1Images[0].Position.X = -145;
            _defaultAlignmentMarksInput.Site1Images[0].Position.Y = 0;
            _defaultAlignmentMarksInput.Site2Images[0].Position.X = 145;
            _defaultAlignmentMarksInput.Site2Images[0].Position.Y = 0;
        }

        [TestMethod]
        public void Alignment_mark_flow_nominal_case()
        {
            // Given result for patternRec on site 1 image : Actual mark pos = (-144.496, -0.012)
            var shiftXSite1 = (-0.004 - 0.5).Millimeters(); // shift for angle - shift for X
            var shiftYSite1 = (1.012 - 1).Millimeters(); // shift for angle - shift for Y
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (145.496, 2.012)
            var shiftXSite2 = (0.004 - 0.5).Millimeters();
            var shiftYSite2 = (-1.012 - 1).Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = -0.5;
            var ExpectedShiftY = -1;
            var ExpectedAngle = -0.4.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Correct_angle_and_shift()
        {
            // Given result for patternRec on site 1 image : Actual mark pos = (-145, 0)
            var shiftXSite1 = 0.Millimeters();
            var shiftYSite1 = 0.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (145, 0)
            var shiftXSite2 = 0.Millimeters();
            var shiftYSite2 = 0.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = 0;
            var ExpectedShiftY = 0;
            var ExpectedAngle = 0.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_negative_angle()
        {
            // Given result for patternRec on site 1 image : Actual mark pos = (-144.996, -1.012)
            var shiftXSite1 = -0.004.Millimeters();
            var shiftYSite1 = 1.012.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (144.996, 1.012)
            var shiftXSite2 = 0.004.Millimeters();
            var shiftYSite2 = -1.012.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = 0;
            var ExpectedShiftY = 0;
            var ExpectedAngle = -0.4.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_positive_angle()
        {
            // Given result for patternRec on site 1 image : Actual mark pos = (-144.996, 1.012)
            var shiftXSite1 = -0.004.Millimeters();
            var shiftYSite1 = -1.012.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (144.996, -1.012)
            var shiftXSite2 = 0.004.Millimeters();
            var shiftYSite2 = 1.012.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = 0;
            var ExpectedShiftY = 0;
            var ExpectedAngle = 0.4.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_shiftX_and_shiftY()
        {
            // Given result for patternRec on site 1 image : Actual mark pos = (-144.5, -1)
            var shiftXSite1 = -0.5.Millimeters();
            var shiftYSite1 = 1.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (145.5, -1)
            var shiftXSite2 = -0.5.Millimeters();
            var shiftYSite2 = 1.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = -0.5;
            var ExpectedShiftY = 1;
            var ExpectedAngle = 0.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_negative_angle_with_aligned_marks_with_non_zero_Y()
        {
            // Use consistent values for an alignment marks
            _defaultAlignmentMarksInput.Site1Images[0].Position.Y = 5;
            _defaultAlignmentMarksInput.Site2Images[0].Position.Y = 5;

            // Given result for patternRec on site 1 image : Actual mark pos = (-145.0381, 3.7345)
            var shiftXSite1 = 0.0381.Millimeters();
            var shiftYSite1 = 1.2655.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (144.9508, 6.2651)
            var shiftXSite2 = 0.0492.Millimeters();
            var shiftYSite2 = -1.2651.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = 0;
            var ExpectedShiftY = 0;
            var ExpectedAngle = -0.5.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_shiftX_and_shiftY_with_aligned_marks_with_non_zero_Y()
        {
            // Use consistent values for an alignment marks
            _defaultAlignmentMarksInput.Site1Images[0].Position.Y = 5;
            _defaultAlignmentMarksInput.Site2Images[0].Position.Y = 5;

            // Given result for patternRec on site 1 image : Actual mark pos = (-144.5, 4)
            var shiftXSite1 = -0.5.Millimeters();
            var shiftYSite1 = 1.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (145.5, 4)
            var shiftXSite2 = -0.5.Millimeters();
            var shiftYSite2 = 1.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = -0.5;
            var ExpectedShiftY = 1;
            var ExpectedAngle = 0.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_negative_angle_and_shiftX_and_shiftY_with_aligned_marks_with_non_zero_Y()
        {
            // Use consistent values for an alignment marks
            _defaultAlignmentMarksInput.Site1Images[0].Position.Y = 5;
            _defaultAlignmentMarksInput.Site2Images[0].Position.Y = 5;

            // Given result for patternRec on site 1 image : Actual mark pos = (-145.6381, 3.5345)
            var shiftXSite1 = (0.0381 + 0.6).Millimeters(); // shift for angle - shift for X
            var shiftYSite1 = (1.2655 - 0.2).Millimeters(); // shift for angle - shift for y
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (144.3508, 6.6051)
            var shiftXSite2 = (0.0492 + 0.6).Millimeters();
            var shiftYSite2 = (-1.2651 - 0.2).Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = 0.6;
            var ExpectedShiftY = -0.2;
            var ExpectedAngle = -0.5.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_negative_angle_with_asymetrical_aligned_marks()
        {
            // Use consistent values for an alignment marks
            _defaultAlignmentMarksInput.Site1Images[0].Position.X = -145;
            _defaultAlignmentMarksInput.Site1Images[0].Position.Y = 5;
            _defaultAlignmentMarksInput.Site2Images[0].Position.X = 120;
            _defaultAlignmentMarksInput.Site2Images[0].Position.Y = -30;

            // Given result for patternRec on site 1 image : Actual mark pos = (-145.0381, 3.7345)
            var shiftXSite1 = 0.0381.Millimeters();
            var shiftYSite1 = 1.2655.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (120.2572, -28.9517)
            var shiftXSite2 = -0.2572.Millimeters();
            var shiftYSite2 = -1.0483.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = 0;
            var ExpectedShiftY = 0;
            var ExpectedAngle = -0.5.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_shiftX_and_shiftY_with_asymetrical_aligned_marks()
        {
            // Use consistent values for an alignment marks
            _defaultAlignmentMarksInput.Site1Images[0].Position.X = -145;
            _defaultAlignmentMarksInput.Site1Images[0].Position.Y = 5;
            _defaultAlignmentMarksInput.Site2Images[0].Position.X = 120;
            _defaultAlignmentMarksInput.Site2Images[0].Position.Y = -30;

            // Given result for patternRec on site 1 image : Actual mark pos = (-144.5, 4)
            var shiftXSite1 = -0.5.Millimeters();
            var shiftYSite1 = 1.Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (120.5, -31)
            var shiftXSite2 = -0.5.Millimeters();
            var shiftYSite2 = 1.Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = -0.5;
            var ExpectedShiftY = 1;
            var ExpectedAngle = 0.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Incorrect_negative_angle_and_shiftX_and_shiftY_with_asymetrical_aligned_marks()
        {
            // Use consistent values for an alignment marks
            _defaultAlignmentMarksInput.Site1Images[0].Position.X = -145;
            _defaultAlignmentMarksInput.Site1Images[0].Position.Y = 5;
            _defaultAlignmentMarksInput.Site2Images[0].Position.X = 120;
            _defaultAlignmentMarksInput.Site2Images[0].Position.Y = -30;

            // Given result for patternRec on site 1 image : Actual mark pos = (-146,0381, 4.4345)
            var shiftXSite1 = (0.0381 + 1).Millimeters();
            var shiftYSite1 = (1.2655 - 0.7).Millimeters();
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite1, shiftYSite1, null);

            // Given result for patternRec on site 2 image : Actual mark pos = (119.2572, -28.2517)
            var shiftXSite2 = (-0.2572 + 1).Millimeters();
            var shiftYSite2 = (-1.0483 - 0.7).Millimeters();
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftXSite2, shiftYSite2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            var ExpectedShiftX = 1;
            var ExpectedShiftY = -0.7;
            var ExpectedAngle = -0.5.Degrees();

            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(ExpectedShiftX, result.ShiftX.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedShiftY, result.ShiftY.Millimeters, 1e-1);
            Assert.AreEqual(ExpectedAngle.Degrees, result.RotationAngle.Degrees, 1e-1);
        }

        [TestMethod]
        public void Alignment_mark_flow_fails_if_site1_pattern_rec_fails()
        {
            // Given result for patternRec on site 1 image fails
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Error), 1, 1.Millimeters(), 1.Millimeters(), null);

            // Given result for patternRec on site 2 image
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 1.Millimeters(), 1.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Alignment_mark_flow_fails_if_site2_pattern_rec_fails()
        {
            // Given result for patternRec on site 1 image fails
            var simulatedPatternRecResultSite1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, 1.Millimeters(), 1.Millimeters(), null);

            // Given result for patternRec on site 2 image
            var simulatedPatternRecResultSite2 = new PatternRecResult(new FlowStatus(FlowState.Error), 1, 1.Millimeters(), 1.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultSite1)
                .Returns(simulatedPatternRecResultSite2);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Quality_score_is_mean_of_all_pattern_recognition_qualities()
        {
            // Given
            double qualityForShiftComputation = 0.9;
            double qualityForAngleComputation = 0.8;

            var simulatedPatternRecResultForShiftComputation = new PatternRecResult(new FlowStatus(FlowState.Success), qualityForShiftComputation, 1.Millimeters(), 1.Millimeters(), null);
            var simulatedPatternRecResultForAngleComputation = new PatternRecResult(new FlowStatus(FlowState.Success), qualityForAngleComputation, 1.Millimeters(), 1.Millimeters(), null);
            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(simulatedPatternRecResultForShiftComputation)
                .Returns(simulatedPatternRecResultForAngleComputation);

            var alignmentMarks = new AlignmentMarksFlow(_defaultAlignmentMarksInput, _simulatedPatternRecFlow.Object);

            // When
            var result = alignmentMarks.Execute();

            // Then
            double expectedQuality = (qualityForShiftComputation + qualityForAngleComputation) / 2;
            Assert.AreEqual(expectedQuality, result.Confidence);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestAlignmentMarksReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAlignmentMarksInput;

            var flow = new AlignmentMarksFlow(input);
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
            string directoryPath = "TestAlignmentMarksReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultAlignmentMarksInput;
            _simulatedPatternRecFlow.Setup(_ => _.Execute()).Returns(new PatternRecResult(new FlowStatus(FlowState.Success), 1, 0.Centimeters(), 0.Centimeters(), null));

            var flow = new AlignmentMarksFlow(input, _simulatedPatternRecFlow.Object);
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
    }
}
