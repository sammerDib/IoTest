using System;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Camera;

namespace UnitySC.PM.DMT.Service.Flows.AcquireOneImage
{
    public class AcquireOneImageFlow : FlowComponent<AcquireOneImageInput, AcquireOneImageResult>
    {
        private readonly CameraBase _camera;

        private readonly IDMTInternalCameraMethods _cameraManager;

        private readonly ScreenBase _screen;

        private string _measureTypeString;

        public const int MaximumNumberOfSteps = 5;

        public AcquireOneImageFlow(AcquireOneImageInput input, IDMTInternalCameraMethods cameraService,
            DMTHardwareManager hardwareManager) : base(input, "AcquireOneImage")
        {
            _screen = hardwareManager.ScreensBySide[input.ScreenSide];
            _camera = hardwareManager.CamerasBySide[input.CameraSide];
            _cameraManager = cameraService;
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Input.CameraSide}side {Name} flow");
            _measureTypeString = Enum.GetName(typeof(MeasureType), Input.MeasureType);
            SetProgressMessage("Preparing screen for image acquisition");
            SetScreenForAcquisition();
            CheckCancellation();

            SetProgressMessage($"Starting {_measureTypeString} {Input.CameraSide} image acquisition");
            AcquireImageFromCamera();
            SetProgressMessage($"{_measureTypeString} {Input.CameraSide} image acquired");
            _screen.Clear();
        }

        private void SetScreenForAcquisition()
        {
            switch (Input.DisplayImageType)
            {
                case AcquisitionScreenDisplayImage.HighAngleDarkFieldMask:
                    _screen.DisplayImage(Input.ScreenImage);
                    break;

                case AcquisitionScreenDisplayImage.Color:
                    _screen.ClearAsync(Input.ScreenColor).Wait();
                    break;
            }
        }

        private void AcquireImageFromCamera()
        {
            Result.ExposureTimeMs = Input.ExposureTimeMs;
            _cameraManager.SetExposureTime(_camera, Input.ExposureTimeMs);
            Result.AcquiredImage = _cameraManager.GrabNextImage(_camera).Clone();
        }
    }
}
