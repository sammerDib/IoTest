using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.WaferMap
{
    [DataContract]
    public class WaferMapSettings
    {
        [DataMember]
        public DieAndStreetSizesResult DieAndStreetSizes { get; set; }

        [DataMember]
        public WaferMapResult WaferMapData { get; set; }

        [DataMember]
        public Length EdgeExclusion { get; set; }

        [DataMember]
        public bool IsDieSizeSet { get; set; }

        [DataMember]
        public Length DieWidth { get; set; }

        [DataMember]
        public Length DieHeight { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocus { get; set; }
    }
}
