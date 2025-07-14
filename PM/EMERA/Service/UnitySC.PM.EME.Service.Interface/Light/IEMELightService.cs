using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.Light
{
    [ServiceContract(CallbackContract = typeof(IEMELightServiceCallback))]
    [ServiceKnownType(typeof(EMELightConfig))]
    public interface IEMELightService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<List<EMELightConfig>> GetLightsConfig();

        [OperationContract]
        Response<VoidResult> InitLightSources();

        [OperationContract]
        Response<VoidResult> SwitchOn(string lightID, bool powerOn);

        [OperationContract]
        Response<VoidResult> SetLightPower(string lightID, double power);

        [OperationContract]
        Response<VoidResult> RefreshPower(string lightID);

        [OperationContract]
        Response<VoidResult> RefreshSwitchOn(string lightID);

        [OperationContract]
        Response<VoidResult> RefreshLightSource(string lightID);
    }
}
