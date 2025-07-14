using System;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.TC.Shared.Data
{

    public class TransferValidationData
    {
        // Values
        public EnumPMTransferState PMTransferState;
        public bool MaterialDimensionValidated;
        public bool MaterialUnclamped = true;
    }
    public abstract class TransferValidationStateBase
    {
        private bool _state;
        private TransferValidationData _transferValidationData;
        public TransferValidationStateBase(TransferValidationData transferValidationData)
        {
            TransferValidationData = transferValidationData;
        }

        public TransferValidationData TransferValidationData { get => _transferValidationData; set => _transferValidationData = value; }
        public bool State { get => _state; set => _state = value; }

        public TransferValidationStateBase ChangeState_PMTransferStateChanged(EnumPMTransferState pmTransferState)
        {
            TransferValidationData.PMTransferState = pmTransferState;
            return NextState();
        }

        public TransferValidationStateBase ChangeState_MaterialDimensionValidatedChanged(bool validated)
        {
            TransferValidationData.MaterialDimensionValidated = validated;
            return NextState();
        }
        public TransferValidationStateBase ChangeState_MaterialClampChanged(bool materialClamped)
        {
            TransferValidationData.MaterialUnclamped = !materialClamped;
            return NextState();
        }
        public abstract TransferValidationStateBase NextState();
    }
}
