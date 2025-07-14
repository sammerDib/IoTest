using System.Threading;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class AutoExposureFlowDummy : AutoExposureFlow
    {
        public AutoExposureFlowDummy(AutoExposureInput input, DMTHardwareManager hardwareManager
            , IDMTInternalCameraMethods cameraService) : base(input, hardwareManager, cameraService)
        {
        }

        protected override void Process()
        {
            Camera = HardwareManager.CamerasBySide[Input.Side];

            SetProgressMessage($"Starting {Input.Side}side Auto-Exposure flow for {Input.MeasureType} {Input.Side}");

            Thread.Sleep(200);

            Result.TotalSteps = NbLoops;
            Result.CurrentStep = NbLoops;
            Result.WaferSide = Input.Side;
            Result.ExposureTimeMs = 110;
            Result.ResultImage = CameraManager.GrabNextImage(Camera).ToServiceImage();
        }
    }
}
