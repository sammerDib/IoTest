using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class CheckPatternRecTest : TestWithMockedHardware<CheckPatternRecTest>, ITestWithAxes
    {
        private Mock<PatternRecFlow> _simulatedPatternRecFlow;

        private CheckPatternRecInput _defaultCheckPatternRecInput;

        #region Interfaces properties

        public Mock<IAxes> SimulatedAxes { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            _simulatedPatternRecFlow = new Mock<PatternRecFlow>(new PatternRecInput(null), null, null, null);

            _defaultCheckPatternRecInput = SimulatedData.ValidCheckPatternRecInput();
        }

        [TestMethod]
        public void Check_pattern_recognition_flow_nominal_case()
        {
            // Given: two validation positions with known offset from pattern recognition image position
            var shiftX = 15.Millimeters();
            var shiftY = 25.Millimeters();
            var shiftX2 = 10.Millimeters();
            var shiftY2 = 9.Millimeters();

            var input = _defaultCheckPatternRecInput;
            input.ValidationPositions = new List<XYZTopZBottomPosition>() {
                new XYZTopZBottomPosition(new WaferReferential(), -shiftX.Millimeters, -shiftY.Millimeters, 0, 0),
                new XYZTopZBottomPosition(new WaferReferential(), -shiftX2.Millimeters, -shiftY2.Millimeters, 0, 0)};

            var checkPatternRec = new CheckPatternRecFlow(input, _simulatedPatternRecFlow.Object);

            // Given: pattern recognition flow return right offset for each positions
            var patternRecResultPos1 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftX, shiftY, null);
            var patternRecResultPos2 = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftX2, shiftY2, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultPos1)
                .Returns(patternRecResultPos2);

            // When
            var checkPatternRecResult = checkPatternRec.Execute();

            // Then: flow succeeds, returns a checking success and all sub result are success
            Assert.AreEqual(FlowState.Success, checkPatternRecResult.Status.State);
            Assert.AreEqual(true, checkPatternRecResult.Succeeded);
            Assert.AreEqual(FlowState.Success, checkPatternRecResult.SingleResults[0].Status.State);
            Assert.AreEqual(FlowState.Success, checkPatternRecResult.SingleResults[1].Status.State);
        }

        [TestMethod]
        public void Sub_results_contains_the_difference_between_expected_position_and_corrected_position_instead_of_raw_offset()
        {
            // Given: one validation positions with known offset from pattern recognition image position
            var shiftX = 15.Millimeters();
            var shiftY = 25.Millimeters();

            var input = _defaultCheckPatternRecInput;
            input.ValidationPositions = new List<XYZTopZBottomPosition>() {
                new XYZTopZBottomPosition(new WaferReferential(), -shiftX.Millimeters, -shiftY.Millimeters, 0, 0)};

            var checkPatternRec = new CheckPatternRecFlow(input, _simulatedPatternRecFlow.Object);

            // Given: pattern recognition flow return right offset for each positions
            var patternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftX, shiftY, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute()).Returns(patternRecResult);

            // When
            var checkPatternRecResult = checkPatternRec.Execute();

            // Then
            Assert.AreEqual(0, checkPatternRecResult.SingleResults[0].ShiftX.Millimeters);
            Assert.AreEqual(0, checkPatternRecResult.SingleResults[0].ShiftY.Millimeters);
        }

        [TestMethod]
        public void All_validation_positions_are_checked()
        {
            // Given: three validation positions with known offset from pattern recognition image position
            var shiftX = 15.Millimeters();
            var shiftY = 25.Millimeters();

            var input = _defaultCheckPatternRecInput;
            input.ValidationPositions = new List<XYZTopZBottomPosition>() {
                new XYZTopZBottomPosition(new WaferReferential(), -shiftX.Millimeters, -shiftY.Millimeters, 0, 0),
                new XYZTopZBottomPosition(new WaferReferential(), -shiftX.Millimeters, -shiftY.Millimeters, 0, 0),
                new XYZTopZBottomPosition(new WaferReferential(), -shiftX.Millimeters, -shiftY.Millimeters, 0, 0)};

            var checkPatternRec = new CheckPatternRecFlow(input, _simulatedPatternRecFlow.Object);

            // Given: pattern recognition flow return right offset for first position, bad offset for second and error for the last
            var patternRecResultWithRightShift = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftX, shiftY, null);
            var patternRecResultWithBadShift = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftX * 2, shiftY * 2, null);
            var patternRecResultFailed = new PatternRecResult(new FlowStatus(FlowState.Error), 1, shiftX, shiftY, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultWithRightShift)
                .Returns(patternRecResultWithBadShift)
                .Returns(patternRecResultFailed);

            // When
            var checkPatternRecResult = checkPatternRec.Execute();

            // Then :
            Assert.AreEqual(0, checkPatternRecResult.SingleResults[0].ShiftX.Millimeters);
            Assert.AreEqual(0, checkPatternRecResult.SingleResults[0].ShiftY.Millimeters);
            Assert.AreEqual(FlowState.Success, checkPatternRecResult.SingleResults[0].Status.State);

            Assert.AreEqual(shiftX.Millimeters, checkPatternRecResult.SingleResults[1].ShiftX.Millimeters);
            Assert.AreEqual(shiftY.Millimeters, checkPatternRecResult.SingleResults[1].ShiftY.Millimeters);
            Assert.AreEqual(FlowState.Success, checkPatternRecResult.SingleResults[1].Status.State);

            Assert.AreEqual(0, checkPatternRecResult.SingleResults[2].ShiftX.Millimeters);
            Assert.AreEqual(0, checkPatternRecResult.SingleResults[2].ShiftY.Millimeters);
            Assert.AreEqual(FlowState.Error, checkPatternRecResult.SingleResults[2].Status.State);
        }

        [TestMethod]
        public void Checking_error_is_returned_when_at_least_one_validation_position_fails()
        {
            // Given: three validation positions with known offset from pattern recognition image position
            var shiftX = 15.Millimeters();
            var shiftY = 25.Millimeters();

            var input = _defaultCheckPatternRecInput;
            input.ValidationPositions = new List<XYZTopZBottomPosition>() {
                new XYZTopZBottomPosition(new WaferReferential(), shiftX.Millimeters, shiftY.Millimeters, 0, 0),
                new XYZTopZBottomPosition(new WaferReferential(), shiftX.Millimeters, shiftY.Millimeters, 0, 0),
                new XYZTopZBottomPosition(new WaferReferential(), shiftX.Millimeters, shiftY.Millimeters, 0, 0)};

            var checkPatternRec = new CheckPatternRecFlow(input, _simulatedPatternRecFlow.Object);

            // Given: pattern recognition flow return error for the first position and then success for the others
            var successPatternRecResult = new PatternRecResult(new FlowStatus(FlowState.Success), 1, shiftX, shiftY, null);
            var failsPatternRecResult = new PatternRecResult(new FlowStatus(FlowState.Error), 1, shiftX, shiftY, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(failsPatternRecResult)
                .Returns(successPatternRecResult)
                .Returns(successPatternRecResult);

            // When
            var checkPatternRecResult = checkPatternRec.Execute();

            // Then: flow succeeds but returns a checking fail
            Assert.AreEqual(FlowState.Success, checkPatternRecResult.Status.State);
            Assert.IsFalse(checkPatternRecResult.Succeeded);
        }

        [TestMethod]
        public void Sub_results_contains_quality_score_of_associated_pattern_recognition()
        {
            // Given: three validation positions with associated pattern recognition quality
            double quality1 = 0.9;
            double quality2 = 0.86;
            double quality3 = 0.92;

            var input = _defaultCheckPatternRecInput;
            input.ValidationPositions = new List<XYZTopZBottomPosition>() {
                new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0),
                new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0),
                new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0)};

            var checkPatternRec = new CheckPatternRecFlow(input, _simulatedPatternRecFlow.Object);

            // Given: pattern recognition flow return error for the first position and then success for the others
            var patternRecResult1 = new PatternRecResult(new FlowStatus(FlowState.Success), quality1, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResult2 = new PatternRecResult(new FlowStatus(FlowState.Success), quality2, 0.Millimeters(), 0.Millimeters(), null);
            var patternRecResult3 = new PatternRecResult(new FlowStatus(FlowState.Success), quality3, 0.Millimeters(), 0.Millimeters(), null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResult1)
                .Returns(patternRecResult2)
                .Returns(patternRecResult3);

            // When
            var checkPatternRecResult = checkPatternRec.Execute();

            // Then
            Assert.AreEqual(quality1, checkPatternRecResult.SingleResults[0].Confidence);
            Assert.AreEqual(quality2, checkPatternRecResult.SingleResults[1].Confidence);
            Assert.AreEqual(quality3, checkPatternRecResult.SingleResults[2].Confidence);
        }

        [TestMethod]
        public void Null_shifts_do_not_crash()
        {
            // Given: one position with known offset from pattern recognition image position
            var shiftX = 15.Millimeters();
            var shiftY = 25.Millimeters();

            var input = _defaultCheckPatternRecInput;
            input.ValidationPositions = new List<XYZTopZBottomPosition>() {
                new XYZTopZBottomPosition(new WaferReferential(), -shiftX.Millimeters, -shiftY.Millimeters, 0, 0)};

            var checkPatternRec = new CheckPatternRecFlow(input, _simulatedPatternRecFlow.Object);

            // Given: pattern recognition flow return null shifts
            var patternRecResultFailed = new PatternRecResult(new FlowStatus(FlowState.Error), 1, null, null, null);

            _simulatedPatternRecFlow.SetupSequence(_ => _.Execute())
                .Returns(patternRecResultFailed);

            // When
            var checkPatternRecResult = checkPatternRec.Execute();

            // Then :
            Assert.IsNull(checkPatternRecResult.SingleResults[0].ShiftX);
            Assert.IsNull(checkPatternRecResult.SingleResults[0].ShiftY);
            Assert.AreEqual(FlowState.Error, checkPatternRecResult.SingleResults[0].Status.State);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestCheckPatternRecReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultCheckPatternRecInput;

            var flow = new CheckPatternRecFlow(input);
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
            string directoryPath = "TestCheckPatternRecReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultCheckPatternRecInput;

            var flow = new CheckPatternRecFlow(input);
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
