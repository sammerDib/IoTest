using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Service.Interface
{
    // PM (UTO.API) Serveur -> UTO Client
    [ServiceContract]
    public interface IAlarmServiceCB
    {
        [OperationContract(Name = "SetAlarmIDList", IsOneWay = true)]
        void SetAlarm(List<Alarm> alarms);

        [OperationContract(Name = "ResetSetAlarmIDList", IsOneWay = true)]
        void ResetAlarm(List<Alarm> alarms);

        [OperationContract(IsOneWay = true)]
        void StopCancelAllJobs();
    }
}
