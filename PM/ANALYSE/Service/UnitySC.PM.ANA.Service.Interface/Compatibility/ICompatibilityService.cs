using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Compatibility
{
    [ServiceContract]
    [ServiceKnownType(typeof(DistanceMeasure))]
    [ServiceKnownType(typeof(ThicknessMeasure))]
    [ServiceKnownType(typeof(CrossLayer))]
    public interface ICompatibilityService
    {
        [OperationContract]
        Response<ProbeCompatibility> GetProbeCompatibility();

        [OperationContract]
        Response<VoidResult> SaveProbeCompatibility(ProbeCompatibility probeCompatibility);

        [OperationContract]
        Response<ProbeCompatibilityResult> TestProbeCompatibility(ProbeCompatibilityTestInput input);

    }
}
