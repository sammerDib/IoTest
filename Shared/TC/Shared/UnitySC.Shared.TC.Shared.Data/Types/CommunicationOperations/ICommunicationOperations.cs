using System;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.TC.Shared.Data
{
    public interface ICommunicationOperations
    {
        void Init(String communicationParametersDisplayed, ICommunicationOperationsCB cb);

        UnitySC.Shared.Data.Enum.ECommunicationState State { get; }
        EnableState SwitchState { get; set; }

        void AttemptCommunicationFailedOrCommunicationLost();

        void AttemptCommunicationSucceed();
    }
}
