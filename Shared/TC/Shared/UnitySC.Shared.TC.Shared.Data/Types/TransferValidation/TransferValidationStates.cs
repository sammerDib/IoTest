using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.TC.Shared.Data.Types.TransferValidation
{
    public class NotValidated_State : TransferValidationStateBase
    {
        public NotValidated_State()
            : base(new TransferValidationData())
        {
            State = false;
        }
        public NotValidated_State(TransferValidationData transferStateData)
            : base(transferStateData)
        {
            State = false;
        }

        public override TransferValidationStateBase NextState()
        {
            if ((TransferValidationData.PMTransferState != EnumPMTransferState.ReadyToUnload_SlitDoorOpened) &&
                (TransferValidationData.PMTransferState != EnumPMTransferState.ReadyToLoad_SlitDoorOpened) ||
                !TransferValidationData.MaterialDimensionValidated ||
                !TransferValidationData.MaterialUnclamped)
                return this;
            else
                return new Validated_State(TransferValidationData);
        }
    }
    public class Validated_State : TransferValidationStateBase
    {
        public Validated_State()
            : base(new TransferValidationData())
        {
            State = true;
        }
        public Validated_State(TransferValidationData transferStateData)
            : base(transferStateData)
        {
            State = true;
        }

        public override TransferValidationStateBase NextState()
        {
            if ((TransferValidationData.PMTransferState == EnumPMTransferState.ReadyToUnload_SlitDoorOpened) ||
                (TransferValidationData.PMTransferState == EnumPMTransferState.ReadyToLoad_SlitDoorOpened) &&
                TransferValidationData.MaterialDimensionValidated &&
                TransferValidationData.MaterialUnclamped)
                return this;
            else
                return new NotValidated_State(TransferValidationData);
        }
    }
}

