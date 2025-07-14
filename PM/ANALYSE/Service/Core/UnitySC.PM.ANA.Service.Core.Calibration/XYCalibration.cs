using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.XYCalibration;
using UnitySC.Shared.Format.XYCalibration;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.Calibration
{
    public static class XYCalibration
    {
        private class CalibrationPoint
        {
            //point measured and reference are expressed in motor referential
            public CalibrationPoint(MeasurePointResult calibrationResult, DieIndex die = null)
            {
                ReferentialBase referential = new WaferReferential();
                if (!(die is null))
                    referential = new DieReferential(die.Column, die.Row);

                var initialPosition = new XYPosition(referential, calibrationResult.XPosition, calibrationResult.YPosition);
                XYPosition motorPosition = ClassLocator.Default.GetInstance<IReferentialManager>().ConvertTo(initialPosition.ToXYZTopZBottomPosition(), ReferentialTag.Motor).ToXYPosition();

                Reference = new Point2d(motorPosition.X, motorPosition.Y);
                var measuredPoint = calibrationResult.Datas[0] as XYCalibrationPointData;
                if (measuredPoint == null || measuredPoint.ShiftX == null || measuredPoint.ShiftY == null)
                    // for robustness purpose only to check on a longer-life basis,
                    // if we get here a null measure or shift this a non - sense since the measure has been tag as successful
                    // this is either a simulation or an error made earlier while this it has been tagged as successful
                    Measured = new Point2d(motorPosition.X, motorPosition.Y);
                else
                    Measured = new Point2d(motorPosition.X + measuredPoint.ShiftX.Millimeters, motorPosition.Y + measuredPoint.ShiftY.Millimeters);

                //System.Diagnostics.Debug.WriteLine($"CalibrationPoint : {die.Column} | {die.Row}) -- [{Reference.X} {Reference.Y}] => [{Measured.X} {Measured.Y}] ");
                //System.Diagnostics.Debug.WriteLine($"CalibrationPoint : {die.Column} | {die.Row}) SHIFT [{measuredPoint.ShiftX} {measuredPoint.ShiftY}]");
            }

            public Point2d Reference { get; set; }
            public Point2d Measured { get; set; }
        }

        public static TXYCalibrationData ComputeCalibration<TXYCalibrationData>(XYCalibrationResult measureResult, Length waferCalibrationDiameter, bool searchOptimalTransform = true)
            where TXYCalibrationData : XYCalibrationData, new()
        {
            var successfulCalibraitonPoints = ExtractSuccessfulCalibrationPoints(measureResult);

            if (successfulCalibraitonPoints is null)
                throw new ArgumentException("The xy calibration measure result should have a list of points or dies.");

            if (successfulCalibraitonPoints.Count() < 3)
                throw new ArgumentException("The xy calibration measure result should have at least 3 successful points.");

            TransformationParameters transformationParameters = null;
            if (searchOptimalTransform)
            {
                transformationParameters = PointsDetector.OptimalTransformationParameters(successfulCalibraitonPoints.Select(p => p.Reference).ToArray(), successfulCalibraitonPoints.Select(p => p.Measured).ToArray());
            }
            else
            {
                transformationParameters = new TransformationParameters() { RotationRad = 0.0, Scale = 1.0 };
                transformationParameters.Translation.X = 0.0;
                transformationParameters.Translation.Y = 0.0;
            }

            if (transformationParameters is null)
            {
                throw new Exception("Could not compute the rotation/translation of the wafer for the xy-calibration");
            }

            var result = new TXYCalibrationData();

            result.WaferCalibrationDiameter = waferCalibrationDiameter;

            result.ShiftAngle = transformationParameters.RotationRad.Radians().ToUnit(AngleUnit.Degree);
            result.ShiftX = transformationParameters.Translation.X.Millimeters();
            result.ShiftY = transformationParameters.Translation.Y.Millimeters();
            result.Corrections = new List<Correction>();
            var rotationCenter = new XYPosition(null, 0.0, 0.0);
            foreach (var calibrationPoint in successfulCalibraitonPoints)
            {
                var theoreticalPoint = calibrationPoint.Reference;
                var measuredPoint = calibrationPoint.Measured;
                var unshiftedPoint = new XYPosition(null, measuredPoint.X - result.ShiftX.Millimeters, measuredPoint.Y - result.ShiftY.Millimeters);
                MathTools.ApplyAntiClockwiseRotation(-result.ShiftAngle, unshiftedPoint, rotationCenter);

                result.Corrections.Add(new Correction()
                {
                    XTheoricalPosition = theoreticalPoint.X.Millimeters(),
                    YTheoricalPosition = theoreticalPoint.Y.Millimeters(),
                    ShiftX = (unshiftedPoint.X - theoreticalPoint.X).Millimeters().ToUnit(LengthUnit.Micrometer),
                    ShiftY = (unshiftedPoint.Y - theoreticalPoint.Y).Millimeters().ToUnit(LengthUnit.Micrometer)
                });
            }
            result.UncomputableCorrections = ExtractUncomputableCorrections(measureResult);
            if (result is XYCalibrationTest resultTest)
                FillBadPoints(resultTest);
            return result;
        }

        private static IEnumerable<CalibrationPoint> ExtractSuccessfulCalibrationPoints(XYCalibrationResult measureResult)
        {
            if (!(measureResult.Points is null))
                return measureResult.Points.Where(p => p.State == MeasureState.Success).Select(p => new CalibrationPoint(p));

            // WARNING: below, we must convert the die Column/RowIndex back from the wafermap dieReference in order to come back to a usable die index
            if (!(measureResult.Dies is null))
                return measureResult.Dies.SelectMany(
                    d => d.Points.Where(p => p.State == MeasureState.Success)
                                 .Select(p => new CalibrationPoint(p, new DieIndex(DieIndexConverter.ConvertColumnFromDieReference(d.ColumnIndex, measureResult.DiesMap.DieReferenceColumnIndex),
                                                                                   DieIndexConverter.ConvertRowFromDieReference(d.RowIndex, measureResult.DiesMap.DieReferenceRowIndex)))));

            return null;
        }

        private static List<XYPosition> ExtractUncomputableCorrections(XYCalibrationResult measureResult)
        {
            if (!(measureResult.Points is null))
                return measureResult.Points.Where(p => p.State != MeasureState.Success).Select(p => new XYPosition(new WaferReferential(), p.XPosition, p.YPosition)).ToList();

            if (!(measureResult.Dies is null))
                return measureResult.Dies.SelectMany(
                    d => d.Points.Where(p => p.State != MeasureState.Success)
                                 .Select(p => new XYPosition(new DieReferential(DieIndexConverter.ConvertColumnFromDieReference(d.ColumnIndex, measureResult.DiesMap.DieReferenceColumnIndex),
                                                                                DieIndexConverter.ConvertRowFromDieReference(d.RowIndex, measureResult.DiesMap.DieReferenceRowIndex)), p.WaferRelativeXPosition, p.WaferRelativeYPosition))).ToList();

            return new List<XYPosition>();
        }

        private static void FillBadPoints(XYCalibrationTest calibrationData)
        {
            Length precision = ClassLocator.Default.GetInstance<AnaHardwareManager>().Axes.AxesConfiguration.Precision;
            foreach (var correction in calibrationData.Corrections)
            {
                var shiftedDistance = Math.Sqrt(correction.ShiftX.Micrometers * correction.ShiftX.Micrometers + correction.ShiftY.Micrometers * correction.ShiftY.Micrometers).Micrometers();
                if (shiftedDistance > precision)
                {
                    calibrationData.BadPoints.Add(correction);
                }
            }
        }
    }
}
