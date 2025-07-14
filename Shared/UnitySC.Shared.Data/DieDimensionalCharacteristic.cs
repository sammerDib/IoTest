using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    [DataContract]
    public class DieDimensionalCharacteristic
    {
        public DieDimensionalCharacteristic()
        {
        }

        public DieDimensionalCharacteristic(Length dieWidth, Length dieHeight, Length streetWidth, Length streetHeight, Angle dieAngle)
        {
            DieWidth = dieWidth;
            DieHeight = dieHeight;
            StreetWidth = streetWidth;
            StreetHeight = streetHeight;
            DieAngle = dieAngle;
        }

        [DataMember]
        public Length DieWidth { get; set; }

        [DataMember]
        public Length DieHeight { get; set; }

        [DataMember]
        public Length StreetWidth { get; set; }

        [DataMember]
        public Length StreetHeight { get; set; }

        [DataMember]
        public Angle DieAngle { get; set; }
    }
}
