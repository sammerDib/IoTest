using System;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class CurvatureDynamicsCalibrationInput : IFlowInput
    {
        [XmlIgnore]
        public ImageData XRawCurvatureMap { get; set; }

        [XmlIgnore]
        public ImageData YRawCurvatureMap { get; set; }

        [XmlIgnore]
        public ImageData CurvatureMapMask { get; set; }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (XRawCurvatureMap is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot calibrate curvature dynamics without a raw curvature map for X direction");
            }

            if (YRawCurvatureMap is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot calibrate curvature dynamics without a raw curvature map for Y direction");
            }

            if (CurvatureMapMask is null)
            {
                result.IsValid = false;
                result.Message.Add("Cannot calibrate curvature dynamics without a curvature map mask");
            }

            return result;
        }
    }
}
