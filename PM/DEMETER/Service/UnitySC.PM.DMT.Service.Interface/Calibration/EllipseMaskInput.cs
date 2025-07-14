using System;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    public class EllipseMaskInput
    {
        [XmlAttribute]
        public Side Side;

        public double ScreenWidthRatio;

        public double ScreenHeightRatio;

        public int XShiftFromCenterInPixels;
    }
}
