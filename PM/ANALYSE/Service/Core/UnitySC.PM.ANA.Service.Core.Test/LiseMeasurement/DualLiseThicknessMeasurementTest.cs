using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.LiseMeasurement
{
    [TestClass]
    public class DualLiseThicknessMeasurementTest : TestWithMockedHardware<DualLiseThicknessMeasurementTest>, ITestWithProbeLise
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
        public void Thickness_measure_with_dual_lise_nominal_case()
        {
            // Given : The dual lise probe provides signal corresponding at the probe sample layers that we want to measure
            // This probe sample includes data on four layers but the second is unknown

            var layersUp = new List<ProbeSampleLayer> { Layer750 };
            var layersDown = new List<ProbeSampleLayer> { Layer200, Layer750 };
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForLayersSeparatedByOneUnknownLayer(layersUp, layersDown, AirGapUp, AirGapDown);
            var sampleUp = new ProbeSample(layersUp, "Name", "SampleInfo");
            var sampleDown = new ProbeSample(layersDown, "Name", "SampleInfo");

            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, sampleAndSignalForMeasure.SignalLiseUp, this);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, sampleAndSignalForMeasure.SignalLiseDown, this);

            var expectedUnknownLayerThickness = 225.Micrometers();
            var globalThickness = Layer750.Thickness + Layer750.Thickness + Layer200.Thickness + AirGapUp + AirGapDown + expectedUnknownLayerThickness;

            // When : Measure sample with an unknown layer

            var acquisitionUpParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sampleUp);
            var acquisitionDownParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sampleDown);

            var acquisitionParams = new LiseSignalAcquisition.DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);
            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoUnknownLayerMeasure(TestWithProbeLiseHelper.GetFakeDualLise(this).Object, globalThickness, acquisitionParams, unknownLayer);

            // Then : MeasurableLayers thickness of the unknown layer is correct

            Assert.AreEqual(4, probeResults.LayersThickness.Count);
            Assert.AreEqual(expectedUnknownLayerThickness.Micrometers, probeResults.LayersThickness[1].Thickness.Micrometers, 10);
        }

        [TestMethod]
        public void Thickness_measure_with_dual_lise_return_thickness_for_each_layers_even_if_some_measurement_fails()
        {
            // Given : Probe Lise up provides signal corresponding at the probe sample layers that we want to measure but probe Lise down provides signal without ref peak

            var invalidAirGapDown = -10.Micrometers();
            var expectedUnknownLayerThickness = 255.Micrometers();
            var globalThickness = AirGapUp + Thickness750 + expectedUnknownLayerThickness + Thickness750 + invalidAirGapDown;

            var signalUp = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { Layer750 }, AirGapUp.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            var signalDownWithTwoPotentialRefPeak = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { Layer750 }, invalidAirGapDown.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalDownWithTwoPotentialRefPeak, this);

            // When : Measure sample with an unknown layer

            var sampleUp = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");
            var sampleDown = new ProbeSample(new List<ProbeSampleLayer> { Layer750 }, "Name", "SampleInfo");
            var acquisitionUpParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sampleUp);
            var acquisitionDownParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sampleDown);
            var acquisitionParams = new LiseSignalAcquisition.DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);

            var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);
            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoUnknownLayerMeasure(TestWithProbeLiseHelper.GetFakeDualLise(this).Object, globalThickness, acquisitionParams, unknownLayer);

            // Then
            var expectedLayersCount = acquisitionParams.LiseUpParams.Sample.Layers.Count + acquisitionParams.LiseDownParams.Sample.Layers.Count + 1;
            Assert.AreEqual(expectedLayersCount, probeResults.LayersThickness.Count);
        }

        [TestMethod]
        public void Thickness_measure_with_dual_lise_returns_good_thickness_and_quality_when_signal_allow_to_measure_this_layer_thickness_even_if_some_other_measurements_fails()
        {
            // Given : Probe Lise up provides signal corresponding at the probe sample layers that we want to measure but probe Lise down provides signal without ref peak

            var invalidAirGapDown = -10.Micrometers();
            var expectedUnknownLayerThickness = 255.Micrometers();
            var globalThickness = AirGapUp + Thickness750 + expectedUnknownLayerThickness + Thickness750 + invalidAirGapDown;

            var signalUp = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { Layer750 }, AirGapUp.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            var signalDownWithTwoPotentialRefPeak = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { Layer750 }, invalidAirGapDown.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalDownWithTwoPotentialRefPeak, this);

            // When : Measure sample with an unknown layer

            var acquisitionUpParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 0.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));
            var acquisitionDownParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 0.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));

            var acquisitionParams = new LiseSignalAcquisition.DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);
            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoUnknownLayerMeasure(TestWithProbeLiseHelper.GetFakeDualLise(this).Object, globalThickness, acquisitionParams, unknownLayer);

            // Then
            Assert.AreEqual(acquisitionParams.LiseUpParams.Sample.Layers[0].Thickness.Micrometers, probeResults.LayersThickness[0].Thickness.Micrometers, 10);
            Assert.AreNotEqual(0, probeResults.LayersThickness[0].Quality);
        }

        [TestMethod]
        public void Thickness_measure_with_dual_lise_return_zero_thickness_and_zero_quality_when_signal_does_not_allow_to_measure_the_layer_thickness()
        {
            // Given : Probe Lise up provides signal corresponding at the probe sample layers that we want to measure

            var layersUp = new List<ProbeSampleLayer> { Layer200 };
            var layersDown = new List<ProbeSampleLayer> { Layer750 };
            var sampleAndSignalForMeasure = CreateSampleAndItsAssociatedSplitSignalForLayersSeparatedByOneUnknownLayer(layersUp, layersDown, AirGapUp, AirGapDown);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, sampleAndSignalForMeasure.SignalLiseUp, this);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, sampleAndSignalForMeasure.SignalLiseDown, this);

            var expectedUnknownLayerThickness = 225.Micrometers();
            var globalThickness = Layer750.Thickness + Layer200.Thickness + AirGapUp + AirGapDown + expectedUnknownLayerThickness;

            // When : Measure sample with an unknown layer

            var acquisitionUpParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 0.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));
            var acquisitionDownParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 0.Micrometers(), 750.Micrometers() }, MaterialRefractionIndex));

            var acquisitionParams = new LiseSignalAcquisition.DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);
            var unknownLayer = new ProbeSampleLayer(0.Millimeters(), new LengthTolerance(0, LengthToleranceUnit.Millimeter), 1);

            var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoUnknownLayerMeasure(TestWithProbeLiseHelper.GetFakeDualLise(this).Object, globalThickness, acquisitionParams, unknownLayer);

            // Then
            Assert.AreEqual(0, probeResults.LayersThickness[1].Thickness.Micrometers);
            Assert.AreEqual(0, probeResults.LayersThickness[1].Quality);
        }
    }
}
