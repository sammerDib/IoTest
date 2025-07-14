using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Collection;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{

    [Serializable]
    public class ComputeUnwrappedPhaseMapForDirectionInput : IFlowInput
    {

        [XmlIgnore]
        public Dictionary<int, PSDResult> PhaseMaps { get; set; }

        public Measure.Fringe Fringe { get; set; }

        public FringesDisplacement FringesDisplacementDirection { get; set; }

        public bool IsNeededForSlopeMaps { get; set; }

        public bool IsNeededForTopography { get; set; }

        public Side Side { get; set; }

        public ComputeUnwrappedPhaseMapForDirectionInput()
        {
        }

        public ComputeUnwrappedPhaseMapForDirectionInput(Measure.Fringe fringe,
            FringesDisplacement displacementDirection, Side side, bool isNeededForSlopeMaps = true, bool isNeededForTopography = false)
        {
            Fringe = fringe;
            FringesDisplacementDirection = displacementDirection;
            IsNeededForSlopeMaps = isNeededForSlopeMaps;
            IsNeededForTopography = isNeededForTopography;
            Side = side;
            PhaseMaps = new Dictionary<int, PSDResult>(fringe.Periods.Count);
        }

        public ComputeUnwrappedPhaseMapForDirectionInput(
            DeflectometryMeasure dfMeasure, FringesDisplacement displacementDirection)
        {
            Fringe = dfMeasure.Fringe;
            FringesDisplacementDirection = displacementDirection;
            IsNeededForSlopeMaps = !dfMeasure.Outputs.HasFlag(DeflectometryOutput.GlobalTopo) &&
                                   !dfMeasure.Outputs.HasFlag(DeflectometryOutput.NanoTopo);
            Side = dfMeasure.Side;
            PhaseMaps = new Dictionary<int, PSDResult>();
        }


        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (Fringe is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot unwrap phases without a fringe");
            }
            else
            {
                if (Fringe.FringeType != Interface.Fringe.FringeType.Multi)
                {
                    result.IsValid = false;
                    result.Message.Add("Cannot unwrap phases for a single period fringe");
                }
            }
            if (PhaseMaps is null || PhaseMaps.Values.IsNullOrEmpty())
            {
                result.IsValid = false;
                result.Message.Add("Cannot unwrap phases without Phase maps");
            }
            if (IsNeededForTopography && IsNeededForSlopeMaps)
            {
                result.IsValid = false;
                result.Message.Add("Cannot unwrap phase for topography and for slope maps at the same time");
            }

            return result;
        }
    }
}
