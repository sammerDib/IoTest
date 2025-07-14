using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Topography
{
    [DataContract]
    public class TopographyResultSettings
    {
        [DataMember]
        public List<ExternalProcessingOutput> ExternalProcessingOutputs { get; set; }
    }  
}
