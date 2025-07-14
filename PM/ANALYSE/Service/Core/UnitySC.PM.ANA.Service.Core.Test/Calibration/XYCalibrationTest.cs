using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Referential;
using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.XYCalibration;
using UnitySC.Shared.Format.XYCalibration;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Test.Calibration
{
    internal static class XYCalibrationResultFactory
    {
        public static MeasurePointResult Build(double x, double y, Length shiftX, Length shiftY)
        {
            return new MeasurePointResult()
            {
                State = MeasureState.Success,
                XPosition = x,
                YPosition = y,
                Datas = new List<MeasurePointDataResultBase> {
                    new XYCalibrationPointData()
                    {
                        ShiftX = shiftX,
                        ShiftY = shiftY,
                        State = MeasureState.Success
                    }
                }
            };
        }
    }

    [TestClass]
    public class XYCalibrationTest
    {
        private static readonly double s_precision = 8.Nanometers().Millimeters;

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            Bootstrapper.Register(container);
            // Set up the referential manager to return given pos
            Bootstrapper.SimulatedReferentialManager.Setup(_ => _.ConvertTo(It.Is<PositionBase>(pos => pos.Referential.Tag == ReferentialTag.Wafer), ReferentialTag.Stage))
            .Returns((PositionBase pos, ReferentialTag _) => pos);
            Bootstrapper.SimulatedReferentialManager.Setup(_ => _.ConvertTo(It.Is<PositionBase>(pos => pos.Referential.Tag == ReferentialTag.Wafer), ReferentialTag.Motor))
           .Returns((PositionBase pos, ReferentialTag _) => pos);
        }

        [TestMethod]
        public void CalibrationResultWithNullPointsThrows()
        {
            // Given a result with null points
            var measureResult = new XYCalibrationResult()
            {
                Points = null
            };

            // When computing the calibration
            var exception = Assert.ThrowsException<ArgumentException>(() => XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 100.Millimeters()));

            // Then it throws
            Assert.AreEqual("The xy calibration measure result should have a list of points or dies.", exception.Message);
        }

        [TestMethod]
        public void CalibrationResultWithTwoPointsThrows()
        {
            // Given a result with one point
            var measureResult = new XYCalibrationResult()
            {
                Points = new List<MeasurePointResult> {
                    XYCalibrationResultFactory.Build(0.0, 0.0, 1.Millimeters(), 2.Millimeters()),
                    XYCalibrationResultFactory.Build(1.0, 0.0, 1.Millimeters(), 2.Millimeters())
                }
            };

            // When computing the calibration
            var exception = Assert.ThrowsException<ArgumentException>(() => XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 100.Millimeters()));

            // Then it throws
            Assert.AreEqual("The xy calibration measure result should have at least 3 successful points.", exception.Message);
        }

        [TestMethod]
        public void CalibrationResultWithOnlyOneSuccessPointThrows()
        {
            // Given a result with one point
            var measureResult = new XYCalibrationResult()
            {
                Points = new List<MeasurePointResult> {
                    new MeasurePointResult() { State = MeasureState.Error },
                    XYCalibrationResultFactory.Build(0.0, 0.0, 1.Millimeters(), 2.Millimeters()),
                    XYCalibrationResultFactory.Build(1.0, 0.0, 1.Millimeters(), 2.Millimeters()),
                }
            };

            // When computing the calibration
            var exception = Assert.ThrowsException<ArgumentException>(() => XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 100.Millimeters()));

            // Then it throws
            Assert.AreEqual("The xy calibration measure result should have at least 3 successful points.", exception.Message);
        }

        [TestMethod]
        public void CalibrationResultWithInfeasibleInputThrows()
        {
            // Given an infeasible result with the same point but that has different shifts
            var measureResult = new XYCalibrationResult()
            {
                Points = new List<MeasurePointResult> {
                    XYCalibrationResultFactory.Build(0.0, 0.0, 1.Millimeters(), 2.Millimeters()),
                    XYCalibrationResultFactory.Build(0.0, 0.0, -1.Millimeters(), -1.Millimeters()),
                    XYCalibrationResultFactory.Build(0.0, 0.0, 1.Micrometers(), 2.Micrometers()),
                }
            };

            // When computing the calibration
            var exception = Assert.ThrowsException<Exception>(() => XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 100.Millimeters()));

            // Then it throws
            Assert.AreEqual("Could not compute the rotation/translation of the wafer for the xy-calibration", exception.Message);
        }

        [TestMethod]
        public void CalibrationResultWithThreeSuccessPointOnXAxisFindsParametersAndCorrections()
        {
            // Given a result with two points aligned on X axis
            var measureResult = new XYCalibrationResult()
            {
                Points = new List<MeasurePointResult> {
                    XYCalibrationResultFactory.Build(0.0, 0.0, 1.Micrometers(), 2.Micrometers()),
                    XYCalibrationResultFactory.Build(1.0, 0.0, 1.Micrometers(), 3.Micrometers()),
                    XYCalibrationResultFactory.Build(2.0, 0.0, 1.Micrometers(), 4.Micrometers()),
                }
            };

            // When computing the calibration
            XYCalibrationData calibration = XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 300.Millimeters());

            // Then it finds the good parameters and correcitons
            calibration.UncomputableCorrections.Should().BeEmpty();
            calibration.ShiftAngle.Radians.Should().BeApproximately(0.001, s_precision);
            calibration.ShiftX.Millimeters.Should().BeApproximately(0.001, s_precision);
            calibration.ShiftY.Millimeters.Should().BeApproximately(0.002, s_precision);
            calibration.Corrections.Count.Should().Be(3);
            for (int i = 0; i < calibration.Corrections.Count; i++)
            {
                Correction correction = calibration.Corrections[i];
                var resultPoint = measureResult.Points[i];
                correction.XTheoricalPosition.Millimeters.Should().BeApproximately(resultPoint.XPosition, s_precision);
                correction.YTheoricalPosition.Millimeters.Should().BeApproximately(resultPoint.YPosition, s_precision);
                correction.ShiftX.Millimeters.Should().BeApproximately(0.0, s_precision);
                correction.ShiftY.Millimeters.Should().BeApproximately(0.0, s_precision);
            }
        }

        [TestMethod]
        public void CalibrationResultProperlyFillsUncomputableCorrections()
        {
            // Given a result with one error point
            var measureResult = new XYCalibrationResult()
            {
                Points = new List<MeasurePointResult> {
                    XYCalibrationResultFactory.Build(0.0, 0.0, 1.Micrometers(), 2.Micrometers()),
                    XYCalibrationResultFactory.Build(1.0, 0.0, 1.Micrometers(), 3.Micrometers()),
                    XYCalibrationResultFactory.Build(2.0, 0.0, 1.Micrometers(), 4.Micrometers()),
                    new MeasurePointResult() { State = MeasureState.Error, XPosition = 1.5, YPosition = 2.3 },
                }
            };

            // When computing the calibration
            XYCalibrationData calibration = XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 300.Millimeters());

            // Then it fills the uncomputable corrections
            calibration.UncomputableCorrections.Should().HaveCount(1);
            calibration.UncomputableCorrections[0].Should().Be(new XYPosition(new WaferReferential(), 1.5, 2.3));
        }

        [TestMethod]
        public void CalibrationResultWithThreeSuccessPointOnXAxisFindsParametersAndCorrections_OnStageReferential()
        {
            // Given a result with two points aligned on X axis
            var measureResult = new XYCalibrationResult()
            {
                Points = new List<MeasurePointResult> {
                    XYCalibrationResultFactory.Build(0.0, 0.0, 1.Micrometers(), 2.Micrometers()),
                    XYCalibrationResultFactory.Build(1.0, 0.0, 1.Micrometers(), 3.Micrometers()),
                    XYCalibrationResultFactory.Build(2.0, 0.0, 1.Micrometers(), 4.Micrometers()),
                }
            };

            // Given referential parameters for the wafer/stage conversion
            var waferSettings = new WaferReferentialSettings()
            {
                ShiftX = 7.Micrometers(),
                ShiftY = 8.Micrometers()
            };
            Bootstrapper.SimulatedReferentialManager.Setup(_ => _.ConvertTo(It.Is<PositionBase>(pos => pos.Referential.Tag == ReferentialTag.Wafer), ReferentialTag.Motor))
                .Returns((PositionBase pos, ReferentialTag _) =>
                {
                    var res = pos.ToXYZTopZBottomPosition();
                    res.X += waferSettings.ShiftX.Millimeters;
                    res.Y += waferSettings.ShiftY.Millimeters;
                    return res;
                });

            // When computing the calibration
            XYCalibrationData calibration = XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 300.Millimeters());

            // Then it finds the good parameters and corrections on the stage referential
            calibration.UncomputableCorrections.Should().BeEmpty();
            calibration.ShiftAngle.Radians.Should().BeApproximately(0.001, s_precision);
            calibration.ShiftX.Millimeters.Should().BeApproximately(0.001, s_precision);
            calibration.ShiftY.Millimeters.Should().BeApproximately(0.002, s_precision);
            calibration.Corrections.Count.Should().Be(3);
            for (int i = 0; i < calibration.Corrections.Count; i++)
            {
                Correction correction = calibration.Corrections[i];
                var resultPoint = measureResult.Points[i];
                correction.XTheoricalPosition.Millimeters.Should().BeApproximately(resultPoint.XPosition + waferSettings.ShiftX.Millimeters, s_precision);
                correction.YTheoricalPosition.Millimeters.Should().BeApproximately(resultPoint.YPosition + waferSettings.ShiftY.Millimeters, s_precision);
                correction.ShiftX.Millimeters.Should().BeApproximately(0.0, s_precision);
                correction.ShiftY.Millimeters.Should().BeApproximately(0.0, s_precision);
            }
        }

        [TestMethod]
        public void CalibrationResultNominalCase()
        {
            // Given a result with points on each axis
            var expectedRotation = 0.005.Radians();
            var expectedXShift = 7.Micrometers();
            var expectedYShift = 2.Micrometers();
            var oneMinusCosRotation = (1 - Math.Cos(expectedRotation.Radians)).Millimeters();
            var sinRotation = Math.Sin(expectedRotation.Radians).Millimeters();
            Length[] expectedXCorrections = { 50.Nanometers(), 12.Nanometers(), -23.Nanometers(), 7.Nanometers(), -17.Nanometers() };
            Length[] expectedYCorrections = { 10.Nanometers(), -21.Nanometers(), -16.Nanometers(), -13.Nanometers(), 36.Nanometers() };
            var measureResult = new XYCalibrationResult()
            {
                Points = new List<MeasurePointResult> {
                    XYCalibrationResultFactory.Build(0.0, 0.0, expectedXShift + expectedXCorrections[0], expectedYShift + expectedYCorrections[0]),
                    XYCalibrationResultFactory.Build(1.0, 0.0, expectedXShift - oneMinusCosRotation + expectedXCorrections[1], expectedYShift + sinRotation +expectedYCorrections[1]),
                    XYCalibrationResultFactory.Build(0.0, 1.0, expectedXShift - sinRotation + expectedXCorrections[2], expectedYShift - oneMinusCosRotation +expectedYCorrections[2]),
                    XYCalibrationResultFactory.Build(-1.0, 0.0, expectedXShift + oneMinusCosRotation + expectedXCorrections[3], expectedYShift - sinRotation+ expectedYCorrections[3]),
                    XYCalibrationResultFactory.Build(0.0, -1.0, expectedXShift + sinRotation + expectedXCorrections[4], expectedYShift + oneMinusCosRotation+ expectedYCorrections[4]),
                }
            };

            // When computing the calibration
            XYCalibrationData calibration = XYCalibration.ComputeCalibration<XYCalibrationData>(measureResult, 300.Millimeters());

            // Then it finds the good parameters and correcitons
            calibration.UncomputableCorrections.Should().BeEmpty();
            calibration.ShiftX.Millimeters.Should().BeApproximately(expectedXShift.Millimeters, s_precision);
            calibration.ShiftY.Millimeters.Should().BeApproximately(expectedYShift.Millimeters, s_precision);
            calibration.ShiftAngle.Radians.Should().BeApproximately(expectedRotation.Radians, s_precision);
            calibration.Corrections.Count.Should().Be(5);
            for (int i = 0; i < calibration.Corrections.Count; i++)
            {
                Correction correction = calibration.Corrections[i];
                var resultPoint = measureResult.Points[i];
                correction.XTheoricalPosition.Millimeters.Should().BeApproximately(resultPoint.XPosition, s_precision);
                correction.YTheoricalPosition.Millimeters.Should().BeApproximately(resultPoint.YPosition, s_precision);
                // Estimated shifts are not exactly the one provided because currently we compute the transformation parameters
                // with a potential scale. Plus we only have a few points. With more points it should be better.
                correction.ShiftX.Millimeters.Should().BeApproximately(expectedXCorrections[i].Millimeters, s_precision);
                correction.ShiftY.Millimeters.Should().BeApproximately(expectedYCorrections[i].Millimeters, s_precision);
            }
        }
    }
}
