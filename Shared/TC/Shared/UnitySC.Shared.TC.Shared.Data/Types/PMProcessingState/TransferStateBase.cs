using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.TC.Shared.Data.PMProcessingState
{

    public class TransferStateData
    {
        // Values
        public MaterialPresence MaterialPresence = MaterialPresence.Unknown;
        public SlitDoorPosition SlitDoorState = SlitDoorPosition.UnknownPosition;
        public bool LoadingUnloadingPosition = false;
        public bool ProcessingState_Idle = false;
        public bool GrantedForMeState = false;
    }
    public abstract class TransferStateBase
    {
        private EnumPMTransferState _state;
        private TransferStateData _transferStateData;
        public TransferStateBase(TransferStateData transferStateData)
        {
            TransferStateData = transferStateData;
        }

        public TransferStateData TransferStateData { get => _transferStateData; set => _transferStateData = value; }
        public EnumPMTransferState State { get => _state; set => _state = value; }

        public TransferStateBase ChangeState_MaterialPresenceChanged(MaterialPresence materialPresence)
        {
            TransferStateData.MaterialPresence = materialPresence;
            return NextState();
        }

        public TransferStateBase ChangeState_DoorStateChanged(SlitDoorPosition slitDoorState)
        {
            TransferStateData.SlitDoorState = slitDoorState;
            return NextState();
        }
        public TransferStateBase ChangeState_LoadingUnloadingPositionChanged(bool isInLoadingUnlaodingPosition)
        {
            TransferStateData.LoadingUnloadingPosition = isInLoadingUnlaodingPosition;
            return NextState();
        }

        public TransferStateBase ChangeState_ProcessingStateChanged(bool isProcessingStateIdle)
        {
            TransferStateData.ProcessingState_Idle = isProcessingStateIdle;
            return NextState();
        }
        public TransferStateBase ChangeState_LocalHardwareGrantedStateChanged(bool granted)
        {
            TransferStateData.GrantedForMeState = granted;
            return NextState();
        }
        public abstract TransferStateBase NextState();
    }
}
