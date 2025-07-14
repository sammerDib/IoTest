using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.TC.Shared.Data.PMProcessingState
{

    // Not ready for example stage is not at loading position
    public class NotReady_State : TransferStateBase
    {
        public NotReady_State()
            : base(new TransferStateData())
        {
            State = EnumPMTransferState.NotReady;
        }
        public NotReady_State(TransferStateData transferStateData)
            : base(transferStateData)
        {
            State = EnumPMTransferState.NotReady;
        }

        public override TransferStateBase NextState()
        {
            if (!TransferStateData.LoadingUnloadingPosition || !TransferStateData.ProcessingState_Idle || !TransferStateData.GrantedForMeState) return this;

            switch (TransferStateData.MaterialPresence)
            {
                case MaterialPresence.Present:
                    switch (TransferStateData.SlitDoorState)
                    {  
                        case SlitDoorPosition.OpenPosition: return new ReadyToUnload_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToUnload_SlitDoorClosed_State(TransferStateData);
                    }
                case MaterialPresence.NotPresent:
                    switch (TransferStateData.SlitDoorState)
                    { 
                        case SlitDoorPosition.OpenPosition: return new ReadyToLoad_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToLoad_SlitDoorClosed_State(TransferStateData);
                    }

                default:
                case MaterialPresence.Unknown: return this;
            }
        }
    }

    // Stage at load-unload position - door is opened
    public class ReadyToLoad_SlitDoorOpened_State : TransferStateBase
    {
        public ReadyToLoad_SlitDoorOpened_State(TransferStateData transferStateData)
            : base(transferStateData)
        {
            State = EnumPMTransferState.ReadyToLoad_SlitDoorOpened;
        }

        public override TransferStateBase NextState()
        {
            if (!TransferStateData.LoadingUnloadingPosition || !TransferStateData.ProcessingState_Idle || !TransferStateData.GrantedForMeState) return new NotReady_State(TransferStateData);

            // In Loading or unlaoding position
            switch (TransferStateData.MaterialPresence)
            {
                default:
                case MaterialPresence.Unknown: return new NotReady_State(TransferStateData);
                case MaterialPresence.Present:
                    switch (TransferStateData.SlitDoorState)
                    {
                        case SlitDoorPosition.OpenPosition: return new ReadyToUnload_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToUnload_SlitDoorClosed_State(TransferStateData);
                    }
                case MaterialPresence.NotPresent:
                    switch (TransferStateData.SlitDoorState)
                    {
                        case SlitDoorPosition.OpenPosition: return this;
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToLoad_SlitDoorClosed_State(TransferStateData);
                    }
            }
        }
    }

    // Stage at load-unload position - door is closed
    public class ReadyToLoad_SlitDoorClosed_State : TransferStateBase
    {
        public ReadyToLoad_SlitDoorClosed_State(TransferStateData transferStateData)
            : base(transferStateData)
        {
            State = EnumPMTransferState.ReadyToLoad_SlitDoorClosed;
        }

        public override TransferStateBase NextState()
        {
            if (!TransferStateData.LoadingUnloadingPosition || !TransferStateData.ProcessingState_Idle || !TransferStateData.GrantedForMeState) return new NotReady_State(TransferStateData);

            // In Loading or unlaoding position
            switch (TransferStateData.MaterialPresence)
            {
                case MaterialPresence.Present:
                    switch (TransferStateData.SlitDoorState)
                    {
                        case SlitDoorPosition.OpenPosition: return new ReadyToUnload_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToUnload_SlitDoorClosed_State(TransferStateData);
                    }
                case MaterialPresence.NotPresent:
                    switch (TransferStateData.SlitDoorState)
                    {
                        case SlitDoorPosition.OpenPosition: return new ReadyToLoad_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return this;
                    }

                default:
                case MaterialPresence.Unknown: return new NotReady_State(TransferStateData);

            }
        }
    }

    // Stage at load-unload position - material present - door is opened
    public class ReadyToUnload_SlitDoorOpened_State : TransferStateBase
    {
        public ReadyToUnload_SlitDoorOpened_State(TransferStateData transferStateData)
            : base(transferStateData)
        {
            State = EnumPMTransferState.ReadyToUnload_SlitDoorOpened;
        }

        public override TransferStateBase NextState()
        {
            if (!TransferStateData.LoadingUnloadingPosition || !TransferStateData.ProcessingState_Idle || !TransferStateData.GrantedForMeState) return new NotReady_State(TransferStateData);

            switch (TransferStateData.MaterialPresence)
            {

                case MaterialPresence.Present:
                    switch (TransferStateData.SlitDoorState)
                    { 
                        case SlitDoorPosition.OpenPosition: return this;
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToUnload_SlitDoorClosed_State(TransferStateData);
                    }
                case MaterialPresence.NotPresent:
                    switch (TransferStateData.SlitDoorState)
                    {
                        case SlitDoorPosition.OpenPosition: return new ReadyToLoad_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToLoad_SlitDoorClosed_State(TransferStateData);
                    }

                default:
                case MaterialPresence.Unknown: return new NotReady_State(TransferStateData);
            }
        }
    }

    // Stage at load-unload position - material present - door is opened
    public class ReadyToUnload_SlitDoorClosed_State : TransferStateBase
    {
        public ReadyToUnload_SlitDoorClosed_State(TransferStateData transferStateData)
            : base(transferStateData)
        {
            State = EnumPMTransferState.ReadyToUnload_SlitDoorClosed;
        }

        public override TransferStateBase NextState()
        {
            if (!TransferStateData.LoadingUnloadingPosition || !TransferStateData.ProcessingState_Idle || !TransferStateData.GrantedForMeState) return new NotReady_State(TransferStateData);

            switch (TransferStateData.MaterialPresence)
            {

                case MaterialPresence.Present:
                    switch (TransferStateData.SlitDoorState)
                    {
                        case SlitDoorPosition.OpenPosition: return new ReadyToUnload_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return this;
                    }
                case MaterialPresence.NotPresent:
                    switch (TransferStateData.SlitDoorState)
                    {
                        case SlitDoorPosition.OpenPosition: return new ReadyToLoad_SlitDoorOpened_State(TransferStateData);
                        default:
                        case SlitDoorPosition.UnknownPosition:
                        case SlitDoorPosition.ClosePosition: return new ReadyToLoad_SlitDoorClosed_State(TransferStateData);
                    }
                default:
                case MaterialPresence.Unknown: return new NotReady_State(TransferStateData);
            }
        }
    }
}
