using System.Runtime.Serialization;

namespace UnitySC.Shared.Data
{
    [DataContract]
    public class DefectBin
    {
        [DataMember]
        public int RoughBin { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public int Color { get; set; }

        public DefectBin()
        {
        }

        public DefectBin(int roughBin, string label, int color)
        {
            RoughBin = roughBin;
            Label = label;
            Color = color;
        }
    }
}