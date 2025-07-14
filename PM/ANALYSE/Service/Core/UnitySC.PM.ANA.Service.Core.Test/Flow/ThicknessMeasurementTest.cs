using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;
using System.IO;
using System.Collections.Generic;
using Moq;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Hardware.Probe.Lise;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class ThicknessMeasurementTest : TestWithMockedHardware<ThicknessMeasurementTest>, ITestWithProbeLise, ITestWithCamera
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
        public void Thickness_measurement_flow_with_lise_nominal_case()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 and 200 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);
                // When : Try to measure two layers of 750 and 200 micrometers
                var measureLiseData = new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain);
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();
                // Then : Measurement succeed
                Assert.AreEqual(FlowState.Success, result.Status.State);
                Assert.AreEqual(2, result.LayersThickness.Count);
                Assert.AreEqual(Thickness750.Micrometers, result.LayersThickness.ElementAt(0).Thickness.Micrometers, 1);
                Assert.AreEqual(Thickness200.Micrometers, result.LayersThickness.ElementAt(1).Thickness.Micrometers, 1);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_fails_if_mandatory_layers_cannot_be_measured()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to one layer of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);
                // When : Try to measure two layers of 750 micrometers
                var measureLiseData = new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex), DefaultGain);
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();
                // Then : Measurement failed
                Assert.AreEqual(FlowState.Error, result.Status.State);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_success_if_only_not_mandatory_layers_cannot_be_measured()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to one layer of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);
                // When : Try to measure two layers of 750 micrometers
                var measureLiseData = new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex), DefaultGain);
                measureLiseData.Sample.Layers.ElementAt(1).IsMandatory = false;
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();
                // Then : Successful measurement
                Assert.AreEqual(FlowState.Success, result.Status.State);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_fails_if_mandatory_layer_thickness_are_not_inside_tolerance()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);
                // When : Try to measure two layers of 750 and 200 micrometers
                var measureLiseData = new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain);
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();
                // Then : Measurement failed
                Assert.AreEqual(FlowState.Error, result.Status.State);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_success_if_only_not_mandatory_layers_thickness_are_not_inside_tolerance()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);
                // When : Try to measure two layers of 750 and 200 micrometers
                var measureLiseData = new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain);
                measureLiseData.Sample.Layers.ElementAt(1).IsMandatory = false;
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();
                // Then : Successful measurement
                Assert.AreEqual(FlowState.Success, result.Status.State);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_returns_all_layers_thickness_even_if_some_measurement_fails()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);
                // When : Try to measure two layers of 750 and 200 micrometers
                var measureLiseData = new ThicknessLiseInput(probeId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain);
                measureLiseData.Sample.Layers.ElementAt(1).IsMandatory = false;
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();
                // Then
                Assert.AreEqual(measureLiseData.Sample.Layers.Count, result.LayersThickness.Count);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_success_when_we_have_a_non_mandatory_unknown_layer_to_measure()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                signal.DiscardedPeaks.Add(new ProbePoint(signal.SelectedPeaks[2].X, signal.SelectedPeaks[2].Y));
                signal.SelectedPeaks.RemoveAt(2);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);

                // When : Try to measure one layer with valid refractive index and another with an invalid refractive index. The second is not mandatory
                var probeSample = CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                probeSample.Layers[1].RefractionIndex = double.NaN;

                var measureLiseData = new ThicknessLiseInput(probeId, probeSample, DefaultGain);
                measureLiseData.Sample.Layers.ElementAt(1).IsMandatory = false;
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();

                // Then
                Assert.AreEqual(FlowState.Success, result.Status.State);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_fails_when_we_have_a_mandatory_unknown_layer_to_measure()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                signal.DiscardedPeaks.Add(new ProbePoint(signal.SelectedPeaks[2].X, signal.SelectedPeaks[2].Y));
                signal.SelectedPeaks.RemoveAt(2);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);

                // When : Try to measure one layer with valid refractive index and another with an invalid refractive index. The second is mandatory
                var probeSample = CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                probeSample.Layers[1].RefractionIndex = double.NaN;

                var measureLiseData = new ThicknessLiseInput(probeId, probeSample, DefaultGain);
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();

                // Then
                Assert.AreEqual(FlowState.Error, result.Status.State);
            }
        }

        [TestMethod]
        public void Thickness_measurement_flow_with_lise_all_layers_thicknesses_with_thickness_and_quality_of_zero_for_unknown_layers()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                signal.DiscardedPeaks.Add(new ProbePoint(signal.SelectedPeaks[2].X, signal.SelectedPeaks[2].Y));
                signal.SelectedPeaks.RemoveAt(2);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);

                // When : Try to measure one layer with valid refractive index and another with an invalid refractive index.
                var probeSample = CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                probeSample.Layers[1].RefractionIndex = double.NaN;

                var measureLiseData = new ThicknessLiseInput(probeId, probeSample, DefaultGain);
                var measureLise = new LiseThicknessMeasurementFlow(new MeasureLiseInput(measureLiseData));
                var result = measureLise.Execute();

                // Then
                Assert.AreEqual(measureLiseData.Sample.Layers.Count, result.LayersThickness.Count);
                Assert.AreEqual(measureLiseData.Sample.Layers[0].Thickness.Micrometers, result.LayersThickness[0].Thickness.Micrometers, 1);
                Assert.AreEqual(0, result.LayersThickness[1].Thickness.Micrometers, 1);
            }
        }

        [TestMethod]
        public void Report_of_input_is_working()
        {
            //Given
            string directoryPath = "TestLiseThicknessMeasurementReport";
            Directory.CreateDirectory(directoryPath);

            var data = new ThicknessLiseInput(LiseUpId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain);

            var flow = new LiseThicknessMeasurementFlow(new MeasureLiseInput(data));
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
            string directoryPath = "TestLiseThicknessMeasurementReport";
            Directory.CreateDirectory(directoryPath);

            var data = new ThicknessLiseInput(LiseUpId, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex), DefaultGain);

            var flow = new LiseThicknessMeasurementFlow(new MeasureLiseInput(data));
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
