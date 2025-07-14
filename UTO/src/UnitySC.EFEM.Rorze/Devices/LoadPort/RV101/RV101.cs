using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Configuration;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Helpers;
using UnitySC.EFEM.Rorze.Drivers;
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

using CarrierType = UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums.CarrierType;
using MappingEventArgs =
    UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.EventArgs.MappingEventArgs;
using OperationMode = UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101
{
    public partial class RV101 : IConfigurableDevice<RorzeLoadPortConfiguration>, IVersionedDevice
    {
        #region Fields

        private const int NbSpeedLevels = 10;

        #endregion Fields

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
                //Stop the load port if a movement is in progress
                if (OperationStatus == OperationStatus.Moving
                    || CommandProcessing == CommandProcessing.Processing)
                {
                    DriverWrapper.RunCommand(delegate { Driver.Stop(); }, RV101Command.Stop);
                }

                //Init the load port
                DriverWrapper.RunCommand(
                    delegate { Driver.Initialization(); },
                    RV101Command.Initialization);

                //Get the software switches in order to update them
                DriverWrapper.RunCommand(
                    delegate { Driver.GetSystemDataConfig(SystemDataProperty.SoftwareSwitch); },
                    RV101Command.GetSystemDataConfig);

                //TODO Find something better
                //Sleep in order to wait that carrier data type has been retrieved
                Thread.Sleep(500);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IGenericDevice Commands

        #region Properties

        private LoadPortDriver Driver { get; set; }
        private DriverWrapper DriverWrapper { get; set; }

        #endregion Properties

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
                        Driver.VersionReceived += Driver_VersionReceived;
                        CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
                        Driver.GetCarrierIdPageInterval = () =>
                        {
                            if (Configuration == null
                                || Configuration.UseDefaultPageIntervalForReading)
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

                    AccessMode = LoadingType.Manual;
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication() => Driver.EnableCommunications();

        protected override void InternalStopCommunication() => Driver.Disconnect();

        #endregion ICommunicatingDevice Commands

        #region ILoadPort Commands

        protected override void InternalClamp()
        {
            try
            {
                DriverWrapper.RunCommand(delegate { Driver.Clamp(); }, RV101Command.Clamp);
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
                DriverWrapper.RunCommand(delegate { Driver.Unclamp(); }, RV101Command.Unclamp);
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
                if (!IsClamped)
                {
                    InternalClamp();
                }

                DriverWrapper.RunCommand(delegate { Driver.Dock(); }, RV101Command.Dock);
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
                DriverWrapper.RunCommand(delegate { Driver.Undock(); }, RV101Command.Undock);
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
                DriverWrapper.RunCommand(
                    delegate { Driver.Open(performMapping); },
                    RV101Command.Open);
                if (performMapping)
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetLastMapping(); },
                        RV101Command.GetLastMapping);
                }
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
                DriverWrapper.RunCommand(delegate { Driver.Close(); }, RV101Command.Close);
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
                DriverWrapper.RunCommand(delegate { Driver.Map(); }, RV101Command.Map);
                DriverWrapper.RunCommand(
                    delegate { Driver.GetLastMapping(); },
                    RV101Command.GetLastMapping);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void RequestCarrierIdFromBarcodeReader()
            => throw new InvalidOperationException("Barcode reader not supported.");

        protected override void RequestCarrierIdFromTagReader()
            => throw new InvalidOperationException("Tag reader not supported.");

        protected override void InternalReleaseCarrier()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.ReleaseCarrier(); },
                    RV101Command.ReleaseCarrier);
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
                    delegate { Driver.SetSignalOutput(Converters.ToRv101Light(role), lightState); },
                    RV101Command.SetSignalOutput);
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
                    RV101Command.SetDateAndTime);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalEnableE84()
        {
            try
            {
                throw new InvalidOperationException("E84 not handled on RV101 load port");
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalDisableE84()
        {
            try
            {
                throw new InvalidOperationException("E84 not handled on RV101 load port");
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetE84Timeouts(int tp1, int tp2, int tp3, int tp4, int tp5)
        {
            try
            {
                throw new InvalidOperationException("E84 not handled on RV101 load port");
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalManageEsSignal(bool isActive)
        {
            try
            {
                throw new InvalidOperationException("E84 not handled on RV101 load port");
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetAccessMode(LoadingType accessMode)
            => throw new InvalidOperationException("E84 not handled on RV101 load port");

        protected override void InternalRequestLoad()
        {
            try
            {
                throw new InvalidOperationException("E84 not handled on RV101 load port");
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalRequestUnload()
        {
            try
            {
                throw new InvalidOperationException("E84 not handled on RV101 load port");
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
                    RV101Command.GetStatuses);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetCarrierType(uint carrierType)
        {
            try
            {
                var carrierTypeFromConfig =
                    Configuration.CarrierTypes.First(x => x.Id == carrierType);
                DriverWrapper.RunCommand(
                    delegate { Driver.SetCarrierType(carrierTypeFromConfig.Name); },
                    RV101Command.SetCarrierType);
                CarrierTypeNumber = carrierType;
                CarrierTypeName = carrierTypeFromConfig.Name;
                CarrierTypeDescription = carrierTypeFromConfig.Description;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion ILoadPort Commands

        #region Configuration

        public new RorzeLoadPortConfiguration Configuration
            => base.Configuration.Cast<RorzeLoadPortConfiguration>();

        public RorzeLoadPortConfiguration CreateDefaultConfiguration() => new();

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.LoadPort.LoadPort)}/{nameof(RV101)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
            => ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);

        #endregion Configuration

        #region Event Handlers

        protected override void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            base.LoadPort_StatusValueChanged(sender, e);

            // For now we monitor only changes on ErrorCode
            if (e.Status.Name == nameof(LoadPort.RV101.Driver.Enums.ErrorCode))
            {
                CurrentE84Error = E84Errors.None;
            }
        }

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
            if (CarrierPresence != e.Presence)
            {
                CarrierPresence = e.Presence;
            }
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

        private void Driver_StatusReceived(object sender, StatusEventArgs<RV101LoadPortStatus> e)
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
            if (e.Status.ErrorCode != LoadPort.RV101.Driver.Enums.ErrorCode.None
                && CurrentCommand != nameof(Initialize))
            {
                //New alarm detected
                SetAlarmById(((int)e.Status.ErrorCode + 1000).ToString());
            }
            else
            {
                //Clear the previously set alarm
                ClearAlarmById(((int)ErrorDescription + 1000).ToString());
            }

            ErrorCode = ((int)e.Status.ErrorCode).ToString("X2");
            ErrorDescription = e.Status.ErrorCode;
            if (State != OperatingModes.Executing
                || (OperationStatus == OperationStatus.Stop
                    && CommandProcessing == CommandProcessing.Stop))
            {
                UpdateDeviceState();
            }
        }

        private void Driver_GpioReceived(object sender, StatusEventArgs<LoadPortGpioStatus> e)
        {
            #region General Inputs

            I_EmergencyStop = e.Status.I_EmergencyStop;
            I_TemporarilyStop = e.Status.I_TemporarilyStop;
            I_VacuumSourcePressure = e.Status.I_VacuumSourcePressure;
            I_AirSupplySourcePressure = e.Status.I_AirSupplySourcePressure;
            I_ProtrusionDetection = e.Status.I_ProtrusionDetection;
            I_Cover = e.Status.I_Cover;
            I_DrivePower = e.Status.I_DrivePower;
            I_MappingSensor = e.Status.I_MappingSensor;
            I_ShutterOpen = e.Status.I_ShutterOpen;
            I_ShutterClose = e.Status.I_ShutterClose;
            I_PresenceLeft = e.Status.I_PresenceLeft;
            I_PresenceRight = e.Status.I_PresenceRight;
            I_PresenceMiddle = e.Status.I_PresenceMiddle;
            I_InfoPadA = e.Status.I_InfoPadA;
            I_InfoPadB = e.Status.I_InfoPadB;
            I_InfoPadC = e.Status.I_InfoPadC;
            I_InfoPadD = e.Status.I_InfoPadD;
            I_200mmPresenceLeft = e.Status.I_200mmPresenceLeft;
            I_200mmPresenceRight = e.Status.I_200mmPresenceRight;
            I_150mmPresenceLeft = e.Status.I_150mmPresenceLeft;
            I_150mmPresenceRight = e.Status.I_150mmPresenceRight;
            I_AccessSwitch1 = e.Status.I_AccessSwitch1;
            I_AccessSwitch2 = e.Status.I_AccessSwitch2;

            #endregion General Inputs

            #region General Outputs

            O_PreparationCompleted = e.Status.O_PreparationCompleted;
            O_TemporarilyStop = e.Status.O_TemporarilyStop;
            O_SignificantError = e.Status.O_SignificantError;
            O_LightError = e.Status.O_LightError;
            O_ClampMovingDirection = e.Status.O_ClampMovingDirection;
            O_ClampMovingStart = e.Status.O_ClampMovingStart;
            O_ShutterOpen = e.Status.O_ShutterOpen;
            O_ShutterClose = e.Status.O_ShutterClose;
            O_ShutterMotionDisabled = e.Status.O_ShutterMotionDisabled;
            O_ShutterOpen2 = e.Status.O_ShutterOpen2;
            O_CoverLock = e.Status.O_CoverLock;
            O_CarrierPresenceSensorOn = e.Status.O_CarrierPresenceSensorOn;
            O_PreparationCompleted2 = e.Status.O_PreparationCompleted2;
            O_CarrierProperlyPlaced = e.Status.O_CarrierProperlyPlaced;
            O_AccessSwitch1 = e.Status.O_AccessSwitch1;
            O_AccessSwitch2 = e.Status.O_AccessSwitch2;
            O_LOAD_LED = e.Status.O_LOAD_LED;
            O_UNLOAD_LED = e.Status.O_UNLOAD_LED;
            O_PRESENCE_LED = e.Status.O_PRESENCE_LED;
            O_PLACEMENT_LED = e.Status.O_PLACEMENT_LED;
            O_LATCH_LED = e.Status.O_LATCH_LED;
            O_ERROR_LED = e.Status.O_ERROR_LED;
            O_BUSY_LED = e.Status.O_BUSY_LED;

            #endregion General Outputs

            // Update abstraction statuses
            IsClamped = O_CoverLock;
            IsDoorOpen = O_ShutterOpen2;
            IsDocked = I_ShutterOpen && !I_ShutterClose;
            AutoModeLightState = LightState.Off;
            ManualModeLightState = LightState.On;
            LoadLightState = e.Status.O_LOAD_LED
                ? LightState.On
                : LightState.Off;
            UnloadLightState = e.Status.O_UNLOAD_LED
                ? LightState.On
                : LightState.Off;
            ReservedLightState = LightState.Off;
            ErrorLightState = e.Status.O_ERROR_LED
                ? LightState.On
                : LightState.Off;
            HandOffLightState = LightState.Off;
        }

        private void Driver_GposReceived(object sender, StatusEventArgs<LoadPortGposStatus> e)
        {
            if (e == null)
            {
            }

            //TODO get position for docked
            //IsDocked = e.Status.
        }

        private void Driver_CarrierIdentificationMethodReceived(
            object sender,
            StatusEventArgs<CarrierTypeStatus> e)
            => CarrierDetectionMode = e.Status.CarrierType;

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
                        RV101Command.InitializeCommunication);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        RV101Command.GetStatuses);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetVersion(); },
                        RV101Command.GetVersion);
                    for (var iCarrierType = 0; iCarrierType <= 31; iCarrierType++)
                    {
                        DriverWrapper.RunCommand(
                            delegate
                            {
                                Driver.GetDataPerCarrierType(
                                    (uint)iCarrierType,
                                    CarrierDataProperty.CarrierIdentificationCharacters);
                            },
                            RV101Command.GetSystemDataConfig);
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
        #endregion Event Handlers

        #region Other Methods

        public override bool NeedsInitAfterE84Error()
            => throw new InvalidOperationException("E84 not handled on RV101 load port");

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            if (ExecutionMode == ExecutionMode.Real)
            {
                Driver.EmergencyStop();
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
                // TODO: In order to save time, only the carrier capacity is treated for now.
                // TODO: All useful other CarrierDataProperties should be treated here.
                capacity = byte.Parse(data[(int)CarrierDataProperty.NumberOfSlots]);
            }
            else
            {
                var carrierDataProperty =
                    (CarrierDataProperty)int.Parse(getCarrierDataCommandParameters[1]);

                // TODO: In order to save time, only the carrier capacity is treated for now.
                // TODO: All useful other CarrierDataProperties should be treated here.
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
            }

            // not needed for the moment
        }

        private SampleDimension DetermineSubstrateSize()
        {
            if (I_150mmPresenceLeft
                && I_150mmPresenceRight
                && !I_200mmPresenceLeft
                && !I_200mmPresenceRight)
            {
                return SampleDimension.S150mm;
            }

            if (I_200mmPresenceLeft && I_200mmPresenceRight)
            {
                return SampleDimension.S200mm;
            }

            return RorzeConstants.SubstrateDimension;
        }

        private void UpdateDeviceState()
        {
            if (!IsCommunicating)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (ErrorDescription != LoadPort.RV101.Driver.Enums.ErrorCode.None)
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

        #endregion Other Methods

        #region IDisposable

        private bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (Driver != null)
                {
                    if (IsCommunicating)
                    {
                        if (OperationStatus == OperationStatus.Moving
                            || CommandProcessing == CommandProcessing.Processing)
                        {
                            DriverWrapper.RunCommand(
                                delegate { Driver.Stop(); },
                                RV101Command.Stop);
                        }
                    }

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
                    Driver.VersionReceived -= Driver_VersionReceived;
                    CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                    Driver = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
