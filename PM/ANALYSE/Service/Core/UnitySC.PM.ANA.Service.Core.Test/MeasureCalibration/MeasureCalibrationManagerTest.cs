using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.MeasureCalibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.MeasureCalibration
{
    [TestClass]
    public class CalibrationManagerTest : TestWithMockedHardware<CalibrationManagerTest>, ITestWithAxes, ITestWithProbeLise, ITestWithChuck, ITestWithCamera
    {
        #region parameters

        private readonly uint _minuteBetweenTwoDualLiseCalibration = 10;
        private readonly CancellationToken _cancellationToken = new CancellationToken();
        public Mock<IAxes> SimulatedAxes { get; set; }
        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<IProbeDualLise> FakeDualLise { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion parameters

        #region unitTests

        [TestMethod]
        public void CalibrationManager_do_and_save_one_calibration()
        {
            // Given
            SampleAndSignal sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseDown }, this);

            var calibrationManager = new ProbeCalibrationManagerLise("ProbeLiseDouble",_cancellationToken, _minuteBetweenTwoDualLiseCalibration);
            var firstMeasurePoint = new PointPosition(1, 1, 1, 1);

            // When
            calibrationManager.GetCalibration(false,"ProbeLiseDouble",null, firstMeasurePoint); // The first calibration
            var measureTimestamp = SimulateMeasure();

            // Then
            var (calibrationResult1, calibrationResult2) = calibrationManager.GetClosestsCalibrations(measureTimestamp);

            Assert.AreEqual(FlowState.Success, calibrationResult1.Status.State);
            Assert.IsNull(calibrationResult2);
        }

        [TestMethod]
        public void CalibrationManager_do_and_save_multiple_calibrations()
        {
            // Given
            SampleAndSignal sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, sampleAndSignalForCalibration.SignalLiseUp, this);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, sampleAndSignalForCalibration.SignalLiseDown, this);

            var calibrationManager = new ProbeCalibrationManagerLise("ProbeLiseDouble", _cancellationToken, _minuteBetweenTwoDualLiseCalibration);
            var firstMeasurePoint = new PointPosition(1, 1, 1, 1);
            var secondMeasurePoint = new PointPosition(1, 1, 2, 1);
            var thirdMeasurePoint = new PointPosition(1, 1, 3, 1);

            // When
            calibrationManager.GetCalibration(false, "ProbeLiseDouble",null,firstMeasurePoint); // The first calibration
            var measureTimestamp1 = SimulateMeasure();
            calibrationManager.GetCalibration(false, "ProbeLiseDouble", null, secondMeasurePoint); // The last calibration for the previous measure and the first for the next
            var measureTimestamp2 = SimulateMeasure();
            calibrationManager.GetCalibration(false, "ProbeLiseDouble", null, thirdMeasurePoint); // The last calibration for the previous measure and the first for the next
            var measureTimestamp3 = SimulateMeasure();
            calibrationManager.DoLastCalibration(); // The last calibration

            // Then
            var (calibrationResult1, calibrationResult2) = calibrationManager.GetClosestsCalibrations(measureTimestamp1);
            var (calibrationResult3, calibrationResult4) = calibrationManager.GetClosestsCalibrations(measureTimestamp2);
            var (calibrationResult5, calibrationResult6) = calibrationManager.GetClosestsCalibrations(measureTimestamp3);
            Assert.AreEqual(FlowState.Success, calibrationResult1.Status.State);
            Assert.AreEqual(FlowState.Success, calibrationResult2.Status.State);
            Assert.AreEqual(FlowState.Success, calibrationResult3.Status.State);
            Assert.AreEqual(FlowState.Success, calibrationResult4.Status.State);
            Assert.AreEqual(FlowState.Success, calibrationResult5.Status.State);
            Assert.AreEqual(FlowState.Success, calibrationResult6.Status.State);
            Assert.IsTrue(calibrationResult1.Timestamp < calibrationResult2.Timestamp);
            Assert.IsTrue(calibrationResult2.Timestamp <= calibrationResult3.Timestamp); // Could be equal in test because DateTime.UtcNow.Tick isn't all that precise.
            Assert.IsTrue(calibrationResult3.Timestamp <= calibrationResult4.Timestamp); // Could be equal in test because DateTime.UtcNow.Tick isn't all that precise.
            Assert.IsTrue(calibrationResult4.Timestamp <= calibrationResult5.Timestamp); // Could be equal in test because DateTime.UtcNow.Tick isn't all that precise.
            Assert.IsTrue(calibrationResult5.Timestamp < calibrationResult6.Timestamp);
        }

        [TestMethod]
        public void CalibrationManager_do_and_save_one_calibration_for_multiple_measure_with_same_Z()
        {
            // Given
            SampleAndSignal sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            SampleAndSignal sampleAndSignalForLastCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp, sampleAndSignalForLastCalibration.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseDown, sampleAndSignalForLastCalibration.SignalLiseDown }, this);

            var calibrationManager = new ProbeCalibrationManagerLise("ProbeLiseDouble", _cancellationToken, _minuteBetweenTwoDualLiseCalibration);
            var firstMeasurePoint = new PointPosition(1, 1, 1, 1);
            var secondeMeasurePoint = new PointPosition(2, 4, 1, 1);
            var thirdMeasurePoint = new PointPosition(1, 9, 1, 1);

            // When
            calibrationManager.GetCalibration(false, "ProbeLiseDouble", null, firstMeasurePoint); // The first calibration
            var measureTimestamp1 = SimulateMeasure();
            calibrationManager.GetCalibration(false, "ProbeLiseDouble", null, secondeMeasurePoint); // No calibration
            var measureTimestamp2 = SimulateMeasure();
            calibrationManager.GetCalibration(false, "ProbeLiseDouble", null, thirdMeasurePoint); // No calibration
            var measureTimestamp3 = SimulateMeasure();
            calibrationManager.DoLastCalibration(); // The last calibration

            // Then
            var (calibrationResult1, calibrationResult2) = calibrationManager.GetClosestsCalibrations(measureTimestamp1);
            var (calibrationResult3, calibrationResult4) = calibrationManager.GetClosestsCalibrations(measureTimestamp2);
            var (calibrationResult5, calibrationResult6) = calibrationManager.GetClosestsCalibrations(measureTimestamp3);
            Assert.AreEqual(FlowState.Success, calibrationResult1.Status.State);
            Assert.AreEqual(FlowState.Success, calibrationResult2.Status.State);
            Assert.IsTrue(calibrationResult1.Timestamp < calibrationResult2.Timestamp);
            Assert.IsTrue(calibrationResult1.Equals(calibrationResult3) && calibrationResult1.Equals(calibrationResult5));
            Assert.IsTrue(calibrationResult2.Equals(calibrationResult4) && calibrationResult2.Equals(calibrationResult6));
        }

        [TestMethod]
        public void CalibrationManager_get_closests_calibrations_without_any_calibration_saved()
        {
            // Given
            var calibrationManager = new ProbeCalibrationManagerLise("ProbeLiseDouble", _cancellationToken, _minuteBetweenTwoDualLiseCalibration);
            var timestamp = DateTime.UtcNow;

            // When
            var (calibrationResult1, calibrationResult2) = calibrationManager.GetClosestsCalibrations(timestamp);

            // Then
            Assert.IsNull(calibrationResult1);
            Assert.IsNull(calibrationResult2);
        }

        #endregion unitTests

        #region privateMethods

        private DateTime SimulateMeasure()
        {
            // Need to sleep 1ms because DateTime.UtcNow.Tick isn't all that precise.
            Thread.Sleep(1);
            var timestamp = DateTime.UtcNow;
            Thread.Sleep(1);
            return timestamp;
        }

        #endregion privateMethods
    }
}
