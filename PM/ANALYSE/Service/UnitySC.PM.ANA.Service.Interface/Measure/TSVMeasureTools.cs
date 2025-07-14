using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [DataContract]
    public class TSVMeasureTools : MeasureToolsBase
    {
        [DataMember]
        public List<ProbeWithObjectivesMaterial> Probes { get; set; }
    }
}
