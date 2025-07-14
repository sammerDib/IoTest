using System.Xml.Serialization;

using UnitySC.Shared.Format.Metro;

namespace UnitySC.Shared.Format.XYCalibration
{
    public class XYCalibrationResult : MeasureResultBase
    {
        [XmlAttribute("MeasureVersion")]
        public string MeasureVersion { get; set; } = "1.0.0";
    }
}
