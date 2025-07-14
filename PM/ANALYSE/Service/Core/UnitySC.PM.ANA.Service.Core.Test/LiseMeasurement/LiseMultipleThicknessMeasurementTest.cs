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
    public class LiseMultipleThicknessMeasurementTest : TestWithMockedHardware<LiseMultipleThicknessMeasurementTest>, ITestWithProbeLise
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
        public void Multiple_thickness_measures_nominal_case()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE returns continuously a raw signal corresponding to two layers of 750 and 200 micrometers
                var signal = CreateLiseSignalFromLayerThicknesses(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);
                // When : Try to do x measures of two layers of 750 and 200 micrometers with the probe LISE
                int measuresNb = 5;
                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers(), 200.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMultipleMeasures(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams, measuresNb);
                // Then : All layers are correctly measured at each time
                foreach (var result in probeResults)
                {
                    Assert.AreEqual(2, result.LayersThickness.Count);
                    Assert.AreEqual(Thickness750.Micrometers, result.LayersThickness.ElementAt(0).Thickness.Micrometers, 1);
                    Assert.AreEqual(Thickness200.Micrometers, result.LayersThickness.ElementAt(1).Thickness.Micrometers, 1);
                }
            }
        }

        [TestMethod]
        public void Multiple_thickness_measures_returns_correct_number_of_measures()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return continuously a raw signal without peaks
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength), this);
                // When : Try to do x measures with the probe LISE
                int measuresNb = 10;
                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMultipleMeasures(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams, measuresNb);
                // Then : The number of measurements taken is the same as requested
                Assert.AreEqual(measuresNb, probeResults.Count);
            }
        }

        [TestMethod]
        public void Multiple_thickness_measures_returns_thickness_for_each_layers_even_if_some_measurement_fails()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return continuously a raw signal without peaks
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength), this);
                // When : Try to do x measures with the probe LISE
                int measuresNb = 10;
                var acquisitionParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex));
                var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);
                var probeResults = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.DoMultipleMeasures(TestWithProbeLiseHelper.GetFakeProbeLise(probeId, this).Object, acquisitionParams, analysisParams, measuresNb);
                // Then :
                foreach (var measure in probeResults)
                {
                    Assert.AreEqual(acquisitionParams.Sample.Layers.Count, measure.LayersThickness.Count);
                }
            }
        }
    }
}
