using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.Common.Configuration;
using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Helpers;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Configuration;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.EventArgs;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

using CarrierType = UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums.CarrierType;
using MappingEventArgs =
    UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.EventArgs.MappingEventArgs;
using OperationMode = UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201
{
    public partial class RE201 : IConfigurableDevice<RorzeLoadPortConfiguration>, IVersionedDevice
    {
        #region Fields

        private const int NbSpeedLevels = 10;

        #endregion

        #region Public Methods

        public override bool NeedsInitAfterE84Error()
        {
            //E84 is not handled on SMIF load port
            return false;
        }

        #endregion

        #region IRE201 Commands

        protected override void InternalGoToSlot(byte slot)
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.GoToSlot(CarrierTypeIndex, slot); },
                    RE201Command.GoToSlot);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (Driver != null)
            {
                Driver.StatusReceived -= Driver_StatusReceived;
                Driver.GpioReceived -= Driver_GpioReceived;
                Driver.CarrierIdentificationMethodReceived -=
                    Driver_CarrierIdentificationMethodReceived;
                Driver.CarrierTypeReceived -= Driver_CarrierTypeReceived;
                Driver.InfoPadsInputReceived -= Driver_InfoPadsInputReceived;
                Driver.DeviceDataReceived -= Driver_DeviceDataReceived;
                Driver.CommunicationEstablished -= Driver_CommunicationEstablished;
                Driver.CommunicationClosed -= Driver_CommunicationClosed;
                Driver.CommunicationStarted -= Driver_CommunicationStarted;
                Driver.CommunicationStopped -= Driver_CommunicationStopped;
                Driver.CarrierMapped -= Driver_CarrierMapped;
                Driver.CarrierPresenceChanged -= Driver_CarrierPresenceChanged;
                Driver.GposReceived -= Driver_GposReceived;
                Driver.CarrierIdReceived -= Driver_CarrierIdReceived;
                Driver.VersionReceived -= Driver_VersionReceived;
                CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                Driver = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            base.InternalInitialize(mustForceInit);
            CarrierTypeNumber = 0;
            CarrierTypeName = UnknownCarrierName;
            CarrierTypeDescription = UnknownCarrierName;
            if (State == OperatingModes.Idle && !mustForceInit)
            {
                Logger.Info("No need to initialize the device because State is already Idle");
                return;
            }

            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.Initialization(); },
                    RE201Command.Initialization);
                ConfigureCarrierIdAcquisition();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGetStatuses()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.GetStatuses(); },
                    RE201Command.GetStatuses);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region Properties

        private LoadPortDriver Driver { get; set; }
        private DriverWrapper DriverWrapper { get; set; }

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
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        Driver = new LoadPortDriver(
                            Logger,
                            Configuration.CommunicationConfig.ConnectionMode,
                            (byte)InstanceId,
                            Configuration.CommunicationConfig.AliveBitPeriod);
                        Driver.Setup(
                            Configuration.CommunicationConfig.IpAddress,
                            Configuration.CommunicationConfig.TcpPort,
                            Configuration.CommunicationConfig.AnswerTimeout,
                            Configuration.CommunicationConfig.ConfirmationTimeout,
                            Configuration.CommunicationConfig.InitTimeout,
                            maxNbRetry: Configuration.CommunicationConfig.MaxNbRetry,
                            connectionRetryDelay: Configuration.CommunicationConfig
                                .ConnectionRetryDelay);
                        Driver.StatusReceived += Driver_StatusReceived;
                        Driver.GpioReceived += Driver_GpioReceived;
                        Driver.CarrierIdentificationMethodReceived +=
                            Driver_CarrierIdentificationMethodReceived;
                        Driver.CarrierTypeReceived += Driver_CarrierTypeReceived;
                        Driver.InfoPadsInputReceived += Driver_InfoPadsInputReceived;
                        Driver.DeviceDataReceived += Driver_DeviceDataReceived;
                        Driver.CommunicationEstablished += Driver_CommunicationEstablished;
                        Driver.CommunicationClosed += Driver_CommunicationClosed;
                        Driver.CommunicationStarted += Driver_CommunicationStarted;
                        Driver.CommunicationStopped += Driver_CommunicationStopped;
                        Driver.CarrierMapped += Driver_CarrierMapped;
                        Driver.CarrierPresenceChanged += Driver_CarrierPresenceChanged;
                        Driver.GposReceived += Driver_GposReceived;
                        Driver.CarrierIdReceived += Driver_CarrierIdReceived;
                        Driver.VersionReceived += Driver_VersionReceived;
                        CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
                        Driver.GetCarrierIdPageInterval = () =>
                        {
                            if (Configuration == null
                                || Configuration.UseDefaultPageIntervalForReading
                                || Configuration.CarrierIdentificationConfig.CarrierIdAcquisition
                                == CarrierIDAcquisitionType.CodeReader)
                            {
                                return null;
                            }

                            return new Tuple<uint, uint>(
                                Configuration.CarrierIdStartPage,
                                Configuration.CarrierIdStopPage);
                        };
                        Driver.GetCarrierIdSubstringIndexes = () =>
                        {
                            if (Configuration == null)
                            {
                                return null;
                            }

                            return new Tuple<int, int>(
                                Configuration.CarrierIdentificationConfig.CarrierIdStartIndex,
                                Configuration.CarrierIdentificationConfig.CarrierIdStopIndex);
                        };
                        DriverWrapper = new DriverWrapper(Driver, Logger);
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication() => Driver.EnableCommunications();

        protected override void InternalStopCommunication() => Driver.Disconnect();

        #endregion

        #region ILoadPort Commands

        protected override void InternalClamp()
        {
            try
            {
                DriverWrapper.RunCommand(delegate { Driver.Clamp(); }, RE201Command.Clamp);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalUnclamp()
        {
            try
            {
                DriverWrapper.RunCommand(delegate { Driver.Unclamp(); }, RE201Command.Unclamp);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalDock()
        {
            try
            {
                //Because dock capability does not exist on SMIF load port
                //We are just going to perform the clamp action
                DriverWrapper.RunCommand(delegate { Driver.Clamp(); }, RE201Command.Clamp);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalUndock()
        {
            try
            {
                //Because undock capability does not exist on SMIF load port
                //We are just going to perform the close action
                DriverWrapper.RunCommand(delegate { Driver.Close(); }, RE201Command.Close);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalOpen(bool performMapping)
        {
            try
            {
                if (!performMapping)
                {
                    return;
                }

                DriverWrapper.RunCommand(delegate { Driver.Open(); }, RE201Command.Open);
                DriverWrapper.RunCommand(
                    delegate { Driver.GetLastMapping(); },
                    RE201Command.GetLastMapping);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalClose()
        {
            try
            {
                DriverWrapper.RunCommand(delegate { Driver.Close(); }, RE201Command.Close);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalMap()
        {
            try
            {
                DriverWrapper.RunCommand(delegate { Driver.Map(); }, RE201Command.Map);
                DriverWrapper.RunCommand(
                    delegate { Driver.GetLastMapping(); },
                    RE201Command.GetLastMapping);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void RequestCarrierIdFromBarcodeReader()
        {
            if (Carrier == null || CarrierPresence != CassettePresence.Correctly)
            {
                MarkExecutionAsFailed("No correctly placed carrier detected.");
                return;
            }

            DriverWrapper.RunCommand(
                delegate { Driver.ReadCarrierId(); },
                RE201Command.ReadCarrierId);
        }

        protected override void RequestCarrierIdFromTagReader()
        {
            if (Carrier == null || CarrierPresence != CassettePresence.Correctly)
            {
                MarkExecutionAsFailed("No correctly placed carrier detected.");
                return;
            }

            DriverWrapper.RunCommand(
                delegate { Driver.ReadCarrierId(); },
                RE201Command.ReadCarrierId);
        }

        protected override void InternalReleaseCarrier()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.ReleaseCarrier(); },
                    RE201Command.ReleaseCarrier);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetLight(LoadPortLightRoleType role, LightState lightState)
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.SetSignalOutput(Converters.ToRe201Light(role), lightState); },
                    RE201Command.SetSignalOutput);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetDateAndTime()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.SetDateAndTime(); },
                    RE201Command.SetDateAndTime);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalEnableE84() => throw new NotImplementedException();

        protected override void InternalDisableE84() => throw new NotImplementedException();

        protected override void InternalSetE84Timeouts(int tp1, int tp2, int tp3, int tp4, int tp5)
            => throw new NotImplementedException();

        protected override void InternalManageEsSignal(bool isActive)
            => throw new NotImplementedException();

        protected override void InternalRequestLoad() => throw new NotImplementedException();

        protected override void InternalRequestUnload() => throw new NotImplementedException();

        public override bool IsReadyForTransfer(
            EffectorType effector,
            out List<string> errorMessages,
            Material armMaterial = null,
            byte slot = 1)
        {
            if (slot == byte.MaxValue)
            {
                slot = CurrentSlot;
            }

            return base.IsReadyForTransfer(effector, out errorMessages, armMaterial, slot);
        }

        protected override void InternalSetAccessMode(LoadingType accessMode)
            => throw new InvalidOperationException("E84 not handled on RE201 load port");

        protected override void InternalSetCarrierType(uint carrierType)
        {
            try
            {
                var carrierTypeFromConfig =
                    Configuration.CarrierTypes.First(x => x.Id == carrierType);
                DriverWrapper.RunCommand(
                    delegate { Driver.SetCarrierType(carrierTypeFromConfig.Name); },
                    RE201Command.SetCarrierType);
                CarrierTypeNumber = carrierType;
                CarrierTypeName = carrierTypeFromConfig.Name;
                CarrierTypeDescription = carrierTypeFromConfig.Description;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region Configuration

        public new RorzeLoadPortConfiguration Configuration
            => base.Configuration.Cast<RorzeLoadPortConfiguration>();

        public RorzeLoadPortConfiguration CreateDefaultConfiguration() => new();

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.LoadPort.LoadPort)}/{nameof(RE201)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
            => ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);

        #endregion

        #region Event Handlers

        private void Driver_CarrierPresenceChanged(object sender, CarrierPresenceEventArgs e)
        {
            // Does not update Status in case Load Port is not in service.
            if (e == null || !IsInService)
            {
                Logger.Debug(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - {1} - Event ignored because EventArgs is null or LoadPort is not in service.",
                        Name,
                        MethodBase.GetCurrentMethod()?.Name));
                return;
            }

            Logger.Debug(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Driver's CarrierPresenceChanged event received. Presence={0}.",
                    e.Presence));
            CarrierPresence = e.Presence;
        }

        private void Driver_CarrierMapped(object sender, MappingEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            var carrierTypeFromConfig =
                Configuration.CarrierTypes.FirstOrDefault(x => x.Id == CarrierTypeNumber);
            Carrier.SetSlotMap(
                e.Mapping.Select(Converters.ToAbstractionSlotState).ToList(),
                (byte)InstanceId,
                carrierTypeFromConfig?.MaterialType ?? MaterialType.SiliconWithNotch);
        }

        private void Driver_CarrierIdReceived(object sender, CarrierIdReceivedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (e.IsSucceed)
            {
                ApplyCarrierIdTreatment(e.CarrierId);
            }
            else
            {
                OnCarrierIDChanged(
                    new CarrierIdChangedEventArgs(e.CarrierId, CommandStatusCode.Error));
            }
        }

        private void Driver_StatusReceived(object sender, StatusEventArgs<RE201LoadPortStatus> e)
        {
            Logger.Debug(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Driver's StatusReceived event received. Source={0}, Status={1}.",
                    e.SourceName,
                    e.Status));
            OperationMode = e.Status.OperationMode;
            OriginReturnCompletion = e.Status.OriginReturnCompletion;
            CommandProcessing = e.Status.CommandProcessing;
            OperationStatus = e.Status.OperationStatus;
            if (e.Status.MotionSpeed == 0)
            {
                IsNormalSpeed = true;
                MotionSpeedPercentage = "100%";
            }
            else
            {
                IsNormalSpeed = false;
                MotionSpeedPercentage = $"{100 / NbSpeedLevels * e.Status.MotionSpeed}%";
            }

            ErrorControllerCode = ((int)e.Status.ErrorControllerId).ToString("X2");
            ErrorControllerName = e.Status.ErrorControllerId;
            ErrorCode = ((int)e.Status.ErrorCode).ToString("X2");
            ErrorDescription = e.Status.ErrorCode;
            if (State != OperatingModes.Executing
                || (OperationStatus == OperationStatus.Stop
                    && CommandProcessing == CommandProcessing.Stop))
            {
                UpdateDeviceState();
            }
        }

        private void Driver_GpioReceived(object sender, StatusEventArgs<RE201GpioStatus> e)
        {
            #region General Inputs

            I_SubstrateDetection = e.Status.I_SubstrateDetection;
            I_MotionProhibited = e.Status.I_MotionProhibited;
            I_ClampRightClose = e.Status.I_ClampRightClose;
            I_ClampLeftClose = e.Status.I_ClampLeftClose;
            I_ClampRightOpen = e.Status.I_ClampRightOpen;
            I_ClampLeftOpen = e.Status.I_ClampLeftOpen;
            I_CarrierPresenceMiddle = e.Status.I_CarrierPresenceMiddle;
            I_CarrierPresenceLeft = e.Status.I_CarrierPresenceLeft;
            I_CarrierPresenceRight = e.Status.I_CarrierPresenceRight;
            I_AccessSwitch = e.Status.I_AccessSwitch;
            I_ProtrusionDetection = e.Status.I_ProtrusionDetection;
            I_InfoPadA = e.Status.I_InfoPadA;
            I_InfoPadB = e.Status.I_InfoPadB;
            I_InfoPadC = e.Status.I_InfoPadC;
            I_InfoPadD = e.Status.I_InfoPadD;
            I_PositionForReadingId = e.Status.I_PositionForReadingId;

            #endregion General Inputs

            #region General Outputs

            O_PreparationCompleted = e.Status.O_PreparationCompleted_SigNotConnected;
            O_TemporarilyStop = e.Status.O_TemporarilyStop_SigNotConnected;
            O_SignificantError = e.Status.O_SignificantError_SigNotConnected;
            O_LightError = e.Status.O_LightError_SigNotConnected;
            O_LaserStop = e.Status.O_LaserStop;
            O_InterlockCancel = e.Status.O_InterlockCancel;
            O_CarrierClampCloseRight = e.Status.O_CarrierClampCloseRight;
            O_CarrierClampOpenRight = e.Status.O_CarrierClampOpenRight;
            O_CarrierClampCloseLeft = e.Status.O_CarrierClampCloseLeft;
            O_CarrierClampOpenLeft = e.Status.O_CarrierClampOpenLeft;
            O_GreenIndicator = e.Status.O_GreenIndicator;
            O_RedIndicator = e.Status.O_RedIndicator;
            O_LoadIndicator = e.Status.O_LoadIndicator;
            O_UnloadIndicator = e.Status.O_UnloadIndicator;
            O_AccessSwitchIndicator = e.Status.O_AccessSwitchIndicator;
            O_CarrierOpen = e.Status.O_CarrierOpen_SigNotConnected;
            O_CarrierClamp = e.Status.O_CarrierClamp_SigNotConnected;
            O_PodPresenceSensorOn = e.Status.O_PodPresenceSensorOn_SigNotConnected;
            O_CarrierProperPlaced = e.Status.O_CarrierProperPlaced_SigNotConnected;

            #endregion General Outputs

            // Update abstraction statuses
            IsClamped = O_CarrierClamp;
            AutoModeLightState = LightState.Off; // E84 not handled on RE201 load port
            ManualModeLightState = LightState.On;
            LoadLightState = e.Status.O_LoadIndicator
                ? LightState.On
                : LightState.Off;
            UnloadLightState = e.Status.O_UnloadIndicator
                ? LightState.On
                : LightState.Off;
            ReservedLightState = LightState.Off;
            ErrorLightState = e.Status.O_LightError_SigNotConnected
                ? LightState.On
                : LightState.Off;
            HandOffLightState = LightState.Off;

            // No docking position on this load port
            IsDocked = O_CarrierClamp;
            IsDoorOpen = O_CarrierOpen;
        }

        private void Driver_GposReceived(object sender, StatusEventArgs<RE201GposStatus> e)
        {
            if (e == null)
            {
                return;
            }

            CurrentSlot = (byte)e.Status.ZAxis;
        }

        private void Driver_CarrierIdentificationMethodReceived(
            object sender,
            StatusEventArgs<CarrierTypeStatus> e)
        {
            CarrierDetectionMode = e.Status.CarrierType;
            switch (e.Status.CarrierType)
            {
                case CarrierType.Auto:
                case CarrierType.Cassette:
                    break;
                case CarrierType.NotIdentified:
                    var userMessage = $"Identification method is {e.Status.CarrierType}. "
                                      + "However, the system does not manage such carriers.";
                    OnUserErrorRaised(userMessage);
                    Logger.Error(userMessage);
                    break;
                default:
                    throw new ArgumentException(nameof(e.Status.CarrierType));
            }
        }

        private void Driver_CarrierTypeReceived(object sender, StatusEventArgs<CarrierTypeStatus> e)
        {
            CarrierType = e.Status.CarrierType;
            CarrierTypeIndex = e.Status.CarrierTypeIndex ?? 0;
            CarrierProfileName = e.Status.CarrierProfileName;
            if (!Configuration.IsManualCarrierType)
            {
                var carrierTypeFromConfig =
                    Configuration.CarrierTypes.FirstOrDefault(x => x.Name == CarrierProfileName);
                if (carrierTypeFromConfig != null)
                {
                    CarrierTypeNumber = carrierTypeFromConfig.Id;
                    CarrierTypeName = carrierTypeFromConfig.Name;
                    CarrierTypeDescription = carrierTypeFromConfig.Description;
                }
            }

            switch (e.Status.CarrierType)
            {
                case CarrierType.Cassette:
                case CarrierType.NotIdentified: // When no carrier is placed.
                    break;
                case CarrierType.Auto:
                    var userMessage = $"Carrier type is {e.Status.CarrierType}. "
                                      + "However, the system does not manage such carriers.";
                    OnUserErrorRaised(userMessage);
                    Logger.Error(userMessage);
                    break;
                default:
                    throw new ArgumentException(nameof(e.Status.CarrierType));
            }

            if (CarrierType != CarrierType.NotIdentified)
            {
                // We need to collect carrier capacity to be able to create carrier object
                Driver.GetDataPerCarrierType(
                    CarrierType,
                    e.Status.CarrierTypeIndex,
                    CarrierDataProperty.NumberOfSlots);
            }
        }

        private void
            Driver_InfoPadsInputReceived(object sender, StatusEventArgs<InfoPadsInputStatus> e)
            => Logger.Warning(
                "Not managed info pads data received: "
                + $"A:{e.Status.InfoPadAPresence}"
                + $"B:{e.Status.InfoPadBPresence}"
                + $"C:{e.Status.InfoPadCPresence}"
                + $"D:{e.Status.InfoPadDPresence}");

        private void Driver_DeviceDataReceived(object sender, GetDeviceDataEventArgs e)
        {
            switch (e.DevicePart)
            {
                case Constants.DevicePartIds.DataByCarrierType:
                    TreatCarrierTypeData(e.CommandParameters, e.Data);
                    break;
                case Constants.DevicePartIds.SystemData:
                    TreatSystemData(e.CommandParameters, e.Data);
                    break;
                default:
                    Logger.Warning(
                        $"{Name} received unexpected data from HW. Data concerns {e.DevicePart}.\n"
                        + $"Data contains \"{e.Data}\" for the given parameter(s) {e.CommandParameters}.");
                    break;
            }
        }

        private void Driver_CommunicationEstablished(object sender, EventArgs e)
        {
            IsCommunicationStarted = IsCommunicating = true;

            // As soon as connected we want to get current HW status and enable events
            Task.Factory.StartNew(
                () =>
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.InitializeCommunication(); },
                        RE201Command.InitializeCommunication);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        RE201Command.GetStatuses);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetVersion(); },
                        RE201Command.GetVersion);
                    for (var iCarrierType = 0; iCarrierType <= 16; iCarrierType++)
                    {
                        DriverWrapper.RunCommand(
                            delegate
                            {
                                Driver.GetDataPerCarrierType(
                                    (uint)iCarrierType,
                                    CarrierDataProperty.CarrierIdentificationCharacters);
                            },
                            RE201Command.GetSystemDataConfig);
                    }
                });
        }

        private void Driver_CommunicationClosed(object sender, EventArgs e)
        {
            IsCommunicating = false;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStopped(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStarted(object sender, EventArgs e)
            => IsCommunicationStarted = Driver.IsCommunicationStarted;

        private void LoadPort_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            // Update device state on command ended
            if (e.PreviousState != ExecutionState.Running)
            {
                return;
            }

            UpdateDeviceState();
        }

        private void Driver_VersionReceived(object sender, VersionAcquisitionEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            Version = e.Version;
        }
        #endregion

        #region Private Methods

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (ExecutionMode == ExecutionMode.Real)
            {
                //Call STOP on driver only if the device is moving
                if (OperationStatus == OperationStatus.Moving
                    || CommandProcessing == CommandProcessing.Processing)
                {
                    DriverWrapper?.InterruptTask();
                    Driver.EmergencyStop();
                }
            }
        }

        private void TreatCarrierTypeData(string[] getCarrierDataCommandParameters, string[] data)
        {
            if (getCarrierDataCommandParameters.Length < 1
                || getCarrierDataCommandParameters.Length > 2)
            {
                throw new ArgumentException(
                    @"Argument length should be 1 or 2.",
                    nameof(getCarrierDataCommandParameters));
            }

            byte capacity = 0;
            if (getCarrierDataCommandParameters.Length == 1)
            {
                capacity = byte.Parse(data[(int)CarrierDataProperty.NumberOfSlots]);
            }
            else
            {
                var carrierDataProperty =
                    (CarrierDataProperty)int.Parse(getCarrierDataCommandParameters[1]);
                if (carrierDataProperty == CarrierDataProperty.NumberOfSlots)
                {
                    capacity = byte.Parse(data[0]);
                }
                else if (carrierDataProperty == CarrierDataProperty.CarrierIdentificationCharacters)
                {
                    var carrierType = data[0].Replace("\"", string.Empty);
                    if (!string.IsNullOrWhiteSpace(carrierType)
                        && !AvailableCarrierTypes.Contains(carrierType))
                    {
                        AvailableCarrierTypes.Add(carrierType);
                    }

                    return;
                }
            }

            Carrier = new Carrier(string.Empty, capacity, DetermineSubstrateSize());
            Carrier.SetLocation(
                IsDocked
                    ? DockedLocation
                    : UndockedLocation);
            Logger.Info(
                $"New carrier instantiated: Capacity => {Carrier.Capacity},Sample size => {Carrier.SampleSize}");
        }

        private void TreatSystemData(string[] getSystemDataCommandParameters, string[] data)
        {
            if (getSystemDataCommandParameters.Length != 1
                || getSystemDataCommandParameters.Length != data.Length)
            {
                return;
            }

            if (getSystemDataCommandParameters.FirstOrDefault()
                == ((int)SystemDataProperty.SoftwareSwitch).ToString())
            {
                // For now we only use this block to enable E84 I/Os at LoadPort side
                // This should be done only if E84 is enabled in configuration (E84 IO card might not be connected to LP)
                if (Configuration.IsE84Enabled)
                {
                    throw new InvalidOperationException("E84 not handled on RE201 load port");
                }
            }
        }

        private SampleDimension DetermineSubstrateSize()
        {
            //For now we only handle 200mm
            //If other size needs to be handled, this method will need to be updated
            return SampleDimension.S200mm;
        }

        private void UpdateDeviceState()
        {
            if (!IsCommunicating)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (ErrorDescription != LoadPort.RE201.Driver.Enums.ErrorCode.None)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (OperationMode == OperationMode.Initializing)
            {
                SetState(OperatingModes.Initialization);
            }
            else if (OperationStatus == OperationStatus.Moving
                     || CommandProcessing == CommandProcessing.Processing)
            {
                SetState(OperatingModes.Executing);
            }
            else if (OperationStatus == OperationStatus.Stop
                     && CommandProcessing == CommandProcessing.Stop)
            {
                SetState(
                    OriginReturnCompletion != OriginReturnCompletion.Completed
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
            else if (OperationStatus == OperationStatus.TemporarilyStop)
            {
                SetState(OperatingModes.Executing); // Maybe we'll need an additional 'Pause' status
            }
            else
            {
                SetState(
                    OriginReturnCompletion == OriginReturnCompletion.Completed
                        ? OperatingModes.Idle
                        : OperatingModes.Maintenance);
            }
        }

        private void ConfigureCarrierIdAcquisition()
        {
            switch (Configuration.CarrierIdentificationConfig.CarrierIdAcquisition)
            {
                case CarrierIDAcquisitionType.TagReader:
                    DriverWrapper.RunCommand(
                        delegate
                        {
                            Driver.SetSystemDataConfig(
                                SystemDataProperty.Rs232cSetting,
                                (int)Rs232Settings.Heart);
                        },
                        RE201Command.SetSystemDataConfig);
                    break;
                case CarrierIDAcquisitionType.CodeReader:
                    DriverWrapper.RunCommand(
                        delegate
                        {
                            Driver.SetSystemDataConfig(
                                SystemDataProperty.Rs232cSetting,
                                (int)Rs232Settings.Keyence);
                        },
                        RE201Command.SetSystemDataConfig);
                    break;
            }
        }

        #endregion
    }
}
