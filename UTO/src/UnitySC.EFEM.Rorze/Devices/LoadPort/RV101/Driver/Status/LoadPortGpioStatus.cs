using System;
using System.Globalization;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status
{
    #region Enums

    /// <summary>
    /// RV101 GPIO inputs statuses for system type 1 to 3.
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
        VacuumSourcePressure       = (Int64) 1 <<  2,
        AirSupplySourcePressure    = (Int64) 1 <<  3,
        /* bits 4 to 6 are not connected */
        ProtrusionDetection        = (Int64) 1 <<  7,

        Cover                      = (Int64) 1 <<  8,
        DrivePower                 = (Int64) 1 <<  9,
        /* bits 9 to 11 are not connected */
        MappingSensor              = (Int64) 1 <<  12,
        /* bits 13 to 15 are not connected */

        ShutterOpen                = (Int64) 1 <<  16,
        ShutterClose               = (Int64) 1 <<  17,
        PresenceLeft               = (Int64) 1 << 18,
        PresenceRight              = (Int64) 1 << 19,
        PresenceMiddle             = (Int64) 1 << 20,
        InfoPadA                   = (Int64) 1 << 21,
        InfoPadB                   = (Int64) 1 << 22,
        InfoPadC                   = (Int64) 1 << 23,

        InfoPadD                   = (Int64) 1 << 24,
        /* bits 25 and 27 are not connected */
        PresenceLeft200mm = (Int64) 1 << 28,
        PresenceRight200mm = (Int64) 1 << 29,
        PresenceLeft150mm = (Int64) 1 << 30,
        PresenceRight150mm = (Int64) 1 << 31,

        /* bits 32 and 39 are not connected */

        AccessSwitch1 = (Int64) 1 << 40,
        AccessSwitch2 = (Int64) 1 << 41,
        /* bits 42 and 47 are not connected */

        /* bits 48 and 55 are not connected */

        /* bits 56 and 63 are not connected */
    }

    /// <summary>
    /// RV101 GPIO outputs statuses for system type 1 to 3.
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
        /* bits 4 to 7 are not connected */

        ClampMovingDirection                 = (Int64)1 << 8,
        ClampMovingStart                     = (Int64)1 << 9,
        /* bits 10 to 15 are not connected */

        ShutterOpen                          = (Int64) 1 << 16,
        ShutterClose                         = (Int64) 1 << 17,
        ShutterMotionDisabled                = (Int64) 1 << 18,
        /* bits 19 and 23 are not connected */

        ShutterOpen2                         = (Int64) 1 << 24,
        CoverLock                            = (Int64) 1 << 25,
        CarrierPresenceSensorOn              = (Int64) 1 << 26,
        PreparationCompleted2                = (Int64) 1 << 27,
        CarrierProperlyPlaced                = (Int64) 1 << 28,
        /* bits 29 to 31 are not connected */

        /* bits 32 to 39 are not connected */

        AccessSwitch1                        = (Int64) 1 << 40,
        AccessSwitch2                        = (Int64) 1 << 41,
        LOAD_LED                             = (Int64) 1 << 42,
        UNLOAD_LED                           = (Int64) 1 << 43,
        PRESENCE_LED                         = (Int64) 1 << 44,
        PLACEMENT_LED                        = (Int64) 1 << 45,
        LATCH_LED                            = (Int64) 1 << 46,
        ERROR_LED                            = (Int64) 1 << 47,

        /* bits 48 to 49 are not connected */
        BUSY_LED                             = (Int64) 1 << 50,
        /* bits 51 to 55 are not connected */

        /* bits 56 to 63 are not connected */
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
            I_VacuumSourcePressure = (pi & LoadPortGeneralInputs.VacuumSourcePressure) != 0;
            I_AirSupplySourcePressure = (pi & LoadPortGeneralInputs.AirSupplySourcePressure) != 0;
            I_ProtrusionDetection = (pi & LoadPortGeneralInputs.ProtrusionDetection) != 0;
            I_Cover = (pi & LoadPortGeneralInputs.Cover) != 0;
            I_DrivePower = (pi & LoadPortGeneralInputs.DrivePower) != 0;
            I_MappingSensor = (pi & LoadPortGeneralInputs.MappingSensor) != 0;
            I_ShutterOpen = (pi & LoadPortGeneralInputs.ShutterOpen) != 0;
            I_ShutterClose = (pi & LoadPortGeneralInputs.ShutterClose) != 0;
            I_PresenceLeft               = (pi & LoadPortGeneralInputs.PresenceLeft) != 0;
            I_PresenceRight              = (pi & LoadPortGeneralInputs.PresenceRight) != 0;
            I_PresenceMiddle             = (pi & LoadPortGeneralInputs.PresenceMiddle) != 0;
            I_InfoPadA                   = (pi & LoadPortGeneralInputs.InfoPadA) != 0;
            I_InfoPadB                   = (pi & LoadPortGeneralInputs.InfoPadB) != 0;
            I_InfoPadC                   = (pi & LoadPortGeneralInputs.InfoPadC) != 0;
            I_InfoPadD                   = (pi & LoadPortGeneralInputs.InfoPadD) != 0;
            I_200mmPresenceLeft = (pi & LoadPortGeneralInputs.PresenceLeft200mm) != 0;
            I_200mmPresenceRight = (pi & LoadPortGeneralInputs.PresenceRight200mm) != 0;
            I_150mmPresenceLeft = (pi & LoadPortGeneralInputs.PresenceLeft150mm) != 0;
            I_150mmPresenceRight = (pi & LoadPortGeneralInputs.PresenceRight150mm) != 0;
            I_AccessSwitch1 = (pi & LoadPortGeneralInputs.AccessSwitch1) != 0;
            I_AccessSwitch2 = (pi & LoadPortGeneralInputs.AccessSwitch2) != 0;

            var po = (LoadPortGeneralOutputs)Int64.Parse(statuses[1], NumberStyles.AllowHexSpecifier);

            O_PreparationCompleted = (po & LoadPortGeneralOutputs.PreparationCompleted_SigNotConnected) != 0;
            O_TemporarilyStop = (po & LoadPortGeneralOutputs.TemporarilyStop_SigNotConnected) != 0;
            O_SignificantError = (po & LoadPortGeneralOutputs.SignificantError_SigNotConnected) != 0;
            O_LightError = (po & LoadPortGeneralOutputs.LightError_SigNotConnected) != 0;
            O_ClampMovingDirection = (po & LoadPortGeneralOutputs.ClampMovingDirection) != 0;
            O_ClampMovingStart = (po & LoadPortGeneralOutputs.ClampMovingStart) != 0;
            O_ShutterOpen = (po & LoadPortGeneralOutputs.ShutterOpen) != 0;
            O_ShutterClose = (po & LoadPortGeneralOutputs.ShutterClose) != 0;
            O_ShutterMotionDisabled = (po & LoadPortGeneralOutputs.ShutterMotionDisabled) != 0;
            O_ShutterOpen2 = (po & LoadPortGeneralOutputs.ShutterOpen2) != 0;
            O_CoverLock = (po & LoadPortGeneralOutputs.CoverLock) != 0;
            O_CarrierPresenceSensorOn = (po & LoadPortGeneralOutputs.CarrierPresenceSensorOn) != 0;
            O_PreparationCompleted2 = (po & LoadPortGeneralOutputs.PreparationCompleted2) != 0;
            O_CarrierProperlyPlaced = (po & LoadPortGeneralOutputs.CarrierProperlyPlaced) != 0;
            O_AccessSwitch1 = (po & LoadPortGeneralOutputs.AccessSwitch1) != 0;
            O_AccessSwitch2 = (po & LoadPortGeneralOutputs.AccessSwitch2) != 0;
            O_LOAD_LED                             = (po & LoadPortGeneralOutputs.LOAD_LED) != 0;
            O_UNLOAD_LED                           = (po & LoadPortGeneralOutputs.UNLOAD_LED) != 0;
            O_PRESENCE_LED                         = (po & LoadPortGeneralOutputs.PRESENCE_LED) != 0;
            O_PLACEMENT_LED                        = (po & LoadPortGeneralOutputs.PLACEMENT_LED) != 0;
            O_LATCH_LED                        = (po & LoadPortGeneralOutputs.LATCH_LED) != 0;
            O_ERROR_LED                            = (po & LoadPortGeneralOutputs.ERROR_LED) != 0;
            O_BUSY_LED                             = (po & LoadPortGeneralOutputs.BUSY_LED) != 0;
        }

        #endregion Constructors

        #region Properties

        public bool I_EmergencyStop { get; internal set; }
        public bool I_TemporarilyStop { get; internal set; }
        public bool I_VacuumSourcePressure { get; internal set; }
        public bool I_AirSupplySourcePressure { get; internal set; }
        public bool I_ProtrusionDetection { get; internal set; }
        public bool I_Cover { get; internal set; }
        public bool I_DrivePower { get; internal set; }
        public bool I_MappingSensor { get; internal set; }
        public bool I_ShutterOpen { get; internal set; }
        public bool I_ShutterClose { get; internal set; }
        public bool I_PresenceLeft { get; internal set; }
        public bool I_PresenceRight { get; internal set; }
        public bool I_PresenceMiddle { get; internal set; }
        public bool I_InfoPadA { get; internal set; }
        public bool I_InfoPadB { get; internal set; }
        public bool I_InfoPadC { get; internal set; }
        public bool I_InfoPadD { get; internal set; }
        public bool I_200mmPresenceLeft { get; internal set; }
        public bool I_200mmPresenceRight { get; internal set; }
        public bool I_150mmPresenceLeft { get; internal set; }
        public bool I_150mmPresenceRight { get; internal set; }
        public bool I_AccessSwitch1 { get; internal set; }
        public bool I_AccessSwitch2 { get; internal set; }

        public bool O_PreparationCompleted { get; internal set; }
        public bool O_TemporarilyStop { get; internal set; }
        public bool O_SignificantError { get; internal set; }
        public bool O_LightError { get; internal set; }
        public bool O_ClampMovingDirection { get; internal set; }
        public bool O_ClampMovingStart { get; internal set; }
        public bool O_ShutterOpen { get; internal set; }
        public bool O_ShutterClose { get; internal set; }
        public bool O_ShutterMotionDisabled { get; internal set; }
        public bool O_ShutterOpen2 { get; internal set; }
        public bool O_CoverLock { get; internal set; }
        public bool O_CarrierPresenceSensorOn { get; internal set; }
        public bool O_PreparationCompleted2 { get; internal set; }
        public bool O_CarrierProperlyPlaced { get; internal set; }
        public bool O_AccessSwitch1 { get; internal set; }
        public bool O_AccessSwitch2 { get; internal set; }
        public bool O_LOAD_LED { get; internal set; }
        public bool O_UNLOAD_LED { get; internal set; }
        public bool O_PRESENCE_LED { get; internal set; }
        public bool O_PLACEMENT_LED { get; internal set; }
        public bool O_LATCH_LED { get; internal set; }
        public bool O_ERROR_LED { get; internal set; }
        public bool O_BUSY_LED { get; internal set; }

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
                    I_VacuumSourcePressure = false;
                    I_AirSupplySourcePressure = false;
                    I_ProtrusionDetection = false;
                    I_Cover = false;
                    I_DrivePower = false;
                    I_MappingSensor = false;
                    I_ShutterOpen = false;
                    I_ShutterClose = false;
                    I_PresenceLeft               = false;
                    I_PresenceRight              = false;
                    I_PresenceMiddle             = false;
                    I_InfoPadA                   = false;
                    I_InfoPadB                   = false;
                    I_InfoPadC                   = false;
                    I_InfoPadD                   = false;
                    I_200mmPresenceLeft = false;
                    I_200mmPresenceRight = false;
                    I_150mmPresenceLeft = false;
                    I_150mmPresenceRight = false;
                    I_AccessSwitch1 = false;
                    I_AccessSwitch2 = false;

                    O_PreparationCompleted = false;
                    O_TemporarilyStop = false;
                    O_SignificantError = false;
                    O_LightError = false;
                    O_ClampMovingDirection = false;
                    O_ClampMovingStart = false;
                    O_ShutterOpen = false;
                    O_ShutterClose = false;
                    O_ShutterMotionDisabled = false;
                    O_ShutterOpen2 = false;
                    O_CoverLock                            = false;
                    O_CarrierPresenceSensorOn = false;
                    O_PreparationCompleted2 = false;
                    O_CarrierProperlyPlaced = false;
                    O_AccessSwitch1 = false;
                    O_AccessSwitch2 = false;
                    O_LOAD_LED                             = false;
                    O_UNLOAD_LED                           = false;
                    O_PRESENCE_LED                         = false;
                    O_PLACEMENT_LED                        = false;
                    O_LATCH_LED                           = false;
                    O_ERROR_LED                            = false;
                    O_BUSY_LED                             = false;
                }
                else
                {
                    I_EmergencyStop              = other.I_EmergencyStop;
                    I_TemporarilyStop            = other.I_TemporarilyStop;
                    I_VacuumSourcePressure = other.I_VacuumSourcePressure;
                    I_AirSupplySourcePressure = other.I_AirSupplySourcePressure;
                    I_ProtrusionDetection = other.I_ProtrusionDetection;
                    I_Cover = other.I_Cover;
                    I_DrivePower = other.I_DrivePower;
                    I_MappingSensor = other.I_MappingSensor;
                    I_ShutterOpen = other.I_ShutterOpen;
                    I_ShutterClose = other.I_ShutterClose;
                    I_PresenceLeft               = other.I_PresenceLeft;
                    I_PresenceRight              = other.I_PresenceRight;
                    I_PresenceMiddle             = other.I_PresenceMiddle;
                    I_InfoPadA                   = other.I_InfoPadA;
                    I_InfoPadB                   = other.I_InfoPadB;
                    I_InfoPadC                   = other.I_InfoPadC;
                    I_InfoPadD                   = other.I_InfoPadD;
                    I_200mmPresenceLeft = other.I_200mmPresenceLeft;
                    I_200mmPresenceRight = other.I_200mmPresenceRight;
                    I_150mmPresenceLeft = other.I_150mmPresenceLeft;
                    I_150mmPresenceRight = other.I_150mmPresenceRight;
                    I_AccessSwitch1 = other.I_AccessSwitch1;
                    I_AccessSwitch2 = other.I_AccessSwitch2;

                    O_PreparationCompleted = other.O_PreparationCompleted;
                    O_TemporarilyStop = other.O_TemporarilyStop;
                    O_SignificantError = other.O_SignificantError;
                    O_LightError = other.O_LightError;
                    O_ClampMovingDirection = other.O_ClampMovingDirection;
                    O_ClampMovingStart = other.O_ClampMovingStart;
                    O_ShutterOpen = other.O_ShutterOpen;
                    O_ShutterClose = other.O_ShutterClose;
                    O_ShutterMotionDisabled = other.O_ShutterMotionDisabled;
                    O_ShutterOpen2 = other.O_ShutterOpen2;
                    O_CoverLock = other.O_CoverLock;
                    O_CarrierPresenceSensorOn = other.O_CarrierPresenceSensorOn;
                    O_PreparationCompleted2 = other.O_PreparationCompleted2;
                    O_CarrierProperlyPlaced = other.O_CarrierProperlyPlaced;
                    O_AccessSwitch1 = other.O_AccessSwitch1;
                    O_AccessSwitch2 = other.O_AccessSwitch2;
                    O_LOAD_LED                             = other.O_LOAD_LED;
                    O_UNLOAD_LED                           = other.O_UNLOAD_LED;
                    O_PRESENCE_LED                         = other.O_PRESENCE_LED;
                    O_PLACEMENT_LED                        = other.O_PLACEMENT_LED;
                    O_LATCH_LED                           = other.O_LATCH_LED;
                    O_ERROR_LED                            = other.O_ERROR_LED;
                    O_BUSY_LED                             = other.O_BUSY_LED;
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
