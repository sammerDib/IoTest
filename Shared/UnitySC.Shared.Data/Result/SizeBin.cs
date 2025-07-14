using System.Runtime.Serialization;

namespace UnitySC.Shared.Data
{
    [DataContract]
    public class SizeBin
    {
        [DataMember]
        public long AreaMax_um { get; set; }

        [DataMember]
        public int Size_um { get; set; }

        public SizeBin(long areaMax_um, int size_um)
        {
            AreaMax_um = areaMax_um;
            Size_um = size_um;
        }
    }
}