using System.Collections.Generic;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.Shared.Image;

namespace UnitySC.PM.DMT.Service.Flows.Dummy
{
    public class AcquirePhaseImagesForPeriodAndDirectionFlowDummy : AcquirePhaseImagesForPeriodAndDirectionFlow
    {
        public AcquirePhaseImagesForPeriodAndDirectionFlowDummy(AcquirePhaseImagesForPeriodAndDirectionInput input,
            DMTHardwareManager hardwareManager, IDMTInternalCameraMethods cameraService, IFringeManager fringeManager)
            : base(input, hardwareManager, cameraService, fringeManager)
        {
        }

        protected override void Process()
        {
            Result.Fringe = Input.Fringe;
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;
            Result.Period = Input.Period;
            Result.ExposureTimeMs = 110;
            Result.TemporaryResults = new List<ServiceImage>();
            for (int i = 0; i < Input.Fringe.NbImagesPerDirection; i++)
            {
                Result.TemporaryResults.Add(new ServiceImage());
            }
        }
    }
}
