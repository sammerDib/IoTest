using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.MeasureCalibration;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Thickness;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Service.Measure.Test.ThicknessTestUtils;
using static UnitySC.PM.ANA.Service.Shared.TestUtils.LiseTestUtils;

namespace UnitySC.PM.ANA.Service.Measure.Test
{
    [TestClass]
    public class MeasureWarpInThicknessTest : TestWithMockedHardware<MeasureWarpInThicknessTest>,
        ITestWithProbeLise, ITestWithCamera, ITestWithAxes, ITestWithChuck
    {
        #region parameters

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
        public Mock<IAxes> SimulatedAxes { get; set; }
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        public double ThicknessThresholdInTheAir { get; set; }

        #endregion parameters

        protected override void SpecializeRegister()
        {
            ClassLocator.Default.Register(() =>
               new MeasuresConfiguration()
               {
                   AuthorizedMeasures = new List<MeasureType>
                   {
                        MeasureType.Thickness
                   },
                   Measures = new List<MeasureConfigurationBase>()
                   {
                       new MeasureThicknessConfiguration(),
                   }
               });
        }

        [TestMethod]
        public void WarpInThickness_NominalCase()
        {
            // Given all points around horizontal plane
            var thicknessSettings = CreateThicknessSettings(null, null);
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>
            {
                CreateThicknessPointResult(x: 0, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5),
                CreateThicknessPointResult(x: 0, y: 0, airGapUp: 3.Millimeters(), airGapDown: null, totalThicknessMm: 0.5),
                CreateThicknessPointResult(x: 1, y: 0, airGapUp: 0.Millimeters(), airGapDown: null, totalThicknessMm: 0.5),
                CreateThicknessPointResult(x: 1, y: 0, airGapUp: 4.Millimeters(), airGapDown: null, totalThicknessMm: 0.5),
                CreateThicknessPointResult(x: 0, y: 1, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5),
                CreateThicknessPointResult(x: 0, y: 1, airGapUp: 3.Millimeters(), airGapDown: null, totalThicknessMm: 0.5)
            }
            };

            // When computing the result
            var measure = new MeasureThickness();
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then the warp is found to be the good one
            Assert.AreEqual(1, result.WarpWaferResults.Count);
            Assert.AreEqual(4.Millimeters(), result.WarpWaferResults[0]);
            Assert.AreEqual(1.Millimeters(), (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(2.Millimeters(), (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-2.Millimeters(), (result.Points[3].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(1.Millimeters(), (result.Points[4].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[5].Datas[0] as ThicknessPointData).WarpResult.RPD);
        }

        [TestMethod]
        public void WarpInThickness_AllPointsOnSamePlane()
        {
            // Given all points on same plane
            var thicknessSettings = CreateThicknessSettings(null, null);
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            result.Points.Add(CreateThicknessPointResult(x: 1, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 1, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));

            // When computing the result
            var measure = new MeasureThickness();
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then the warp is found to be 0 and RPDs are filled
            Assert.AreEqual(1, result.WarpWaferResults.Count);
            Assert.AreEqual(0.Millimeters(), result.WarpWaferResults[0]);
            Assert.AreEqual(0.Millimeters(), (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(0.Millimeters(), (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(0.Millimeters(), (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD);
        }

        [TestMethod, Ignore("Since repeta is done on measure, this test is irrelevant")]
        public void WarpInThickness_WithRepeat()
        {
            // Given all points on same plane
            var thicknessSettings = CreateThicknessSettings(null, null);
            thicknessSettings.WarpTargetMax = 3.Millimeters();

            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            var point1 = CreateThicknessPointResult(x: 0, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point1.Datas.Add(CreateThicknessPointData(airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            var point2 = CreateThicknessPointResult(x: 0, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point2.Datas.Add(CreateThicknessPointData(airGapUp: 3.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            var point3 = CreateThicknessPointResult(x: 1, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point3.Datas.Add(CreateThicknessPointData(airGapUp: 0.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            var point4 = CreateThicknessPointResult(x: 1, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point4.Datas.Add(CreateThicknessPointData(airGapUp: 4.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            var point5 = CreateThicknessPointResult(x: 0, y: 1, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point5.Datas.Add(CreateThicknessPointData(airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            var point6 = CreateThicknessPointResult(x: 0, y: 1, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point6.Datas.Add(CreateThicknessPointData(airGapUp: 3.Millimeters(), airGapDown: null, totalThicknessMm: 0.5));
            result.Points.Add(point1);
            result.Points.Add(point2);
            result.Points.Add(point3);
            result.Points.Add(point4);
            result.Points.Add(point5);
            result.Points.Add(point6);

            // When computing the result
            var measure = new MeasureThickness();
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then the warp is found to be 0 and RPDs are filled
            Assert.AreEqual(2, result.WarpWaferResults.Count);
            Assert.AreEqual(0, result.WarpWaferResults[0].Micrometers, 1E-6);
            Assert.AreEqual(4.Millimeters(), result.WarpWaferResults[1]);
            Assert.AreEqual(0, (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[3].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[4].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[5].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(1.Millimeters(), (result.Points[0].Datas[1] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[1].Datas[1] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(2.Millimeters(), (result.Points[2].Datas[1] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-2.Millimeters(), (result.Points[3].Datas[1] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(1.Millimeters(), (result.Points[4].Datas[1] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[5].Datas[1] as ThicknessPointData).WarpResult.RPD);
        }

        [TestMethod, Ignore("Since repeta is done on measure, this test is irrelevant")]
        public void WarpInThickness_WithRepeat_AllPointsOnSamePlane_with_airGap_Up()
        {
            // Given all points on same plane
            var thicknessSettings = CreateThicknessSettings(null, null);
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            var point1 = CreateThicknessPointResult(x: 0, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point1.Datas.Add(CreateThicknessPointData(airGapUp: 2.Millimeters(), airGapDown: null, totalThicknessMm: 0.3));
            var point2 = CreateThicknessPointResult(x: 1, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point2.Datas.Add(CreateThicknessPointData(airGapUp: 0.Millimeters(), airGapDown: null, totalThicknessMm: 0.7));
            var point3 = CreateThicknessPointResult(x: 0, y: 1, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0.5);
            point3.Datas.Add(CreateThicknessPointData(airGapUp: 4.Millimeters(), airGapDown: null, totalThicknessMm: 0.1));
            result.Points.Add(point1);
            result.Points.Add(point2);
            result.Points.Add(point3);

            // When computing the result
            var measure = new MeasureThickness();
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then the warp is found to be 0 and RPDs are filled
            Assert.AreEqual(2, result.WarpWaferResults.Count);
            Assert.AreEqual(0, result.WarpWaferResults[0].Micrometers, 1E-6);
            Assert.AreEqual(0, result.WarpWaferResults[1].Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[0].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[1].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[2].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
        }

        [TestMethod, Ignore("Since repeta is done on measure, this test is irrelevant")]
        public void WarpInThickness_WithRepeat_AllPointsOnSamePlane_with_airGap_Down()
        {
            // Given all points on same plane
            var thicknessSettings = CreateThicknessSettings(null, null);
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            var point1 = CreateThicknessPointResult(x: 0, y: 0, airGapUp: null, airGapDown: 1.Millimeters(), totalThicknessMm: 0.5);
            point1.Datas.Add(CreateThicknessPointData(airGapUp: null, airGapDown: 2.Millimeters(), totalThicknessMm: 0.3));
            var point2 = CreateThicknessPointResult(x: 1, y: 0, airGapUp: null, airGapDown: 1.Millimeters(), totalThicknessMm: 0.5);
            point2.Datas.Add(CreateThicknessPointData(airGapUp: null, airGapDown: 0.Millimeters(), totalThicknessMm: 0.7));
            var point3 = CreateThicknessPointResult(x: 0, y: 1, airGapUp: null, airGapDown: 1.Millimeters(), totalThicknessMm: 0.5);
            point3.Datas.Add(CreateThicknessPointData(airGapUp: null, airGapDown: 4.Millimeters(), totalThicknessMm: 0.1));
            result.Points.Add(point1);
            result.Points.Add(point2);
            result.Points.Add(point3);

            // When computing the result
            var measure = new MeasureThickness();
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then the warp is found to be 0 and RPDs are filled
            Assert.AreEqual(2, result.WarpWaferResults.Count);
            Assert.AreEqual(0, result.WarpWaferResults[0].Micrometers, 1E-6);
            Assert.AreEqual(0, result.WarpWaferResults[1].Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[0].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[1].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[2].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
        }

        [TestMethod, Ignore("Since repeta is done on measure, this test is irrelevant")]
        public void WarpInThickness_WithRepeat_AllPointsOnSamePlane_with_airGap_Up_And_Down()
        {
            // Given all points on same plane
            var thicknessSettings = CreateThicknessSettings(null, null);
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            var point1 = CreateThicknessPointResult(x: 0, y: 0, airGapUp: 1.Millimeters(), airGapDown: 1.Millimeters(), totalThicknessMm: 0.5);
            point1.Datas.Add(CreateThicknessPointData(airGapUp: 2.Millimeters(), airGapDown: 2.Millimeters(), totalThicknessMm: 0.3));
            var point2 = CreateThicknessPointResult(x: 1, y: 0, airGapUp: 1.Millimeters(), airGapDown: 1.Millimeters(), totalThicknessMm: 0.5);
            point2.Datas.Add(CreateThicknessPointData(airGapUp: 0.Millimeters(), airGapDown: 0.Millimeters(), totalThicknessMm: 0.7));
            var point3 = CreateThicknessPointResult(x: 0, y: 1, airGapUp: 1.Millimeters(), airGapDown: 1.Millimeters(), totalThicknessMm: 0.5);
            point3.Datas.Add(CreateThicknessPointData(airGapUp: 4.Millimeters(), airGapDown: 4.Millimeters(), totalThicknessMm: 0.1));
            result.Points.Add(point1);
            result.Points.Add(point2);
            result.Points.Add(point3);

            // When computing the result
            var measure = new MeasureThickness();
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then the warp is found to be 0 and RPDs are filled
            Assert.AreEqual(2, result.WarpWaferResults.Count);
            Assert.AreEqual(0, result.WarpWaferResults[0].Micrometers, 1E-6);
            Assert.AreEqual(0, result.WarpWaferResults[1].Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[0].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[1].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
            Assert.AreEqual(0, (result.Points[2].Datas[1] as ThicknessPointData).WarpResult.RPD.Micrometers, 1E-6);
        }

        #region testMeasureCorrection

        [TestMethod]
        public void WarpInThickness_apply_measure_correction_air_gap_up()
        {
            // Given
            var calibrationManager = HardwareManager.Probes["ProbeLiseDouble"].CalibrationManager as ProbeCalibrationManagerLise;

            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            var sampleAndSignalForLastCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 3000.Micrometers(), 3000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp, sampleAndSignalForLastCalibration.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseDown, sampleAndSignalForLastCalibration.SignalLiseDown }, this);
            var thicknessSettings = CreateThicknessSettings(null, null);
            thicknessSettings.HasWarpMeasure = true;
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 0, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 0, airGapUp: 3.Millimeters(), airGapDown: null, totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 1, y: 0, airGapUp: 0.Millimeters(), airGapDown: null, totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 1, y: 0, airGapUp: 4.Millimeters(), airGapDown: null, totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 1, airGapUp: 1.Millimeters(), airGapDown: null, totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 1, airGapUp: 3.Millimeters(), airGapDown: null, totalThicknessMm: 0));

            // To simplify and to be determinist, use the same timestamp for each measure point.
            // This timestamp will be right in the middle of the two calibration

            SetMeasureTimestampRightInTheMiddleOfCalibrations(calibrationManager , result);

            // When computing the result
            var measure = new MeasureThickness();
            measure.ApplyMeasureCorrection(result, thicknessSettings);
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then
            // Measures are done right in the middle of the two calibrations.
            // With a first calibration of 1000µm and a second of 3000µm, we can expect
            // a correction around 1000µm = 1/2 * (3000-1000).
            int expectedCorrectionMillimeters = 1;
            Assert.AreEqual(1 + expectedCorrectionMillimeters, (result.Points[0].Datas[0] as ThicknessPointData).AirGapUp.Millimeters, 1E-1);
            Assert.AreEqual(3 + expectedCorrectionMillimeters, (result.Points[1].Datas[0] as ThicknessPointData).AirGapUp.Millimeters, 1E-1);
            Assert.AreEqual(0 + expectedCorrectionMillimeters, (result.Points[2].Datas[0] as ThicknessPointData).AirGapUp.Millimeters, 1E-1);
            Assert.AreEqual(4 + expectedCorrectionMillimeters, (result.Points[3].Datas[0] as ThicknessPointData).AirGapUp.Millimeters, 1E-1);
            Assert.AreEqual(1 + expectedCorrectionMillimeters, (result.Points[4].Datas[0] as ThicknessPointData).AirGapUp.Millimeters, 1E-1);
            Assert.AreEqual(3 + expectedCorrectionMillimeters, (result.Points[5].Datas[0] as ThicknessPointData).AirGapUp.Millimeters, 1E-1);

            // With same correction on each point, RPDs and Warp should be the same with/without correction.
            Assert.AreEqual(1, result.WarpWaferResults.Count);
            Assert.AreEqual(4.Millimeters(), result.WarpWaferResults[0]);
            Assert.AreEqual(1.Millimeters(), (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(2.Millimeters(), (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-2.Millimeters(), (result.Points[3].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(1.Millimeters(), (result.Points[4].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[5].Datas[0] as ThicknessPointData).WarpResult.RPD);
        }

        [TestMethod]
        public void WarpInThickness_apply_measure_correction_air_gap_down()
        {
            // Given
            var calibrationManager = HardwareManager.Probes["ProbeLiseDouble"].CalibrationManager as ProbeCalibrationManagerLise;

            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            var sampleAndSignalForLastCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 3000.Micrometers(), 3000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp, sampleAndSignalForLastCalibration.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseDown, sampleAndSignalForLastCalibration.SignalLiseDown }, this);
            var thicknessSettings = CreateThicknessSettings(null, null);
            thicknessSettings.HasWarpMeasure = true;
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 0, airGapUp: null, airGapDown: 1.Millimeters(), totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 0, airGapUp: null, airGapDown: 3.Millimeters(), totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 1, y: 0, airGapUp: null, airGapDown: 0.Millimeters(), totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 1, y: 0, airGapUp: null, airGapDown: 4.Millimeters(), totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 1, airGapUp: null, airGapDown: 1.Millimeters(), totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 1, airGapUp: null, airGapDown: 3.Millimeters(), totalThicknessMm: 0));

            // To simplify and to be determinist, use the same timestamp for each measure point.
            // This timestamp will be right in the middle of the two calibration

            SetMeasureTimestampRightInTheMiddleOfCalibrations(calibrationManager, result);

            // When computing the result
            var measure = new MeasureThickness();
            measure.ApplyMeasureCorrection(result, thicknessSettings);
            measure.FinalizeMetroResult(result, thicknessSettings);

            // Then
            // Measures are done right in the middle of the two calibrations.
            // With a first calibration of 1000µm and a second of 3000µm, we can expect
            // a correction around 1000µm = 1/2 * (3000-1000).
            int expectedCorrectionMillimeters = 1;
            Assert.AreEqual(1 + expectedCorrectionMillimeters, (result.Points[0].Datas[0] as ThicknessPointData).AirGapDown.Millimeters, 1E-1);
            Assert.AreEqual(3 + expectedCorrectionMillimeters, (result.Points[1].Datas[0] as ThicknessPointData).AirGapDown.Millimeters, 1E-1);
            Assert.AreEqual(0 + expectedCorrectionMillimeters, (result.Points[2].Datas[0] as ThicknessPointData).AirGapDown.Millimeters, 1E-1);
            Assert.AreEqual(4 + expectedCorrectionMillimeters, (result.Points[3].Datas[0] as ThicknessPointData).AirGapDown.Millimeters, 1E-1);
            Assert.AreEqual(1 + expectedCorrectionMillimeters, (result.Points[4].Datas[0] as ThicknessPointData).AirGapDown.Millimeters, 1E-1);
            Assert.AreEqual(3 + expectedCorrectionMillimeters, (result.Points[5].Datas[0] as ThicknessPointData).AirGapDown.Millimeters, 1E-1);

            // With same correction on each point, RPDs and Warp should be the same with/without correction.
            Assert.AreEqual(1, result.WarpWaferResults.Count);
            Assert.AreEqual(4.Millimeters(), result.WarpWaferResults[0]);
            Assert.AreEqual(1.Millimeters(), (result.Points[0].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[1].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(2.Millimeters(), (result.Points[2].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-2.Millimeters(), (result.Points[3].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(1.Millimeters(), (result.Points[4].Datas[0] as ThicknessPointData).WarpResult.RPD);
            Assert.AreEqual(-1.Millimeters(), (result.Points[5].Datas[0] as ThicknessPointData).WarpResult.RPD);
        }

        [TestMethod]
        public void WarpInThickness_apply_measure_correction_fails_with_only_one_calibration_saved()
        {
            // Given
            var token = new CancellationToken();
            uint minuteBetweenTwoDualLiseCalibration = 10;
            var calibrationManager = new ProbeCalibrationManagerLise("ProbeLiseDouble",token, minuteBetweenTwoDualLiseCalibration);
            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseDown }, this);
            var thicknessSettings = CreateThicknessSettings(null, null);
            thicknessSettings.HasWarpMeasure = true;
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0.5, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>()
            };
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 0, airGapUp: null, airGapDown: 1.Millimeters(), totalThicknessMm: 0));
            result.Points.Add(CreateThicknessPointResult(x: 0, y: 0, airGapUp: null, airGapDown: 3.Millimeters(), totalThicknessMm: 0));

            // To simplify and to be determinist, use the same timestamp for each measure point.
            // This timestamp will be right in the middle of the two calibration
            calibrationManager.GetCalibration(false, "ProbeLiseDouble",null,new PointPosition(0, 0, 0, 0));
            Thread.Sleep(10);

            // When computing the result
            var measure = new MeasureThickness();
            measure.ApplyMeasureCorrection(result, thicknessSettings);

            // Then
            Assert.AreEqual(GlobalState.Error, result.State);
            Assert.AreEqual(MeasureState.NotMeasured, (result.Points[0].Datas[0] as ThicknessPointData).State);
            Assert.AreEqual(MeasureState.NotMeasured, (result.Points[1].Datas[0] as ThicknessPointData).State);
        }

        [TestMethod]
        public void WarpInThickness_apply_measure_correction_fails_without_any_calibration_saved()
        {
            // Given
            var token = new CancellationToken();
            uint minuteBetweenTwoDualLiseCalibration = 10;
            var calibrationManager = new ProbeCalibrationManagerLise("ProbeLiseDouble", token, minuteBetweenTwoDualLiseCalibration);
            var sampleAndSignalForFirstCalibration = CreateSampleAndItsAssociatedSignalForOneLayer(Layer750, 1000.Micrometers(), 1000.Micrometers());
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseUpId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseUp }, this);
            TestWithProbeLiseHelper.AssociateSignalsAtSingleAcquisitionWithFakeProbeLise(LiseBottomId, new List<IProbeSignal> { sampleAndSignalForFirstCalibration.SignalLiseDown }, this);
            var thicknessSettings = CreateThicknessSettings(null, null);
            thicknessSettings.HasWarpMeasure = true;
            var result = new ThicknessResult
            {
                Settings = new ThicknessResultSettings()
                {
                    HasWarpMeasure = true,
                    TotalTarget = new Length(0d, LengthUnit.Millimeter)
                },
                Points = new List<MeasurePointResult>
                {
                    CreateThicknessPointResult(x: 0, y: 0, airGapUp: null, airGapDown: 1.Millimeters(), totalThicknessMm: 0),
                    CreateThicknessPointResult(x: 0, y: 0, airGapUp: null, airGapDown: 3.Millimeters(), totalThicknessMm: 0)
                }
            };

            // To simplify and to be determinist, use the same timestamp for each measure point.
            // This timestamp will be right in the middle of the two calibration
            calibrationManager.GetCalibration(false, "ProbeLiseDouble", null, new PointPosition(0, 0, 0, 0));
            Thread.Sleep(10);

            // When computing the result
            var measure = new MeasureThickness();
            measure.ApplyMeasureCorrection(result, thicknessSettings);

            // Then
            Assert.AreEqual(GlobalState.Error, result.State);
            Assert.AreEqual(MeasureState.NotMeasured, (result.Points[0].Datas[0] as ThicknessPointData).State);
            Assert.AreEqual(MeasureState.NotMeasured, (result.Points[1].Datas[0] as ThicknessPointData).State);
        }

        #endregion testMeasureCorrection

        #region privateMethods

        private void SetMeasureTimestampRightInTheMiddleOfCalibrations(ProbeCalibrationManagerLise calibrationManager, ThicknessResult result)
        {
            calibrationManager.GetCalibration(false, "ProbeLiseDouble", null, new PointPosition(0, 0, 0, 0));
            Thread.Sleep(10);
            var measuresTimestamp = DateTime.UtcNow;
            Thread.Sleep(10);
            calibrationManager.DoLastCalibration();

            var (calib1, calib2) = calibrationManager.GetClosestsCalibrations(measuresTimestamp);
            measuresTimestamp = calib1.Timestamp.AddTicks((calib2.Timestamp.Ticks - calib1.Timestamp.Ticks) / 2);
            foreach (var point in result.Points)
            {
                foreach (var repeat in point.Datas)
                {
                    repeat.Timestamp = measuresTimestamp;
                }
            }
        }

        #endregion privateMethods
    }
}
