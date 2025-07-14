using System.Xml.Serialization;

namespace UnitySC.PM.DMT.Service.Interface.Measure
{
    public enum MeasureType
    {
        [XmlEnum] BrightFieldMeasure,
        [XmlEnum] DeflectometryMeasure,
        [XmlEnum] HighAngleDarkFieldMeasure,
        [XmlEnum] BacklightMeasure,
        [XmlEnum] BrightFieldCorrector
    }
}
