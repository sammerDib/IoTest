using System;

using Agileo.EquipmentModeling;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions
{
    public interface IFfuIos
    {
        #region Properties

        bool Alarm { get; }

        Pressure MeanPressure { get; }

        RotationalSpeed FanSpeed { get; }

        bool IsCommunicationStarted { get; }

        bool IsCommunicating { get; }

        OperatingModes State { get; }

        #endregion

        #region Methods

        void StartCommunication();

        void StopCommunication();

        void SetFfuSpeed(RotationalSpeed setPoint);

        void SetDateAndTime();

        #endregion

        #region Events

        event EventHandler<StatusChangedEventArgs> StatusValueChanged;

        #endregion
    }
}
