using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Core.Test.Referential
{
    [TestClass]
    public class XYCalibrationHelperTest
    {
        [TestMethod]
        public void Correction_with_empty_corrections_is_0()
        {
            // Given position
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 5, 10, 15, 20);

            // Given calibration with empty corrections
            var calibrationData = new XYCalibrationData()
            {
                Corrections = new List<Correction>()
            };

            // When computing the correction
            var result = XYCalibrationHelper.ComputeCorrection(initialPosition, calibrationData);

            // Then correction is 0
            Assert.AreEqual(0.0, result.Item1.Millimeters, 1e-6);
            Assert.AreEqual(0.0, result.Item2.Millimeters, 1e-6);
        }

        [TestMethod]
        public void Correction_with_exact_data_point_is_data_correction()
        {
            // Given position
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 5, 10, 15, 20);

            // Given calibration with one correction on the same point as given position
            Length shiftX = 1.Micrometers();
            Length shiftY = 2.Micrometers();
            var calibrationData = new XYCalibrationData()
            {
                Corrections = new List<Correction> {
                    new Correction() {
                    XTheoricalPosition = 0.Millimeters(),
                    YTheoricalPosition = 0.Millimeters(),
                    ShiftX = 0.Millimeters(),
                    ShiftY = 0.Millimeters()
                },
                    new Correction() {
                    XTheoricalPosition = initialPosition.X.Millimeters(),
                    YTheoricalPosition = initialPosition.Y.Millimeters(),
                    ShiftX = shiftX,
                    ShiftY = shiftY
                }}
            };
            Assert.IsTrue(XYCalibrationCalcul.PreCompute(calibrationData, InterpolateAlgoType.fNN));

            // When computing the correction
            var result = XYCalibrationHelper.ComputeCorrection(initialPosition, calibrationData);

            // Then correction is the closest
            Assert.AreEqual(shiftX.Micrometers, result.Item1.Micrometers, 1e-6);
            Assert.AreEqual(shiftY.Micrometers, result.Item2.Micrometers, 1e-6);
        }

        [TestMethod]
        public void Correction_with_non_exact_data_point_is_closest_data_correction()
        {
            // Given position
            var initialPosition = new XYZTopZBottomPosition(new WaferReferential(), 5, 10, 15, 20);

            // Given calibration with no correction on the same point as given position
            Length shiftX = 1.Micrometers();
            Length shiftY = 2.Micrometers();
            var calibrationData = new XYCalibrationData()
            {
                Corrections = new List<Correction> {
                    new Correction() {
                    XTheoricalPosition = initialPosition.X.Millimeters() - 6.Millimeters(),
                    YTheoricalPosition = initialPosition.Y.Millimeters() - 5.Millimeters(),
                    ShiftX = 0.Millimeters(),
                    ShiftY = 0.Millimeters()
                },
                    new Correction() {
                    XTheoricalPosition = initialPosition.X.Millimeters() + 5.Millimeters(),
                    YTheoricalPosition = initialPosition.Y.Millimeters() + 5.Millimeters(),
                    ShiftX = shiftX,
                    ShiftY = shiftY
                }}
            };

            // When computing the correction
            var result = XYCalibrationHelper.ComputeClosestCorrection(initialPosition, calibrationData);

            // Then correction is the closest
            Assert.AreEqual(shiftX.Micrometers, result.Item1.Micrometers, 1e-6);
            Assert.AreEqual(shiftY.Micrometers, result.Item2.Micrometers, 1e-6);
        }

        [TestMethod]
        public void Correction_with_non_exact_data_point_is_interpolate_correction()
        {   
            double PlaneCoefAx = 1.0;   double PlaneCoefAy = 0.0;   
            double PlaneCoefBx = 0.5;   double PlaneCoefBy = 1.0;
            double PlaneCoefCx = -2.0;  double PlaneCoefCy = 3.0;

            double shiftx = 0.0;
            double shifty = 0.0;

            var corrections = new List<Correction>();
            
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    shiftx = i * PlaneCoefAx + j * PlaneCoefBx + PlaneCoefCx;
                    shifty = i * PlaneCoefAy + j * PlaneCoefBy + PlaneCoefCy;

                    corrections.Add(new Correction() {
                        XTheoricalPosition = i.Millimeters(),
                        YTheoricalPosition = j.Millimeters(),
                        ShiftX = shiftx.Millimeters(),
                        ShiftY = shifty.Millimeters()
                    });;
                }
            }

            var calibrationData = new XYCalibrationData() { Corrections = corrections, WaferCalibrationDiameter = new Length(4, LengthUnit.Millimeter),
                ShiftAngle = 0.Degrees(), ShiftX = 0.Micrometers(), ShiftY = 0.Micrometers() };
            bool bPrecomputeSuccess = XYCalibrationCalcul.PreCompute(calibrationData, InterpolateAlgoType.QuadNN);
            Assert.IsTrue(bPrecomputeSuccess, "Pre compute should have succeeded");


            // point sur grid
            var Position = new XYZTopZBottomPosition(new WaferReferential(), 1.0, 1.0, 15, 20);
            shiftx = Position.X * PlaneCoefAx + Position.Y * PlaneCoefBx + PlaneCoefCx;
            shifty = Position.X * PlaneCoefAy + Position.Y * PlaneCoefBy + PlaneCoefCy;
            var result = XYCalibrationHelper.ComputeInterpolateCorrection(Position, calibrationData);
            Assert.AreEqual(shiftx, result.Item1.Millimeters, 1e-6);
            Assert.AreEqual(shifty, result.Item2.Millimeters, 1e-6);

            // point hors-grid - point le plus eloigné d'un point de grid (dist = 0.707)
            Position = new XYZTopZBottomPosition(new WaferReferential(), 1.5, 1.5, 15, 20);
            shiftx = Position.X * PlaneCoefAx + Position.Y * PlaneCoefBx + PlaneCoefCx;
            shifty = Position.X * PlaneCoefAy + Position.Y * PlaneCoefBy + PlaneCoefCy;
            result = XYCalibrationHelper.ComputeInterpolateCorrection(Position, calibrationData);

            //0.2mm sur un pitch de 1mm soit 20% d'erreur de gap entre pitch
            Assert.AreEqual(shiftx, result.Item1.Millimeters, 1e-6);
            Assert.AreEqual(shifty, result.Item2.Millimeters, 1e-6);

            // point hors-grid 2 - point un peu moins eloigné autre quadrant
            Position = new XYZTopZBottomPosition(new WaferReferential(), -1.25, -0.75, 15, 20);
            shiftx = Position.X * PlaneCoefAx + Position.Y * PlaneCoefBx + PlaneCoefCx;
            shifty = Position.X * PlaneCoefAy + Position.Y * PlaneCoefBy + PlaneCoefCy;
            result = XYCalibrationHelper.ComputeInterpolateCorrection(Position, calibrationData);

            //0.2mm sur un pitch de 1mm soit 20% d'erreur de gap entre pitch
            Assert.AreEqual(shiftx, result.Item1.Millimeters, 1e-6);
            Assert.AreEqual(shifty, result.Item2.Millimeters, 1e-6);

            // point hors-grid 3- autre quadrant
            Position = new XYZTopZBottomPosition(new WaferReferential(), -1.45, 1.55, 15, 20);
            shiftx = Position.X * PlaneCoefAx + Position.Y * PlaneCoefBx + PlaneCoefCx;
            shifty = Position.X * PlaneCoefAy + Position.Y * PlaneCoefBy + PlaneCoefCy;
            result = XYCalibrationHelper.ComputeInterpolateCorrection(Position, calibrationData);

            //0.2mm sur un pitch de 1mm soit 20% d'erreur de gap entre pitch
            Assert.AreEqual(shiftx, result.Item1.Millimeters, 1e-6);
            Assert.AreEqual(shifty, result.Item2.Millimeters, 1e-6);

        }
    }
}
