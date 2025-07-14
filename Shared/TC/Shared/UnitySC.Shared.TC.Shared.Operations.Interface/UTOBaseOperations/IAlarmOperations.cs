using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface IAlarmOperations
    {
        void Init();
        void ReInitialize();

        List<Alarm> GetAllAlarms();
        Alarm GetAlarm(ActorType actor, ErrorID error);
        void ResetAlarm(List<Alarm> alarms);
        void SetAlarm(Identity identity, ErrorID error);
        void NotifyAlarmChanged(Alarm alarm);
    }
}
