using System;
using System.Xml.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [XmlInclude(typeof(LiseHFCalibrationData))]
    [XmlInclude(typeof(ObjectivesCalibrationData))]
    [XmlInclude(typeof(XYCalibrationData))]
    public interface ICalibrationData
    {
        DateTime CreationDate { get; set; }
        string User { get; set; }
    }
}
