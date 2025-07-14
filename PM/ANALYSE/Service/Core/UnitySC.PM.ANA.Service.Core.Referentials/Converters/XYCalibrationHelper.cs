// uncomment below to use closest correction method instead of MBA interpolation
//#define USE_CLOSEST_CORRECTION

using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Referentials.Converters
{
    public static class XYCalibrationHelper
    {
        public static Tuple<Length, Length> ComputeCorrection(XYZTopZBottomPosition position, XYCalibrationData calibrationData)
        {

#if USE_CLOSEST_CORRECTION
            return ComputeClosestCorrection(position, calibrationData);
#else
            return ComputeInterpolateCorrection(position, calibrationData);
#endif
        }

        public static Tuple<Length, Length> ComputeCorrection(double positionX, double positionY, XYCalibrationData calibrationData)
        {
            return ComputeCorrection(new XYZTopZBottomPosition() { X = positionX, Y = positionY }, calibrationData);
        }

        #region Interpolate Correction
        public static Tuple<Length, Length> ComputeInterpolateCorrection(XYZTopZBottomPosition position, XYCalibrationData calibrationData)
        {
            if (!calibrationData.IsInterpReady)
            {
                // Can happen if the calibration is empty
                if (calibrationData.Corrections == null || calibrationData.Corrections.Count == 0)
                    return new Tuple<Length, Length>(0.Micrometers(), 0.Micrometers());

                if (!XYCalibrationCalcul.PreCompute(calibrationData))
                    throw new Exception($"xyCalibration Pre computation fail"); 
            }

            if (double.IsNaN(position.X) || double.IsNaN(position.Y))
                return new Tuple<Length, Length>(0.Micrometers(), 0.Micrometers());

            // XY position nous renverrai des millimètres
            return calibrationData.GetInterpolateCorrection(new Length(position.X, LengthUnit.Millimeter), new Length(position.Y, LengthUnit.Millimeter));
        }
        #endregion

        #region Closest Correction
        public static Tuple<Length, Length> ComputeClosestCorrection(XYZTopZBottomPosition position, XYCalibrationData calibrationData)
        {
            Correction closestCorrection = FindClosestCorrection(position, calibrationData.Corrections);

            // Can happen if given a position with NaN values, or if the calibration is empty
            if (closestCorrection is null)
                return new Tuple<Length, Length>(0.Millimeters(), 0.Millimeters());

            return new Tuple<Length, Length>(closestCorrection.ShiftX, closestCorrection.ShiftY);

        }

        private static Correction FindClosestCorrection(XYZTopZBottomPosition position, List<Correction> corrections)
        {
            Correction closestCorrection = null;
            double minDist = double.PositiveInfinity;
            foreach (Correction correction in corrections)
            {
                double xDiff = (correction.XTheoricalPosition.Millimeters - position.X);
                double yDiff = (correction.YTheoricalPosition.Millimeters - position.Y);
                double distance = xDiff * xDiff + yDiff * yDiff;
                if (distance < minDist)
                {
                    closestCorrection = correction;
                    minDist = distance;
                }
            }
            return closestCorrection;
        }
        #endregion

    }
}
