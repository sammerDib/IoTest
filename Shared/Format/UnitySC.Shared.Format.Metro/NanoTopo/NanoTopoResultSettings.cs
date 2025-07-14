using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.NanoTopo
{
    [DataContract]
    public class NanoTopoResultSettings
    {
        [DataMember]
        public LengthTolerance RoughnessTolerance { get; set; }
        [DataMember]
        public Length RoughnessTarget { get; set; }
        [DataMember]
        public LengthTolerance StepHeightTolerance { get; set; }
        [DataMember]
        public Length StepHeightTarget { get; set; }
        [DataMember]
        public List<ExternalProcessingOutput> ExternalProcessingOutputs { get; set; }
    }  
}
