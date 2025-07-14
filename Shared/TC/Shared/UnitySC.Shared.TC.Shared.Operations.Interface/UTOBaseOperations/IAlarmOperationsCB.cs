using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface IAlarmOperationsCB
    {
        void OnErrorAcknowledged(Identity identity, ErrorID error);
        void OnErrorReset(Identity identity, ErrorID error);
        void SetCriticalErrorState(Identity identity, ErrorID error);
    }
}
