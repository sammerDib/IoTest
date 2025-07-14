using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class AlignmentLiseHFXYAnalysisInput : IANAInputFlow
    {
        [DataMember] public double StepSize { get; set; }

        [DataMember] public double StandardStepSize { get; set; } = 0.6;

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);
            return result;
        }

        [DataMember] public ANAContextBase InitialContext { get; set; }
    }
}
