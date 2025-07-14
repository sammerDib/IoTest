using System;
using System.Linq;

using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Calibration
{
    public class SystemUniformityCalibrationFlow : FlowComponent
        <SystemUniformityCalibrationInput,
        SystemUniformityCalibrationResult,
        SystemUniformityCalibrationConfiguration>
    {
        public SystemUniformityCalibrationFlow(SystemUniformityCalibrationInput input)
            : base(input, "SystemUniformityCalibrationFlow")
        {
        }

        protected override void Process()
        {
            var brightField = Input.BrightFieldImage;
            var waferRadiusInMm = Input.WaferRadiusInMm;
            var pixelSizeInMm = Input.PixelSizeInMm;
            var cameraCalibResult = Input.GlobalTopoCameraCalibResult;
            var systemCalibResult = Input.GlobalTopoSystemCalibResult;
            var fresnelCoeffs = Input.FresnelCoefficients;
            var screenWavelengthPeaks = Input.ScreenWavelengthPeaks;
            var polarisation = Input.Polarisation;

            var cameraIntrinsic = ArrayUtils.JaggedArrayTo2D(cameraCalibResult.CameraIntrinsic.Values);

            var rWaferToCamera = ArrayUtils.JaggedArrayTo2D(systemCalibResult.ExtrinsicCamera.RWaferToCamera.Values);
            var tWaferToCamera = systemCalibResult.ExtrinsicCamera.TWaferToCamera;

            var rScreenToWafer = ArrayUtils.JaggedArrayTo2D(systemCalibResult.ExtrinsicScreen.RScreenToWafer.Values);
            var tScreenToWafer = systemCalibResult.ExtrinsicScreen.TScreenToWafer;

            bool useVerticalPolarisation;

            switch(polarisation)
            {
                case UnitySC.Shared.Data.Enum.Polarisation.Vertical:
                    useVerticalPolarisation = true;
                    break;
                case UnitySC.Shared.Data.Enum.Polarisation.Horizontal:
                    useVerticalPolarisation = false;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var calibrationParams = new CalibrationParameters(cameraIntrinsic, cameraCalibResult.Distortion, cameraCalibResult.RMS);
            var extrinsicCameraParams = new ExtrinsicCameraParameters(rWaferToCamera, tWaferToCamera);
            var extrinsicScreenParams = new ExtrinsicScreenParameters(rScreenToWafer, tScreenToWafer);

            var refractiveIndexByWavelength = fresnelCoeffs.Coefficients.ToDictionary(fresnel => fresnel.WaveLength, fresnel
                                                                                      => new Tuple<double, double>(fresnel.Coefficient.Real, fresnel.Coefficient.Imaginary));

            var screenWaveLengths = screenWavelengthPeaks.Select(w => (int)w.Nanometers).ToArray();

            Result.UnifomityCorrection = PSDCalibration.CalibrateUniformityCorrection(brightField, waferRadiusInMm, pixelSizeInMm,
                calibrationParams, extrinsicCameraParams, extrinsicScreenParams,
                refractiveIndexByWavelength, screenWaveLengths, Configuration.PolynomialFitPatternThreshold,
                useVerticalPolarisation, ReportFolder);

        }

    }
}
