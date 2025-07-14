using System;
using System.Globalization;

namespace UnitySC.EFEM.Controller.HostInterface.Statuses
{
    public class SystemStatus
    {
        #region Enum

        [Flags]
        public enum InputSignal
        {
            IsRemoteMode = 1 << 0,
            IsFfuInAlarmState = 1 << 1,
            VacuumIsNormal = 1 << 2,
            AirStateIsNormal = 1 << 3,
            IonizerAirStateIsNormal = 1 << 5,
            IonizerAlarmActive = 1 << 6,
            IsDoorClosed = 1 << 7,
            LightCurtainBeamIsObstructed = 1 << 8,
            Interlocked = 1 << 9
        }

        #endregion Enum

        #region Constructors

        public SystemStatus(
            bool isRemoteMode,
            bool isFfuInAlarmState,
            bool vacuumIsNormal,
            bool airStateIsNormal,
            bool ionizerAirStateIsNormal,
            bool ionizerAlarmActive,
            bool isDoorClosed,
            bool lightCurtainBeamIsObstructed,
            bool interlocked)
        {
            IsRemoteMode                 = isRemoteMode;
            IsFfuInAlarmState            = isFfuInAlarmState;
            VacuumIsNormal               = vacuumIsNormal;
            AirStateIsNormal             = airStateIsNormal;
            IonizerAirStateIsNormal      = ionizerAirStateIsNormal;
            IonizerAlarmActive           = ionizerAlarmActive;
            IsDoorClosed                 = isDoorClosed;
            LightCurtainBeamIsObstructed = lightCurtainBeamIsObstructed;
            Interlocked                  = interlocked;
        }

        public SystemStatus(string status)
        {
            var statusData = status.Replace(":", string.Empty);

            if (statusData.Length != 4)
                throw new InvalidOperationException(
                    $"Error while parsing general status: \"{statusData}\" has not the expected length.");

            UpdateFromStringStatus(statusData);
        }

        #endregion Constructors

        #region Properties

        public bool IsRemoteMode { get; internal set; }

        public bool IsFfuInAlarmState { get; internal set; }

        public bool VacuumIsNormal { get; internal set; }

        public bool AirStateIsNormal { get; internal set; }

        public bool IonizerAirStateIsNormal { get; internal set; }

        public bool IonizerAlarmActive { get; internal set; }

        public bool IsDoorClosed { get; internal set; }

        public bool LightCurtainBeamIsObstructed { get; internal set; }

        public bool Interlocked { get; internal set; }

        #endregion Properties

        #region Overrides

        public override string ToString()
        {
            return GetSignalAsInt().ToString("X4");
        }

        #endregion

        #region Private Methods

        private int GetSignalAsInt()
        {
            int signalAsInt = 0;

            signalAsInt += IsRemoteMode ? (int)InputSignal.IsRemoteMode : 0;
            signalAsInt += IsFfuInAlarmState ? (int)InputSignal.IsFfuInAlarmState : 0;
            signalAsInt += VacuumIsNormal ? (int)InputSignal.VacuumIsNormal : 0;
            signalAsInt += AirStateIsNormal ? (int)InputSignal.AirStateIsNormal : 0;
            signalAsInt += IonizerAirStateIsNormal ? (int)InputSignal.IonizerAirStateIsNormal : 0;
            signalAsInt += IonizerAlarmActive ? (int)InputSignal.IonizerAlarmActive : 0;
            signalAsInt += IsDoorClosed ? (int)InputSignal.IsDoorClosed : 0;
            signalAsInt += LightCurtainBeamIsObstructed ? (int)InputSignal.LightCurtainBeamIsObstructed : 0;
            signalAsInt += Interlocked ? (int)InputSignal.Interlocked : 0;

            return signalAsInt;
        }

        private void UpdateFromStringStatus(string status)
        {
            var inputFlags = (InputSignal)uint.Parse(status, NumberStyles.AllowHexSpecifier);

            IsRemoteMode = (inputFlags & InputSignal.IsRemoteMode) != 0;
            IsFfuInAlarmState = (inputFlags & InputSignal.IsFfuInAlarmState) != 0;
            VacuumIsNormal = (inputFlags & InputSignal.VacuumIsNormal) != 0;
            AirStateIsNormal = (inputFlags & InputSignal.AirStateIsNormal) != 0;
            IonizerAirStateIsNormal = (inputFlags & InputSignal.IonizerAirStateIsNormal) != 0;
            IonizerAlarmActive = (inputFlags & InputSignal.IonizerAlarmActive) != 0;
            IsDoorClosed = (inputFlags & InputSignal.IsDoorClosed) != 0;
            LightCurtainBeamIsObstructed = (inputFlags & InputSignal.LightCurtainBeamIsObstructed) != 0;
            Interlocked = (inputFlags & InputSignal.Interlocked) != 0;
        }

        #endregion
    }
}
