using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{

    [Serializable]
    public class ComputePhaseMapAndMaskForPeriodAndDirectionInput : IFlowInput
    {

        [XmlIgnore]
        public List<ServiceImage> PhaseImages { get; set; }

        public Measure.Fringe Fringe { get; set; }

        public int Period { get; set; }

        public FringesDisplacement FringesDisplacementDirection { get; set; }
        
        public DMTTransform PerspectiveCalibration { get; set; }
        
        public CorrectorResult CorrectorResult { get; set; }
        
        public Side Side { get; set; }
        
        public Length WaferDiameter { get; set; }

        public bool UseEnhancedMask { get; set; } = true;

        public bool UseMaskWaferFill { get; set; } = false;

        public double MaskFillExclusionInMicrons { get; set; } = 3000.0;

        public ComputePhaseMapAndMaskForPeriodAndDirectionInput(Measure.Fringe fringe, int period,
            FringesDisplacement displacementDirection, Side side)
        {
            Fringe = fringe;
            Period = period;
            Side = side;
            FringesDisplacementDirection = displacementDirection;
        }

        public ComputePhaseMapAndMaskForPeriodAndDirectionInput(
            DeflectometryMeasure dfMeasure, int period, FringesDisplacement displacementDirection)
        {
            Fringe = dfMeasure.Fringe;
            Period = period;
            Side = dfMeasure.Side;
            FringesDisplacementDirection = displacementDirection;
        }

        public ComputePhaseMapAndMaskForPeriodAndDirectionInput()
        {
        }
        
        
        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (PhaseImages is null || PhaseImages.Count == 0)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot process flow without phase images");
            }
            
            if (!(PhaseImages is null) && PhaseImages.Count != Fringe.NbImagesPerDirection)
            {
                result.IsValid = false;
                result.Message.Add($"The number of images provided ({PhaseImages.Count}) is different from the number of images expected ({Fringe.NbImagesPerDirection})");
            }

            if (!Fringe.Periods.Contains(Period))
            {
                result.IsValid = false;
                result.Message.Add($"The provided period ({Period}) is not in the period list of the fringe object");
            }

            if (WaferDiameter is null || WaferDiameter.Value <= 0)
            {
                result.IsValid = false;
                result.Message.Add("Cannot process flow without wafer diameter");
            }

            if (UseEnhancedMask && PerspectiveCalibration is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot process flow without perspective calibration when using enhanced mask");
            }

            return result;
        }
    }
}
