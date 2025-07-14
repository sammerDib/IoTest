using System;
using System.Xml.Serialization;

namespace UnitySC.PM.EME.Service.Interface.Calibration
{
    [XmlInclude(typeof(CameraCalibrationData))]
    [XmlInclude(typeof(WaferReferentialCalibrationData))]
    [XmlInclude(typeof(FilterData))]
    [XmlInclude(typeof(DistortionCalibrationData))]
    [XmlInclude(typeof(DistanceSensorCalibrationData))]
    [XmlInclude(typeof(AxisOrthogonalityCalibrationData))]
    public interface ICalibrationData
    {
        DateTime CreationDate { get; set; }        
    }
}
