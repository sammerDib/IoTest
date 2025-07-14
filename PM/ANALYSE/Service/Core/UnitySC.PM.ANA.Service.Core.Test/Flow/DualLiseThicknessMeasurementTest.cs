using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class DualLiseThicknessMeasurementTest : TestWithMockedHardware<DualLiseThicknessMeasurementTest>, ITestWithAxes, ITestWithProbeLise, ITestWithCamera
    {
        #region Interfaces properties

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
        public UnitySC.Shared.Tools.Units.Length PixelSizeX { get; set; }
        public UnitySC.Shared.Tools.Units.Length PixelSizeY { get; set; }
        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        [TestMethod]
        public void Thickness_measurement_flow_with_dual_lise_nominal_case()
        {
            // Given : Dual probe Lise provides good signals for calibration and measure
            var globalThickness = AirGapUp + AirGapDown + Thickness750;
            var unknownThickness = Thickness750 / 2;
            var layerUpThickness = (Thickness750 - unknownThickness) / 2;
            var layerDownThickness = Thickness750 - unknownThickness - layerUpThickness;

            var layerUp = new ProbeSampleLayer(layerUpThickness, LiseTestUtils.Tolerance, 1.4621);
            var layerDown = new ProbeSampleLayer(layerDownThickness, LiseTestUtils.Tolerance, 1.4621);

            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(layerUp, layerDown, AirGapUp, AirGapDown);
            var sampleUp = new ProbeSample(new List<ProbeSampleLayer> { layerUp }, "Name", "SampleInfo");
            var sampleDown = new ProbeSample(new List<ProbeSampleLayer> { layerDown }, "Name", "SampleInfo");

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseUp, sampleAndSignalForMeasure.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseDown, sampleAndSignalForMeasure.SignalLiseDown }, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);
            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleUp));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleDown));
            var calibrationLiseInput = new CalibrationDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, sampleAndSignalForCalibration.Sample, calibrationPosition);
            var calibrationFlow = new DualLiseCalibrationFlow(calibrationLiseInput);
            var calibrationResult = calibrationFlow.Execute();

            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var measureLiseInput = new MeasureDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, unknownLayer, calibrationResult.CalibResult);
            var measureLise = new DualLiseThicknessMeasurementFlow(measureLiseInput);

            // When : Performs calibration & measure
            var result = measureLise.Execute();

            // Then : Calibration and measure are both correct
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(globalThickness.Micrometers, calibrationResult.CalibResult.GlobalDistance.Micrometers, 10);
            Assert.AreEqual(unknownThickness.Micrometers, result.LayersThickness[1].Thickness.Micrometers, 10);
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_dual_lise_fails_when_we_can_not_measure_unknown_layer()
        {
            // Given : Use correct sample for calibration and invalid sample for measurement
            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer750, AirGapUp, AirGapDown);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseUp, sampleAndSignalForMeasure.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseDown, sampleAndSignalForMeasure.SignalLiseDown }, this);

            // When
            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);

            var measureCalibrationLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleAndSignalForCalibration.Sample));
            var measureCalibrationLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleAndSignalForCalibration.Sample));

            var calibrationLiseInput = new CalibrationDualLiseInput(DualLiseId, measureCalibrationLiseUpInput, measureCalibrationLiseDownInput, sampleAndSignalForCalibration.Sample, calibrationPosition);
            var calibrationFlow = new DualLiseCalibrationFlow(calibrationLiseInput);
            var calibrationResult = calibrationFlow.Execute();

            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleAndSignalForMeasure.Sample));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleAndSignalForMeasure.Sample));

            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var measureLiseInput = new MeasureDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, unknownLayer, calibrationResult.CalibResult);
            var measureLise = new DualLiseThicknessMeasurementFlow(measureLiseInput);
            var result = measureLise.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_dual_lise_fails_when_we_try_to_calibrate_with_bad_sample()
        {
            // Given : Use invalid sample for calibration and correct sample for measurement
            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var fakeSampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer200, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer750, AirGapUp, AirGapDown);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseUp, sampleAndSignalForMeasure.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseDown, sampleAndSignalForMeasure.SignalLiseDown }, this);

            // When
            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);
            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleAndSignalForMeasure.Sample));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleAndSignalForMeasure.Sample));

            var calibrationLiseInput = new CalibrationDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, sampleAndSignalForCalibration.Sample, calibrationPosition);
            var calibrationFlow = new DualLiseCalibrationFlow(calibrationLiseInput);
            var calibrationResult = calibrationFlow.Execute();

            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var measureLiseInput = new MeasureDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, unknownLayer, calibrationResult.CalibResult);
            var measureLise = new DualLiseThicknessMeasurementFlow(measureLiseInput);
            var result = measureLise.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Thickness_measurement_calibration_becomes_valid_when_the_flow_is_executed_successfully()
        {
            // Given : Dual probe Lise provides good signals for calibration and measure
            var globalThickness = AirGapUp + AirGapDown + Thickness750;

            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer750, AirGapUp, AirGapDown);
            var sampleUp = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");
            var sampleDown = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseUp, sampleAndSignalForMeasure.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseDown, sampleAndSignalForMeasure.SignalLiseDown }, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);
            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleUp));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleDown));

            var calibrationLiseInput = new CalibrationDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, sampleAndSignalForCalibration.Sample, calibrationPosition);
            var calibrationFlow = new DualLiseCalibrationFlow(calibrationLiseInput);

            // When : We execute the flow, which succeeds
            var calibrationResult = calibrationFlow.Execute();

            // Then : Calibration is valid
            Assert.AreEqual(FlowState.Success, calibrationResult.Status.State);
            Assert.IsTrue(calibrationFlow.CheckCalibrationValidity());
            Assert.AreEqual(globalThickness.Micrometers, calibrationResult.CalibResult.GlobalDistance.Micrometers, 10);
        }

        [TestMethod]
        public void Thickness_measurement_calibration_becomes_invalid_when_the_z_axes_position_changes()
        {
            // Given : Dual probe Lise provides good signals for calibration and measure
            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer750, AirGapUp, AirGapDown);
            var sampleUp = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");
            var sampleDown = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseUp, sampleAndSignalForMeasure.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseDown, sampleAndSignalForMeasure.SignalLiseDown }, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);
            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleUp));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleDown));

            var calibrationLiseInput = new CalibrationDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, sampleAndSignalForCalibration.Sample, calibrationPosition);
            var calibrationFlow = new DualLiseCalibrationFlow(calibrationLiseInput);
            var calibrationResult = calibrationFlow.Execute();

            // When : We move axes after running the flow
            var axesMovedPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, probesCalibrationPosition.ZTop + 5, probesCalibrationPosition.ZBottom - 5);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(axesMovedPosition);

            // Then : Calibration is invalid
            Assert.AreEqual(FlowState.Success, calibrationResult.Status.State);
            Assert.IsFalse(calibrationFlow.CheckCalibrationValidity());
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_dual_lise_returns_thickness_for_each_layers_even_if_signal_does_not_allow_to_calculate_them_all()
        {
            // Given : Probe LISE return no signal
            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var fakeSampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer200, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer750, AirGapUp, AirGapDown);
            var sampleUp = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");
            var sampleDown = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseUp, fakeSampleAndSignalForMeasure.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForCalibration.SignalLiseDown, fakeSampleAndSignalForMeasure.SignalLiseDown }, this);

            var currentPos = SimulatedAxes.Object.GetPos().ToXYZTopZBottomPosition();
            var probesCalibrationPosition = new XYZTopZBottomPosition(currentPos.Referential, currentPos.X, currentPos.Y, 15, 0);
            SimulatedAxes.Setup(_ => _.GetPos()).Returns(probesCalibrationPosition);

            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);
            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleUp));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleDown));

            var calibrationLiseInput = new CalibrationDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, sampleAndSignalForCalibration.Sample, calibrationPosition);
            var calibrationFlow = new DualLiseCalibrationFlow(calibrationLiseInput);
            var calibrationResult = calibrationFlow.Execute();

            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var measureLiseInput = new MeasureDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, unknownLayer, calibrationResult.CalibResult);
            var measureLise = new DualLiseThicknessMeasurementFlow(measureLiseInput);

            // When : Try to measure a layer with probe LISE
            var result = measureLise.Execute();

            // Then
            Assert.AreEqual(sampleAndSignalForMeasure.Sample.Layers.Count, result.LayersThickness.Count);
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestDualLiseThicknessMeasurementReport";
            Directory.CreateDirectory(directoryPath);

            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer750, AirGapUp, AirGapDown);
            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);
            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleAndSignalForMeasure.Sample));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleAndSignalForMeasure.Sample));

            CalibrationDualLiseFlowResult calibration = new CalibrationDualLiseFlowResult();
            calibration.CalibResult = new ProbeDualLiseCalibResult();
            calibration.CalibResult.GlobalDistance = 750.Micrometers();
            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var input = new MeasureDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, unknownLayer, calibration.CalibResult    );

            var flow = new DualLiseThicknessMeasurementFlow(input);
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
            string directoryPath = "TestDualLiseThicknessMeasurementReport";
            Directory.CreateDirectory(directoryPath);

            var sampleAndSignalForCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, AirGapUp, AirGapDown);
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForTwoLayersSeparatedByOneUnknownLayer(Layer750, Layer750, AirGapUp, AirGapDown);
            var calibrationPosition = new XYPosition(new StageReferential(), 750, 750);
            var measureLiseUpInput = new MeasureLiseInput(new ThicknessLiseInput(LiseUpId, sampleAndSignalForMeasure.Sample));
            var measureLiseDownInput = new MeasureLiseInput(new ThicknessLiseInput(LiseBottomId, sampleAndSignalForMeasure.Sample));

            CalibrationDualLiseFlowResult calibration = new CalibrationDualLiseFlowResult();
            calibration.CalibResult = new ProbeDualLiseCalibResult();
            calibration.CalibResult.GlobalDistance = 750.Micrometers();
            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var input = new MeasureDualLiseInput(DualLiseId, measureLiseUpInput, measureLiseDownInput, unknownLayer, calibration.CalibResult); ;

            var flow = new DualLiseThicknessMeasurementFlow(input);
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
