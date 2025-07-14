using System;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    public class ExposureMatchingGoldenValues
    {
        
        [XmlAttribute]
        public Side Side { get; set; }
        public ulong Top { get; set; }
        public ulong Left { get; set; }
        public ulong Bottom { get; set; }
        public ulong Right { get; set; }
        public ulong Center { get; set; }
        
        public uint[] GetGoldenValuesArray()
        {
            return new [] { (uint)Top, (uint)Left, (uint)Bottom, (uint)Right, (uint)Center };
        }

        public ExposureMatchingGoldenValues()
        {
            
        }

        public ExposureMatchingGoldenValues(Side side, uint[] goldenValues)
        {
            Side = side;
            Top = goldenValues[0];
            Left = goldenValues[1];
            Bottom = goldenValues[2];
            Right = goldenValues[3];
            Center = goldenValues[4];
        }
        
        public ExposureMatchingGoldenValues(Side side, ulong[] goldenValues)
        {
            Side = side;
            Top = goldenValues[0];
            Left = goldenValues[1];
            Bottom = goldenValues[2];
            Right = goldenValues[3];
            Center = goldenValues[4];
        }
    }
}
