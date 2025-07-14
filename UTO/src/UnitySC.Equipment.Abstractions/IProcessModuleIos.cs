using System;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions
{
    public interface IProcessModuleIos
    {
        #region Properties

        bool I_PM1_DoorOpened { get; }

        bool I_PM2_DoorOpened { get; }

        bool I_PM3_DoorOpened { get; }

        bool I_PM1_ReadyToLoadUnload { get; }

        bool I_PM2_ReadyToLoadUnload { get; }

        bool I_PM3_ReadyToLoadUnload { get; }

        bool IsCommunicationStarted { get; }

        bool IsCommunicating { get; }

        OperatingModes State { get; }

        #endregion

        #region Methods

        void Interrupt(InterruptionKind kind);

        void StartCommunication();

        void StopCommunication();

        #endregion

        #region Events

        event EventHandler<StatusChangedEventArgs> StatusValueChanged;

        #endregion
    }
}
