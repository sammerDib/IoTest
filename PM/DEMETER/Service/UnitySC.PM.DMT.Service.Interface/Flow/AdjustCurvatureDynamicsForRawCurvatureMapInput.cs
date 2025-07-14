using System;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class AdjustCurvatureDynamicsForRawCurvatureMapInput : IFlowInput
    {

        [XmlIgnore]
        public ImageData RawCurvatureMap { get; set; }

        public float CurvatureDynamicsCalibrationCoefficient { get; set; }

        public float DynamicsCoefficient { get; set; }

        public FringesDisplacement FringesDisplacementDirection { get; set; }

        public int Period { get; set; }

        public Measure.Fringe Fringe { get; set; }
        
        public Side Side { get; set; }

        [XmlIgnore]
        public ImageData Mask { get; set; }

        public AdjustCurvatureDynamicsForRawCurvatureMapInput()
        {
        }

        public AdjustCurvatureDynamicsForRawCurvatureMapInput(DeflectometryMeasure dfMeasure, FringesDisplacement direction, float dynamicsCalibrationCoefficient)
        {
            Fringe = dfMeasure.Fringe;
            Period = dfMeasure.Fringe.Period;
            Side = dfMeasure.Side;
            FringesDisplacementDirection = direction;
            CurvatureDynamicsCalibrationCoefficient = dynamicsCalibrationCoefficient;
            DynamicsCoefficient = (float)dfMeasure.CurvatureDynamic;
        }


        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);
            if (Fringe is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot adjust the curvature dynamics without a fringe description");
            }

            if (RawCurvatureMap is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot adjust the curvature dynamics without a raw curvature map");
            }

            if (Mask is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot adjust the curvature dynamics without a mask for the raw curvature map");
            }

            if (Period == 0)
            {
                result.IsValid = false;
                result.Message.Add("A valid period must be defined to adjust the curvature dynamics");
            }

            if (CurvatureDynamicsCalibrationCoefficient < 0)
            {
                result.IsValid = false;
                result.Message.Add("A valid curvature dynamics calibration coefficient must be supplied to adjust curvature dynamics");
            }

            if (DynamicsCoefficient < 0)
            {
                result.IsValid = false;
                result.Message.Add("A valid measure curvature dynamics coefficient must be supplied to adjust the curvature dynamics");
            }

            return result;
        }
    }
}
