using System;
using System.Xml.Serialization;

namespace UnitySC.PM.DMT.Service.Interface.OpticalMount
{
    public enum OpticalMountShape : UInt32
    {
        [XmlEnum]
        Cross = 0,

        [XmlEnum]
        LTopLeft = 1,

        [XmlEnum]
        LTopRight = 2,

        [XmlEnum]
        LBottomLeft = 3,

        [XmlEnum]
        LBottomRight = 4,

        [XmlEnum]
        SquarePlusCenter = 5,
    }
}
