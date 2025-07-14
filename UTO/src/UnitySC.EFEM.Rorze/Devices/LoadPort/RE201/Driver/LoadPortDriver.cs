using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Helpers;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.EventArgs;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands;

using GpioCommand = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands.GpioCommand;
using GposCommand = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands.GposCommand;
using InitializeStatusCommand = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands.InitializeStatusCommand;
using MappingEventArgs = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.EventArgs.MappingEventArgs;
using OriginSearchCommand = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands.OriginSearchCommand;
using StatusAcquisitionCommand = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands.StatusAcquisitionCommand;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver
{
    /// <summary>
    ///     Class responsible to communicate with the RORZE Loadport model RE201.
    /// </summary>
    internal class LoadPortDriver : DriverBase
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadPortDriver" /> class.
        /// </summary>
        /// <param name="logger">The logger to use to trace any information</param>
        /// <param name="connectionMode">Indicates which connection mode the driver must have (client or server).</param>
        /// <param name="port">Port's number of the device.</param>
        public LoadPortDriver(
            ILogger logger,
            ConnectionMode connectionMode,
            byte port = 1,
            double aliveBitPeriod = 1000)
            : base(logger, nameof(Equipment.Abstractions.Devices.LoadPort.LoadPort), connectionMode, port, RorzeConstants.DeviceTypeAbb.LoadPort, aliveBitPeriod)
        {
            _commandsSubscriber = AddReplySubscriber(SubscriberType.SenderAndListener);
            _stateChangedSubscriber = AddReplySubscriber(SubscriberType.ListenForEverything);
            _gpioStatusChangedSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _carrierTypeChangedSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _gposStatusChangedSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);

            _previousCassettePresence = _currentCassettePresence = CassettePresence.Unknown;
        }

        #endregion Constructors

        public Func<Tuple<uint, uint>> GetCarrierIdPageInterval { get; set; }

        public Func<Tuple<int, int>> GetCarrierIdSubstringIndexes { get; set; }

        #region IDisposable

        /// <summary>
        ///     Performs the actual cleanup actions on managed/unmanaged resources.
        /// </summary>
        /// <param name="disposing">When <see Langword="true" />, managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveReplySubscriber(_commandsSubscriber);
                RemoveReplySubscriber(_carrierTypeChangedSubscriber);
                RemoveReplySubscriber(_stateChangedSubscriber);
                RemoveReplySubscriber(_gpioStatusChangedSubscriber);
                RemoveReplySubscriber(_gposStatusChangedSubscriber);
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable

        #region Fields

        private readonly IMacroCommandSubscriber _commandsSubscriber;
        private readonly IMacroCommandSubscriber _stateChangedSubscriber;
        private readonly IMacroCommandSubscriber _gpioStatusChangedSubscriber;
        private readonly IMacroCommandSubscriber _carrierTypeChangedSubscriber;
        private readonly IMacroCommandSubscriber _gposStatusChangedSubscriber;

        private CassettePresence _previousCassettePresence;
        private CassettePresence _currentCassettePresence;

        #endregion Fields

        #region Commands to Hardware

        /// <summary>
        ///     Sets the device in a safe known state and makes it ready for production.
        /// </summary>
        /// <remarks>To be used at software's start-up, after an abort command, ...</remarks>
        /// <remarks>Send INIT Rorze message</remarks>
        public override void Initialization()
        {
            var initCmd = InitializeStatusCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                Sender,
                this);

            var originSearchCmd = OriginSearchCommand.NewOrder(
                Port,
                Sender,
                this);

            var setCarrierTypeCmd = SetCarrierTypeCommand.NewOrder(
                Port,
                Sender,
                this,
                CarrierType.Auto);

            // Create the Macro Command
            var macroCommand = BuildInitMacroCommand((int)EFEMEvents.InitCompleted);

            macroCommand.AddMacroCommand(initCmd);
            macroCommand.AddMacroCommand(originSearchCmd);
            macroCommand.AddMacroCommand(setCarrierTypeCmd);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        ///     Enable loadport events and reset error.
        /// </summary>
        public void InitializeCommunication()
        {
            var macroCommand = BuildInitMacroCommand((int)EFEMEvents.InitializeCommunicationCompleted);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        ///     Gets the load port statuses.
        /// </summary>
        public void GetStatuses()
        {
            // Create commands
            var statCmd = StatusAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.LoadPort, Port, Sender, this);
            var gpioCmd = GpioCommand.NewOrder(RorzeConstants.DeviceTypeAbb.LoadPort, Port, Sender, this);
            var gwidCmd = GetCarrierTypeCommand.NewOrder(Port, Sender, this, GetCarrierTypeParameter.GetRealCarrierTypeOnly);
            var gposCmd = GposCommand.NewOrder(RorzeConstants.DeviceTypeAbb.LoadPort, Port, Sender, this);

            // Create the Macro Command
            var macroCmd = new BaseMacroCommand(this, (int)EFEMEvents.GetStatusesCompleted);
            macroCmd.AddMacroCommand(statCmd);
            macroCmd.AddMacroCommand(gpioCmd);
            macroCmd.AddMacroCommand(gwidCmd);
            macroCmd.AddMacroCommand(gposCmd);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCmd);
        }

        /// <summary>
        ///     Performs signal output to the designated load port.
        /// </summary>
        /// <param name="lpIndicator">Designate a target signal.</param>
        /// <param name="mode">Designate signal status to output.</param>
        /// <remarks>Send SIGOUT Rorze message</remarks>
        public void SetSignalOutput(LoadPortIndicators lpIndicator, LightState mode)
        {
            // Check that LightState should be changed
            if (mode == LightState.Undetermined) { return; }

            // Create command
            var setLightCommand = SetLightCommand.NewOrder(Port, Sender, this, lpIndicator, mode);

            // Send the command
            _commandsSubscriber.AddMacro(setLightCommand);
        }

        public void SetDateAndTime()
        {
            // Create the command
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                Sender,
                this,
                true);

            // Send the command
            _commandsSubscriber.AddMacro(setTimeCmd);
        }

        /// <summary>
        ///     Close LoadPort door and make the carrier ready to be carried out.
        /// </summary>
        public void ReleaseCarrier()
        {
            // Create command
            RorzeCommand releaseCarrierCommand = ReleaseCarrierCommand.NewOrder(
                Port,
                ReleaseCarrierOperationMode.ClosesTheCarrier,
                ReleaseCarrierCloseOperation.ClosesTheCarrierToUnclamp,
                Sender,
                this);

            // Send the command
            _commandsSubscriber.AddMacro(releaseCarrierCommand);
        }

        /// <summary>
        ///     Lock a carrier on the load port.
        /// </summary>
        public void Clamp()
        {
            // Create commands
            var secureCarrierCommand = SecureCarrierCommand.NewOrder(
                Port,
                SecureCarrierOperationParameter.ClampsTheCarrier,
                Sender,
                this);

            // Send the command
            _commandsSubscriber.AddMacro(secureCarrierCommand);
        }

        /// <summary>
        ///     Unlock a carrier on the load port.
        /// </summary>
        public void Unclamp()
        {
            // Create command
            RorzeCommand releaseCarrierCommand = ReleaseCarrierCommand.NewOrder(
                Port,
                ReleaseCarrierOperationMode.UnclampsTheCarrier,
                ReleaseCarrierCloseOperation.NotSet,
                Sender,
                this);

            // Send the command
            _commandsSubscriber.AddMacro(releaseCarrierCommand);
        }

        /// <summary>
        ///     Open a carrier on the load port.
        /// </summary>
        public void Open()
        {
            // Create commands
            var secureCarrierCommand = SecureCarrierCommand.NewOrder(
                Port,
                SecureCarrierOperationParameter.OpensTheCarrier,
                Sender,
                this);

            // Create Macro Command
            var macroCommand = new BaseMacroCommand(this);
            macroCommand.AddMacroCommand(secureCarrierCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        ///     Close a carrier on the load port.
        /// </summary>
        public void Close()
        {
            // Create command
            var closeLp = ReleaseCarrierCommand.NewOrder(
                Port,
                ReleaseCarrierOperationMode.ClosesTheCarrier,
                ReleaseCarrierCloseOperation.ClosesTheCarrierNotToUnclamp,
                Sender,
                this);

            // Send the command
            _commandsSubscriber.AddMacro(closeLp);
        }

        /// <summary>
        ///     Perform mapping of the carrier.
        /// </summary>
        /// <remarks>
        ///     Carrier requires to be opened first.
        /// </remarks>
        public void Map()
        {
            // Create command
            var performWaferMappingCommand = PerformWaferMappingCommand.NewOrder(Port, Sender, this);

            // Send the command
            _commandsSubscriber.AddMacro(performWaferMappingCommand);
        }

        /// <summary>
        ///     Go to selected slot
        /// </summary>
        public void GoToSlot(uint carrierTypeIndex, byte slot)
        {
            // Create command
            RorzeCommand homeCommand = HomeCommand.NewOrder(
                Port,
                Sender,
                this,
                carrierTypeIndex + 1,
                slot);

            // Send the command
            _commandsSubscriber.AddMacro(homeCommand);
        }

        /// <summary>
        ///     Gets last mapping results.
        /// </summary>
        public void GetLastMapping()
        {
            // Create command
            var mappingPatternAcquisitionCommand = MappingPatternAcquisitionCommand.NewOrder(Port, Sender, this);

            // Send the command
            _commandsSubscriber.AddMacro(mappingPatternAcquisitionCommand);
        }

        /// <summary>
        ///     Reads identifier of the carrier present on Load Port.
        /// </summary>
        public void ReadCarrierId()
        {
            // Create command
            var carrierIdPageInterval = GetCarrierIdPageInterval?.Invoke();
            var readCarrierIdCommand = ReadCarrierIdCommand.NewOrder(Port, Sender, this, Logger, carrierIdPageInterval);

            // Send the command
            _commandsSubscriber.AddMacro(readCarrierIdCommand);
        }

        public void GetDataPerCarrierType(CarrierType carrierType, uint? carrierTypeIndex)
        {
            // Create commands
            var carrierTypeDataIndex = CarrierTypeConverter.ToCarrierTypeDataIndex(carrierType, carrierTypeIndex);
            var getDataCmd = new GetDataSubCommand(
                RorzeConstants.CommandTypeAbb.Order,
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                Constants.DevicePartIds.DataByCarrierType,
                Constants.DeviceParts,
                Sender,
                this,
                carrierTypeDataIndex.ToString());

            // Send the command.
            _commandsSubscriber.AddMacro(getDataCmd);
        }

        /// <summary>
        ///     Get data for the given carrier type and property. Returned values would come in a raised event when received.
        /// </summary>
        public void GetDataPerCarrierType(
            CarrierType carrierType,
            uint? carrierTypeIndex,
            CarrierDataProperty carrierProperty)
        {
            // Create commands
            var carrierTypeDataIndex = CarrierTypeConverter.ToCarrierTypeDataIndex(carrierType, carrierTypeIndex);
            var getDataCmd = new GetDataSubCommand(
                RorzeConstants.CommandTypeAbb.Order,
                RorzeConstants.DeviceTypeAbb.LoadPort, Port,
                Constants.DevicePartIds.DataByCarrierType,
                Constants.DeviceParts,
                Sender,
                this,
                carrierTypeDataIndex.ToString(),
                ((int)carrierProperty).ToString());

            // Send the command.
            _commandsSubscriber.AddMacro(getDataCmd);
        }

        /// <summary>
        /// Get data for the given carrier index. Returned values would come in a raised event when received.
        /// </summary>
        public void GetDataPerCarrierType(
            uint? carrierIndex,
            CarrierDataProperty carrierProperty)
        {
            // Create commands
            var getDataCmd = new GetDataSubCommand(
                RorzeConstants.CommandTypeAbb.Order,
                RorzeConstants.DeviceTypeAbb.LoadPort, Port,
                Constants.DevicePartIds.DataByCarrierType,
                Constants.DeviceParts,
                Sender,
                this,
                carrierIndex.ToString(),
                ((int)carrierProperty).ToString());

            // Send the command.
            _commandsSubscriber.AddMacro(getDataCmd);
        }

        /// <summary>
        ///     Get data for the system data specified. Returned values would come in a raised event when received.
        /// </summary>
        /// <param name="paramIndex"></param>
        public void GetSystemDataConfig(SystemDataProperty paramIndex)
        {
            // Create command
            var getDataCmd = new GetDataSubCommand(
                RorzeConstants.CommandTypeAbb.Order,
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                Constants.DevicePartIds.SystemData,
                Constants.DeviceParts,
                Sender,
                this,
                ((int)paramIndex).ToString());

            // Send the command.
            _commandsSubscriber.AddMacro(getDataCmd);
        }

        /// <summary>
        ///     Set data for the system data specified.
        /// </summary>
        /// <param name="paramIndex"></param>
        /// <param name="value"></param>
        public void SetSystemDataConfig(SystemDataProperty paramIndex, int value)
        {
            // Create command
            var cmd = SetDataSubCommand.NewIndividualSettingCommand(
                RorzeConstants.DeviceTypeAbb.LoadPort, Port,
                Constants.DevicePartIds.SystemData,
                Constants.DeviceParts,
                Sender,
                this,
                true,
                value.ToString(),
                (uint)paramIndex);

            // Send the command.
            _commandsSubscriber.AddMacro(cmd);
        }

        public void SetCarrierType(string carrierType)
        {
            var setCarrierTypeCommand =
                SetCarrierTypeCommand.NewOrder(Port, Sender, this, carrierType);

            _commandsSubscriber.AddMacro(setCarrierTypeCommand);
        }

        public void GetVersion()
        {
            // Create commands
            var acquisitionCommand = VersionAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.LoadPort, Port, Sender, this);

            // Send the command.
            _commandsSubscriber.AddMacro(acquisitionCommand);
        }
        #endregion

        #region Events

        /// <summary>
        ///     Occurs when status received from Load Port.
        /// </summary>
        public event EventHandler<StatusEventArgs<RE201LoadPortStatus>> StatusReceived;

        /// <summary>
        ///     Sends the <see cref="StatusReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{LoadPortStatus}" /> to be attached with the event.</param>
        protected virtual void OnStatusReceived(StatusEventArgs<RE201LoadPortStatus> args)
        {
            try
            {
                StatusReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Occurs when GPIO status received from Load Port.
        /// </summary>
        public event EventHandler<StatusEventArgs<RE201GpioStatus>> GpioReceived;

        /// <summary>
        ///     Sends the <see cref="GpioReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{RE201GpioStatus}" /> to be attached with the event.</param>
        protected virtual void OnGpioReceived(StatusEventArgs<RE201GpioStatus> args)
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

        /// <summary>
        ///     Occurs when GPOS status received from Load Port.
        /// </summary>
        public event EventHandler<StatusEventArgs<RE201GposStatus>> GposReceived;

        /// <summary>
        ///     Sends the <see cref="GposReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{RE201GposStatus}" /> to be attached with the event.</param>
        protected virtual void OnGposReceived(StatusEventArgs<RE201GposStatus> args)
        {
            try
            {
                GposReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Occurs when carrier identification methods received from Load Port.
        /// </summary>
        public event EventHandler<StatusEventArgs<CarrierTypeStatus>> CarrierIdentificationMethodReceived;

        /// <summary>
        ///     Sends the <see cref="CarrierIdentificationMethodReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{CarrierTypeStatus}" /> to be attached with the event.</param>
        protected virtual void OnCarrierIdentificationMethodReceived(StatusEventArgs<CarrierTypeStatus> args)
        {
            try
            {
                CarrierIdentificationMethodReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Occurs when carrier type received from Load Port.
        /// </summary>
        public event EventHandler<StatusEventArgs<CarrierTypeStatus>> CarrierTypeReceived;

        /// <summary>
        ///     Sends the <see cref="CarrierTypeReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{CarrierTypeStatus}" /> to be attached with the event.</param>
        protected virtual void OnCarrierTypeReceived(StatusEventArgs<CarrierTypeStatus> args)
        {
            try
            {
                CarrierTypeReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Occurs when carrier info pad input statuses received from Load Port.
        /// </summary>
        public event EventHandler<StatusEventArgs<InfoPadsInputStatus>> InfoPadsInputReceived;

        /// <summary>
        ///     Sends the <see cref="InfoPadsInputReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{InfoPadsInputReceived}" /> to be attached with the event.</param>
        protected virtual void OnInfoPadsInputReceived(StatusEventArgs<InfoPadsInputStatus> args)
        {
            try
            {
                InfoPadsInputReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Occurs when device data is received.
        /// </summary>
        public event EventHandler<GetDeviceDataEventArgs> DeviceDataReceived;

        /// <summary>
        ///     Sends the <see cref="DeviceDataReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="GetDeviceDataEventArgs" /> to be attached with the event.</param>
        protected virtual void OnDeviceDataReceived(GetDeviceDataEventArgs args)
        {
            try
            {
                DeviceDataReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Notifies that a carrier has been placed/removed.
        ///     NO EVENTS Are sent by RORZE. I/O values are stored within <see cref="LoadPortStatus" />
        /// </summary>
        public event EventHandler<CarrierPresenceEventArgs> CarrierPresenceChanged;

        /// <summary>
        ///     Sends the <see cref="CarrierPresenceChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="CarrierPresenceEventArgs" /> to be attached with the event.</param>
        protected virtual void OnCarrierPresenceChanged(CarrierPresenceEventArgs args)
        {
            try
            {
                CarrierPresenceChanged?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Notifies that the mapping of the carrier present on LoadPort is completed.
        /// </summary>
        /// <remarks>
        ///     This event may be "command-initiated" (sent at completion of map command),
        ///     or "self-initiated" (in case mapping is automatically performed when opening the carrier).
        /// </remarks>
        public event EventHandler<MappingEventArgs> CarrierMapped;

        /// <summary>
        ///     Sends the <see cref="CarrierMapped" /> event.
        /// </summary>
        /// <param name="args">The <see cref="MappingEventArgs" /> to be attached with the event.</param>
        protected virtual void OnCarrierMapped(MappingEventArgs args)
        {
            try
            {
                CarrierMapped?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        ///     Occurs when carrier identifier is read or received.
        /// </summary>
        public event EventHandler<CarrierIdReceivedEventArgs> CarrierIdReceived;

        /// <summary>
        ///     Sends the <see cref="CarrierIdReceived" /> event.
        /// </summary>
        /// <param name="args">The <see cref="CarrierIdReceivedEventArgs" /> to be attached with the event.</param>
        protected virtual void OnCarrierIDReceived(CarrierIdReceivedEventArgs args)
        {
            try
            {
                CarrierIdReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>Occurs when firmware version received.</summary>
        public event EventHandler<VersionAcquisitionEventArgs> VersionReceived;

        /// <summary>Sends the <see cref="VersionReceived" /> event.</summary>
        /// <param name="args">
        /// The <see cref="VersionAcquisitionEventArgs" /> to be attached with the event.
        /// </param>
        protected virtual void OnVersionReceived(VersionAcquisitionEventArgs args)
        {
            try
            {
                VersionReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
        #endregion

        #region Overrides

        /// <summary>
        ///     Software equivalent to an EMO switch.
        ///     Immediately stops any movement on the device. Leaves the device in an unknown state.
        /// </summary>
        public override void EmergencyStop()
        {
            ClearCommandsQueue();
        }

        /// <summary>
        ///     Called when a command is completed by the hardware.
        /// </summary>
        /// <param name="evtId">Identifies the command that ended.</param>
        /// <param name="evtResults">
        ///     Contains the results of the command. To be cast in the appropriate type if command results are
        ///     expected.
        /// </param>
        protected override void CommandEndedCallback(int evtId, System.EventArgs evtResults)
        {
            if (!Enum.IsDefined(typeof(EFEMEvents), evtId))
            {
                Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                return;
            }

            switch ((EFEMEvents)evtId)
            {
                case EFEMEvents.InitCompleted:
                    OnCommandDone(new CommandEventArgs(LoadPortCommands.Initialization));
                    break;

                case EFEMEvents.InitializeCommunicationCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(InitializeCommunication)));
                    break;

                case EFEMEvents.GetStatusesCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetStatuses)));
                    break;

                case EFEMEvents.SecureCarrierCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SecureCarrierCommand)));
                    break;

                case EFEMEvents.ReleaseCarrierCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(ReleaseCarrierCommand)));
                    break;

                case EFEMEvents.GetLastMappingCompleted:
                    OnCarrierMapped(evtResults as MappingEventArgs);
                    OnCommandDone(new CommandEventArgs(nameof(MappingPatternAcquisitionCommand)));
                    break;

                case EFEMEvents.GetCarrierTypeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetCarrierTypeCommand)));
                    break;

                case EFEMEvents.SetCarrierTypeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetCarrierTypeCommand)));
                    break;

                case EFEMEvents.SetLightCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetLightCommand)));
                    break;

                case EFEMEvents.PerformWaferMappingCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(PerformWaferMappingCommand)));
                    break;

                case EFEMEvents.GoToHomeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(HomeCommand)));
                    break;

                case EFEMEvents.GetDeviceDataCompleted:
                    if (evtResults is not GetDeviceDataEventArgs deviceDataEventArgs)
                    {
                        break;
                    }

                    OnDeviceDataReceived(deviceDataEventArgs);
                    OnCommandDone(new CommandEventArgs(nameof(GetDataSubCommand)));
                    break;

                case EFEMEvents.SetDeviceDataCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetDataSubCommand)));
                    break;

                case EFEMEvents.SetDateTimeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetDateAndTimeCommand)));
                    break;

                case EFEMEvents.ReadCarrierIdCompleted:
                    if (evtResults is not CarrierIdReceivedEventArgs carrierIdEvtArgs)
                    {
                        Logger.Error($"{nameof(EFEMEvents.ReadCarrierIdCompleted)} returned invalid event args.");
                        break;
                    }

                    OnCarrierIDReceived(carrierIdEvtArgs);
                    OnCommandDone(new CommandEventArgs(
                        nameof(ReadCarrierIdCommand),
                        carrierIdEvtArgs.IsSucceed ? CommandStatusCode.Ok : CommandStatusCode.Error));
                    break;

                case EFEMEvents.GetVersionCompleted:
                    if (evtResults is not VersionAcquisitionEventArgs version)
                    {
                        break;
                    }

                    OnVersionReceived(version);
                    OnCommandDone(new CommandEventArgs(nameof(VersionAcquisitionCommand)));
                    break;

                case EFEMEvents.StatusReceived:
                    OnStatusReceived(evtResults as StatusEventArgs<RE201LoadPortStatus>);
                    break;

                case EFEMEvents.GpioEventReceived:
                    var gpioEvtArgs = evtResults as StatusEventArgs<RE201GpioStatus>;
                    OnGpioReceived(gpioEvtArgs);
                    if (gpioEvtArgs != null)
                    {
                        var status = gpioEvtArgs.Status;

                        UpdateAndCheckCassettePresence(
                            status.I_CarrierPresenceLeft
                            || status.I_CarrierPresenceMiddle
                            || status.I_CarrierPresenceRight,
                            status.I_CarrierPresenceLeft
                            && status.I_CarrierPresenceMiddle
                            && status.I_CarrierPresenceRight);
                    }

                    break;

                case EFEMEvents.GposEventReceived:
                    OnGposReceived(evtResults as StatusEventArgs<RE201GposStatus>);
                    break;

                case EFEMEvents.CarrierIdentificationMethodReceived:
                    if (evtResults is not StatusEventArgs<CarrierTypeStatus> identificationMethod)
                    {
                        break;
                    }

                    OnCarrierIdentificationMethodReceived(identificationMethod);

                    break;

                case EFEMEvents.CarrierTypeReceived:
                    if (evtResults is not StatusEventArgs<CarrierTypeStatus> carrierType)
                    {
                        break;
                    }

                    OnCarrierTypeReceived(carrierType);

                    break;

                case EFEMEvents.InfoPadInputReceived:

                    if (evtResults is not StatusEventArgs<InfoPadsInputStatus> infoPadsStatus)
                    {
                        break;
                    }

                    OnInfoPadsInputReceived(infoPadsStatus);

                    break;

                // Not managed by this driver
                default:
                    Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                    break;
            }
        }

        /// <summary>
        ///     Enable Aligner listeners
        /// </summary>
        protected override void EnableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabling", Name));

            var statusCmd =
                StatusAcquisitionCommand.NewEvent(RorzeConstants.DeviceTypeAbb.LoadPort, Port, Sender, this);
            _stateChangedSubscriber.AddMacro(statusCmd);

            var gpioCmd = GpioCommand.NewEvent(RorzeConstants.DeviceTypeAbb.LoadPort, Port, Sender, this);
            _gpioStatusChangedSubscriber.AddMacro(gpioCmd);

            var carrierTypeEvt = GetCarrierTypeCommand.NewEvent(Port, Sender, this);
            _carrierTypeChangedSubscriber.AddMacro(carrierTypeEvt);

            var gposCmd = GposCommand.NewEvent(RorzeConstants.DeviceTypeAbb.LoadPort, Port, Sender, this);
            _gposStatusChangedSubscriber.AddMacro(gposCmd);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabled", Name));
        }

        /// <summary>
        ///     Flush Listeners
        /// </summary>
        protected override void DisableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabling", Name));

            DiscardOpenTransactions(_carrierTypeChangedSubscriber);
            DiscardOpenTransactions(_gpioStatusChangedSubscriber);
            DiscardOpenTransactions(_stateChangedSubscriber);
            DiscardOpenTransactions(_gposStatusChangedSubscriber);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabled", Name));
        }

        /// <summary>
        ///     Flush the queue holding commands to be sent to the device.
        /// </summary>
        /// <remarks>In case a command is in progress when this method is called, the command's completion will NOT be notified.</remarks>
        public override void ClearCommandsQueue()
        {
            base.ClearCommandsQueue();
            DiscardOpenTransactions(_commandsSubscriber);
            OnCommandInterrupted();
        }
        protected override void AliveBitRequest()
        {
            GetVersion();
        }
        #endregion

        #region Helper

        private BaseMacroCommand BuildInitMacroCommand(int eventToFacade)
        {
            // Create each individual command
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                Sender,
                this,
                false);

            var disableEventCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                EventTargetParameter.AllEvents,
                EventEnableParameter.Disable,
                Sender,
                this);

            var enableStatusCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                EventTargetParameter.StatusEvent,
                EventEnableParameter.Enable,
                Sender,
                this);

            var enableGpioCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                EventTargetParameter.PioEvent,
                EventEnableParameter.Enable,
                Sender,
                this);

            var enableGposCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                EventTargetParameter.StoppingPositionEvent,
                EventEnableParameter.Enable,
                Sender,
                this);

            var enableCarrierTypeCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.LoadPort,
                Port,
                EventTargetParameter.CarrierTypeEvent,
                EventEnableParameter.Enable,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, eventToFacade);
            macroCommand.AddMacroCommand(setTimeCmd);
            macroCommand.AddMacroCommand(disableEventCmd);
            macroCommand.AddMacroCommand(enableStatusCmd);
            macroCommand.AddMacroCommand(enableGpioCmd);
            macroCommand.AddMacroCommand(enableGposCmd);
            macroCommand.AddMacroCommand(enableCarrierTypeCmd);

            return macroCommand;
        }

        /// <summary>
        ///     Updates the cassette presence and calls OnCassettePresenceChanged if it changed.
        /// </summary>
        /// <param name="isPresent">Represents the current cassette presence signal.</param>
        /// <param name="isCorrectlyPlaced">Represent the current cassette placement signal.</param>
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "Useful for readability.")]
        private void UpdateAndCheckCassettePresence(bool isPresent, bool isCorrectlyPlaced)
        {
            _previousCassettePresence = _currentCassettePresence;

            if (!isPresent && !isCorrectlyPlaced)
            {
                _currentCassettePresence = CassettePresence.Absent;
            }
            else if (!isPresent && isCorrectlyPlaced)
            {
                _currentCassettePresence = CassettePresence.NoPresentPlacement;
            }
            else if (isPresent && !isCorrectlyPlaced)
            {
                _currentCassettePresence = CassettePresence.PresentNoPlacement;
            }
            else if (isPresent && isCorrectlyPlaced)
            {
                _currentCassettePresence = CassettePresence.Correctly;
            }

            if (_previousCassettePresence != _currentCassettePresence)
            {
                OnCarrierPresenceChanged(new CarrierPresenceEventArgs(_currentCassettePresence));
            }
        }

        #endregion
    }
}
