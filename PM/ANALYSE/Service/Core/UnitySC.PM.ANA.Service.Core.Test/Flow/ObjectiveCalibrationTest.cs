using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow
{
    [TestClass]
    public class ObjectiveCalibrationTest : TestWithMockedHardware<ObjectiveCalibrationTest>, ITestWithAxes, ITestWithProbeLise, ITestWithCamera, ITestWithChuck
    {
        #region parameters

        public Mock<IAxes> SimulatedAxes { get; set; }
        public string CameraUpId { get; set; }
        public string CameraBottomId { get; set; }
        public Mock<CameraBase> SimulatedCameraUp { get; set; }
        public Mock<CameraBase> SimulatedCameraBottom { get; set; }
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
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        private ObjectiveCalibrationInput _objectiveUpCalibrationInput;

        private ObjectiveCalibrationInput _objectiveBottomCalibrationInput;

        private XYPosition _fakeRefPos = new XYPosition(new StageReferential(), 100, 100);
        private int _zTopPos = 12;
        private int _zBottomPos = 2;
        private int _nbAirGapOnReference = 10; // 10 came from MeasureLiseAirGap function in ObjectiveCalibrationFlow.cs

        #endregion parameters

        protected override void PreGenericSetup()
        {
            _objectiveUpCalibrationInput = new ObjectiveCalibrationInput(
                probeId: "ProbeLiseUp",
                objectiveId: "objectiveUpId",
                gain: 1.8,
                previousCalibration: new ObjectiveCalibration());

            _objectiveBottomCalibrationInput = new ObjectiveCalibrationInput(
                probeId: "ProbeLiseBottom",
                objectiveId: "objectiveBottomId",
                gain: 1.8,
                previousCalibration: new ObjectiveCalibration());
        }

        protected override void PostGenericSetup()
        {
            // By default, have a non open chuck with clamped wafer
            SimulatedChuck.Object.Configuration.IsOpenChuck = false;
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), true);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
        }

        [TestMethod]
        public void Objective_calibration_flow_nominal_case()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapRef = 1100.Micrometers();
            var airGapWafer = 900.Micrometers();
            var airGapWaferAtFocus = 1105.Micrometers();
            var signals = new List<IProbeSignal>();
            for (int i = 0; i < _nbAirGapOnReference; i++)
            {
                signals.Add(CreateLiseSignalWithOnlyAirGap(airGapRef));                             // AirGap on the OpticalReference
            }
            signals.Add(CreateLiseSignalWithOnlyAirGap(1200.Micrometers()));                        // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWafer));                               // AirGap on the wafer center
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWaferAtFocus));                        // AirGap on the wafer center at focus position

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Length expectedOpticalRefElevation = airGapWafer - airGapRef - (airGapRef - airGapWaferAtFocus);
            Assert.IsNotNull(result.AutoFocus.Lise.MinGain);
            Assert.IsNotNull(result.AutoFocus.Lise.MaxGain);
            Assert.IsNotNull(result.Image.PixelSizeX);
            Assert.IsNotNull(result.Image.PixelSizeY);
            Assert.IsNotNull(result.Image.XOffset);
            Assert.IsNotNull(result.Image.YOffset);
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(_zTopPos, result.AutoFocus.ZFocusPosition.Millimeters);
            Assert.AreEqual(_zTopPos, result.AutoFocus.Lise.ZStartPosition.Millimeters);
            Assert.AreEqual(airGapRef.Millimeters, result.AutoFocus.Lise.AirGap.Millimeters, 1E-2);
            Assert.AreEqual(expectedOpticalRefElevation.Millimeters, result.OpticalReferenceElevationFromStandardWafer.Millimeters, 1E-2);
        }

        [TestMethod]
        public void Objective_calibration_flow_wafer_far_under_ref()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapRef = 1100.Micrometers();
            var airGapWafer = 900.Micrometers();
            var airGapWaferAtFocus = 500.Micrometers();
            var signals = new List<IProbeSignal>();
            for (int i = 0; i < _nbAirGapOnReference; i++)
            {
                signals.Add(CreateLiseSignalWithOnlyAirGap(airGapRef));                             // AirGap on the OpticalReference
            }
            signals.Add(CreateLiseSignalWithOnlyAirGap(1200.Micrometers()));                        // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(0.Micrometers()));                           // AirGap on the wafer center not found 1
            signals.Add(CreateLiseSignalWithOnlyAirGap(0.Micrometers()));                           // AirGap on the wafer center not found 2
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWafer));                               // AirGap on the wafer center
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWaferAtFocus));                        // AirGap on the wafer center at focus position

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            var objectivesConfig = HardwareManager.ObjectivesSelectors[ObjectiveUpId].Config.Objectives;
            var BigStepSize = objectivesConfig.Find(_ => _.DeviceID == ObjectiveUpId).BigStepSizeZ;
            Length expectedOpticalRefElevation = airGapWafer - airGapRef - (airGapRef - airGapWaferAtFocus) - 2 * BigStepSize;
            Assert.IsNotNull(result.AutoFocus.Lise.MinGain);
            Assert.IsNotNull(result.AutoFocus.Lise.MaxGain);
            Assert.IsNotNull(result.Image.PixelSizeX);
            Assert.IsNotNull(result.Image.PixelSizeY);
            Assert.IsNotNull(result.Image.XOffset);
            Assert.IsNotNull(result.Image.YOffset);
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(_zTopPos, result.AutoFocus.ZFocusPosition.Millimeters);
            Assert.AreEqual(_zTopPos, result.AutoFocus.Lise.ZStartPosition.Millimeters);
            Assert.AreEqual(airGapRef.Millimeters, result.AutoFocus.Lise.AirGap.Millimeters, 1E-2);
            Assert.AreEqual(expectedOpticalRefElevation.Millimeters, result.OpticalReferenceElevationFromStandardWafer.Millimeters, 1E-2);
        }

        [TestMethod]
        public void Objective_calibration_flow_wafer_not_found_too_far_under_ref()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapRef = 1100.Micrometers();
            var airGapWafer = 900.Micrometers();
            var airGapWaferAtFocus = 500.Micrometers();
            var signals = new List<IProbeSignal>();
            for (int i = 0; i < _nbAirGapOnReference; i++)
            {
                signals.Add(CreateLiseSignalWithOnlyAirGap(airGapRef));                             // AirGap on the OpticalReference
            }
            signals.Add(CreateLiseSignalWithOnlyAirGap(1200.Micrometers()));                        // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(0.Micrometers()));                           // AirGap on the wafer center not found 1
            signals.Add(CreateLiseSignalWithOnlyAirGap(0.Micrometers()));                           // AirGap on the wafer center not found 2
            signals.Add(CreateLiseSignalWithOnlyAirGap(0.Micrometers()));                           // AirGap on the wafer center not found 3
            signals.Add(CreateLiseSignalWithOnlyAirGap(0.Micrometers()));                           // AirGap on the wafer center not found 4
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWafer));                               // AirGap on the wafer center
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWaferAtFocus));                        // AirGap on the wafer center at focus position

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Objective_calibration_with_ref_higher_than_wafer()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapRef = 700.Micrometers();
            var airGapWafer = 1000.Micrometers();
            var airGapWaferAtFocus = 700.Micrometers();
            var signals = new List<IProbeSignal>();
            for (int i = 0; i < _nbAirGapOnReference; i++)
            {
                signals.Add(CreateLiseSignalWithOnlyAirGap(airGapRef));                             // AirGap on the OpticalReference
            }
            signals.Add(CreateLiseSignalWithOnlyAirGap(1200.Micrometers()));                        // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWafer));                               // AirGap on the wafer center
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWaferAtFocus));                        // AirGap on the wafer center at focus position

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Length expectedOpticalRefElevation = airGapWafer - airGapRef - (airGapRef - airGapWaferAtFocus);
            Assert.IsNotNull(result.AutoFocus.Lise.MinGain);
            Assert.IsNotNull(result.AutoFocus.Lise.MaxGain);
            Assert.IsNotNull(result.Image.PixelSizeX);
            Assert.IsNotNull(result.Image.PixelSizeY);
            Assert.IsNotNull(result.Image.XOffset);
            Assert.IsNotNull(result.Image.YOffset);
            Assert.AreEqual(FlowState.Success, result.Status.State);
            Assert.AreEqual(_zTopPos, result.AutoFocus.ZFocusPosition.Millimeters);
            Assert.AreEqual(_zTopPos, result.AutoFocus.Lise.ZStartPosition.Millimeters);
            Assert.AreEqual(airGapRef.Millimeters, result.AutoFocus.Lise.AirGap.Millimeters, 1E-2);
            Assert.AreEqual(expectedOpticalRefElevation.Millimeters, result.OpticalReferenceElevationFromStandardWafer.Millimeters, 1E-2);
        }

        [TestMethod]
        public void Objective_calibration_flow_fails_with_wafer_not_clamped()
        {
            // Given
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(new Length(300, LengthUnit.Millimeter), false);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(new Length(300, LengthUnit.Millimeter), MaterialPresence.Present);
            SimulatedChuck.Setup(_ => _.GetState()).Returns(new PM.Shared.Hardware.Service.Interface.Chuck.ChuckState(clampStates, presenceStates));
            setupAxisConfigs();
            setupObjectiveConfig();

            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Objective_calibration_flow_airgap_failure_on_reference()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapWafer = 1000.Micrometers();
            var airGapWaferAtFocus = 1000.Micrometers();
            var signals = new List<IProbeSignal>()
            {
                CreateNullLiseSignal(),                                                     // AirGap on the OpticalReference
                CreateLiseSignalWithOnlyAirGap(1200.Micrometers()),                         // AFLise calibration Minimum Gain
                CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength),  // AFLise calibration Minimum Gain
                CreateLiseSignalWithOnlyAirGap(1000.Micrometers()),                         // AFLise calibration Maximum Gain
                CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength),  // AFLise calibration Maximum Gain
                CreateLiseSignalWithOnlyAirGap(airGapWafer),                                // AirGap on the wafer center
                CreateLiseSignalWithOnlyAirGap(airGapWaferAtFocus),                         // AirGap on the wafer center at focus position
            };

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Objective_calibration_flow_airgap_failure_on_wafer()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapRef = 700.Micrometers();
            var signals = new List<IProbeSignal>();
            for (int i = 0; i < _nbAirGapOnReference; i++)
            {
                signals.Add(CreateLiseSignalWithOnlyAirGap(airGapRef));                             // AirGap on the OpticalReference
            }
            signals.Add(CreateLiseSignalWithOnlyAirGap(1200.Micrometers()));                        // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Maximum Gain
            signals.Add(CreateNullLiseSignal());                                                    // AirGap on the wafer center

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Objective_calibration_flow_airgap_failure_on_lise_calibration_min_gain()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapWafer = 1000.Micrometers();
            var airGapRef = 700.Micrometers();
            var airGapWaferAtFocus = 700.Micrometers();
            var signals = new List<IProbeSignal>();
            for (int i = 0; i < _nbAirGapOnReference; i++)
            {
                signals.Add(CreateLiseSignalWithOnlyAirGap(airGapRef));                             // AirGap on the OpticalReference
            }
            signals.Add(CreateNullLiseSignal());                                                    // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWafer));                               // AirGap on the wafer center
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWaferAtFocus));                        // AirGap on the wafer center at focus position

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        [TestMethod]
        public void Objective_calibration_flow_airgap_failure_on_lise_calibration_max_gain()
        {
            // Given
            setupAxisConfigs();
            setupObjectiveConfig();
            var airGapWafer = 1000.Micrometers();
            var airGapRef = 700.Micrometers();
            var airGapWaferAtFocus = 700.Micrometers();
            var signals = new List<IProbeSignal>();
            for (int i = 0; i < _nbAirGapOnReference; i++)
            {
                signals.Add(CreateLiseSignalWithOnlyAirGap(airGapRef));                             // AirGap on the OpticalReference
            }
            signals.Add(CreateLiseSignalWithOnlyAirGap(1200.Micrometers()));                        // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithoutPeak(GeometricToMicrometerRatio, LiseSignalLength)); // AFLise calibration Minimum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(1000.Micrometers()));                        // AFLise calibration Maximum Gain
            signals.Add(CreateNullLiseSignal());                                                    // AFLise calibration Maximum Gain
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWafer));                               // AirGap on the wafer center
            signals.Add(CreateLiseSignalWithOnlyAirGap(airGapWaferAtFocus));                        // AirGap on the wafer center at focus position

            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, signals, this);
            var objectiveCalibration = new ObjectiveCalibrationFlow(_objectiveUpCalibrationInput);

            // When
            var result = objectiveCalibration.Execute();

            // Then
            Assert.AreEqual(FlowState.Error, result.Status.State);
        }

        #region private methods

        private void setupAxisConfigs()
        {
            // Setup Axis config
            var axesConfig = new AxesConfig()
            {
                AxisConfigs = new List<AxisConfig>()
                {
                    new AxisConfig()
                    {
                        MovingDirection = MovingDirection.ZBottom,
                        PositionMax = 2.9.Millimeters()
                    },
                    new AxisConfig()
                    {
                        MovingDirection = MovingDirection.ZTop,
                        PositionMax = 19.9.Millimeters()
                    }
                }
            };
            SimulatedAxes.Setup(a => a.AxesConfiguration).Returns(axesConfig);

            // Setup Axis position
            var pos = new XYZTopZBottomPosition(new StageReferential(), _fakeRefPos.X, _fakeRefPos.Y, _zTopPos, _zBottomPos);
            SimulatedAxes.Setup(a => a.GetPos()).Returns(pos);
        }

        private void setupObjectiveConfig()
        {
            var objectivesConfig = HardwareManager.ObjectivesSelectors[ObjectiveUpId].Config.Objectives;
            objectivesConfig.Find(_ => _.DeviceID == ObjectiveUpId).IsMainObjective = true;
            objectivesConfig.Find(_ => _.DeviceID == ObjectiveUpId).BigStepSizeZ = 1.Millimeters();
        }

        #endregion private methods
    }
}
