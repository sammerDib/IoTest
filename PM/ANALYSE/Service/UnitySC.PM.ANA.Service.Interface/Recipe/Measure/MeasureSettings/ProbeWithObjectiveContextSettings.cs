using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    public class ProbeWithObjectiveContextSettings:ProbeSettings
    {
        [DataMember]
        public ObjectiveContext ProbeObjectiveContext { get; set; }

    }
}
