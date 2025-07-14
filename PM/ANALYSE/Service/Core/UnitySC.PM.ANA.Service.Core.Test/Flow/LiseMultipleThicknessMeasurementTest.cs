using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;
using System.IO;
using Moq;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.ANA.Hardware.Probe.Lise;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class LiseMultipleThicknessMeasurementTest : TestWithMockedHardware<LiseMultipleThicknessMeasurementTest>, ITestWithProbeLise, ITestWithCamera
    {
        #region Interfaces properties

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
        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        [TestMethod]
        public void
        Multiple_thickness_measurement_flow_with_lise_nominal_case()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 and 200 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);

                // When : Try to measure two layers of 750 and 200 micrometers, four times
                int measureNb = 4;
                var measureLiseInput = new MultipleMeasuresLiseInput(new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), measureNb);
                var measureLise = new LiseMultipleThicknessMeasurementFlow(measureLiseInput);
                var result = measureLise.Execute();

                // Then : Multiple measurement flow succeeds
                Assert.AreEqual(FlowState.Success, result.Status.State);
                Assert.AreEqual(measureNb, result.ProbeThicknessMeasures.Count);
                foreach (var measure in result.ProbeThicknessMeasures)
                {
                    Assert.AreEqual(2, measure.Count);
                    Assert.AreEqual(Thickness750.Micrometers, measure[0].Thickness.Micrometers, 1);
                    Assert.AreEqual(Thickness200.Micrometers, measure[1].Thickness.Micrometers, 1);
                }
            }
        }

        [TestMethod]
        public void Multiple_thickness_measurement_flow_with_lise_fails_if_mandatory_layers_cannot_be_measured()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns two time a raw signal corresponding to two layers of 750 micrometers and two others time to two layers of 750 and 200 micrometers
                var signal750and750Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                var signal750Layer = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(probeId, new List<IProbeSignal> { signal750and750Layers, signal750and750Layers, signal750Layer, signal750Layer }, this);

                // When : Try to measure two layers of 750 and 200 micrometers, four times
                int measureNb = 4;
                var measureLiseInput = new MultipleMeasuresLiseInput(new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), measureNb);
                var measureLise = new LiseMultipleThicknessMeasurementFlow(measureLiseInput);
                var result = measureLise.Execute();

                // Then : Multiple measurement flow fails if some mandatory measures failed
                Assert.AreEqual(FlowState.Error, result.Status.State);
            }
        }

        [TestMethod]
        public void Multiple_thickness_measurement_flow_with_lise_success_if_only_not_mandatory_layers_cannot_be_measured()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns two time a raw signal corresponding to two layers of 750 micrometers and two others time to two layers of 750 and 200 micrometers
                var signal750and750Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                var signal750Layer = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(probeId, new List<IProbeSignal> { signal750and750Layers, signal750and750Layers, signal750Layer, signal750Layer }, this);

                // When : Try to measure two layers of 750 and 200 micrometers, four times
                int measureNb = 4;
                var measureLiseInput = new MultipleMeasuresLiseInput(new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), measureNb);
                measureLiseInput.MeasureLise.Sample.Layers[1].IsMandatory = false;
                var measureLise = new LiseMultipleThicknessMeasurementFlow(measureLiseInput);
                var result = measureLise.Execute();

                // Then : Multiple measurement flow succeeds even if some measures failed when all mandatory measures succeeds
                Assert.AreEqual(FlowState.Success, result.Status.State);
            }
        }

        [TestMethod]
        public void Multiple_thickness_measurement_flow_with_lise_fails_if_mandatory_layer_thickness_are_not_inside_tolerance()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns two time a raw signal corresponding to two layers of 750 micrometers and two others time to two layers of 750 and 200 micrometers
                var signal750And750Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                var signal750And200Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(probeId, new List<IProbeSignal> { signal750And750Layers, signal750And750Layers, signal750And200Layers, signal750And200Layers }, this);

                // When : Try to measure two layers of 750 and 200 micrometers, four times
                int measureNb = 4;
                var measureLiseInput = new MultipleMeasuresLiseInput(new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), measureNb);
                var measureLise = new LiseMultipleThicknessMeasurementFlow(measureLiseInput);
                var result = measureLise.Execute();

                // Then : Multiple measurement flow fails if some mandatory measures failed
                Assert.AreEqual(FlowState.Error, result.Status.State);
            }
        }

        [TestMethod]
        public void Multiple_thickness_measurement_flow_success_if_only_not_mandatory_layers_thickness_are_not_inside_tolerance()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns two time a raw signal corresponding to two layers of 750 micrometers and two others time to two layers of 750 and 200 micrometers
                var signal750And750Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                var signal750And200Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(probeId, new List<IProbeSignal> { signal750And750Layers, signal750And750Layers, signal750And200Layers, signal750And200Layers }, this);

                // When : Try to measure two layers of 750 and 200 micrometers, four times
                int measureNb = 4;
                var measureLiseInput = new MultipleMeasuresLiseInput(new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), measureNb);
                measureLiseInput.MeasureLise.Sample.Layers[1].IsMandatory = false;
                var measureLise = new LiseMultipleThicknessMeasurementFlow(measureLiseInput);
                var result = measureLise.Execute();

                // Then : Multiple measurement flow succeeds even if some measures failed when all mandatory measures succeeds
                Assert.AreEqual(FlowState.Success, result.Status.State);
            }
        }

        [TestMethod]
        public void Multiple_thickness_measurement_flow_with_lise_returns_all_layers_thickness_even_if_some_measurement_fails()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns two time a raw signal corresponding to two layers of 750 micrometers and two others time to two layers of 750 and 200 micrometers
                var signal750And750Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                var signal750And200Layers = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(probeId, new List<IProbeSignal> { signal750And750Layers, signal750And750Layers, signal750And200Layers, signal750And200Layers }, this);

                // When : Try to measure two layers of 750 and 200 micrometers, four times
                int measureNb = 4;
                var measureLiseInput = new MultipleMeasuresLiseInput(new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), measureNb);
                measureLiseInput.MeasureLise.Sample.Layers[1].IsMandatory = false;
                var measureLise = new LiseMultipleThicknessMeasurementFlow(measureLiseInput);
                var result = measureLise.Execute();

                // Then
                Assert.AreEqual(measureNb, result.ProbeThicknessMeasures.Count);
                foreach (var measure in result.ProbeThicknessMeasures)
                {
                    Assert.AreEqual(measureLiseInput.MeasureLise.Sample.Layers.Count, measure.Count);
                }
            }
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestLiseMultipleThicknessMeasurementReport";
            Directory.CreateDirectory(directoryPath);

            var input = new MultipleMeasuresLiseInput(new ThicknessLiseInput(LiseUpId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), 5); ;

            var flow = new LiseMultipleThicknessMeasurementFlow(input);
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
            string directoryPath = "TestLiseMultipleThicknessMeasurementReport";
            Directory.CreateDirectory(directoryPath);

            var input = new MultipleMeasuresLiseInput(new ThicknessLiseInput(LiseUpId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain), 5); ;

            var flow = new LiseMultipleThicknessMeasurementFlow(input);
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
