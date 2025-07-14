using System;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class ComputeRawCurvatureMapForPeriodAndDirectionInput : IFlowInput
    {
        public Measure.Fringe Fringe { get; set; }

        public FringesDisplacement FringesDisplacementDirection { get; set; }

        public int Period { get; set; }

        public Side Side { get; set; }

        [XmlIgnore]
        public PSDResult PhaseMapAndMask { get; set; }

        public ComputeRawCurvatureMapForPeriodAndDirectionInput()
        {
        }

        public ComputeRawCurvatureMapForPeriodAndDirectionInput(
            Measure.Fringe fringe, int period,
            FringesDisplacement displacementDirection, Side side)
        {
            Fringe = fringe;
            Period = period;
            FringesDisplacementDirection = displacementDirection;
            Side = side;
        }

        public ComputeRawCurvatureMapForPeriodAndDirectionInput(
            DeflectometryMeasure dfMeasure, int period, FringesDisplacement displacementDirection)
        {
            Fringe = dfMeasure.Fringe;
            Period = period;
            FringesDisplacementDirection = displacementDirection;
            Side = dfMeasure.Side;
        }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (Fringe is null)
            {
                result.IsValid = false;
                result.Message.Add($"A fringe object must be provided to acquire phase images");
            }
            else if (!Fringe.Periods.Contains(Period))
            {
                result.IsValid = false;
                result.Message.Add($"The provided period ({Period}) is not in the fringe object period list");
            }

            if (PhaseMapAndMask is null)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a raw curvature map without a phase map and mask");
            }

            return result;
        }
    }
}
