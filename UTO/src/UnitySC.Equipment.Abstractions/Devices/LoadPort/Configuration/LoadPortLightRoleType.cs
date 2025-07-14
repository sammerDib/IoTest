namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration
{
    /// <summary>
    /// Light access role
    /// </summary>
    public enum LoadPortLightRoleType
    {
        /// <summary>
        /// Role which shows is Load Port ready to load or not.
        /// </summary>
        LoadReady,

        /// <summary>
        /// Role which shows is Load Port ready to unload or not.
        /// </summary>
        UnloadReady,

        /// <summary>
        /// Role which shows whether Manual Access Mode is active on the Load Port
        /// </summary>
        AccessModeManual,

        /// <summary>
        /// Role which shows whether Auto Access Mode is active on the Load Port
        /// </summary>
        AccessModeAuto,

        /// <summary>
        /// Role which shows whether Reservation is active on the Load Port
        /// </summary>
        Reserve,

        /// <summary>
        /// Role which shows whether an alarm is active on the Load Port
        /// </summary>
        Alarm,

        /// <summary>
        /// Role which highlights Hand-Off button of the Load Port.
        /// </summary>
        HandOffButton,

        /// <summary>
        /// undetermined state
        /// </summary>
        Undetermined,
    }
}
