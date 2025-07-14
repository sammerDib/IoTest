using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    public static class XYCalibrationCalcul
    {   
        static public bool PreCompute(XYCalibrationData data, InterpolateAlgoType interpolationType = InterpolateAlgoType.QuadNN)
        {
            if (data.WaferCalibrationDiameter == null)
                data.WaferCalibrationDiameter = 300.Millimeters();

            data.UpdateSquaredRadius();
            bool monoInterp = InterpolatorFactory(out Interpolator2D interpSX, out Interpolator2D interpSY, data, interpolationType);
            try
            {
                double[] measuresX = null;
                double[] measuresY = null;
                double[] measuresShiftX = null;
                double[] measuresShiftY = null;

                if (data.Corrections.Count != 0)
                {
                    // Note de RTI for le cout reel en temsp de cette methode si il est necessaire de l'optimiser en incluant du MultiTasking

                    measuresX = data.Corrections.Select(x => x.XTheoricalPosition.GetValueAs(XYCalibrationData.WaferGridUnit)).ToArray();
                    measuresY = data.Corrections.Select(y => y.YTheoricalPosition.GetValueAs(XYCalibrationData.WaferGridUnit)).ToArray();
                    measuresShiftX = data.Corrections.Select(sx => sx.ShiftX.GetValueAs(XYCalibrationData.CorrectionUnit)).ToArray();
                    measuresShiftY = data.Corrections.Select(sy => sy.ShiftY.GetValueAs(XYCalibrationData.CorrectionUnit)).ToArray();
                    if (!monoInterp)
                    {
                        // Shift X
                        if (!interpSX.SetInputsPoints(measuresX, measuresY, measuresShiftX))
                        {
                            Debug.WriteLine($"### XYCalibrationData ERROR : Interpolation SX SetInputsPoints failure ###");
                            throw new Exception("XYCalibrationData Interpolation SX SetInputsPoints failure");
                        }
                        if (!interpSX.ComputeData())
                        {
                            Debug.WriteLine($"### WARNING : Interpolation SX ComputeData failure ###");
                            throw new Exception("XYCalibrationData Interpolation SX ComputeData failure");
                        }

                        // Shift Y
                        if (!interpSY.SetInputsPoints(measuresX, measuresY, measuresShiftY))
                        {
                            Debug.WriteLine($"### XYCalibrationData ERROR : Interpolation SY SetInputsPoints failure ###");
                            throw new Exception("XYCalibrationData Interpolation SY SetInputsPoints failure");
                        }
                        if (!interpSY.ComputeData())
                        {
                            Debug.WriteLine($"### WARNING : Interpolation SY ComputeData failure ###");
                            throw new Exception("XYCalibrationData Interpolation SY ComputeData failure");
                        }
                    }
                    else
                    {
                        // case where interpSX == interpSY
                        // Shift X & Shift Y 
                        if (!interpSX.SetInputsPoints(measuresX, measuresY, measuresShiftX, measuresShiftY))
                        {
                            Debug.WriteLine($"### XYCalibrationData ERROR : Interpolation SXY SetInputsPoints failure ###");
                            throw new Exception("XYCalibrationData Interpolation SXY SetInputsPoints failure");
                        }
                        if (!interpSX.ComputeData())
                        {
                            Debug.WriteLine($"### WARNING : Interpolation SXY ComputeData failure ###");
                            throw new Exception("XYCalibrationData Interpolation SXY ComputeData failure");
                        }
                    }
                }
                data.UpdateInterpolator(interpSX, interpSY);
            }
            catch (Exception ex)
            {
                string smg = ex.Message;

                interpSX.Dispose();
                if (!monoInterp)
                {
                    interpSY.Dispose();
                }

                return false;
            }
            return true;
        }

        static private bool InterpolatorFactory(out Interpolator2D Ix, out Interpolator2D Iy, XYCalibrationData data, InterpolateAlgoType interpolationType, Length targetShift = null)
        {
            bool monoInterp = (interpolationType == InterpolateAlgoType.fNN) || (interpolationType == InterpolateAlgoType.QuadNN);
            Ix = InitInterpolator(data, interpolationType, targetShift);
            if (monoInterp)
               Iy = Ix;
            else
               Iy = InitInterpolator(data, interpolationType, targetShift);
            return monoInterp;
        }

        static private Interpolator2D InitInterpolator(XYCalibrationData data, InterpolateAlgoType interpolationType, Length targetShift = null)
        {
            var interpolator = new Interpolator2D(interpolationType);

            if (interpolationType == InterpolateAlgoType.MBA)
            {
                Length waferDiameter = data.WaferCalibrationDiameter;
                var radius = waferDiameter.GetValueAs(XYCalibrationData.WaferGridUnit) * 0.5;
                var waferoutmargin = XYCalibrationData.OuterExtrapoleMargin.GetValueAs(XYCalibrationData.WaferGridUnit);
                List<double> settings = new List<double>()
                {
                    -radius - waferoutmargin, // Low X
                    -radius - waferoutmargin, // Low Y
                    radius + waferoutmargin,  // Hi X
                    radius + waferoutmargin,  // Hi Y
                    3.0,        // grid m0 - INTEGER coarse grain should be >1 -- the greater coarse is the higher peak will be around scattered data points
                    3.0         // grid n0 - INTEGER coarse grain should be >1 -- most case should be == grid m0 
                };
                if (targetShift != null)
                    settings.Add(targetShift.GetValueAs(XYCalibrationData.CorrectionUnit));
                var settingsarray = settings.ToArray();
                if (!interpolator.InitSettings(settingsarray))
                {
                    throw new Exception("XYCalibrationData Interpolator C MBA Init Settings Failure");
                }
            }
            else if (interpolationType == InterpolateAlgoType.QuadNN)
            {
                double MaximumGridStep_mm = 15.0; // at this time it exist 11.119 mm and 10.0 mm die size, we take some margin
                List<double> settings = new List<double>()
                {
                    data.ShiftAngle.Degrees,
                    data.ShiftX.Millimeters,
                    data.ShiftY.Millimeters,
                    MaximumGridStep_mm
                };
                var settingsarray = settings.ToArray();
                if (!interpolator.InitSettings(settingsarray))
                {
                    throw new Exception("XYCalibrationData Interpolator C QuadNN Init Settings Failure");
                }
            }
            return interpolator;
        }
    }
}
