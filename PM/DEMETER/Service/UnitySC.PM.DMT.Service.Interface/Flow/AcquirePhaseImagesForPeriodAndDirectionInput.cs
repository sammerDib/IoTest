using System;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{

    [Serializable]
    public class AcquirePhaseImagesForPeriodAndDirectionInput : IFlowInput
    {

        public Side AcquisitionSide;

        public Measure.Fringe Fringe;

        public int Period;

        public FringesDisplacement FringesDisplacementDirection;

        public double ExposureTimeMs;

        public AcquirePhaseImagesForPeriodAndDirectionInput(Side acquisitionSide, Measure.Fringe fringe, int period,
            FringesDisplacement displacementDirection, double exposureTimeMs)
        {
            AcquisitionSide = acquisitionSide;
            Fringe = fringe;
            Period = period;
            FringesDisplacementDirection = displacementDirection;
            ExposureTimeMs = exposureTimeMs;
        }

        public AcquirePhaseImagesForPeriodAndDirectionInput(DeflectometryMeasure dfMeasure, int period,
            FringesDisplacement displacementDirection)
        {
            AcquisitionSide = dfMeasure.Side;
            Fringe = dfMeasure.Fringe;
            ExposureTimeMs = dfMeasure.ExposureTimeMs;
            FringesDisplacementDirection = displacementDirection;
            Period = period;
        }

        public AcquirePhaseImagesForPeriodAndDirectionInput()
        {
        }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (Fringe is null)
            {
                result.IsValid = false;
                result.Message.Add($"A fringe object must be provided to acquire phase images");
            } else if (!Fringe.Periods.Contains(Period))
            {
                result.IsValid = false;
                result.Message.Add($"The provided period ({Period}) is not in the fringe object period list");
            }

            if (AcquisitionSide == Side.Unknown)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot acquire image from unknown side");
            }
            
            return result;
        }
    }
}
