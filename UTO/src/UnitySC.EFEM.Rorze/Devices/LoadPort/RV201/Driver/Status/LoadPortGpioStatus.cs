using System;
using System.Globalization;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status
{
    #region Enums

    /// <summary>
    /// RV201 GPIO inputs statuses for system type 1 to 3.
    /// </summary>
    /// <remarks>
    /// Type 1 to 3 is the only to define data for OC adapter which is used by Unity.
    /// If other system type are to be supported, we'll have to refactor this part.
    /// (different enums with a way to switch between them?)
    /// </remarks>
    [Flags]
    public enum LoadPortGeneralInputs : Int64
    {
        EmergencyStop              = (Int64) 1 <<  0,
        TemporarilyStop            = (Int64) 1 <<  1,
        /* bits 2 and 3 are not connected */
        ExhaustFan1                = (Int64) 1 <<  4,
        ExhaustFan2                = (Int64) 1 <<  5,
        Protrusion                 = (Int64) 1 <<  6,
        Protrusion2                = (Int64) 1 <<  7,

        FOUPDoorLeftClose          = (Int64) 1 <<  8,
        FOUPDoorLeftOpen           = (Int64) 1 <<  9,
        FOUPDoorRightClose         = (Int64) 1 << 10,
        FOUPDoorRightOpen          = (Int64) 1 << 11,
        MappingSensorContaining    = (Int64) 1 << 12,
        MappingSensorPreparation   = (Int64) 1 << 13,
        UpperPressureLimit         = (Int64) 1 << 14,
        LowerPressureLimit         = (Int64) 1 << 15,

        CarrierClampOpen           = (Int64) 1 << 16,
        CarrierClampClose          = (Int64) 1 << 17,
        PresenceLeft               = (Int64) 1 << 18,
        PresenceRight              = (Int64) 1 << 19,
        PresenceMiddle             = (Int64) 1 << 20,
        InfoPadA                   = (Int64) 1 << 21,
        InfoPadB                   = (Int64) 1 << 22,
        InfoPadC                   = (Int64) 1 << 23,

        InfoPadD                   = (Int64) 1 << 24,
        Presence                   = (Int64) 1 << 25,
        FOSBIdentificationSensor   = (Int64) 1 << 26,
        ObstacleDetectingSensor    = (Int64) 1 << 27,
        DoorDetection              = (Int64) 1 << 28,
        /* bit 29 is not connected */
        OpenCarrierDetectingSensor = (Int64) 1 << 30,
        /* bit 31 is not connected */

        StageRotationBackward      = (Int64) 1 << 32,
        StageRotationForward       = (Int64) 1 << 33,
        BcrLifting                 = (Int64) 1 << 34,
        BcrLowering                = (Int64) 1 << 35,
        CoverLock                  = (Int64) 1 << 36,
        CoverUnlock                = (Int64) 1 << 37,
        CarrierRetainerLowering    = (Int64) 1 << 38,
        CarrierRetainerLifting     = (Int64) 1 << 39,

        External_SW1_ACCESS        = (Int64) 1 << 40,
        External_SW2_TEST          = (Int64) 1 << 41,
        External_SW3_UNLOAD        = (Int64) 1 << 42,
        /* bits 43 to 45 are not connected */
        PFA_L                      = (Int64) 1 << 46,
        PFA_R                      = (Int64) 1 << 47,

        Dsc300mm                   = (Int64) 1 << 48,
        Dsc200mm                   = (Int64) 1 << 49,
        Dsc150mm                   = (Int64) 1 << 50,
        CstCommon                  = (Int64) 1 << 51,
        Cst200mm                   = (Int64) 1 << 52,
        Cst150mm                   = (Int64) 1 << 53,
        Adapter                    = (Int64) 1 << 54,
        CoverClosed                = (Int64) 1 << 55,

        VALID_84                   = (Int64) 1 << 56,
        CS_0_E84                   = (Int64) 1 << 57,
        CS_1_E84                   = (Int64) 1 << 58,
        /* bit 59 is not connected */
        TR_REQ_E84                 = (Int64) 1 << 60,
        BUSY_E84                   = (Int64) 1 << 61,
        COMPT_E84                  = (Int64) 1 << 62,
        CONT_E84                   = (Int64) 1 << 63
    }

    /// <summary>
    /// RV201 GPIO outputs statuses for system type 1 to 3.
    /// </summary>
    /// <remarks>
    /// Type 1 to 3 is the only to define data for OC adapter which is used by Unity.
    /// If other system type are to be supported, we'll have to refactor this part.
    /// (different enums with a way to switch between them?)
    /// </remarks>
    [Flags]
    public enum LoadPortGeneralOutputs : Int64
    {
        PreparationCompleted_SigNotConnected = (Int64) 1 << 0,
        TemporarilyStop_SigNotConnected      = (Int64) 1 << 1,
        SignificantError_SigNotConnected     = (Int64) 1 << 2,
        LightError_SigNotConnected           = (Int64) 1 << 3,
        Protrusion2Enabled                   = (Int64) 1 << 4,
        AdapterClamp                         = (Int64) 1 << 5,
        AdapterPower                         = (Int64) 1 << 6,
        ObstacleDetectionCancel              = (Int64) 1 << 7,

        /* bits 8 and 9 are not connected */
        CarrierClampClose                    = (Int64) 1 << 10,
        CarrierClampOpen                     = (Int64) 1 << 11,
        FOUPDoorLockOpen                     = (Int64) 1 << 12,
        FOUPDoorLockClose                    = (Int64) 1 << 13,
        /* bits 14 and 15 are not connected */

        MappingSensorPreparation             = (Int64) 1 << 16,
        MappingSensorContaining              = (Int64) 1 << 17,
        ChuckingOn                           = (Int64) 1 << 18,
        ChuckingOff                          = (Int64) 1 << 19,
        CoverLock                            = (Int64) 1 << 20,
        CoverUnlock                          = (Int64) 1 << 21,
        /* bits 22 and 23 are not connected */

        DoorOpen_ExtOutput                   = (Int64) 1 << 24,
        CarrierClamp_ExtOutput               = (Int64) 1 << 25,
        CarrierPresenceOn_ExtOutput          = (Int64) 1 << 26,
        PreparationCompleted_ExtOutput       = (Int64) 1 << 27,
        CarrierProperlyPlaced_ExtOutput      = (Int64) 1 << 28,
        /* bits 29 to 31 are not connected */

        StageRotationBackward                = (Int64) 1 << 32,
        StageRotationForward                 = (Int64) 1 << 33,
        BcrLifting                           = (Int64) 1 << 34,
        BcrLowering                          = (Int64) 1 << 35,
        /* bits 36 and 37 are not connected */
        CarrierRetainerLowering              = (Int64) 1 << 38,
        CarrierRetainerLifting               = (Int64) 1 << 39,

        SW1_LED                              = (Int64) 1 << 40,
        SW3_LED                              = (Int64) 1 << 41,
        LOAD_LED                             = (Int64) 1 << 42,
        UNLOAD_LED                           = (Int64) 1 << 43,
        PRESENCE_LED                         = (Int64) 1 << 44,
        PLACEMENT_LED                        = (Int64) 1 << 45,
        MANUAL_LED                           = (Int64) 1 << 46,
        ERROR_LED                            = (Int64) 1 << 47,

        CLAMP_LED                            = (Int64) 1 << 48,
        DOCK_LED                             = (Int64) 1 << 49,
        BUSY_LED                             = (Int64) 1 << 50,
        AUTO_LED                             = (Int64) 1 << 51,
        RESERVED_LED                         = (Int64) 1 << 52,
        CLOSE_LED                            = (Int64) 1 << 53,
        LOCK_LED                             = (Int64) 1 << 54,
        /* bit 55 is not connected */

        L_REQ_E84                            = (Int64) 1 << 56,
        U_REQ_E84                            = (Int64) 1 << 57,
        /* bit 58 is not connected */
        READY_E84                            = (Int64) 1 << 59,
        /* bit 60 and 61 are not connected */
        HO_AVBL_E84                          = (Int64) 1 << 62,
        ES_E84                               = (Int64) 1 << 63
    }

    #endregion Enums

    public class LoadPortGpioStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPortGpioStatus"/> class.
        /// <param name="other">Create a deep copy of <see cref="LoadPortGpioStatus"/> instance</param>
        /// </summary>
        public LoadPortGpioStatus(LoadPortGpioStatus other)
        {
            Set(other);
        }

        public LoadPortGpioStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            var pi = (LoadPortGeneralInputs)Int64.Parse(statuses[0], NumberStyles.AllowHexSpecifier);

            I_EmergencyStop              = (pi & LoadPortGeneralInputs.EmergencyStop) != 0;
            I_TemporarilyStop            = (pi & LoadPortGeneralInputs.TemporarilyStop) != 0;
            I_ExhaustFan1                = (pi & LoadPortGeneralInputs.ExhaustFan1) != 0;
            I_ExhaustFan2                = (pi & LoadPortGeneralInputs.ExhaustFan2) != 0;
            I_Protrusion                 = (pi & LoadPortGeneralInputs.Protrusion) != 0;
            I_Protrusion2                = (pi & LoadPortGeneralInputs.Protrusion2) != 0;
            I_FOUPDoorLeftClose          = (pi & LoadPortGeneralInputs.FOUPDoorLeftClose) != 0;
            I_FOUPDoorLeftOpen           = (pi & LoadPortGeneralInputs.FOUPDoorLeftOpen) != 0;
            I_FOUPDoorRightClose         = (pi & LoadPortGeneralInputs.FOUPDoorRightClose) != 0;
            I_FOUPDoorRightOpen          = (pi & LoadPortGeneralInputs.FOUPDoorRightOpen) != 0;
            I_MappingSensorContaining    = (pi & LoadPortGeneralInputs.MappingSensorContaining) != 0;
            I_MappingSensorPreparation   = (pi & LoadPortGeneralInputs.MappingSensorPreparation) != 0;
            I_UpperPressureLimit         = (pi & LoadPortGeneralInputs.UpperPressureLimit) != 0;
            I_LowerPressureLimit         = (pi & LoadPortGeneralInputs.LowerPressureLimit) != 0;
            I_CarrierClampOpen           = (pi & LoadPortGeneralInputs.CarrierClampOpen) != 0;
            I_CarrierClampClose          = (pi & LoadPortGeneralInputs.CarrierClampClose) != 0;
            I_PresenceLeft               = (pi & LoadPortGeneralInputs.PresenceLeft) != 0;
            I_PresenceRight              = (pi & LoadPortGeneralInputs.PresenceRight) != 0;
            I_PresenceMiddle             = (pi & LoadPortGeneralInputs.PresenceMiddle) != 0;
            I_InfoPadA                   = (pi & LoadPortGeneralInputs.InfoPadA) != 0;
            I_InfoPadB                   = (pi & LoadPortGeneralInputs.InfoPadB) != 0;
            I_InfoPadC                   = (pi & LoadPortGeneralInputs.InfoPadC) != 0;
            I_InfoPadD                   = (pi & LoadPortGeneralInputs.InfoPadD) != 0;
            I_Presence                   = (pi & LoadPortGeneralInputs.Presence) != 0;
            I_FOSBIdentificationSensor   = (pi & LoadPortGeneralInputs.FOSBIdentificationSensor) != 0;
            I_ObstacleDetectingSensor    = (pi & LoadPortGeneralInputs.ObstacleDetectingSensor) != 0;
            I_DoorDetection              = (pi & LoadPortGeneralInputs.DoorDetection) != 0;
            I_OpenCarrierDetectingSensor = (pi & LoadPortGeneralInputs.OpenCarrierDetectingSensor) != 0;
            I_StageRotationBackward      = (pi & LoadPortGeneralInputs.StageRotationBackward) != 0;
            I_StageRotationForward       = (pi & LoadPortGeneralInputs.StageRotationForward) != 0;
            I_BcrLifting                 = (pi & LoadPortGeneralInputs.BcrLifting) != 0;
            I_BcrLowering                = (pi & LoadPortGeneralInputs.BcrLowering) != 0;
            I_CoverLock                  = (pi & LoadPortGeneralInputs.CoverLock) != 0;
            I_CoverUnlock                = (pi & LoadPortGeneralInputs.CoverUnlock) != 0;
            I_CarrierRetainerLowering    = (pi & LoadPortGeneralInputs.CarrierRetainerLowering) != 0;
            I_CarrierRetainerLifting     = (pi & LoadPortGeneralInputs.CarrierRetainerLifting) != 0;
            I_External_SW1_ACCESS        = (pi & LoadPortGeneralInputs.External_SW1_ACCESS) != 0;
            I_External_SW2_TEST          = (pi & LoadPortGeneralInputs.External_SW2_TEST) != 0;
            I_External_SW3_UNLOAD        = (pi & LoadPortGeneralInputs.External_SW3_UNLOAD) != 0;
            I_PFA_L                      = (pi & LoadPortGeneralInputs.PFA_L) != 0;
            I_PFA_R                      = (pi & LoadPortGeneralInputs.PFA_R) != 0;
            I_Dsc300mm                   = (pi & LoadPortGeneralInputs.Dsc300mm) != 0;
            I_Dsc200mm                   = (pi & LoadPortGeneralInputs.Dsc200mm) != 0;
            I_Dsc150mm                   = (pi & LoadPortGeneralInputs.Dsc150mm) != 0;
            I_CstCommon                  = (pi & LoadPortGeneralInputs.CstCommon) != 0;
            I_Cst200mm                   = (pi & LoadPortGeneralInputs.Cst200mm) != 0;
            I_Cst150mm                   = (pi & LoadPortGeneralInputs.Cst150mm) != 0;
            I_Adapter                    = (pi & LoadPortGeneralInputs.Adapter) != 0;
            I_CoverClosed                = (pi & LoadPortGeneralInputs.CoverClosed) != 0;
            I_VALID_E84                  = (pi & LoadPortGeneralInputs.VALID_84) != 0;
            I_CS_0_E84                   = (pi & LoadPortGeneralInputs.CS_0_E84) != 0;
            I_CS_1_E84                   = (pi & LoadPortGeneralInputs.CS_1_E84) != 0;
            I_TR_REQ_E84                 = (pi & LoadPortGeneralInputs.TR_REQ_E84) != 0;
            I_BUSY_E84                   = (pi & LoadPortGeneralInputs.BUSY_E84) != 0;
            I_COMPT_E84                  = (pi & LoadPortGeneralInputs.COMPT_E84) != 0;
            I_CONT_E84                   = (pi & LoadPortGeneralInputs.CONT_E84) != 0;

            var po = (LoadPortGeneralOutputs)Int64.Parse(statuses[1], NumberStyles.AllowHexSpecifier);

            O_PreparationCompleted_SigNotConnected = (po & LoadPortGeneralOutputs.PreparationCompleted_SigNotConnected) != 0;
            O_TemporarilyStop_SigNotConnected      = (po & LoadPortGeneralOutputs.TemporarilyStop_SigNotConnected) != 0;
            O_SignificantError_SigNotConnected     = (po & LoadPortGeneralOutputs.SignificantError_SigNotConnected) != 0;
            O_LightError_SigNotConnected           = (po & LoadPortGeneralOutputs.LightError_SigNotConnected) != 0;
            O_Protrusion2Enabled                   = (po & LoadPortGeneralOutputs.Protrusion2Enabled) != 0;
            O_AdapterClamp                         = (po & LoadPortGeneralOutputs.AdapterClamp) != 0;
            O_AdapterPower                         = (po & LoadPortGeneralOutputs.AdapterPower) != 0;
            O_ObstacleDetectionCancel              = (po & LoadPortGeneralOutputs.ObstacleDetectionCancel) != 0;
            O_CarrierClampClose                    = (po & LoadPortGeneralOutputs.CarrierClampClose) != 0;
            O_CarrierClampOpen                     = (po & LoadPortGeneralOutputs.CarrierClampOpen) != 0;
            O_FOUPDoorLockOpen                     = (po & LoadPortGeneralOutputs.FOUPDoorLockOpen) != 0;
            O_FOUPDoorLockClose                    = (po & LoadPortGeneralOutputs.FOUPDoorLockClose) != 0;
            O_MappingSensorPreparation             = (po & LoadPortGeneralOutputs.MappingSensorPreparation) != 0;
            O_MappingSensorContaining              = (po & LoadPortGeneralOutputs.MappingSensorContaining) != 0;
            O_ChuckingOn                           = (po & LoadPortGeneralOutputs.ChuckingOn) != 0;
            O_ChuckingOff                          = (po & LoadPortGeneralOutputs.ChuckingOff) != 0;
            O_CoverLock                            = (po & LoadPortGeneralOutputs.CoverLock) != 0;
            O_CoverUnlock                          = (po & LoadPortGeneralOutputs.CoverUnlock) != 0;
            O_DoorOpen_ExtOutput                   = (po & LoadPortGeneralOutputs.DoorOpen_ExtOutput) != 0;
            O_CarrierClamp_ExtOutput               = (po & LoadPortGeneralOutputs.CarrierClamp_ExtOutput) != 0;
            O_CarrierPresenceOn_ExtOutput          = (po & LoadPortGeneralOutputs.CarrierPresenceOn_ExtOutput) != 0;
            O_PreparationCompleted_ExtOutput       = (po & LoadPortGeneralOutputs.PreparationCompleted_ExtOutput) != 0;
            O_CarrierProperlyPlaced_ExtOutput      = (po & LoadPortGeneralOutputs.CarrierProperlyPlaced_ExtOutput) != 0;
            O_StageRotationBackward                = (po & LoadPortGeneralOutputs.StageRotationBackward) != 0;
            O_StageRotationForward                 = (po & LoadPortGeneralOutputs.StageRotationForward) != 0;
            O_BcrLifting                           = (po & LoadPortGeneralOutputs.BcrLifting) != 0;
            O_BcrLowering                          = (po & LoadPortGeneralOutputs.BcrLowering) != 0;
            O_CarrierRetainerLowering              = (po & LoadPortGeneralOutputs.CarrierRetainerLowering) != 0;
            O_CarrierRetainerLifting               = (po & LoadPortGeneralOutputs.CarrierRetainerLifting) != 0;
            O_SW1_LED                              = (po & LoadPortGeneralOutputs.SW1_LED) != 0;
            O_SW3_LED                              = (po & LoadPortGeneralOutputs.SW3_LED) != 0;
            O_LOAD_LED                             = (po & LoadPortGeneralOutputs.LOAD_LED) != 0;
            O_UNLOAD_LED                           = (po & LoadPortGeneralOutputs.UNLOAD_LED) != 0;
            O_PRESENCE_LED                         = (po & LoadPortGeneralOutputs.PRESENCE_LED) != 0;
            O_PLACEMENT_LED                        = (po & LoadPortGeneralOutputs.PLACEMENT_LED) != 0;
            O_MANUAL_LED                           = (po & LoadPortGeneralOutputs.MANUAL_LED) != 0;
            O_ERROR_LED                            = (po & LoadPortGeneralOutputs.ERROR_LED) != 0;
            O_CLAMP_LED                            = (po & LoadPortGeneralOutputs.CLAMP_LED) != 0;
            O_DOCK_LED                             = (po & LoadPortGeneralOutputs.DOCK_LED) != 0;
            O_BUSY_LED                             = (po & LoadPortGeneralOutputs.BUSY_LED) != 0;
            O_AUTO_LED                             = (po & LoadPortGeneralOutputs.AUTO_LED) != 0;
            O_RESERVED_LED                         = (po & LoadPortGeneralOutputs.RESERVED_LED) != 0;
            O_CLOSE_LED                            = (po & LoadPortGeneralOutputs.CLOSE_LED) != 0;
            O_LOCK_LED                             = (po & LoadPortGeneralOutputs.LOCK_LED) != 0;
            O_L_REQ_E84                            = (po & LoadPortGeneralOutputs.L_REQ_E84) != 0;
            O_U_REQ_E84                            = (po & LoadPortGeneralOutputs.U_REQ_E84) != 0;
            O_READY_E84                            = (po & LoadPortGeneralOutputs.READY_E84) != 0;
            O_HO_AVBL_E84                          = (po & LoadPortGeneralOutputs.HO_AVBL_E84) != 0;
            O_ES_E84                               = (po & LoadPortGeneralOutputs.ES_E84) != 0;
        }

        #endregion Constructors

        #region Properties

        public bool I_EmergencyStop              { get; internal set; }
        public bool I_TemporarilyStop            { get; internal set; }
        public bool I_ExhaustFan1                { get; internal set; }
        public bool I_ExhaustFan2                { get; internal set; }
        public bool I_Protrusion                 { get; internal set; }
        public bool I_Protrusion2                { get; internal set; }
        public bool I_FOUPDoorLeftClose          { get; internal set; }
        public bool I_FOUPDoorLeftOpen           { get; internal set; }
        public bool I_FOUPDoorRightClose         { get; internal set; }
        public bool I_FOUPDoorRightOpen          { get; internal set; }
        public bool I_MappingSensorContaining    { get; internal set; }
        public bool I_MappingSensorPreparation   { get; internal set; }
        public bool I_UpperPressureLimit         { get; internal set; }
        public bool I_LowerPressureLimit         { get; internal set; }
        public bool I_CarrierClampOpen           { get; internal set; }
        public bool I_CarrierClampClose          { get; internal set; }
        public bool I_PresenceLeft               { get; internal set; }
        public bool I_PresenceRight              { get; internal set; }
        public bool I_PresenceMiddle             { get; internal set; }
        public bool I_InfoPadA                   { get; internal set; }
        public bool I_InfoPadB                   { get; internal set; }
        public bool I_InfoPadC                   { get; internal set; }
        public bool I_InfoPadD                   { get; internal set; }
        public bool I_Presence                   { get; internal set; }
        public bool I_FOSBIdentificationSensor   { get; internal set; }
        public bool I_ObstacleDetectingSensor    { get; internal set; }
        public bool I_DoorDetection              { get; internal set; }
        public bool I_OpenCarrierDetectingSensor { get; internal set; }
        public bool I_StageRotationBackward      { get; internal set; }
        public bool I_StageRotationForward       { get; internal set; }
        public bool I_BcrLifting                 { get; internal set; }
        public bool I_BcrLowering                { get; internal set; }
        public bool I_CoverLock                  { get; internal set; }
        public bool I_CoverUnlock                { get; internal set; }
        public bool I_CarrierRetainerLowering    { get; internal set; }
        public bool I_CarrierRetainerLifting     { get; internal set; }
        public bool I_External_SW1_ACCESS        { get; internal set; }
        public bool I_External_SW2_TEST          { get; internal set; }
        public bool I_External_SW3_UNLOAD        { get; internal set; }
        public bool I_PFA_L                      { get; internal set; }
        public bool I_PFA_R                      { get; internal set; }
        public bool I_Dsc300mm                   { get; internal set; }
        public bool I_Dsc200mm                   { get; internal set; }
        public bool I_Dsc150mm                   { get; internal set; }
        public bool I_CstCommon                  { get; internal set; }
        public bool I_Cst200mm                   { get; internal set; }
        public bool I_Cst150mm                   { get; internal set; }
        public bool I_Adapter                    { get; internal set; }
        public bool I_CoverClosed                { get; internal set; }
        public bool I_VALID_E84                  { get; internal set; }
        public bool I_CS_0_E84                   { get; internal set; }
        public bool I_CS_1_E84                   { get; internal set; }
        public bool I_TR_REQ_E84                 { get; internal set; }
        public bool I_BUSY_E84                   { get; internal set; }
        public bool I_COMPT_E84                  { get; internal set; }
        public bool I_CONT_E84                   { get; internal set; }

        public bool O_PreparationCompleted_SigNotConnected { get; internal set; }
        public bool O_TemporarilyStop_SigNotConnected      { get; internal set; }
        public bool O_SignificantError_SigNotConnected     { get; internal set; }
        public bool O_LightError_SigNotConnected           { get; internal set; }
        public bool O_Protrusion2Enabled                   { get; internal set; }
        public bool O_AdapterClamp                         { get; internal set; }
        public bool O_AdapterPower                         { get; internal set; }
        public bool O_ObstacleDetectionCancel              { get; internal set; }
        public bool O_CarrierClampClose                    { get; internal set; }
        public bool O_CarrierClampOpen                     { get; internal set; }
        public bool O_FOUPDoorLockOpen                     { get; internal set; }
        public bool O_FOUPDoorLockClose                    { get; internal set; }
        public bool O_MappingSensorPreparation             { get; internal set; }
        public bool O_MappingSensorContaining              { get; internal set; }
        public bool O_ChuckingOn                           { get; internal set; }
        public bool O_ChuckingOff                          { get; internal set; }
        public bool O_CoverLock                            { get; internal set; }
        public bool O_CoverUnlock                          { get; internal set; }
        public bool O_DoorOpen_ExtOutput                   { get; internal set; }
        public bool O_CarrierClamp_ExtOutput               { get; internal set; }
        public bool O_CarrierPresenceOn_ExtOutput          { get; internal set; }
        public bool O_PreparationCompleted_ExtOutput       { get; internal set; }
        public bool O_CarrierProperlyPlaced_ExtOutput      { get; internal set; }
        public bool O_StageRotationBackward                { get; internal set; }
        public bool O_StageRotationForward                 { get; internal set; }
        public bool O_BcrLifting                           { get; internal set; }
        public bool O_BcrLowering                          { get; internal set; }
        public bool O_CarrierRetainerLowering              { get; internal set; }
        public bool O_CarrierRetainerLifting               { get; internal set; }
        public bool O_SW1_LED                              { get; internal set; }
        public bool O_SW3_LED                              { get; internal set; }
        public bool O_LOAD_LED                             { get; internal set; }
        public bool O_UNLOAD_LED                           { get; internal set; }
        public bool O_PRESENCE_LED                         { get; internal set; }
        public bool O_PLACEMENT_LED                        { get; internal set; }
        public bool O_MANUAL_LED                           { get; internal set; }
        public bool O_ERROR_LED                            { get; internal set; }
        public bool O_CLAMP_LED                            { get; internal set; }
        public bool O_DOCK_LED                             { get; internal set; }
        public bool O_BUSY_LED                             { get; internal set; }
        public bool O_AUTO_LED                             { get; internal set; }
        public bool O_RESERVED_LED                         { get; internal set; }
        public bool O_CLOSE_LED                            { get; internal set; }
        public bool O_LOCK_LED                             { get; internal set; }
        public bool O_L_REQ_E84                            { get; internal set; }
        public bool O_U_REQ_E84                            { get; internal set; }
        public bool O_READY_E84                            { get; internal set; }
        public bool O_HO_AVBL_E84                          { get; internal set; }
        public bool O_ES_E84                               { get; internal set; }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(LoadPortGpioStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    I_EmergencyStop              = false;
                    I_TemporarilyStop            = false;
                    I_ExhaustFan1                = false;
                    I_ExhaustFan2                = false;
                    I_Protrusion                 = false;
                    I_Protrusion2                = false;
                    I_FOUPDoorLeftClose          = false;
                    I_FOUPDoorLeftOpen           = false;
                    I_FOUPDoorRightClose         = false;
                    I_FOUPDoorRightOpen          = false;
                    I_MappingSensorContaining    = false;
                    I_MappingSensorPreparation   = false;
                    I_UpperPressureLimit         = false;
                    I_LowerPressureLimit         = false;
                    I_CarrierClampOpen           = false;
                    I_CarrierClampClose          = false;
                    I_PresenceLeft               = false;
                    I_PresenceRight              = false;
                    I_PresenceMiddle             = false;
                    I_InfoPadA                   = false;
                    I_InfoPadB                   = false;
                    I_InfoPadC                   = false;
                    I_InfoPadD                   = false;
                    I_Presence                   = false;
                    I_FOSBIdentificationSensor   = false;
                    I_ObstacleDetectingSensor    = false;
                    I_DoorDetection              = false;
                    I_OpenCarrierDetectingSensor = false;
                    I_StageRotationBackward      = false;
                    I_StageRotationForward       = false;
                    I_BcrLifting                 = false;
                    I_BcrLowering                = false;
                    I_CoverLock                  = false;
                    I_CoverUnlock                = false;
                    I_CarrierRetainerLowering    = false;
                    I_CarrierRetainerLifting     = false;
                    I_External_SW1_ACCESS        = false;
                    I_External_SW2_TEST          = false;
                    I_External_SW3_UNLOAD        = false;
                    I_PFA_L                      = false;
                    I_PFA_R                      = false;
                    I_Dsc300mm                   = false;
                    I_Dsc200mm                   = false;
                    I_Dsc150mm                   = false;
                    I_CstCommon                  = false;
                    I_Cst200mm                   = false;
                    I_Cst150mm                   = false;
                    I_Adapter                    = false;
                    I_CoverClosed                = false;
                    I_VALID_E84                  = false;
                    I_CS_0_E84                   = false;
                    I_CS_1_E84                   = false;
                    I_TR_REQ_E84                 = false;
                    I_BUSY_E84                   = false;
                    I_COMPT_E84                  = false;
                    I_CONT_E84                   = false;

                    O_PreparationCompleted_SigNotConnected = false;
                    O_TemporarilyStop_SigNotConnected      = false;
                    O_SignificantError_SigNotConnected     = false;
                    O_LightError_SigNotConnected           = false;
                    O_Protrusion2Enabled                   = false;
                    O_AdapterClamp                         = false;
                    O_AdapterPower                         = false;
                    O_ObstacleDetectionCancel              = false;
                    O_CarrierClampClose                    = false;
                    O_CarrierClampOpen                     = false;
                    O_FOUPDoorLockOpen                     = false;
                    O_FOUPDoorLockClose                    = false;
                    O_MappingSensorPreparation             = false;
                    O_MappingSensorContaining              = false;
                    O_ChuckingOn                           = false;
                    O_ChuckingOff                          = false;
                    O_CoverLock                            = false;
                    O_CoverUnlock                          = false;
                    O_DoorOpen_ExtOutput                   = false;
                    O_CarrierClamp_ExtOutput               = false;
                    O_CarrierPresenceOn_ExtOutput          = false;
                    O_PreparationCompleted_ExtOutput       = false;
                    O_CarrierProperlyPlaced_ExtOutput      = false;
                    O_StageRotationBackward                = false;
                    O_StageRotationForward                 = false;
                    O_BcrLifting                           = false;
                    O_BcrLowering                          = false;
                    O_CarrierRetainerLowering              = false;
                    O_CarrierRetainerLifting               = false;
                    O_SW1_LED                              = false;
                    O_SW3_LED                              = false;
                    O_LOAD_LED                             = false;
                    O_UNLOAD_LED                           = false;
                    O_PRESENCE_LED                         = false;
                    O_PLACEMENT_LED                        = false;
                    O_MANUAL_LED                           = false;
                    O_ERROR_LED                            = false;
                    O_CLAMP_LED                            = false;
                    O_DOCK_LED                             = false;
                    O_BUSY_LED                             = false;
                    O_AUTO_LED                             = false;
                    O_RESERVED_LED                         = false;
                    O_CLOSE_LED                            = false;
                    O_LOCK_LED                             = false;
                    O_L_REQ_E84                            = false;
                    O_U_REQ_E84                            = false;
                    O_READY_E84                            = false;
                    O_HO_AVBL_E84                          = false;
                    O_ES_E84                               = false;
                }
                else
                {
                    I_EmergencyStop              = other.I_EmergencyStop;
                    I_TemporarilyStop            = other.I_TemporarilyStop;
                    I_ExhaustFan1                = other.I_ExhaustFan1;
                    I_ExhaustFan2                = other.I_ExhaustFan2;
                    I_Protrusion                 = other.I_Protrusion;
                    I_Protrusion2                = other.I_Protrusion2;
                    I_FOUPDoorLeftClose          = other.I_FOUPDoorLeftClose;
                    I_FOUPDoorLeftOpen           = other.I_FOUPDoorLeftOpen;
                    I_FOUPDoorRightClose         = other.I_FOUPDoorRightClose;
                    I_FOUPDoorRightOpen          = other.I_FOUPDoorRightOpen;
                    I_MappingSensorContaining    = other.I_MappingSensorContaining;
                    I_MappingSensorPreparation   = other.I_MappingSensorPreparation;
                    I_UpperPressureLimit         = other.I_UpperPressureLimit;
                    I_LowerPressureLimit         = other.I_LowerPressureLimit;
                    I_CarrierClampOpen           = other.I_CarrierClampOpen;
                    I_CarrierClampClose          = other.I_CarrierClampClose;
                    I_PresenceLeft               = other.I_PresenceLeft;
                    I_PresenceRight              = other.I_PresenceRight;
                    I_PresenceMiddle             = other.I_PresenceMiddle;
                    I_InfoPadA                   = other.I_InfoPadA;
                    I_InfoPadB                   = other.I_InfoPadB;
                    I_InfoPadC                   = other.I_InfoPadC;
                    I_InfoPadD                   = other.I_InfoPadD;
                    I_Presence                   = other.I_Presence;
                    I_FOSBIdentificationSensor   = other.I_FOSBIdentificationSensor;
                    I_ObstacleDetectingSensor    = other.I_ObstacleDetectingSensor;
                    I_DoorDetection              = other.I_DoorDetection;
                    I_OpenCarrierDetectingSensor = other.I_OpenCarrierDetectingSensor;
                    I_StageRotationBackward      = other.I_StageRotationBackward;
                    I_StageRotationForward       = other.I_StageRotationForward;
                    I_BcrLifting                 = other.I_BcrLifting;
                    I_BcrLowering                = other.I_BcrLowering;
                    I_CoverLock                  = other.I_CoverLock;
                    I_CoverUnlock                = other.I_CoverUnlock;
                    I_CarrierRetainerLowering    = other.I_CarrierRetainerLowering;
                    I_CarrierRetainerLifting     = other.I_CarrierRetainerLifting;
                    I_External_SW1_ACCESS        = other.I_External_SW1_ACCESS;
                    I_External_SW2_TEST          = other.I_External_SW2_TEST;
                    I_External_SW3_UNLOAD        = other.I_External_SW3_UNLOAD;
                    I_PFA_L                      = other.I_PFA_L;
                    I_PFA_R                      = other.I_PFA_R;
                    I_Dsc300mm                   = other.I_Dsc300mm;
                    I_Dsc200mm                   = other.I_Dsc200mm;
                    I_Dsc150mm                   = other.I_Dsc150mm;
                    I_CstCommon                  = other.I_CstCommon;
                    I_Cst200mm                   = other.I_Cst200mm;
                    I_Cst150mm                   = other.I_Cst150mm;
                    I_Adapter                    = other.I_Adapter;
                    I_CoverClosed                = other.I_CoverClosed;
                    I_VALID_E84                  = other.I_VALID_E84;
                    I_CS_0_E84                   = other.I_CS_0_E84;
                    I_CS_1_E84                   = other.I_CS_1_E84;
                    I_TR_REQ_E84                 = other.I_TR_REQ_E84;
                    I_BUSY_E84                   = other.I_BUSY_E84;
                    I_COMPT_E84                  = other.I_COMPT_E84;
                    I_CONT_E84                   = other.I_CONT_E84;

                    O_PreparationCompleted_SigNotConnected = other.O_PreparationCompleted_SigNotConnected;
                    O_TemporarilyStop_SigNotConnected      = other.O_TemporarilyStop_SigNotConnected;
                    O_SignificantError_SigNotConnected     = other.O_SignificantError_SigNotConnected;
                    O_LightError_SigNotConnected           = other.O_LightError_SigNotConnected;
                    O_Protrusion2Enabled                   = other.O_Protrusion2Enabled;
                    O_AdapterClamp                         = other.O_AdapterClamp;
                    O_AdapterPower                         = other.O_AdapterPower;
                    O_ObstacleDetectionCancel              = other.O_ObstacleDetectionCancel;
                    O_CarrierClampClose                    = other.O_CarrierClampClose;
                    O_CarrierClampOpen                     = other.O_CarrierClampOpen;
                    O_FOUPDoorLockOpen                     = other.O_FOUPDoorLockOpen;
                    O_FOUPDoorLockClose                    = other.O_FOUPDoorLockClose;
                    O_MappingSensorPreparation             = other.O_MappingSensorPreparation;
                    O_MappingSensorContaining              = other.O_MappingSensorContaining;
                    O_ChuckingOn                           = other.O_ChuckingOn;
                    O_ChuckingOff                          = other.O_ChuckingOff;
                    O_CoverLock                            = other.O_CoverLock;
                    O_CoverUnlock                          = other.O_CoverUnlock;
                    O_DoorOpen_ExtOutput                   = other.O_DoorOpen_ExtOutput;
                    O_CarrierClamp_ExtOutput               = other.O_CarrierClamp_ExtOutput;
                    O_CarrierPresenceOn_ExtOutput          = other.O_CarrierPresenceOn_ExtOutput;
                    O_PreparationCompleted_ExtOutput       = other.O_PreparationCompleted_ExtOutput;
                    O_CarrierProperlyPlaced_ExtOutput      = other.O_CarrierProperlyPlaced_ExtOutput;
                    O_StageRotationBackward                = other.O_StageRotationBackward;
                    O_StageRotationForward                 = other.O_StageRotationForward;
                    O_BcrLifting                           = other.O_BcrLifting;
                    O_BcrLowering                          = other.O_BcrLowering;
                    O_CarrierRetainerLowering              = other.O_CarrierRetainerLowering;
                    O_CarrierRetainerLifting               = other.O_CarrierRetainerLifting;
                    O_SW1_LED                              = other.O_SW1_LED;
                    O_SW3_LED                              = other.O_SW3_LED;
                    O_LOAD_LED                             = other.O_LOAD_LED;
                    O_UNLOAD_LED                           = other.O_UNLOAD_LED;
                    O_PRESENCE_LED                         = other.O_PRESENCE_LED;
                    O_PLACEMENT_LED                        = other.O_PLACEMENT_LED;
                    O_MANUAL_LED                           = other.O_MANUAL_LED;
                    O_ERROR_LED                            = other.O_ERROR_LED;
                    O_CLAMP_LED                            = other.O_CLAMP_LED;
                    O_DOCK_LED                             = other.O_DOCK_LED;
                    O_BUSY_LED                             = other.O_BUSY_LED;
                    O_AUTO_LED                             = other.O_AUTO_LED;
                    O_RESERVED_LED                         = other.O_RESERVED_LED;
                    O_CLOSE_LED                            = other.O_CLOSE_LED;
                    O_LOCK_LED                             = other.O_LOCK_LED;
                    O_L_REQ_E84                            = other.O_L_REQ_E84;
                    O_U_REQ_E84                            = other.O_U_REQ_E84;
                    O_READY_E84                            = other.O_READY_E84;
                    O_HO_AVBL_E84                          = other.O_HO_AVBL_E84;
                    O_ES_E84                               = other.O_ES_E84;
                }
            }
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new LoadPortGpioStatus(this);
        }

        #endregion Status Override
    }
}
