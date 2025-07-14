using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class WaferMapTest : TestWithMockedHardware<WaferMapTest>, ITestWithAxes
    {
        private WaferMapInput _defaultWaferMapInput;

        #region Interfaces properties

        public Mock<IAxes> SimulatedAxes { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            _defaultWaferMapInput = SimulatedData.ValidWaferMapInput();
        }

        [TestMethod]
        public void Wafer_map_flow_nominal_case()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Specific die grid
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 145.Millimeters();
            die.StreetWidth = 20.Millimeters();
            die.StreetHeight = 5.Millimeters();
            die.DieAngle = 0.Degrees();
            input.DieDimensions = die;

            //Given: Specific top left corner position
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -120, 0, 0, 0);

            //Given: Specific edge exclusion
            input.EdgeExclusion = 10.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(-220, result.DieGridTopLeft.X, 10e-10);
            Assert.AreEqual(150, result.DieGridTopLeft.Y, 10e-10);
            Assert.AreEqual(4, result.NbColumns);
            Assert.AreEqual(2, result.NbRows);

            var diesPresence = new bool[2, 4] {
                { false /*col0 - row1*/, false /*col1 - row1*/, false /*col2 - row1*/, false /*col3 - row1*/},
                { false /*col0 - row0*/, false /*col1 - row0*/, false /*col2 - row0*/, false /*col3 - row0*/},};
            var diesPresenceExpected = new Matrix<bool>(diesPresence);
            Assert.IsTrue(diesPresenceExpected.Equals(result.DiesPresence));
        }

        [TestMethod]
        public void Die_grid_top_left_position_is_correct_when_known_die_corner_position_is_in_top_left_dial()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Specific die grid
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 95.Millimeters();
            die.StreetWidth = 15.Millimeters();
            die.StreetHeight = 5.Millimeters();
            die.DieAngle = 0.Degrees();
            input.DieDimensions = die;

            //Given: Known die corner position is in top left dial of wafer
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -10, 50, 0, 0);

            //Given: Specific edge exclusion
            input.EdgeExclusion = 20.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(-200, result.DieGridTopLeft.X, 10e-10);
            Assert.AreEqual(150, result.DieGridTopLeft.Y, 10e-10);
        }

        [TestMethod]
        public void Die_grid_top_left_position_is_correct_when_known_die_corner_position_is_in_bottom_right_dial()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Specific die grid
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 95.Millimeters();
            die.StreetWidth = 15.Millimeters();
            die.StreetHeight = 5.Millimeters();
            die.DieAngle = 0.Degrees();
            input.DieDimensions = die;

            //Given: Known die corner position is in bottom right dial of wafer
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 16, -20, 0, 0);

            //Given: Specific edge exclusion
            input.EdgeExclusion = 20.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(-174, result.DieGridTopLeft.X, 10e-10);
            Assert.AreEqual(180, result.DieGridTopLeft.Y, 10e-10);
        }

        [TestMethod]
        public void Die_grid_top_left_position_is_correct_when_grid_angle_is_zero()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Die grid not aligned on wafer
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 11.Millimeters();
            die.DieHeight = 9.Millimeters();
            die.StreetWidth = 0.Millimeters();
            die.StreetHeight = 0.Millimeters();
            die.DieAngle = 0.Degrees();
            input.DieDimensions = die;

            //Given: Known die corner position at center of wafer
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -100, 85, 0, 0);

            //Given: Specific edge exclusion
            input.EdgeExclusion = 1.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(-155, result.DieGridTopLeft.X, 10e-10);
            Assert.AreEqual(157, result.DieGridTopLeft.Y, 10e-10);
        }

        [TestMethod]
        public void Die_grid_top_left_position_is_correct_when_grid_angle_is_positive()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 600.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Die grid angle is positive
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 11.Millimeters();
            die.DieHeight = 9.Millimeters();
            die.StreetWidth = 0.Millimeters();
            die.StreetHeight = 0.Millimeters();
            die.DieAngle = 90.Degrees(); //counterclockwise
            input.DieDimensions = die;

            //Given: Known die corner position at center of wafer
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -232, 185, 0, 0);

            //Given: Specific edge exclusion
            input.EdgeExclusion = 1.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(-304, result.DieGridTopLeft.X, 10e-10);
            Assert.AreEqual(-310, result.DieGridTopLeft.Y, 10e-10);
        }

        [TestMethod]
        public void Die_grid_top_left_position_is_correct_when_grid_angle_is_negative()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 600.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Die grid angle is negative
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 11.Millimeters();
            die.DieHeight = 9.Millimeters();
            die.StreetWidth = 0.Millimeters();
            die.StreetHeight = 0.Millimeters();
            die.DieAngle = -90.Degrees(); //clockwise
            input.DieDimensions = die;

            //Given: Known die corner position at center of wafer
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), 0, 0, 0, 0);

            //Given: Specific edge exclusion
            input.EdgeExclusion = 0.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(306, result.DieGridTopLeft.X, 10e-10);
            Assert.AreEqual(308, result.DieGridTopLeft.Y, 10e-10);
        }

        [TestMethod]
        public void Die_grid_size_is_correct_when_angle_is_zero()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Die grid is aligned on wafer
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 95.Millimeters();
            die.StreetWidth = 5.Millimeters();
            die.StreetHeight = 15.Millimeters();
            die.DieAngle = 0.Degrees();
            input.DieDimensions = die;

            //Given: Known die corner position
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -10, 50, 0, 0);

            //Given: No edge exclusion
            input.EdgeExclusion = 0.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(4, result.NbColumns);
            Assert.AreEqual(3, result.NbRows);
        }

        [TestMethod]
        public void Die_grid_size_is_correct_when_angle_is_not_zero()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Die grid is not aligned on wafer
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 95.Millimeters();
            die.StreetWidth = 30.Millimeters();
            die.StreetHeight = 5.Millimeters();
            die.DieAngle = 90.Degrees(); //counterclockwise
            input.DieDimensions = die;

            //Given: Known die corner position
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -10, 50, 0, 0);

            //Given: No edge exclusion
            input.EdgeExclusion = 0.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(3, result.NbColumns);
            Assert.AreEqual(4, result.NbRows);
        }

        [TestMethod]
        public void Die_grid_presence_is_correct_when_angle_is_zero()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();

            input.WaferCharacteristics = wafer;

            //Given: Die grid aligned on wafer
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 95.Millimeters();
            die.StreetWidth = 20.Millimeters();
            die.StreetHeight = 5.Millimeters();
            die.DieAngle = 0.Degrees();
            input.DieDimensions = die;

            //Given: Known die corner position
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -30, 0, 0, 0);

            //Given: No edge exclusion
            input.EdgeExclusion = 0.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            var diesPresence = new bool[4, 4] {
                { false /*col0 - row3*/, false /*col1 - row3*/, false /*col2 - row3*/, false /*col3 - row3*/},
                { false /*col0 - row2*/, false /*col1 - row2*/, true /*col2 - row2*/, false /*col3 - row2*/},
                { false /*col0 - row1*/, false /*col1 - row1*/, true /*col2 - row1*/, false /*col3 - row1*/},
                { false /*col0 - row0*/, false /*col1 - row0*/, false /*col2 - row0*/, false /*col3 - row0*/},};
            var diesPresenceExpected = new Matrix<bool>(diesPresence);
            Assert.IsTrue(diesPresenceExpected.Equals(result.DiesPresence));
        }

        [TestMethod]
        public void Die_grid_presence_is_correct_when_angle_is_not_zero()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Die grid not aligned on wafer
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 95.Millimeters();
            die.StreetWidth = 20.Millimeters();
            die.StreetHeight = 5.Millimeters();
            die.DieAngle = 90.Degrees(); //counterclockwise
            input.DieDimensions = die;

            //Given: Known die corner position
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -30, 0, 0, 0);

            //Given: No edge exclusion
            input.EdgeExclusion = 0.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            var diesPresence = new bool[4, 4] {
                { false /*col0 - row3*/, false /*col1 - row3*/, false /*col2 - row3*/, false /*col3 - row3*/},
                { false /*col0 - row2*/, false /*col1 - row2*/, false /*col2 - row2*/, false /*col3 - row2*/},
                { false /*col0 - row1*/, true /*col1 - row1*/, true /*col2 - row1*/, false /*col3 - row1*/},
                { false /*col0 - row0*/, false /*col1 - row0*/, false /*col2 - row0*/, false /*col3 - row0*/},};
            var diesPresenceExpected = new Matrix<bool>(diesPresence);
            Assert.IsTrue(diesPresenceExpected.Equals(result.DiesPresence));
        }

        [TestMethod]
        public void Known_die_corner_position_must_be_inside_zone_bounded_by_the_exclusion_zone()
        {
            var input = _defaultWaferMapInput;

            //Given: Default wafer (circular without flat or notch)
            var wafer = new WaferDimensionalCharacteristic();
            wafer.WaferShape = WaferShape.NonFlat;
            wafer.Diameter = 300.Millimeters();
            input.WaferCharacteristics = wafer;

            //Given: Die grid not aligned on wafer
            var die = new DieDimensionalCharacteristic();
            die.DieWidth = 80.Millimeters();
            die.DieHeight = 95.Millimeters();
            die.StreetWidth = 20.Millimeters();
            die.StreetHeight = 5.Millimeters();
            die.DieAngle = 90.Degrees(); //counterclockwise
            input.DieDimensions = die;

            //Given: Known die corner position is outside area bounded by the exclusion zone
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -120, 0, 0, 0);

            //Given: Edge exclusion
            input.EdgeExclusion = 35.Millimeters();

            // When
            var waferMap = new WaferMapFlow(input);
            var result = waferMap.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestWaferMapReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultWaferMapInput;

            var flow = new WaferMapFlow(input);
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
            string directoryPath = "TestWaferMapReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultWaferMapInput;

            var flow = new WaferMapFlow(input);
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
        public void Report_of_die_presence_is_working()
        {
            //Given
            string directoryPath = "TestWaferMapReport";
            Directory.CreateDirectory(directoryPath);

            var input = _defaultWaferMapInput;
            input.TopLeftCorner.Position = new XYZTopZBottomPosition(new WaferReferential(), -30, 0, 0, 0);
            input.WaferCharacteristics.Diameter = 300.Millimeters();
            input.DieDimensions.DieWidth = 80.Millimeters();
            input.DieDimensions.DieHeight = 95.Millimeters();
            input.DieDimensions.StreetWidth = 20.Millimeters();
            input.DieDimensions.StreetHeight = 5.Millimeters();
            input.DieDimensions.DieAngle = 90.Degrees(); //counterclockwise
            input.EdgeExclusion = 0.Millimeters();

            var flow = new WaferMapFlow(input);
            flow.Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;
            flow.ReportFolder = directoryPath;

            //When: Run autofocus
            flow.Execute();

            //Then
            var filename = Path.Combine(flow.ReportFolder, $"diesPresenceArray.csv");
            Assert.IsTrue(File.Exists(filename));

            Directory.Delete(directoryPath, true);
        }
    }
}
