using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.LightTower
{
    public partial class LightTower
    {
        #region Properties

        private ILightTowerIos IoModule { get; set; }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.LightTower.LightTower)}/Resources";

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    var ioModule = this.GetTopDeviceContainer()
                        .AllDevices()
                        .FirstOrDefault(d => d is ILightTowerIos);
                    if (ioModule is not ILightTowerIos lightTowerIos)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(ILightTowerIos)} is not found in equipment model tree.");
                    }

                    IoModule = lightTowerIos;
                    if (ExecutionMode == ExecutionMode.Simulated)
                    {
                        SetUpSimulatedMode();
                    }

                    IoModule.StatusValueChanged += IoModule_StatusValueChanged;
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            IoModule.StartCommunication();
        }

        protected override void InternalStopCommunication()
        {
            IoModule.StopCommunication();
        }

        #endregion ICommunicatingDevice Commands

        #region ILightTower Commands

        protected override void InternalSetDateAndTime()
        {
            try
            {
                IoModule.SetDateAndTime();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion ILightTower Commands

        #region Event Handlers

        private void IoModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                // Green Light
                case nameof(ILightTowerIos.O_SignalTower_BlinkingGreen):
                case nameof(ILightTowerIos.O_SignalTower_LightningGreen):
                    if (IoModule.O_SignalTower_BlinkingGreen)
                    {
                        GreenLight = LightState.Flashing;
                    }
                    else if (IoModule.O_SignalTower_LightningGreen)
                    {
                        GreenLight = LightState.On;
                    }
                    else
                    {
                        GreenLight = LightState.Off;
                    }

                    break;

                // Orange Light
                case nameof(ILightTowerIos.O_SignalTower_BlinkingYellow):
                case nameof(ILightTowerIos.O_SignalTower_LightningYellow):
                    if (IoModule.O_SignalTower_BlinkingYellow)
                    {
                        OrangeLight = LightState.Flashing;
                    }
                    else if (IoModule.O_SignalTower_LightningYellow)
                    {
                        OrangeLight = LightState.On;
                    }
                    else
                    {
                        OrangeLight = LightState.Off;
                    }

                    break;

                // Blue Light
                case nameof(ILightTowerIos.O_SignalTower_BlinkingBlue):
                case nameof(ILightTowerIos.O_SignalTower_LightningBlue):
                    if (IoModule.O_SignalTower_BlinkingBlue)
                    {
                        BlueLight = LightState.Flashing;
                    }
                    else if (IoModule.O_SignalTower_LightningBlue)
                    {
                        BlueLight = LightState.On;
                    }
                    else
                    {
                        BlueLight = LightState.Off;
                    }

                    break;

                // Red Light
                case nameof(ILightTowerIos.O_SignalTower_BlinkingRed):
                case nameof(ILightTowerIos.O_SignalTower_LightningRed):
                    if (IoModule.O_SignalTower_BlinkingRed)
                    {
                        RedLight = LightState.Flashing;
                    }
                    else if (IoModule.O_SignalTower_LightningRed)
                    {
                        RedLight = LightState.On;
                    }
                    else
                    {
                        RedLight = LightState.Off;
                    }

                    break;

                // Buzzer
                case nameof(ILightTowerIos.O_SignalTower_Buzzer1):
                case nameof(ILightTowerIos.O_SignalTower_Buzzer2):
                    if (IoModule.O_SignalTower_Buzzer1)
                    {
                        BuzzerState = BuzzerState.Slow;
                    }
                    else if (IoModule.O_SignalTower_Buzzer2)
                    {
                        BuzzerState = BuzzerState.Fast;
                    }
                    else
                    {
                        BuzzerState = BuzzerState.Off;
                    }

                    break;
                case nameof(ILightTowerIos.IsCommunicating):
                    IsCommunicating = IoModule.IsCommunicating;
                    break;
                case nameof(ILightTowerIos.IsCommunicationStarted):
                    IsCommunicationStarted = IoModule.IsCommunicationStarted;
                    break;
            }

            if (State is OperatingModes.Maintenance or OperatingModes.Idle)
            {
                SetState(
                    !IsCommunicating || IoModule.State == OperatingModes.Maintenance
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
        }

        #endregion Event Handlers

        #region Other Methods

        protected override void SetLightColor(LightColors color, LightState mode)
        {
            IoModule.SetLightColor(color, mode);
        }

        protected override void SetBuzzerState(BuzzerState state)
        {
            IoModule.SetBuzzerState(state);
        }

        #endregion Other Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IoModule != null)
                {
                    IoModule.StatusValueChanged -= IoModule_StatusValueChanged;
                    IoModule = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
