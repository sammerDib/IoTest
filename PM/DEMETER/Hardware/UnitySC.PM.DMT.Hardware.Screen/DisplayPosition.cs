using System.Xml.Serialization;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.PM.DMT.Hardware.Screen
{
    public enum DisplayPosition
    {
        [XmlEnum]
        Top = MIL.M_TOP,
        [XmlEnum]
        Bottom = MIL.M_BOTTOM,
        [XmlEnum]
        Left = MIL.M_LEFT,
        [XmlEnum]
        Right = MIL.M_RIGHT,
        [XmlEnum]
        TopRight = MIL.M_TOP + MIL.M_RIGHT,
        [XmlEnum]
        TopLeft = MIL.M_TOP + MIL.M_LEFT,
        [XmlEnum]
        BottomLeft = MIL.M_BOTTOM + MIL.M_LEFT,
        [XmlEnum]
        BottomRight = MIL.M_BOTTOM + MIL.M_RIGHT
    }
}
