using UnitySC.Equipment.Abstractions.Drivers.Common.Enums;

namespace UnitySC.EFEM.Rorze.Drivers
{
    /// <summary>
    /// Provides all events corresponding to all commands that can be sent by all devices
    /// </summary>
    public enum EFEMEvents
    {
        #region General
        // "inherit" from common (generic part of code handle the enum as int, so values should not overlap)
        ConnectedReceived = CommandEvents.Last,

        InitCompleted,
        GetStatusesCompleted,
        InitializeCommunicationCompleted,
        GetVersionCompleted,

        ResetErrorCompleted,

        StatusReceived,
        GpioEventReceived,
        SetDateTimeCompleted,
        StopMotionCommandCompleted,
        #endregion General

        #region LoadPort
        SecureCarrierCompleted,
        ReleaseCarrierCompleted,
        GetLastMappingCompleted,
        GetCarrierTypeCompleted,
        SetCarrierTypeCompleted,
        SetLightCompleted,
        PerformWaferMappingCompleted,
        ReadCarrierIdCompleted,

        CarrierIdentificationMethodReceived,
        CarrierTypeReceived,
        InfoPadInputReceived,
        GoToHomeCompleted,
        E84LoadCompleted,
        E84UnloadCompleted,

        #endregion LoadPort

        #region Robot
        GoToPosVisitingHomeCompleted,
        LoadWaferCompleted,
        UnloadWaferCompleted,
        RetainWaferCompleted,
        ReleaseWaferRetentionCompleted,
        SetMotionSpeedCompleted,
        WaferPresenceAndHistoryCompleted,
        ExtendArmCompleted,
        SwapCompleted,
        TransferCompleted,

        WaferPresenceAndHistoryReceived,
        #endregion Robot

        #region Aligner
        ChuckSubstrateCompleted,
        CancelSubstrateChuckCompleted,
        AlignCommandCompleted,
        GetSubstratePresenceCompleted,
        GetSubstrateSizeCompleted,
        SetSubstrateSizeCompleted,
        HomeCompleted,

        SubstratePresenceReceived,
        SubstrateSizeReceived,
        #endregion Aligner

        #region IO Devices
        ChangeOutputSignalCompleted,

        ExpansionIOSignalReceived,
        FansRotationSpeedReceived,
        PressureSensorsValuesReceived,
        FanRotationSpeedStarted,
        FanRotationSpeedStopped,
        #endregion IO Devices

        #region SubCommands
        GetDeviceDataCompleted,
        SetDeviceDataCompleted,
        GposEventReceived,
        #endregion SubCommands
    }
}
