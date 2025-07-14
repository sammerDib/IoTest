using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Measure.AutofocusTracker;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureAutofocusTrackerTest : TestWithMockedHardware<MeasureAutofocusTrackerTest>, ITestWithCamera, ITestWithProbeLise
    {
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

        private readonly Length _zOffset5XWithMain = 0.Millimeters();
        private readonly Length _zOffset10XWithMain = 6.Millimeters();
        private readonly Length _zOffset20XWithMain = 11.Millimeters();
        private readonly Length _zOffset50XWithMain = 12.Millimeters();
        private readonly Length _zOffset5XWithMainBottom = 0.Millimeters();

        private double _doublePrecision = 1E9; // Double are not that accurate

        [TestMethod]
        public void MeasureAutofocusTracker_GetCorrectedAutofocusPosition_with_same_objective()
        {
            // Given
            var measureAutofocusTracker = new MeasureAutofocusTracker();

            var m1 = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 12.7, 0);
            var af1 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m2 = new XYZTopZBottomPosition(new StageReferential(), 50, 0, 6.6, 0);
            var af2 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m3 = new XYZTopZBottomPosition(new StageReferential(), 0, 50, 1.2, 0);
            var af3 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-20XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };
            measureAutofocusTracker.SaveAutofocusResult(af1, m1);
            measureAutofocusTracker.SaveAutofocusResult(af2, m2);
            measureAutofocusTracker.SaveAutofocusResult(af3, m3);

            var measurePosition = new XYZTopZBottomPosition(new StageReferential(), 52, 0, 6.5, 0);
            AutoFocusSettings afSettings = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            // When try to find the closest autofocus
            var correctedAfPosition = measureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);

            // Then
            Assert.IsNotNull(correctedAfPosition);
            Assert.AreEqual(m2.ZTop, correctedAfPosition.ZTop, _doublePrecision);
        }

        [TestMethod]
        public void MeasureAutofocusTracker_GetCorrectedAutofocusPosition_with_closest_autofocus_used_10X_and_measure_use_5X()
        {
            // Given
            var measureAutofocusTracker = new MeasureAutofocusTracker();

            var m1 = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 12.7, 0);
            var af1 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m2 = new XYZTopZBottomPosition(new StageReferential(), 50, 0, 6.4, 0);
            var af2 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m3 = new XYZTopZBottomPosition(new StageReferential(), 0, 50, 1.2, 0);
            var af3 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-20XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };
            measureAutofocusTracker.SaveAutofocusResult(af1, m1);
            measureAutofocusTracker.SaveAutofocusResult(af2, m2);
            measureAutofocusTracker.SaveAutofocusResult(af3, m3);

            var measurePosition = new XYZTopZBottomPosition(new StageReferential(), 52, 0, 12.7, 0);
            AutoFocusSettings afSettings = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            // When try to find the closest autofocus, should return af2 with 10XNIR
            var correctedAfPosition = measureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);

            // Then
            var expectedZPosition = m2.ZTop + _zOffset10XWithMain.Millimeters;
            Assert.IsNotNull(correctedAfPosition);
            Assert.AreEqual(expectedZPosition, correctedAfPosition.ZTop, _doublePrecision);
        }

        [TestMethod]
        public void MeasureAutofocusTracker_GetCorrectedAutofocusPosition_with_closest_autofocus_used_5X_and_measure_use_20X()
        {
            // Given
            var measureAutofocusTracker = new MeasureAutofocusTracker();

            var m1 = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 12.5, 0);
            var af1 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m2 = new XYZTopZBottomPosition(new StageReferential(), 50, 0, 6.4, 0);
            var af2 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m3 = new XYZTopZBottomPosition(new StageReferential(), 0, 50, 1.2, 0);
            var af3 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-20XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };
            measureAutofocusTracker.SaveAutofocusResult(af1, m1);
            measureAutofocusTracker.SaveAutofocusResult(af2, m2);
            measureAutofocusTracker.SaveAutofocusResult(af3, m3);

            var measurePosition = new XYZTopZBottomPosition(new StageReferential(), 5, 5, 1.2, 0);
            AutoFocusSettings afSettings = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-20XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            // When try to find the closest autofocus, should return af1 with 5XNIR
            var correctedAfPosition = measureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);

            // Then
            var expectedZFocusPosition = m1.ZTop - _zOffset20XWithMain.Millimeters;
            Assert.IsNotNull(correctedAfPosition);
            Assert.AreEqual(expectedZFocusPosition, correctedAfPosition.ZTop, _doublePrecision);
        }

        [TestMethod]
        public void MeasureAutofocusTracker_GetCorrectedAutofocusPosition_with_closest_autofocus_used_20X_and_measure_use_10X()
        {
            // Given
            var measureAutofocusTracker = new MeasureAutofocusTracker();

            var m1 = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 12.5, 0);
            var af1 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m2 = new XYZTopZBottomPosition(new StageReferential(), 50, 0, 6.4, 0);
            var af2 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m3 = new XYZTopZBottomPosition(new StageReferential(), 0, 50, 1.2, 0);
            var af3 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-20XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };
            measureAutofocusTracker.SaveAutofocusResult(af1, m1);
            measureAutofocusTracker.SaveAutofocusResult(af2, m2);
            measureAutofocusTracker.SaveAutofocusResult(af3, m3);

            var measurePosition = new XYZTopZBottomPosition(new StageReferential(), 5, 55, 6, 0);
            AutoFocusSettings afSettings = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            // When try to find the closest autofocus, should return af3 with 20XNIR
            var correctedAfPosition = measureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);

            // Then
            var expectedZFocusPosition = m3.ZTop + (_zOffset20XWithMain.Millimeters - _zOffset10XWithMain.Millimeters);
            Assert.IsNotNull(correctedAfPosition);
            Assert.AreEqual(expectedZFocusPosition, correctedAfPosition.ZTop, _doublePrecision);
        }

        [TestMethod]
        public void MeasureAutofocusTracker_GetCorrectedAutofocusPosition_with_missing_focus_positiont()
        {
            // Given
            var measureAutofocusTracker = new MeasureAutofocusTracker();

            XYZTopZBottomPosition m1 = null;
            var af1 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };
            measureAutofocusTracker.SaveAutofocusResult(af1, m1);

            var measurePosition = new XYZTopZBottomPosition(new StageReferential(), 5, 55, 6, 0);
            AutoFocusSettings afSettings = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            // When try to find the closest autofocus, should return af2 with 10XNIR
            var correctedAfPosition = measureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);

            // Then
            Assert.IsNull(correctedAfPosition);
        }


        [TestMethod]
        public void MeasureAutofocusTracker_GetCorrectedAutofocusPosition_with_bottom_probe_return_result()
        {
            // Given
            var measureAutofocusTracker = new MeasureAutofocusTracker();

            var m1 = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 12.5, 0);
            var af1 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m2 = new XYZTopZBottomPosition(new StageReferential(), 50, 0, 6.4, 0);
            var af2 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m3 = new XYZTopZBottomPosition(new StageReferential(), 0, 50, 1.2, 0);
            var af3 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-20XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise,
            };

            var m4 = new XYZTopZBottomPosition(new StageReferential(), 0, 50, 1.2, -0.26);
            var af4 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR02"),
                ProbeId = LiseBottomId,
                Type = AutoFocusType.Lise
            };

            measureAutofocusTracker.SaveAutofocusResult(af1, m1);
            measureAutofocusTracker.SaveAutofocusResult(af2, m2);
            measureAutofocusTracker.SaveAutofocusResult(af3, m3);
            measureAutofocusTracker.SaveAutofocusResult(af4, m4);

            var measurePosition = new XYZTopZBottomPosition(new StageReferential(), 5, 55, 20, 0.22);
            AutoFocusSettings afSettings = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR02"),
                ProbeId = LiseBottomId,
                Type = AutoFocusType.Lise
            };

            // When try to find the closest autofocus, should return af4 with 10XNIR
            var correctedAfPosition = measureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);

            // Then
            Assert.IsNotNull(correctedAfPosition);
            Assert.AreEqual(m4.ZBottom, correctedAfPosition.ZBottom, _doublePrecision);
        }

        [TestMethod]
        public void MeasureAutofocusTracker_Reset_lists()
        {
            // Given
            var measureAutofocusTracker = new MeasureAutofocusTracker();

            var m1 = new XYZTopZBottomPosition(new StageReferential(), 0, 0, 12.7, 0);
            var af1 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-5XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m2 = new XYZTopZBottomPosition(new StageReferential(), 50, 0, 6.6, 0);
            var af2 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            var m3 = new XYZTopZBottomPosition(new StageReferential(), 0, 50, 1.2, 0);
            var af3 = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-20XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };
            measureAutofocusTracker.SaveAutofocusResult(af1, m1);
            measureAutofocusTracker.SaveAutofocusResult(af2, m2);
            measureAutofocusTracker.SaveAutofocusResult(af3, m3);

            var measurePosition = new XYZTopZBottomPosition(new StageReferential(), 52, 0, 6.5, 0);
            AutoFocusSettings afSettings = new AutoFocusSettings()
            {
                LiseAutoFocusContext = new ObjectiveContext("ID-10XNIR01"),
                ProbeId = LiseUpId,
                Type = AutoFocusType.Lise
            };

            // When
            measureAutofocusTracker.Reset();
            var correctedAfPosition = measureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);

            // Then
            Assert.IsNull(correctedAfPosition);
        }

        protected override void PostGenericSetup()
        {
            CalibManager.UpdateCalibration(new ObjectivesCalibrationData()
            {
                User = "Default",
                Calibrations = new List<ObjectiveCalibration>()
                {
                    new ObjectiveCalibration()
                    {
                        DeviceId = "ID-5XNIR01",
                        ZOffsetWithMainObjective = _zOffset5XWithMain,
                        Image = new ImageParameters() { XOffset = 37.Micrometers(), YOffset = 15.Micrometers() }
                    },
                    new ObjectiveCalibration()
                    {
                        DeviceId = "ID-10XNIR01",
                        ZOffsetWithMainObjective = _zOffset10XWithMain,
                        Image = new ImageParameters() { XOffset = 32.Micrometers(), YOffset = 11.Micrometers() }
                    },
                    new ObjectiveCalibration()
                    {
                        DeviceId = "ID-20XNIR01",
                        ZOffsetWithMainObjective = _zOffset20XWithMain,
                        Image = new ImageParameters() { XOffset = 94.Micrometers(), YOffset = 18.Micrometers() }
                    },
                    new ObjectiveCalibration()
                    {
                        DeviceId = "ID-50XNIR01",
                        ZOffsetWithMainObjective = _zOffset50XWithMain,
                        Image = new ImageParameters() { XOffset = 8.Micrometers(), YOffset = 46.Micrometers() }
                    },
                    new ObjectiveCalibration()
                    {
                        DeviceId = "ID-5XNIR02",
                        ZOffsetWithMainObjective = _zOffset5XWithMainBottom,
                        Image = new ImageParameters() { XOffset = 15.Micrometers(), YOffset = 4.Micrometers() }
                    }
                }
            });
        }
    }
}
