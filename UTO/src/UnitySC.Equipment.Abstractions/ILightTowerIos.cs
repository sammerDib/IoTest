using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.Equipment.Abstractions
{
    public interface ILightTowerIos
    {
        #region Properties

        bool O_SignalTower_BlinkingGreen { get; }

        bool O_SignalTower_LightningGreen { get; }

        bool O_SignalTower_BlinkingYellow { get; }

        bool O_SignalTower_LightningYellow { get; }

        bool O_SignalTower_BlinkingBlue { get; }

        bool O_SignalTower_LightningBlue { get; }

        bool O_SignalTower_BlinkingRed { get; }

        bool O_SignalTower_LightningRed { get; }

        bool O_SignalTower_Buzzer1 { get; }

        bool O_SignalTower_Buzzer2 { get; }

        bool IsCommunicationStarted { get; }

        bool IsCommunicating { get; }

        OperatingModes State { get; }

        void SetLightColor(LightColors color, LightState mode);

        void SetBuzzerState(BuzzerState state);

        #endregion

        #region Methods

        void StartCommunication();

        void StopCommunication();

        void SetDateAndTime();

        #endregion

        #region Events

        event EventHandler<StatusChangedEventArgs> StatusValueChanged;

        #endregion
    }
}
