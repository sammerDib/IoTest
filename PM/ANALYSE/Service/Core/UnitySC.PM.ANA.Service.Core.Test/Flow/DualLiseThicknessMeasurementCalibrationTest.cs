using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.ANA.Service.Shared.TestUtils;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class DualLiseThicknessMeasurementCalibrationTest : TestWithMockedHardware<DualLiseThicknessMeasurementCalibrationTest>, ITestWithProbeLise
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
        public UnitySC.Shared.Tools.Units.Length PixelSizeX { get; set; }
        public UnitySC.Shared.Tools.Units.Length PixelSizeY { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        [TestMethod]
        public void Dual_Lise_thickness_measurement_calibration_succeeds_when_signal_matches_sample()
        {
            // Given : The dual lise probe provides signal corresponding at the probe sample layer of reference

            var expectedGlobalThickness = AirGapUp + Thickness750 + AirGapDown;
            var probeLayers = new List<ProbeSampleLayer> { Layer750 };
            var sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            var signalUp = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { Layer750 }, AirGapUp.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);

            var signalDown = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { Layer750 }, AirGapDown.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalDown, this);

            // When : Calibrate dual Lise measure

            var acquisitionUpParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sample);
            var acquisitionDownParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sample);
            var acquisitionParams = new LiseSignalAcquisition.DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);

            var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);
            var topCalibrationResult = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.CalibrateLise(TestWithProbeLiseHelper.GetFakeDualLise(this).Object.ProbeLiseUp, acquisitionUpParams, analysisParams);
            var bottomCalibrationResult = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.CalibrateLise(TestWithProbeLiseHelper.GetFakeDualLise(this).Object.ProbeLiseDown, acquisitionDownParams, analysisParams);
            var dualCalibrationResult = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.CalibrateDualLise(acquisitionParams, topCalibrationResult.Item1.AirGap, bottomCalibrationResult.Item1.AirGap, topCalibrationResult.Item2, bottomCalibrationResult.Item2);

            // Then : Calibration result is valid
            Assert.AreEqual(expectedGlobalThickness.Micrometers, dualCalibrationResult.Micrometers, 10);
        }

        [TestMethod]
        public void Dual_Lise_thickness_measurement_calibration_throws_Exception_when_signal_does_not_match_sample()
        {
            // Given : The lise probe up provides correct signal but lise probe down provides signal doesn't corresponding at the probe sample layer of reference

            var probeLayers = new List<ProbeSampleLayer> { Layer750 };
            var sample = new ProbeSample(probeLayers, "REF 750UM", "SampleInfo");

            var signalUp = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { Layer750 }, AirGapUp.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signalUp, this);

            var signalDown = CreateLiseSignalFromSampleLayers(new List<ProbeSampleLayer> { }, AirGapDown.Micrometers, GeometricToMicrometerRatio, LiseSignalLength);
            TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, signalDown, this);

            // When : Calibrate dual Lise measure

            var acquisitionUpParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sample);
            var acquisitionDownParams = new LiseSignalAcquisition.LiseAcquisitionParams(DefaultGain, 1, sample);
            var acquisitionParams = new LiseSignalAcquisition.DualLiseAcquisitionParams(acquisitionUpParams, acquisitionDownParams);

            var analysisParams = new LiseSignalAnalysisParams(1000, 9, 0);

            // Then : Throws expected exception
            var topCalibrationResult = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.CalibrateLise(TestWithProbeLiseHelper.GetFakeDualLise(this).Object.ProbeLiseUp, acquisitionUpParams, analysisParams);
            var bottomCalibrationResult = UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.CalibrateLise(TestWithProbeLiseHelper.GetFakeDualLise(this).Object.ProbeLiseDown, acquisitionDownParams, analysisParams);

            Assert.ThrowsException<Exception>(() => UnitySC.PM.ANA.Hardware.Probe.Lise.LiseMeasurement.CalibrateDualLise(acquisitionParams, topCalibrationResult.Item1.AirGap, bottomCalibrationResult.Item1.AirGap, topCalibrationResult.Item2, bottomCalibrationResult.Item2));
        }
    }
}
