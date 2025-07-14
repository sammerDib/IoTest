using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    [DataContract]
    public class WaferDimensionalCharacteristic
    {
        [DataMember]
        public WaferShape WaferShape { get; set; }

        [DataMember]
        public Length Diameter { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public Length DiameterTolerance { get; set; }

        [DataMember]
        public List<FlatDimentionalCharacteristic> Flats { get; set; }

        [DataMember]
        public NotchDimentionalCharacteristic Notch { get; set; }

        //When wafer is a simple rectangle
        [DataMember]
        public Length SampleWidth { get; set; }

        //When wafer is a simple rectangle
        [DataMember]
        public Length SampleHeight { get; set; }

        [DataMember]
        public bool IsFilmFrame { get; set; }

        public WaferDimensionalCharacteristic Clone()
        {
            return (WaferDimensionalCharacteristic)MemberwiseClone();
        }

        public override string ToString()
        {
            if (WaferShape == WaferShape.Sample)
                return $"Sample {SampleWidth}/{SampleHeight}";
            else
                return string.Format($"{WaferShape} {Diameter} ({Category})");
        }
    }

    [DataContract]
    public class FlatDimentionalCharacteristic
    {
        [DataMember]
        public Angle Angle { get; set; }

        [DataMember]
        public Length ChordLength { get; set; }

        [DataMember]
        public Length AngleTolerance { get; set; }

        [DataMember]
        public Length ChordLengthTolerance { get; set; }
    }

    [DataContract]
    public class NotchDimentionalCharacteristic
    {
        [DataMember]
        public Length Depth { get; set; }

        [DataMember]
        public Angle Angle { get; set; }

        [DataMember]
        public Length DepthPositiveTolerance { get; set; }

        [DataMember]
        public Angle AngleNegativeTolerance { get; set; }

        [DataMember]
        public Angle AnglePositiveTolerance { get; set; }
    }
}
