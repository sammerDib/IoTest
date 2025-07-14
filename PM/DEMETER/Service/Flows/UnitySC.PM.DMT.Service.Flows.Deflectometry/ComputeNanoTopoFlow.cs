using System.IO;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class ComputeNanoTopoFlow : FlowComponent<ComputeNanoTopoInput, ComputeNanoTopoResult, ComputeNanoTopoConfiguration>
    {
        public const int MaximumNumberOfSteps = 3;
        private readonly ICalibrationManager _calibrationManager;

        public ComputeNanoTopoFlow(ComputeNanoTopoInput input, ICalibrationManager calibrationManager)
            : base(input, "ComputeNanoTopoFlow")
        {
            _calibrationManager = calibrationManager;
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting");

            var cameraCalibResult = _calibrationManager.GetGlobalTopoCameraCalibrationResultBySide(Input.Side);
            var systemCalibResult = _calibrationManager.GetGlobalTopoSystemCalibrationResultBySide(Input.Side);

            var cameraIntrinsic = ArrayUtils.JaggedArrayTo2D(cameraCalibResult.CameraIntrinsic.Values);

            var rWaferToCamera = ArrayUtils.JaggedArrayTo2D(systemCalibResult.ExtrinsicCamera.RWaferToCamera.Values);
            var tWaferToCamera = systemCalibResult.ExtrinsicCamera.TWaferToCamera;

            var rScreenToWafer = ArrayUtils.JaggedArrayTo2D(systemCalibResult.ExtrinsicScreen.RScreenToWafer.Values);
            var tScreenToWafer = systemCalibResult.ExtrinsicScreen.TScreenToWafer;
            var calibrationParams = new CalibrationParameters(cameraIntrinsic, cameraCalibResult.Distortion, cameraCalibResult.RMS);
            var extrinsicCameraParams = new ExtrinsicCameraParameters(rWaferToCamera, tWaferToCamera);
            var extrinsicScreenParams = new ExtrinsicScreenParameters(rScreenToWafer, tScreenToWafer);

            int smallestPeriod = Input.Periods[0];
            
            CheckCancellation();
            NanoTopography.ComputeNanoTopography(Input.UnwrappedX, Input.UnwrappedY, Input.Mask, calibrationParams, extrinsicCameraParams,
                extrinsicScreenParams, (float)Input.ScreenPixelSize, smallestPeriod, ReportFolder);

            SaveReportIfNeeded();
            CheckCancellation();
            SetProgressMessage($"Successfully computed NanoTopo");
        }

        private void SaveReportIfNeeded()
        {
            if (Configuration.IsAnyReportEnabled())
            {
                CheckCancellation();
                using (var UnwrappedX = Input.UnwrappedX.ConvertToUSPImageMil(false))
                {
                    UnwrappedX.Save(Path.Combine(ReportFolder, $"UnwrappedX.tif"));
                }

                CheckCancellation();
                using (var UnwrappedY = Input.UnwrappedY.ConvertToUSPImageMil(false))
                {
                    UnwrappedY.Save(Path.Combine(ReportFolder, $"UnwrappedY.tif"));
                }
            }
        }
    }
}
