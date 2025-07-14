using System;
using System.Threading;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.MessageDataBus;
using Agileo.SemiDefinitions;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Configuration;
using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.EK9000
{
    public partial class EK9000
        : IConfigurableDevice<EK9000Configuration>,
            IProcessModuleIos,
            IFfuIos,
            ILightTowerIos,
            IReaderPositionerIos
    {
        #region Fields

        private const string _safetyDoorOpenAlarmKey = "IoModule_011";
        private readonly Semaphore _setPositionCmdSemaphore = new(0, 1);

        #endregion

        #region Properties

        private EK9000Driver Driver { get; set; }

        #endregion

        #region Configuration

        public IConfigManager ConfigManager { get; protected set; }

        public EK9000Configuration Configuration
            => ConfigManager.Current.Cast<EK9000Configuration>();

        public EK9000Configuration CreateDefaultConfiguration()
        {
            return new EK9000Configuration();
        }

        public string RelativeConfigurationDir
            => $"./Devices/{nameof(IoModule)}/{nameof(EK9000)}/Resources";

        public void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        /// <inheritdoc />
        public void SetExecutionMode(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }

        #endregion Configuration

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
                    LoadConfiguration();
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        Driver = new EK9000Driver(Configuration.ModbusConfiguration);
                        Driver.ConnectionStateChanged += Driver_ConnectionStateChanged;
                        Driver.CommunicationStarted += Driver_CommunicationStarted;
                        Driver.CommunicationStopped += Driver_CommunicationStopped;
                        Driver.TagValueChanged += Driver_TagValueChanged;
                    }
                    else
                    {
                        SetUpSimulatedMode();
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region Commands

        protected override void InternalStartCommunication()
        {
            Driver.EnableCommunications();
        }

        protected override void InternalStopCommunication()
        {
            Driver.Disconnect();
        }

        protected virtual void InternalSetDigitalOutput(DigitalOutputs output, bool value)
        {
            Driver.SetOutputSignal(output.ToString(), value);
        }

        protected virtual void InternalSetAnalogOutput(AnalogOutputs output, double value)
        {
            switch (output)
            {
                case AnalogOutputs.O_FFU_Speed:
                    //Max speed = 2250 rpm for 10V
                    //Output step = 327.68 µV/step
                    Driver.SetOutputSignal(
                        output.ToString(),
                        (ushort)(value * 0.0044 * 1000000 / 327.68));
                    break;
                default:
                    Driver.SetOutputSignal(output.ToString(), (ushort)value);
                    break;
            }
        }

        protected virtual void InternalSetFfuSpeed(RotationalSpeed setPoint)
        {
            InternalSetAnalogOutput(AnalogOutputs.O_FFU_Speed, setPoint.Value);
        }

        protected virtual void InternalSetDateAndTime()
        {
            //Do nothing in case of EK9000
        }

        protected virtual void InternalSetLightColor(LightColors color, LightState mode)
        {
            switch (color)
            {
                case LightColors.Red:
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningRed,
                        mode == LightState.On);
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingRed,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow);
                    break;
                case LightColors.Blue:
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningBlue,
                        mode == LightState.On);
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingBlue,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow);
                    break;
                case LightColors.Yellow:
                case LightColors.Orange:
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningYellow,
                        mode == LightState.On);
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingYellow,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow);
                    break;
                case LightColors.Green:
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_LightningGreen,
                        mode == LightState.On);
                    InternalSetDigitalOutput(
                        DigitalOutputs.O_SignalTower_BlinkingGreen,
                        mode == LightState.Flashing
                        || mode == LightState.FlashingFast
                        || mode == LightState.FlashingSlow);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        protected virtual void InternalSetBuzzerState(BuzzerState state)
        {
            InternalSetDigitalOutput(
                DigitalOutputs.O_SignalTower_Buzzer1,
                state == BuzzerState.Fast || state == BuzzerState.On);
            InternalSetDigitalOutput(
                DigitalOutputs.O_SignalTower_Buzzer2,
                state == BuzzerState.Slow);
        }

        protected virtual void InternalSetReaderPosition(SampleDimension dimension)
        {
            try
            {
                if (dimension == SampleDimension.S200mm)
                {
                    InternalSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve1, false);
                    InternalSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve2, true);
                }
                else
                {
                    InternalSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve1, true);
                    InternalSetDigitalOutput(DigitalOutputs.O_OCRWaferReaderValve2, true);
                }

                Thread.Sleep(20);
                InternalSetDigitalOutput(DigitalOutputs.O_OCRWaferTableDrive, true);
                Thread.Sleep(20);
                InternalSetDigitalOutput(DigitalOutputs.O_OCRWaferTableDrive, false);
                var success = _setPositionCmdSemaphore.WaitOne(10000);
                if (!success)
                {
                    throw new TimeoutException($"Timeout occurred : {Name} acknowledge signal not received during {nameof(SetReaderPosition)} command.");
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region Event handler

        private void Driver_ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            IsCommunicating = e.Connected;
            SetState(
                IsCommunicating
                    ? OperatingModes.Idle
                    : OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStopped(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
        }

        private void Driver_CommunicationStarted(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
        }

        private void Driver_TagValueChanged(object sender, DrivenTagValueChangedEventArgs e)
        {
            switch (e.TagName)
            {
                case nameof(DigitalInputs.I_EMO_Status):
                    I_EMO_Status = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_FFU_Alarm):
                    I_FFU_Alarm = (bool)e.Value;
                    Alarm = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_VacuumPressureSensor):
                    I_VacuumPressureSensor = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_CDA_PressureSensor):
                    I_CDA_PressureSensor = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_ServiceLightLed):
                    I_ServiceLightLed = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_AirFlowPressureSensorIonizer):
                    I_AirFlowPressureSensorIonizer = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_Ionizer1Status):
                    I_Ionizer1Status = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_RV201Interlock):
                    I_RV201Interlock = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_MaintenanceSwitch):
                    I_MaintenanceSwitch = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_RobotDriverPower):
                    I_RobotDriverPower = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_EFEM_DoorStatus):
                    I_EFEM_DoorStatus = (bool)e.Value;
                    if (I_EFEM_DoorStatus)
                    {
                        SetAlarm(_safetyDoorOpenAlarmKey);
                    }

                    break;
                case nameof(DigitalInputs.I_TPMode):
                    I_TPMode = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_OCRTableAlarm):
                    I_OCRTableAlarm = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_OCRTablePositionReach):
                    I_OCRTablePositionReach = (bool)e.Value;
                    if (I_OCRTablePositionReach)
                    {
                        _setPositionCmdSemaphore.Release();
                    }
                    break;
                case nameof(DigitalInputs.I_OCRWaferReaderLimitSensor1):
                    I_OCRWaferReaderLimitSensor1 = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_OCRWaferReaderLimitSensor2):
                    I_OCRWaferReaderLimitSensor2 = (bool)e.Value;
                    break;
                case nameof(AnalogInputs.I_DifferentialAirPressureSensor)
                    : // 0-10 Pa of water column => 1-10V => 1000 to 10000
                    var voltValue =
                        327.68
                        * (ushort)e.Value
                        / 1000000; //Convert value in volt : Resolution 327.68 µV/step
                    I_DifferentialAirPressureSensor = Pressure.FromPascals((voltValue * 0.9) + 1)
                        .ToUnit(PressureUnit.Millipascal);
                    MeanPressure = I_DifferentialAirPressureSensor;
                    break;
                case nameof(DigitalInputs.I_PM1_DoorOpened):
                    I_PM1_DoorOpened = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_PM1_ReadyToLoadUnload):
                    I_PM1_ReadyToLoadUnload = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_PM2_DoorOpened):
                    I_PM2_DoorOpened = (bool)e.Value;
                    break;
                case nameof(DigitalInputs.I_PM2_ReadyToLoadUnload):
                    I_PM2_ReadyToLoadUnload = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_LightningRed):
                    O_SignalTower_LightningRed = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_LightningYellow):
                    O_SignalTower_LightningYellow = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_LightningGreen):
                    O_SignalTower_LightningGreen = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_LightningBlue):
                    O_SignalTower_LightningBlue = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_BlinkingRed):
                    O_SignalTower_BlinkingRed = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_BlinkingYellow):
                    O_SignalTower_BlinkingYellow = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_BlinkingGreen):
                    O_SignalTower_BlinkingGreen = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_BlinkingBlue):
                    O_SignalTower_BlinkingBlue = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_Buzzer1):
                    O_SignalTower_Buzzer1 = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_SignalTower_Buzzer2):
                    O_SignalTower_Buzzer2 = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_OCRWaferReaderValve1):
                    O_OCRWaferReaderValve1 = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_OCRWaferReaderValve2):
                    O_OCRWaferReaderValve2 = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_OCRWaferTableDrive):
                    O_OCRTableDrive = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_OCRWaferTableReset):
                    O_OCRTableReset = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_OCRWaferTableInitialization):
                    O_OCRTableInitialization = (bool)e.Value;
                    break;
                case nameof(AnalogOutputs.O_FFU_Speed):
                    O_FFU_Speed =
                        RotationalSpeed.FromRevolutionsPerMinute(
                            (ushort)e.Value * 327.68 / 1000000 / 0.0044);
                    FanSpeed = O_FFU_Speed;
                    break;
                case nameof(DigitalOutputs.O_RobotArmNotExtended_PM1):
                    O_RobotArmNotExtended_PM1 = (bool)e.Value;
                    break;
                case nameof(DigitalOutputs.O_RobotArmNotExtended_PM2):
                    O_RobotArmNotExtended_PM2 = (bool)e.Value;
                    break;
            }
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (Driver != null)
            {
                Driver.ConnectionStateChanged -= Driver_ConnectionStateChanged;
                Driver.CommunicationStarted -= Driver_CommunicationStarted;
                Driver.CommunicationStopped -= Driver_CommunicationStopped;
                Driver.TagValueChanged -= Driver_TagValueChanged;
                Driver.Dispose();
            }

            if (SimulationData != null)
            {
                DisposeSimulatedMode();
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
