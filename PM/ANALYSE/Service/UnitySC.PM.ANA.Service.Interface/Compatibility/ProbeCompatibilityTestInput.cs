using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;

namespace UnitySC.PM.ANA.Service.Interface.Compatibility
{
    [DataContract]
    public class ProbeCompatibilityTestInput
    {
        [DataMember]
        public ProbeCompatibility Compatibility { get; set; }

        [DataMember]
        public List<Layer> AllLayers { get; set; }

        [DataMember]
        public List<int> LayersIdToMeasure { get; set; }

        /// <summary>
        /// <ProbeName, ProbePositions>
        /// </summary>
        [DataMember]       
        public Dictionary<string, ProbePositions> ProbeConfigurations { get; set; }

    }

    [DataContract]
    public class ProbePositions
    {
        [DataMember]
        public bool IsTop { get; set; }

        [DataMember]
        public bool IsBottom { get; set; }

        [DataMember]
        public bool IsDual { get; set; }

    }
}
