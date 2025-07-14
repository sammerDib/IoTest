using System;
using System.Globalization;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status
{
    #region Enums

    /// <summary>
    ///     RE201 GPIO inputs statuses.
    /// </summary>
    [Flags]
    public enum LoadPortGeneralInputs
    {
        /* bits 0 to 5 are not connected */

        SubstrateDetection = 1 << 6,
        MotionProhibited = 1 << 7,
        ClampRightClose = 1 << 8,
        ClampLeftClose = 1 << 9,
        ClampRightOpen = 1 << 10,
        ClampLeftOpen = 1 << 11,

        /* bits 12 to 16 are not connected */

        CarrierPresenceMiddle = 1 << 17,
        CarrierPresenceLeft = 1 << 18,
        CarrierPresenceRight = 1 << 19,
        AccessSwitch = 1 << 20,
        ProtrusionDetection = 1 << 21,

        /* bits 22 to 23 are not connected */

        InfoPadA = 1 << 24,
        InfoPadB = 1 << 25,
        InfoPadC = 1 << 26,
        InfoPadD = 1 << 27,
        PositionForReadingId = 1 << 28

        /* bit 28 to 31 are not connected */
    }

    /// <summary>
    ///     RE201 GPIO outputs statuses.
    /// </summary>
    [Flags]
    public enum LoadPortGeneralOutputs
    {
        PreparationCompleted_SigNotConnected = 1 << 0,
        TemporarilyStop_SigNotConnected = 1 << 1,
        SignificantError_SigNotConnected = 1 << 2,
        LightError_SigNotConnected = 1 << 3,

        /* bit 4 to 5 are not connected */

        LaserStop = 1 << 6,
        InterlockCancel = 1 << 7,
        CarrierClampCloseRight = 1 << 8,
        CarrierClampOpenRight = 1 << 9,
        CarrierClampCloseLeft = 1 << 10,
        CarrierClampOpenLeft = 1 << 11,

        /* bits 12 to 15 are not connected */

        GreenIndicator = 1 << 16,
        RedIndicator = 1 << 17,
        LoadIndicator = 1 << 18,
        UnloadIndicator = 1 << 19,
        AccessSwitchIndicator = 1 << 20,

        /* bits 21 to 23 are not connected */

        CarrierOpen_SigNotConnected = 1 << 24,
        CarrierClamp_SigNotConnected = 1 << 25,
        PodPresenceSensorOn_SigNotConnected = 1 << 26,
        PreparationCompleted2_SigNotConnected = 1 << 27,
        CarrierProperPlaced_SigNotConnected = 1 << 28

        /* bits 29 to 31 are not connected */
    }

    #endregion Enums

    public class RE201GpioStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Private Methods

        /// <summary>
        ///     Copy statuses from on received data.
        ///     <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(RE201GpioStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    I_SubstrateDetection = false;
                    I_MotionProhibited = false;
                    I_ClampRightClose = false;
                    I_ClampLeftClose = false;
                    I_ClampRightOpen = false;
                    I_ClampLeftOpen = false;
                    I_CarrierPresenceMiddle = false;
                    I_CarrierPresenceLeft = false;
                    I_CarrierPresenceRight = false;
                    I_AccessSwitch = false;
                    I_ProtrusionDetection = false;
                    I_InfoPadA = false;
                    I_InfoPadB = false;
                    I_InfoPadC = false;
                    I_InfoPadD = false;
                    I_PositionForReadingId = false;

                    O_PreparationCompleted_SigNotConnected = false;
                    O_TemporarilyStop_SigNotConnected = false;
                    O_SignificantError_SigNotConnected = false;
                    O_LightError_SigNotConnected = false;
                    O_LaserStop = false;
                    O_InterlockCancel = false;
                    O_CarrierClampCloseRight = false;
                    O_CarrierClampOpenRight = false;
                    O_CarrierClampCloseLeft = false;
                    O_CarrierClampOpenLeft = false;
                    O_GreenIndicator = false;
                    O_RedIndicator = false;
                    O_LoadIndicator = false;
                    O_UnloadIndicator = false;
                    O_AccessSwitchIndicator = false;
                    O_CarrierOpen_SigNotConnected = false;
                    O_CarrierClamp_SigNotConnected = false;
                    O_PodPresenceSensorOn_SigNotConnected = false;
                    O_PreparationCompleted2_SigNotConnected = false;
                    O_CarrierProperPlaced_SigNotConnected = false;
                }
                else
                {
                    I_SubstrateDetection = other.I_SubstrateDetection;
                    I_MotionProhibited = other.I_MotionProhibited;
                    I_ClampRightClose = other.I_ClampRightClose;
                    I_ClampLeftClose = other.I_ClampLeftClose;
                    I_ClampRightOpen = other.I_ClampRightOpen;
                    I_ClampLeftOpen = other.I_ClampLeftOpen;
                    I_CarrierPresenceMiddle = other.I_CarrierPresenceMiddle;
                    I_CarrierPresenceLeft = other.I_CarrierPresenceLeft;
                    I_CarrierPresenceRight = other.I_CarrierPresenceRight;
                    I_AccessSwitch = other.I_AccessSwitch;
                    I_ProtrusionDetection = other.I_ProtrusionDetection;
                    I_InfoPadA = other.I_InfoPadA;
                    I_InfoPadB = other.I_InfoPadB;
                    I_InfoPadC = other.I_InfoPadC;
                    I_InfoPadD = other.I_InfoPadD;
                    I_PositionForReadingId = other.I_PositionForReadingId;

                    O_PreparationCompleted_SigNotConnected = other.O_PreparationCompleted_SigNotConnected;
                    O_TemporarilyStop_SigNotConnected = other.O_TemporarilyStop_SigNotConnected;
                    O_SignificantError_SigNotConnected = other.O_SignificantError_SigNotConnected;
                    O_LightError_SigNotConnected = other.O_LightError_SigNotConnected;
                    O_LaserStop = other.O_LaserStop;
                    O_InterlockCancel = other.O_InterlockCancel;
                    O_CarrierClampCloseRight = other.O_CarrierClampCloseRight;
                    O_CarrierClampOpenRight = other.O_CarrierClampOpenRight;
                    O_CarrierClampCloseLeft = other.O_CarrierClampCloseLeft;
                    O_CarrierClampOpenLeft = other.O_CarrierClampOpenLeft;
                    O_GreenIndicator = other.O_GreenIndicator;
                    O_RedIndicator = other.O_RedIndicator;
                    O_LoadIndicator = other.O_LoadIndicator;
                    O_UnloadIndicator = other.O_UnloadIndicator;
                    O_AccessSwitchIndicator = other.O_AccessSwitchIndicator;
                    O_CarrierOpen_SigNotConnected = other.O_CarrierOpen_SigNotConnected;
                    O_CarrierClamp_SigNotConnected = other.O_CarrierClamp_SigNotConnected;
                    O_PodPresenceSensorOn_SigNotConnected = other.O_PodPresenceSensorOn_SigNotConnected;
                    O_PreparationCompleted2_SigNotConnected = other.O_PreparationCompleted2_SigNotConnected;
                    O_CarrierProperPlaced_SigNotConnected = other.O_CarrierProperPlaced_SigNotConnected;
                }
            }
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new RE201GpioStatus(this);
        }

        #endregion Status Override

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RE201GpioStatus" /> class.
        ///     <param name="other">Create a deep copy of <see cref="RE201GpioStatus" /> instance</param>
        /// </summary>
        public RE201GpioStatus(RE201GpioStatus other)
        {
            Set(other);
        }

        public RE201GpioStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            var pi = (LoadPortGeneralInputs)Int32.Parse(statuses[0], NumberStyles.AllowHexSpecifier);

            I_SubstrateDetection = (pi & LoadPortGeneralInputs.SubstrateDetection) != 0;
            I_MotionProhibited = (pi & LoadPortGeneralInputs.MotionProhibited) != 0;
            I_ClampRightClose = (pi & LoadPortGeneralInputs.ClampRightClose) != 0;
            I_ClampLeftClose = (pi & LoadPortGeneralInputs.ClampLeftClose) != 0;
            I_ClampRightOpen = (pi & LoadPortGeneralInputs.ClampRightOpen) != 0;
            I_ClampLeftOpen = (pi & LoadPortGeneralInputs.ClampLeftOpen) != 0;
            I_CarrierPresenceMiddle = (pi & LoadPortGeneralInputs.CarrierPresenceMiddle) != 0;
            I_CarrierPresenceLeft = (pi & LoadPortGeneralInputs.CarrierPresenceLeft) != 0;
            I_CarrierPresenceRight = (pi & LoadPortGeneralInputs.CarrierPresenceRight) != 0;
            I_AccessSwitch = (pi & LoadPortGeneralInputs.AccessSwitch) != 0;
            I_ProtrusionDetection = (pi & LoadPortGeneralInputs.ProtrusionDetection) != 0;
            I_InfoPadA = (pi & LoadPortGeneralInputs.InfoPadA) != 0;
            I_InfoPadB = (pi & LoadPortGeneralInputs.InfoPadB) != 0;
            I_InfoPadC = (pi & LoadPortGeneralInputs.InfoPadC) != 0;
            I_InfoPadD = (pi & LoadPortGeneralInputs.InfoPadD) != 0;
            I_PositionForReadingId = (pi & LoadPortGeneralInputs.PositionForReadingId) != 0;

            var po = (LoadPortGeneralOutputs)Int64.Parse(statuses[1], NumberStyles.AllowHexSpecifier);

            O_PreparationCompleted_SigNotConnected =
                (po & LoadPortGeneralOutputs.PreparationCompleted_SigNotConnected) != 0;
            O_TemporarilyStop_SigNotConnected = (po & LoadPortGeneralOutputs.TemporarilyStop_SigNotConnected) != 0;
            O_SignificantError_SigNotConnected = (po & LoadPortGeneralOutputs.SignificantError_SigNotConnected) != 0;
            O_LightError_SigNotConnected = (po & LoadPortGeneralOutputs.LightError_SigNotConnected) != 0;
            O_LaserStop = (po & LoadPortGeneralOutputs.LaserStop) != 0;
            O_InterlockCancel = (po & LoadPortGeneralOutputs.InterlockCancel) != 0;
            O_CarrierClampCloseRight = (po & LoadPortGeneralOutputs.CarrierClampCloseRight) != 0;
            O_CarrierClampOpenRight = (po & LoadPortGeneralOutputs.CarrierClampOpenRight) != 0;
            O_CarrierClampCloseLeft = (po & LoadPortGeneralOutputs.CarrierClampCloseLeft) != 0;
            O_CarrierClampOpenLeft = (po & LoadPortGeneralOutputs.CarrierClampOpenLeft) != 0;
            O_GreenIndicator = (po & LoadPortGeneralOutputs.GreenIndicator) != 0;
            O_RedIndicator = (po & LoadPortGeneralOutputs.RedIndicator) != 0;
            O_LoadIndicator = (po & LoadPortGeneralOutputs.LoadIndicator) != 0;
            O_UnloadIndicator = (po & LoadPortGeneralOutputs.UnloadIndicator) != 0;
            O_AccessSwitchIndicator = (po & LoadPortGeneralOutputs.AccessSwitchIndicator) != 0;
            O_CarrierOpen_SigNotConnected = (po & LoadPortGeneralOutputs.CarrierOpen_SigNotConnected) != 0;
            O_CarrierClamp_SigNotConnected = (po & LoadPortGeneralOutputs.CarrierClamp_SigNotConnected) != 0;
            O_PodPresenceSensorOn_SigNotConnected =
                (po & LoadPortGeneralOutputs.PodPresenceSensorOn_SigNotConnected) != 0;
            O_PreparationCompleted2_SigNotConnected =
                (po & LoadPortGeneralOutputs.PreparationCompleted2_SigNotConnected) != 0;
            O_CarrierProperPlaced_SigNotConnected =
                (po & LoadPortGeneralOutputs.CarrierProperPlaced_SigNotConnected) != 0;
        }

        #endregion Constructors

        #region Properties

        public bool I_SubstrateDetection { get; internal set; }

        public bool I_MotionProhibited { get; internal set; }

        public bool I_ClampRightClose { get; internal set; }

        public bool I_ClampLeftClose { get; internal set; }

        public bool I_ClampRightOpen { get; internal set; }

        public bool I_ClampLeftOpen { get; internal set; }

        public bool I_CarrierPresenceMiddle { get; internal set; }

        public bool I_CarrierPresenceLeft { get; internal set; }

        public bool I_CarrierPresenceRight { get; internal set; }

        public bool I_AccessSwitch { get; internal set; }

        public bool I_ProtrusionDetection { get; internal set; }

        public bool I_InfoPadA { get; internal set; }

        public bool I_InfoPadB { get; internal set; }

        public bool I_InfoPadC { get; internal set; }

        public bool I_InfoPadD { get; internal set; }

        public bool I_PositionForReadingId { get; internal set; }

        public bool O_PreparationCompleted_SigNotConnected { get; internal set; }

        public bool O_TemporarilyStop_SigNotConnected { get; internal set; }

        public bool O_SignificantError_SigNotConnected { get; internal set; }

        public bool O_LightError_SigNotConnected { get; internal set; }

        public bool O_LaserStop { get; internal set; }

        public bool O_InterlockCancel { get; internal set; }

        public bool O_CarrierClampCloseRight { get; internal set; }

        public bool O_CarrierClampOpenRight { get; internal set; }

        public bool O_CarrierClampCloseLeft { get; internal set; }

        public bool O_CarrierClampOpenLeft { get; internal set; }

        public bool O_GreenIndicator { get; internal set; }

        public bool O_RedIndicator { get; internal set; }

        public bool O_LoadIndicator { get; internal set; }

        public bool O_UnloadIndicator { get; internal set; }

        public bool O_AccessSwitchIndicator { get; internal set; }

        public bool O_CarrierOpen_SigNotConnected { get; internal set; }

        public bool O_CarrierClamp_SigNotConnected { get; internal set; }

        public bool O_PodPresenceSensorOn_SigNotConnected { get; internal set; }

        public bool O_PreparationCompleted2_SigNotConnected { get; internal set; }

        public bool O_CarrierProperPlaced_SigNotConnected { get; internal set; }

        #endregion Properties
    }
}
