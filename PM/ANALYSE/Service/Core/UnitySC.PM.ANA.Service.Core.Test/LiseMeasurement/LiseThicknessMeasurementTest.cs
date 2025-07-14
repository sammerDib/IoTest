using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.LiseMeasurement
{
    [TestClass]
    public class LiseThicknessMeasurementTest : TestWithMockedHardware<LiseThicknessMeasurementTest>, ITestWithProbeLise
    {
        #region Interfaces properties

        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<Interface.Probe.ProbeLise.IProbeDualLise> FakeDualLise { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        [TestMethod]
        public void Thickness_measure_with_probe_lise_nominal_case()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given :  Probe LISE return continuously a raw signal corresponding to two layers of 750 micrometers
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex), this);

                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);

                // When : Try to measure two layers of 750 micrometers with probe LISE
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMeasure(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams);

                // Then : All layers are correctly measured
                Assert.AreEqual(2, probeResults.LayersThickness.Count);
                Assert.AreEqual(Thickness750.Micrometers, probeResults.LayersThickness.ElementAt(0).Thickness.Micrometers, 1);
                Assert.AreEqual(Thickness750.Micrometers, probeResults.LayersThickness.ElementAt(1).Thickness.Micrometers, 1);
            }
        }

        [TestMethod]
        public void Thickness_measure_with_probe_lise_return_thickness_for_each_layers_even_if_some_measurement_fails()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return no signal
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, CreateNullLiseSignal(), this);

                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);

                // When : Try to measure a layer with probe LISE
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMeasure(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams);

                // Then
                Assert.AreEqual(acquisitionParams.Sample.Layers.Count, probeResults.LayersThickness.Count);
            }
        }

        [TestMethod]
        public void Thickness_measure_with_probe_lise_returns_good_thickness_and_quality_when_signal_allow_to_measure_this_layer_thickness_even_if_some_other_measurements_fails()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return continuously a raw signal corresponding to one layer of 750 micrometers
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex), this);

                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);

                // When : Try to measure two layers of 750 micrometers with probe LISE
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMeasure(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams);

                // Then: first layer is correctly measured
                Assert.AreEqual(Thickness750.Micrometers, probeResults.LayersThickness.ElementAt(0).Thickness.Micrometers, 1);
                Assert.AreNotEqual(0, probeResults.LayersThickness.ElementAt(0).Quality);
            }
        }

        [TestMethod]
        public void Thickness_measure_with_probe_lise_return_zero_thickness_and_zero_quality_when_signal_does_not_allow_to_measure_the_layer_thickness()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return continuously a raw signal corresponding to one layer of 750 micrometers
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex), this);

                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);

                // When : Try to measure two layers of 750 micrometers with probe LISE
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMeasure(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams);

                // Then :zero thickness and zero quality for unmeasurable second layer
                Assert.AreEqual(0, probeResults.LayersThickness.ElementAt(1).Thickness.Micrometers);
                Assert.AreEqual(0, probeResults.LayersThickness.ElementAt(1).Quality);
            }
        }

        [TestMethod]
        public void Thickness_measure_with_probe_lise_return_good_thicknesses_when_signal_allow_to_measure_this_layer_thickness_even_if_signal_contains_interference_peaks()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return continuously a raw signal corresponding to two layer of 750 micrometers separeted by interferences peaks
                var signal = CreateLiseSignalWithInterferences(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex);

                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);

                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);

                // When : Try to measure two layers of 750 micrometers with probe LISE
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMeasure(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams);

                // Then :zero thickness and zero quality for unmeasurable second layer
                Assert.AreEqual(Layer750.Thickness.Micrometers, probeResults.LayersThickness.ElementAt(0).Thickness.Micrometers, 10);
                Assert.AreEqual(Layer750.Thickness.Micrometers, probeResults.LayersThickness.ElementAt(1).Thickness.Micrometers, 10);
            }
        }
    }
}
