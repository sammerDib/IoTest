using System;
using System.Reflection;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver
{
    public class Dio0Driver : GenericRC5xxDriver
    {
        #region Fields

        private readonly IMacroCommandSubscriber _gpioStatusesSubscriber;
        private readonly IMacroCommandSubscriber _fansRotationSpeedChangedSubscriber;
        private readonly IMacroCommandSubscriber _pressureSensorsValueSubscriber;
        private readonly IMacroCommandSubscriber _dio0SignalDataSubscriber;

        private readonly bool _isPressureSensorAvailable;

        #endregion Fields

        #region Constructors

        public Dio0Driver(
            ILogger logger,
            ConnectionMode connectionMode,
            double aliveBitPeriod,
            bool isPressureSensorAvailable)
            : base(logger, connectionMode, 0, aliveBitPeriod)
        {
            _gpioStatusesSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _fansRotationSpeedChangedSubscriber =
                AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _pressureSensorsValueSubscriber =
                AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _dio0SignalDataSubscriber =
                AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            _isPressureSensorAvailable = isPressureSensorAvailable;
        }

        #endregion Constructors

        #region Commands to Hardware

        /// <summary>Starts rotation of the fan or changes the rotational speed.</summary>
        /// <param name="speedRpm">Designates the rotational speed.</param>
        /// <param name="fan">
        /// When 0: Starts rotations of all connected fan. When 1 to 20: Designates the fan to operate.
        /// </param>
        /// <param name="isGroup">
        /// When <see langword="true" /> <paramref name="fan" /> designates the group of fans to operate.
        /// </param>
        public void StartFanRotation(uint speedRpm, byte fan = 0, bool isGroup = false)
        {
            // Create the command
            var startFanCmd = StartFanRotationCommand.NewOrder(
                Port,
                fan,
                isGroup,
                speedRpm,
                Sender,
                this);

            // Send the command
            CommandsSubscriber.AddMacro(startFanCmd);
        }

        /// <summary>Stops the rotation of the operating fan.</summary>
        /// <param name="fan">
        /// When 0: Stops rotations of all connected fan. When 1 to 20: Designates the fan to stop.
        /// </param>
        /// <param name="isGroup">
        /// When <see langword="true" /> <paramref name="fan" /> designates the group of fans to stop.
        /// </param>
        public void StopFanRotation(byte fan = 0, bool isGroup = false)
        {
            // Create the command
            var stopFanCmd = StopFanRotationCommand.NewOrder(Port, fan, isGroup, Sender, this);

            // Send the command
            CommandsSubscriber.AddMacro(stopFanCmd);
        }

        public override void GetStatuses()
        {
            // Create commands
            var statCmd = StatusAcquisitionCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.IO,
                Port,
                Sender,
                this);
            var gpioCmd = GpioCommand.NewOrder(RorzeConstants.DeviceTypeAbb.IO, Port, Sender, this);
            var gdioCmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.RC550_HCL3_ID0,
                1);
            var grevCmd = GetRotationalFansSpeedCommand.NewOrder(Port, Sender, this);
            var gdioE84Lp1ICmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.SB078_Port4_HCL0_ID0,
                1);
            var gdioE84Lp1OCmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.SB078_Port4_HCL0_ID1,
                1);
            var gdioE84Lp2ICmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.SB078_Port4_HCL0_ID2,
                1);
            var gdioE84Lp2OCmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.SB078_Port4_HCL0_ID3,
                1);
            var gdioLp1Cmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.RC550_HCL1_ID0,
                1);
            var gdioLp2Cmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.RC550_HCL1_ID1,
                1);
            var gdioLp3Cmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.RC550_HCL1_ID2,
                1);
            var gdioLp4Cmd = GetDio0SignalCommand.NewOrder(
                Port,
                Sender,
                this,
                IoModuleIds.RC550_HCL1_ID3,
                1);

            // Create the Macro Command
            var macroCmd = new BaseMacroCommand(this, (int)EFEMEvents.GetStatusesCompleted);
            macroCmd.AddMacroCommand(statCmd);
            macroCmd.AddMacroCommand(gpioCmd);
            macroCmd.AddMacroCommand(gdioCmd);
            macroCmd.AddMacroCommand(grevCmd);
            if (_isPressureSensorAvailable)
            {
                var gprsCmd = GetPressureFromSensorsCommand.NewOrder(Port, Sender, this);
                macroCmd.AddMacroCommand(gprsCmd);
            }

            macroCmd.AddMacroCommand(gdioE84Lp1ICmd);
            macroCmd.AddMacroCommand(gdioE84Lp1OCmd);
            macroCmd.AddMacroCommand(gdioE84Lp2ICmd);
            macroCmd.AddMacroCommand(gdioE84Lp2OCmd);
            macroCmd.AddMacroCommand(gdioLp1Cmd);
            macroCmd.AddMacroCommand(gdioLp2Cmd);
            macroCmd.AddMacroCommand(gdioLp3Cmd);
            macroCmd.AddMacroCommand(gdioLp4Cmd);

            // Send the command.
            CommandsSubscriber.AddMacro(macroCmd);
        }

        #endregion Commands to Hardware

        #region Overrides

        /// <summary>Called when a command is completed by the hardware.</summary>
        /// <param name="evtId">Identifies the command that ended.</param>
        /// <param name="evtResults">
        /// Contains the results of the command. To be cast in the appropriate type if command results are
        /// expected.
        /// </param>
        protected override void CommandEndedCallback(int evtId, EventArgs evtResults)
        {
            if (!Enum.IsDefined(typeof(EFEMEvents), evtId))
            {
                base.CommandEndedCallback(evtId, evtResults);
                return;
            }

            switch ((EFEMEvents)evtId)
            {
                // Command completion
                case EFEMEvents.SetDateTimeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetDateAndTimeCommand)));
                    break;

                case EFEMEvents.FanRotationSpeedStarted:
                    OnCommandDone(new CommandEventArgs(nameof(StartFanRotationCommand)));
                    break;

                case EFEMEvents.FanRotationSpeedStopped:
                    OnCommandDone(new CommandEventArgs(nameof(StopFanRotationCommand)));
                    break;

                // Received events
                case EFEMEvents.GpioEventReceived:
                    if (evtResults is not StatusEventArgs<RC550GeneralIoStatus> gpioStatus)
                    {
                        Logger.Warning(
                            $"{Name}. {MethodBase.GetCurrentMethod()?.Name} - {evtId} unexpected event arg type received: {evtResults}");
                        return;
                    }

                    OnGpioReceived(gpioStatus);
                    return;

                case EFEMEvents.FansRotationSpeedReceived:
                    if (evtResults is not StatusEventArgs<FansRotationSpeed> fansRotationEvtArgs)
                    {
                        Logger.Warning(
                            $"{Name}. {MethodBase.GetCurrentMethod()?.Name} - {evtId} unexpected event arg type received: {evtResults}");
                        return;
                    }

                    OnFansRotationSpeedChanged(fansRotationEvtArgs);
                    return;

                case EFEMEvents.PressureSensorsValuesReceived:
                    if (evtResults is not StatusEventArgs<PressureSensorsValues>
                        pressureSensorValues)
                    {
                        Logger.Warning(
                            $"{Name}. {MethodBase.GetCurrentMethod()?.Name} - {evtId} unexpected event arg type received: {evtResults}");
                        return;
                    }

                    OnPressureSensorsValueChanged(pressureSensorValues);
                    return;

                case EFEMEvents.ExpansionIOSignalReceived:
                    if (evtResults is not StatusEventArgs<SignalData> ioStatusEvtArgs)
                    {
                        Logger.Warning(
                            $"{Name}. {MethodBase.GetCurrentMethod()?.Name} - {evtId} unexpected event arg type received: {evtResults}");
                        return;
                    }

                    OnDio0SignalDataReceived(ioStatusEvtArgs);
                    return;

                // Not managed by this driver
                default:
                    base.CommandEndedCallback(evtId, evtResults);
                    break;
            }
        }

        /// <summary>Enable Aligner listeners</summary>
        protected override void EnableListeners()
        {
            base.EnableListeners();

            var gpioStatusEvt = GetGeneralInputOutputCommand.NewEvent(Port, Sender, this);
            _gpioStatusesSubscriber.AddMacro(gpioStatusEvt);

            var fansRotationSpeedChangedEvent =
                GetRotationalFansSpeedCommand.NewEvent(Port, Sender, this);
            _fansRotationSpeedChangedSubscriber.AddMacro(fansRotationSpeedChangedEvent);

            if (_isPressureSensorAvailable)
            {
                var pressureSensorsEvent =
                    GetPressureFromSensorsCommand.NewEvent(Port, Sender, this);
                _pressureSensorsValueSubscriber.AddMacro(pressureSensorsEvent);
            }

            var ioSignalEvent = GetDio0SignalCommand.NewEvent(Port, Sender, this);
            _dio0SignalDataSubscriber.AddMacro(ioSignalEvent);
        }

        /// <summary>Flush Listeners</summary>
        protected override void DisableListeners()
        {
            base.DisableListeners();

            DiscardOpenTransactions(_gpioStatusesSubscriber);
            DiscardOpenTransactions(_fansRotationSpeedChangedSubscriber);
            DiscardOpenTransactions(_pressureSensorsValueSubscriber);
            DiscardOpenTransactions(_dio0SignalDataSubscriber);
        }

        /// <inheritdoc cref="AddSpecificInitializationCommands" />
        protected override void AddSpecificInitializationCommands(
            BaseMacroCommand genericInitializationMacroCommand)
        {
            var enableEvents = RC550EventCommand.NewOrder(
                Port,
                RC550EventTargetParameter.AllEvents,
                EventEnableParameter.Enable,
                Sender,
                this);
            genericInitializationMacroCommand.AddMacroCommand(enableEvents);
        }

        #endregion Overrides

        #region Events

        /// <summary>Occurs when GREV status received from RC550.</summary>
        public event EventHandler<StatusEventArgs<FansRotationSpeed>> FansRotationSpeedChanged;

        /// <summary>Sends the <see cref="FansRotationSpeedChanged" /> event.</summary>
        /// <param name="args">
        /// The <see cref="StatusEventArgs{FansRotationSpeed}" /> to be attached with the event.
        /// </param>
        protected virtual void OnFansRotationSpeedChanged(StatusEventArgs<FansRotationSpeed> args)
        {
            try
            {
                FansRotationSpeedChanged?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>Occurs when GREV status received from RC550.</summary>
        public event EventHandler<StatusEventArgs<PressureSensorsValues>>
            PressureSensorsValueChanged;

        /// <summary>Sends the <see cref="PressureSensorsValueChanged" /> event.</summary>
        /// <param name="args">
        /// The <see cref="StatusEventArgs{PressureSensorsValues}" /> to be attached with the event.
        /// </param>
        protected virtual void OnPressureSensorsValueChanged(
            StatusEventArgs<PressureSensorsValues> args)
        {
            try
            {
                PressureSensorsValueChanged?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>Occurs when GPIO status received from RC550.</summary>
        public event EventHandler<StatusEventArgs<RC550GeneralIoStatus>> GpioReceived;

        /// <summary>Sends the <see cref="GpioReceived" /> event.</summary>
        /// <param name="args">
        /// The <see cref="StatusEventArgs{RC550GeneralIoStatus}" /> to be attached with the event.
        /// </param>
        protected virtual void OnGpioReceived(StatusEventArgs<RC550GeneralIoStatus> args)
        {
            try
            {
                GpioReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>Occurs when GDIO status received from DIO0.</summary>
        public event EventHandler<StatusEventArgs<SignalData>> Dio0SignalDataReceived;

        /// <summary>Sends the <see cref="Dio0SignalDataReceived" /> event.</summary>
        /// <param name="args">
        /// The <see cref="StatusEventArgs{SignalData}" /> to be attached with the event.
        /// </param>
        protected virtual void OnDio0SignalDataReceived(StatusEventArgs<SignalData> args)
        {
            try
            {
                Dio0SignalDataReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion Events

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveReplySubscriber(_gpioStatusesSubscriber);
                RemoveReplySubscriber(_fansRotationSpeedChangedSubscriber);
                RemoveReplySubscriber(_pressureSensorsValueSubscriber);
                RemoveReplySubscriber(_dio0SignalDataSubscriber);
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
