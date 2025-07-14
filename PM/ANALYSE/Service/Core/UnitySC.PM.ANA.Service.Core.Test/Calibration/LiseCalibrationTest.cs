using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;
using UnitySC.Shared.Tools.Units;
using System.Collections.Generic;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.ANA.Hardware.Probe.Lise;

namespace UnitySC.PM.ANA.Service.Core.Test.Calibration
{
    [TestClass]
    public class LiseCalibrationTest : TestWithMockedHardware<LiseCalibrationTest>, ITestWithAxes, ITestWithProbeLise
    {
        private LiseCalibration _liseCalibration;

        #region Interfaces properties

        public Mock<IAxes> SimulatedAxes { get; set; }
        public string ObjectiveUpId { get; set; }
        public string ObjectiveBottomId { get; set; }
        public Length PixelSizeX { get; set; }
        public Length PixelSizeY { get; set; }
        public List<string> SimpleProbesLise { get; set; }
        public string LiseUpId { get; set; }
        public string LiseBottomId { get; set; }
        public string DualLiseId { get; set; }
        public double DefaultGain { get; set; }
        public Mock<ProbeLise> FakeLiseUp { get; set; }
        public Mock<ProbeLise> FakeLiseBottom { get; set; }
        public Mock<IProbeDualLise> FakeDualLise { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion Interfaces properties

        protected override void PostGenericSetup()
        {
            _liseCalibration = new LiseCalibration();
        }

        [TestMethod]
        public void Calibration_flow_of_probe_lise_up_succeeds_when_probe_provides_great_signal()
        {
            // Given : Probe LISE up return a raw signal with peaks between gain 1.9 and gain 1.4, then without visible peaks at gain 1.3 and then completely saturated above gain 1.9.
            var signals = new List<IProbeSignal>();
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal without peak at gain = 1.4
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 1.9
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.0
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.1
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.2
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.3
            signals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength)); // simulate signal saturated at gain = 2.4
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);

            // When : Calibrate the probe LISE up between gain = 1.4 and 2.4 with step of 0.1 (calibration begin with median gain 1.9)
            var sample = CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
            var calibration = new LiseAutofocusCalibration { MinGain = 0, MaxGain = 5.5, ZPosition = -1 };
            bool calibrationSucceeded = _liseCalibration.Calibration(sample, ref calibration, LiseUpId, 11, 15, AxisSpeed.Slow, 0.5, 1.4, 2.4);

            // Then : Calibration succeeded and find gain range [1.4; 1.9]
            Assert.IsTrue(calibrationSucceeded);
            Assert.AreEqual(1.5, calibration.MinGain);
            Assert.AreEqual(1.9, calibration.MaxGain);
        }

        [TestMethod]
        public void Calibration_flow_of_probe_lise_bottom_succeeds_when_probe_provides_great_signal()
        {
            // Given : Probe LISE bottom return a raw signal with peaks between gain 1.9 and gain 1.4, then without visible peaks at gain 1.3 and then completely saturated above gain 1.9.
            var bottomSignals = new List<IProbeSignal>();
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { RefPeakArbitraryPosition }, GeometricToMicrometerRatio, LiseSignalLength));
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal without peak at gain = 1.4
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 1.9
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.0
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.1
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.2
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.3
            bottomSignals.Add(CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength));// simulate signal saturated at gain = 2.4
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, bottomSignals, this);

            // When : Calibrate the probe LISE bottom between gain = 1.4 and 2.4 with step of 0.1 (calibration begin with median gain 1.9)
            var sample = CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
            var calibration = new LiseAutofocusCalibration { MinGain = 0, MaxGain = 5.5, ZPosition = -1 };
            bool calibrationSucceeded = _liseCalibration.Calibration(sample, ref calibration, LiseBottomId, -11, 0, AxisSpeed.Slow, 0.5, 1.4, 2.4);

            // Then : Calibration succeeded and find gain range [1.4; 1.9]
            Assert.IsTrue(calibrationSucceeded);
            Assert.AreEqual(1.5, calibration.MinGain);
            Assert.AreEqual(1.9, calibration.MaxGain);
        }

        [TestMethod]
        public void Calibration_flow_of_probe_lise_fails_when_probe_provides_bad_signal()
        {
            foreach (string probeId in SimpleProbesLise)
            {
                // Given : Probe LISE return continuously a raw signal without peak
                var signal = CreateLiseSignalFromPeakPositions(new List<int> { }, GeometricToMicrometerRatio, LiseSignalLength);
                TestWithProbeLiseHelper.AssociateSignalIndefinitelyAtSingleAcquisitionWithFakeProbeLise(probeId, signal, this);

                // When : Pre-calibrate the probe
                var sample = CreateProbeSample(new List<Length>() { 750.Micrometers() }, MaterialRefractionIndex);
                var calibration = new LiseAutofocusCalibration { MinGain = 0, MaxGain = 5.5, ZPosition = -1 };
                bool calibrationSucceeded = _liseCalibration.Calibration(sample, ref calibration, probeId, 11, 11, AxisSpeed.Slow, 0.5, 1.4, 2.4);

                // Then Calibration failed
                Assert.IsFalse(calibrationSucceeded);
            }
        }
    }
}
