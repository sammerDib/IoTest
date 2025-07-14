using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.TC.Shared.Data
{
    [DataContract]
    public class OCRReadingParameters
    {
        [DataMember]
        public String OCRRecipeName { get; set; }

        public double OCRAngle { get; set; }
    }

    [DataContract]
    public class PMItem
    {
        [DataMember]
        public ActorType PMType { get; set; }

        [DataMember]
        public double OrientationAngle { get; set; }

        public override string ToString()
        {
            return $"{PMType}: {OrientationAngle}°";
        }
    }

    [DataContract]
    public class UTOJobProgram
    {
        [DataMember]
        public List<PMItem> PMItems { get; set; }

        public override string ToString()
        {
            return $"PM list : [{String.Join(",", PMItems)}]";
        }
    }
}
