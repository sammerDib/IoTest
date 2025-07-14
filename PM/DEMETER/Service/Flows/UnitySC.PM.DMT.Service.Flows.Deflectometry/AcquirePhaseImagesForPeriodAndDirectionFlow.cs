using System;
using System.IO;
using System.Linq;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Camera;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class AcquirePhaseImagesForPeriodAndDirectionFlow : FlowComponent<AcquirePhaseImagesForPeriodAndDirectionInput, AcquirePhaseImagesForPeriodAndDirectionResult, AcquirePhaseImagesForPeriodAndDirectionConfiguration>
    {
        private readonly CameraBase _camera;
        private readonly ScreenBase _screen;
        private readonly IDMTInternalCameraMethods _cameraManager;
        private readonly IFringeManager _fringeManager;

        public const int NumberOfStepsPerImage = 2;
        public const int NumberOfImageIndependentSteps = 2;

        public AcquirePhaseImagesForPeriodAndDirectionFlow(AcquirePhaseImagesForPeriodAndDirectionInput input, DMTHardwareManager hardwareManager, IDMTInternalCameraMethods cameraService, IFringeManager fringeManager) : base(input, "AcquirePhaseImagesForPeriodAndDirectionFlow")
        {
            _camera = hardwareManager.CamerasBySide[input.AcquisitionSide];
            _screen = hardwareManager.ScreensBySide[input.AcquisitionSide];
            _cameraManager = cameraService;
            _fringeManager = fringeManager;
        }

        protected override void Process()
        {
            string fringeDisplacementDirectionString =
                Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection);
            SetProgressMessage($"Starting {Name} for {Input.AcquisitionSide}side fringes in {fringeDisplacementDirectionString} direction with {Input.Period}px period");
            var fringeImages =
                _fringeManager.GetFringeImageDict(Input.AcquisitionSide, Input.Fringe)[Input.FringesDisplacementDirection][Input.Period];
            _cameraManager.SetExposureTime(_camera, Input.ExposureTimeMs);
            var imageAcqWaitTime = (int)(1000 / _camera.GetFrameRate());
            Result.Fringe = Input.Fringe;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;
            Result.Period = Input.Period;
            Result.ExposureTimeMs = Input.ExposureTimeMs;
            Result.TemporaryResults = fringeImages.Select((image, index) =>
            {
                CheckCancellation();
                SetProgressMessage(
                    $"Acquiring fringe image #{index + 1} in {fringeDisplacementDirectionString} direction for fringes with {Input.Period}px period");
                _screen.DisplayImage(image);
                
                CheckCancellation();
                var cameraImage = _cameraManager.GrabNextImage(_camera).ToServiceImage();
                SetProgressMessage(
                    $"Acquired fringe image #{index + 1} in {fringeDisplacementDirectionString} direction for fringes with {Input.Period}px period");
                if (Configuration.IsAnyReportEnabled())
                {
                    CheckCancellation();
                    cameraImage.SaveToFile(Path.Combine(ReportFolder, $"Fringe_{Input.Period}px_{fringeDisplacementDirectionString}_{index}.tif"));
                }
                return cameraImage;
            }).ToList();
            _screen.Clear();
        }
    }
}
