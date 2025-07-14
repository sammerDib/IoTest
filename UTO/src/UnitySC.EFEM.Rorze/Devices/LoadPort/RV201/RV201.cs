using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Configuration;
using Agileo.Common.Tracing;
using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Configuration;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Helpers;
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

using CarrierType = UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums.CarrierType;
using MappingEventArgs =
    UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.EventArgs.MappingEventArgs;
using OperationMode = UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201
{
    public partial class RV201 : IConfigurableDevice<RorzeLoadPortConfiguration>, IVersionedDevice
    {
        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            var initAfterE84Error = false;
            base.InternalInitialize(mustForceInit);
            CarrierTypeNumber = 0;
            CarrierTypeName = UnknownCarrierName;
            CarrierTypeDescription = UnknownCarrierName;
            try
            {
                //Stop the load port if a movement is in progress
                if (OperationStatus == OperationStatus.Moving
                    || CommandProcessing == CommandProcessing.Processing)
                {
                    DriverWrapper.RunCommand(delegate { Driver.Stop(); }, RV201Command.Stop);
                }

                if (OriginReturnCompletion == OriginReturnCompletion.Completed
                    && ErrorLightState == LightState.Off
                    && !mustForceInit)
                {
                    InternalSetAccessMode(
                        Configuration.IsE84Enabled
                            ? AccessMode
                            : LoadingType.Manual);
                    if (CarrierPresence == CassettePresence.Absent)
                    {
                        Logger.Info("No need to initialize the device because carrier is absent");
                        return;
                    }

                    if (CarrierPresence == CassettePresence.Correctly
                        && PhysicalState == LoadPortState.Unclamped)
                    {
                        Logger.Info(
                            "No need to initialize the device because carrier is unclamped");
                        return;
                    }

                    DriverWrapper.RunCommand(
                        delegate { Driver.ReleaseCarrier(); },
                        RV201Command.ReleaseCarrier);
                    ConfigureCarrierIdAcquisition();
                    return;
                }

                if (NeedsInitAfterE84Error())
                {
                    initAfterE84Error = true;

                    //Use to ensure that the host won't see the carrier presence changed during init
                    Driver.CarrierPresenceChanged -= Driver_CarrierPresenceChanged;
                }

                //Init the load port
                DriverWrapper.RunCommand(
                    delegate { Driver.Initialization(); },
                    RV201Command.Initialization);
                if (initAfterE84Error)
                {
                    initAfterE84Error = false;

                    //Get the statuses to get the carrier presence
                    Driver.CarrierPresenceChanged += Driver_CarrierPresenceChanged;
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        RV201Command.GetStatuses);
                }

                //TODO Find something better
                //Sleep in order to wait that carrier data type has been retrieved
                Thread.Sleep(500);
                _requestedLoadingType = Configuration.IsE84Enabled
                    ? LoadingType.Auto
                    : LoadingType.Manual;

                //Get the software switches in order to update them
                DriverWrapper.RunCommand(
                    delegate { Driver.GetSystemDataConfig(SystemDataProperty.SoftwareSwitch); },
                    RV201Command.GetSystemDataConfig);
                UpdateDefaultConfig(_data);
                DriverWrapper.RunCommand(
                    delegate { Driver.GetSystemDataConfig(SystemDataProperty.SoftwareSwitch); },
                    RV201Command.GetSystemDataConfig);
                UpdateE84Config(_data, _requestedLoadingType);
                ConfigureCarrierIdAcquisition();
                if (Configuration.HandOffType == HandOffType.Button
                    && CarrierPresence == CassettePresence.Correctly)
                {
                    UpdateHandOffButtonLight(LightState.Flashing);
                }
            }
            catch (Exception e)
            {
                if (initAfterE84Error)
                {
                    //If exception occured we need to subscribe to the event again
                    //And get the statuses to get the carrier presence
                    Driver.CarrierPresenceChanged += Driver_CarrierPresenceChanged;
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        RV201Command.GetStatuses);
                }

                MarkExecutionAsFailed(e);
            }
        }

        #endregion IGenericDevice Commands

        #region Fields

        private const int NbSpeedLevels = 10;
        private LoadingType _requestedLoadingType;
        private bool _lockCarrier;
        private string[] _data;

        #endregion Fields

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
                DriverWrapper.RunCommand(delegate { Driver.Clamp(); }, RV201Command.Clamp);
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
                DriverWrapper.RunCommand(delegate { Driver.Unclamp(); }, RV201Command.Unclamp);
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

                DriverWrapper.RunCommand(delegate { Driver.Dock(); }, RV201Command.Dock);
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
                DriverWrapper.RunCommand(delegate { Driver.Undock(); }, RV201Command.Undock);
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
                if (PhysicalState == LoadPortState.Open && performMapping)
                {
                    DriverWrapper.RunCommand(delegate { Driver.Map(); }, RV201Command.Map);
                }
                else
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.Open(performMapping); },
                        RV201Command.Open);
                }

                if (performMapping)
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetLastMapping(); },
                        RV201Command.GetLastMapping);
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
                DriverWrapper.RunCommand(delegate { Driver.Close(); }, RV201Command.Close);
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
                DriverWrapper.RunCommand(delegate { Driver.Map(); }, RV201Command.Map);
                DriverWrapper.RunCommand(
                    delegate { Driver.GetLastMapping(); },
                    RV201Command.GetLastMapping);
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
                RV201Command.ReadCarrierId);
        }

        protected override void RequestCarrierIdFromTagReader()
        {
            DriverWrapper.RunCommand(
                delegate { Driver.ReadCarrierId(); },
                RV201Command.ReadCarrierId);
        }

        protected override void InternalReleaseCarrier()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.ReleaseCarrier(); },
                    RV201Command.ReleaseCarrier);
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
                    delegate { Driver.SetSignalOutput(Converters.ToRv201Light(role), lightState); },
                    RV201Command.SetSignalOutput);
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
                    RV201Command.SetDateAndTime);
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
                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.SetE84Parameters(
                            new SortedDictionary<E84ParameterProperty, string>
                            {
                                { E84ParameterProperty.OnOffSwitch, "1" },
                                {
                                    E84ParameterProperty.TimerTP1,
                                    (Configuration.E84Configuration.Tp1 * 1000).ToString()
                                },
                                {
                                    E84ParameterProperty.TimerTP2,
                                    (Configuration.E84Configuration.Tp2 * 1000).ToString()
                                },
                                {
                                    E84ParameterProperty.TimerTP3,
                                    (Configuration.E84Configuration.Tp3 * 1000).ToString()
                                },
                                {
                                    E84ParameterProperty.TimerTP4,
                                    (Configuration.E84Configuration.Tp4 * 1000).ToString()
                                },
                                {
                                    E84ParameterProperty.TimerTP5,
                                    (Configuration.E84Configuration.Tp5 * 1000).ToString()
                                }
                            });
                    },
                    RV201Command.SetE84Parameters);
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
                if (OperationStatus == OperationStatus.Moving)
                {
                    DriverWrapper.RunCommand(delegate { Driver.Stop(); }, RV201Command.Stop);
                }

                //To keep in case we need to manage the lights. Christophe asked to put in comment for now
                //Because PTO need to handle it using LPOL command
                //DriverWrapper.RunCommand(
                //    delegate
                //    {
                //        Driver.SetSignalOutput(LoadPortIndicators.Manual, LightState.On);
                //        Driver.SetSignalOutput(LoadPortIndicators.Load, LightState.Off);
                //        Driver.SetSignalOutput(LoadPortIndicators.Unload, LightState.Off);
                //    });
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
                Configuration.E84Configuration.Tp1 = tp1;
                Configuration.E84Configuration.Tp2 = tp2;
                Configuration.E84Configuration.Tp3 = tp3;
                Configuration.E84Configuration.Tp4 = tp4;
                Configuration.E84Configuration.Tp5 = tp5;
                var modifiedConfig = ConfigManager.Modified.Cast<LoadPortConfiguration>();
                modifiedConfig.E84Configuration.Tp1 = tp1;
                modifiedConfig.E84Configuration.Tp2 = tp2;
                modifiedConfig.E84Configuration.Tp3 = tp3;
                modifiedConfig.E84Configuration.Tp4 = tp4;
                modifiedConfig.E84Configuration.Tp5 = tp5;
                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.SetE84Parameters(
                            new SortedDictionary<E84ParameterProperty, string>
                            {
                                { E84ParameterProperty.TimerTP1, (tp1 * 1000).ToString() },
                                { E84ParameterProperty.TimerTP2, (tp2 * 1000).ToString() },
                                { E84ParameterProperty.TimerTP3, (tp3 * 1000).ToString() },
                                { E84ParameterProperty.TimerTP4, (tp4 * 1000).ToString() },
                                { E84ParameterProperty.TimerTP5, (tp5 * 1000).ToString() }
                            });
                    },
                    RV201Command.SetE84Parameters);
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
                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.SetE84Parameters(
                            new SortedDictionary<E84ParameterProperty, string>
                            {
                                {
                                    E84ParameterProperty.OnOffSwitch, isActive
                                        ? "1"
                                        : "0"
                                }
                            });
                    },
                    RV201Command.SetE84Parameters);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetAccessMode(LoadingType accessMode)
        {
            if (AccessMode != accessMode)
            {
                _requestedLoadingType = accessMode;
                DriverWrapper.RunCommand(
                    delegate { Driver.GetSystemDataConfig(SystemDataProperty.SoftwareSwitch); },
                    RV201Command.GetSystemDataConfig);
                UpdateE84Config(_data, _requestedLoadingType);
            }

            InternalSetLight(
                LoadPortLightRoleType.AccessModeManual,
                accessMode == LoadingType.Manual
                    ? LightState.On
                    : LightState.Off);
        }

        protected override void InternalRequestLoad()
        {
            try
            {
                E84TransferInProgress = true;
                DriverWrapper.RunCommand(delegate { Driver.E84Load(); }, RV201Command.E84Load);
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
                E84TransferInProgress = true;
                DriverWrapper.RunCommand(delegate { Driver.E84Unload(); }, RV201Command.E84Unload);
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
                    RV201Command.GetStatuses);
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
                    RV201Command.SetCarrierType);
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
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.LoadPort.LoadPort)}/{nameof(RV201)}/Resources";

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
            if (e.Status.Name == nameof(LoadPort.RV201.Driver.Enums.ErrorCode))
            {
                // Check if error code is related to E84 (if so we need to notify E84 error)
                switch (ErrorCode)
                {
                    case Constants.E84Errors.Tp1Timeout:
                        CurrentE84Error = E84Errors.Tp1Timeout;
                        OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp1Timeout));
                        break;
                    case Constants.E84Errors.Tp2Timeout:
                        CurrentE84Error = E84Errors.Tp2Timeout;
                        OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp2Timeout));
                        break;
                    case Constants.E84Errors.Tp3Timeout_Load:
                    case Constants.E84Errors.Tp3Timeout_Unload:
                        CurrentE84Error = E84Errors.Tp3Timeout;
                        OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp3Timeout));
                        break;
                    case Constants.E84Errors.Tp4Timeout:
                        CurrentE84Error = E84Errors.Tp4Timeout;
                        OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp4Timeout));
                        break;
                    case Constants.E84Errors.Tp5Timeout:
                        CurrentE84Error = E84Errors.Tp5Timeout;
                        OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp5Timeout));
                        break;
                    case Constants.E84Errors.Tp6Timeout:
                    case Constants.E84Errors.E84SignalError_TrReq:
                    case Constants.E84Errors.E84SignalError_Busy:
                    case Constants.E84Errors.E84SignalError_Placement:
                    case Constants.E84Errors.E84SignalError_Complete:
                    case Constants.E84Errors.E84SignalError_Valid:
                    case Constants.E84Errors.E84SignalError_Cs0:
                        CurrentE84Error = E84Errors.SignalError;
                        OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.SignalError));
                        break;
                    default:
                        CurrentE84Error = E84Errors.None;
                        return;
                }
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

            if (CarrierPresence == CassettePresence.Absent)
            {
                _lockCarrier = false;
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

        private void Driver_StatusReceived(object sender, StatusEventArgs<RV201LoadPortStatus> e)
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
            var prevOperationStatus = OperationStatus;
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
            if (e.Status.ErrorCode != LoadPort.RV201.Driver.Enums.ErrorCode.None
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

            if (prevOperationStatus == OperationStatus.Moving
                && OperationStatus == OperationStatus.Stop
                && E84TransferInProgress)
            {
                //If a E84 transfer is in progress, no command can run at the same time
                //So we know that if we switch to moving to stop while an E84 transfer
                //It means that the E84 transfer has ended
                E84TransferInProgress = false;
            }
        }

        private void Driver_GpioReceived(object sender, StatusEventArgs<LoadPortGpioStatus> e)
        {
            #region General Inputs

            I_EmergencyStop = e.Status.I_EmergencyStop;
            I_TemporarilyStop = e.Status.I_TemporarilyStop;
            I_ExhaustFan1 = e.Status.I_ExhaustFan1;
            I_ExhaustFan2 = e.Status.I_ExhaustFan2;
            I_Protrusion = e.Status.I_Protrusion;
            I_Protrusion2 = e.Status.I_Protrusion2;
            I_FOUPDoorLeftClose = e.Status.I_FOUPDoorLeftClose;
            I_FOUPDoorLeftOpen = e.Status.I_FOUPDoorLeftOpen;
            I_FOUPDoorRightClose = e.Status.I_FOUPDoorRightClose;
            I_FOUPDoorRightOpen = e.Status.I_FOUPDoorRightOpen;
            I_MappingSensorContaining = e.Status.I_MappingSensorContaining;
            I_MappingSensorPreparation = e.Status.I_MappingSensorPreparation;
            I_UpperPressureLimit = e.Status.I_UpperPressureLimit;
            I_LowerPressureLimit = e.Status.I_LowerPressureLimit;
            I_CarrierClampOpen = e.Status.I_CarrierClampOpen;
            I_CarrierClampClose = e.Status.I_CarrierClampClose;
            I_PresenceLeft = e.Status.I_PresenceLeft;
            I_PresenceRight = e.Status.I_PresenceRight;
            I_PresenceMiddle = e.Status.I_PresenceMiddle;
            I_InfoPadA = e.Status.I_InfoPadA;
            I_InfoPadB = e.Status.I_InfoPadB;
            I_InfoPadC = e.Status.I_InfoPadC;
            I_InfoPadD = e.Status.I_InfoPadD;
            I_Presence = e.Status.I_Presence;
            I_FOSBIdentificationSensor = e.Status.I_FOSBIdentificationSensor;
            I_ObstacleDetectingSensor = e.Status.I_ObstacleDetectingSensor;
            I_DoorDetection = e.Status.I_DoorDetection;
            I_OpenCarrierDetectingSensor = e.Status.I_OpenCarrierDetectingSensor;
            I_StageRotationBackward = e.Status.I_StageRotationBackward;
            I_StageRotationForward = e.Status.I_StageRotationForward;
            I_BcrLifting = e.Status.I_BcrLifting;
            I_BcrLowering = e.Status.I_BcrLowering;
            I_CoverLock = e.Status.I_CoverLock;
            I_CoverUnlock = e.Status.I_CoverUnlock;
            I_CarrierRetainerLowering = e.Status.I_CarrierRetainerLowering;
            I_CarrierRetainerLifting = e.Status.I_CarrierRetainerLifting;
            I_External_SW1_ACCESS = e.Status.I_External_SW1_ACCESS;
            I_External_SW2_TEST = e.Status.I_External_SW2_TEST;
            I_External_SW3_UNLOAD = e.Status.I_External_SW3_UNLOAD;
            I_PFA_L = e.Status.I_PFA_L;
            I_PFA_R = e.Status.I_PFA_R;
            I_Dsc300mm = e.Status.I_Dsc300mm;
            I_Dsc200mm = e.Status.I_Dsc200mm;
            I_Dsc150mm = e.Status.I_Dsc150mm;
            I_CstCommon = e.Status.I_CstCommon;
            I_Cst200mm = e.Status.I_Cst200mm;
            I_Cst150mm = e.Status.I_Cst150mm;
            I_Adapter = e.Status.I_Adapter;
            I_CoverClosed = e.Status.I_CoverClosed;
            I_VALID = e.Status.I_VALID_E84;
            I_CS_0 = e.Status.I_CS_0_E84;
            I_CS_1 = e.Status.I_CS_1_E84;
            I_TR_REQ = e.Status.I_TR_REQ_E84;
            I_BUSY = e.Status.I_BUSY_E84;
            I_COMPT = e.Status.I_COMPT_E84;
            I_CONT = e.Status.I_CONT_E84;
            IsHandOffButtonPressed = I_External_SW1_ACCESS;

            #endregion General Inputs

            #region General Outputs

            O_PreparationCompleted = e.Status.O_PreparationCompleted_SigNotConnected;
            O_TemporarilyStop = e.Status.O_TemporarilyStop_SigNotConnected;
            O_SignificantError = e.Status.O_SignificantError_SigNotConnected;
            O_LightError = e.Status.O_LightError_SigNotConnected;
            O_Protrusion2Enabled = e.Status.O_Protrusion2Enabled;
            O_AdapterClamp = e.Status.O_AdapterClamp;
            O_AdapterPower = e.Status.O_AdapterPower;
            O_ObstacleDetectionCancel = e.Status.O_ObstacleDetectionCancel;
            O_CarrierClampClose = e.Status.O_CarrierClampClose;
            O_CarrierClampOpen = e.Status.O_CarrierClampOpen;
            O_FOUPDoorLockOpen = e.Status.O_FOUPDoorLockOpen;
            O_FOUPDoorLockClose = e.Status.O_FOUPDoorLockClose;
            O_MappingSensorPreparation = e.Status.O_MappingSensorPreparation;
            O_MappingSensorContaining = e.Status.O_MappingSensorContaining;
            O_ChuckingOn = e.Status.O_ChuckingOn;
            O_ChuckingOff = e.Status.O_ChuckingOff;
            O_CoverLock = e.Status.O_CoverLock;
            O_CoverUnlock = e.Status.O_CoverUnlock;
            O_DoorOpen_Ext = e.Status.O_DoorOpen_ExtOutput;
            O_CarrierClamp_Ext = e.Status.O_CarrierClamp_ExtOutput;
            O_CarrierPresenceOn_Ext = e.Status.O_CarrierPresenceOn_ExtOutput;
            O_PreparationCompleted_Ext = e.Status.O_PreparationCompleted_ExtOutput;
            O_CarrierProperlyPlaced_Ext = e.Status.O_CarrierProperlyPlaced_ExtOutput;
            O_StageRotationBackward = e.Status.O_StageRotationBackward;
            O_StageRotationForward = e.Status.O_StageRotationForward;
            O_BcrLifting = e.Status.O_BcrLifting;
            O_BcrLowering = e.Status.O_BcrLowering;
            O_CarrierRetainerLowering = e.Status.O_CarrierRetainerLowering;
            O_CarrierRetainerLifting = e.Status.O_CarrierRetainerLifting;
            O_SW1_LED = e.Status.O_SW1_LED;
            O_SW3_LED = e.Status.O_SW3_LED;
            O_LOAD_LED = e.Status.O_LOAD_LED;
            O_UNLOAD_LED = e.Status.O_UNLOAD_LED;
            O_PRESENCE_LED = e.Status.O_PRESENCE_LED;
            O_PLACEMENT_LED = e.Status.O_PLACEMENT_LED;
            O_MANUAL_LED = e.Status.O_MANUAL_LED;
            O_ERROR_LED = e.Status.O_ERROR_LED;
            O_CLAMP_LED = e.Status.O_CLAMP_LED;
            O_DOCK_LED = e.Status.O_DOCK_LED;
            O_BUSY_LED = e.Status.O_BUSY_LED;
            O_AUTO_LED = e.Status.O_AUTO_LED;
            O_RESERVED_LED = e.Status.O_RESERVED_LED;
            O_CLOSE_LED = e.Status.O_CLOSE_LED;
            O_LOCK_LED = e.Status.O_LOCK_LED;
            O_L_REQ = e.Status.O_L_REQ_E84;
            O_U_REQ = e.Status.O_U_REQ_E84;
            O_READY = e.Status.O_READY_E84;
            if (O_HO_AVBL != e.Status.O_HO_AVBL_E84
                && e.Status.O_HO_AVBL_E84
                && !E84TransferInProgress)
            {
                //This case happens at startup when no transfer has been requested by the software
                //But E84 transfer is still in progress on the load port
                E84TransferInProgress = true;
            }

            O_HO_AVBL = e.Status.O_HO_AVBL_E84;
            O_ES = e.Status.O_ES_E84;

            #endregion General Outputs

            // Update abstraction statuses
            IsClamped = !I_CarrierClampOpen && I_CarrierClampClose;
            IsDoorOpen = O_DoorOpen_Ext;
            AutoModeLightState = e.Status.O_AUTO_LED
                ? LightState.On
                : LightState.Off;
            ManualModeLightState = e.Status.O_MANUAL_LED
                ? LightState.On
                : LightState.Off;
            LoadLightState = e.Status.O_LOAD_LED
                ? LightState.On
                : LightState.Off;
            UnloadLightState = e.Status.O_UNLOAD_LED
                ? LightState.On
                : LightState.Off;
            ReservedLightState = e.Status.O_RESERVED_LED
                ? LightState.On
                : LightState.Off;
            ErrorLightState = e.Status.O_ERROR_LED
                ? LightState.On
                : LightState.Off;
            HandOffLightState = e.Status.O_SW1_LED
                ? LightState.On
                : LightState.Off;

            //If O_HO_AVBL is active, we need to remain in Idle state
            //Even if operation status is Moving
            if (O_HO_AVBL && State != OperatingModes.Idle)
            {
                SetState(OperatingModes.Idle);
            }
        }

        private void Driver_GposReceived(object sender, StatusEventArgs<LoadPortGposStatus> e)
        {
            if (e == null)
            {
                return;
            }

            IsDocked = e.Status.YAxis is YAxisPositions.DoorChuck or YAxisPositions.WaferCarryInOut;
        }

        private void Driver_CarrierIdentificationMethodReceived(
            object sender,
            StatusEventArgs<CarrierTypeStatus> e)
        {
            CarrierDetectionMode = e.Status.CarrierType;

            // TODO: For now, LoadPorts are working only with FOUP and OC Adapter carrier types.
            switch (e.Status.CarrierType)
            {
                case CarrierType.Auto:
                case CarrierType.FOUP:
                case CarrierType.CarrierOnAdapter:
                    break;
                case CarrierType.Foup1Slot:
                case CarrierType.Special:
                    {
                        var userMessage = $"Identification method is {e.Status.CarrierType}. "
                                          + "However, the system might not manage such carriers.";
                        OnUserWarningRaised(userMessage);
                        Logger.Warning(userMessage);
                    }

                    break;
                case CarrierType.FOSB:
                case CarrierType.OpenCassette:
                case CarrierType.NotIdentified:
                    {
                        var userMessage = $"Identification method is {e.Status.CarrierType}. "
                                          + "However, the system does not manage such carriers.";
                        OnUserErrorRaised(userMessage);
                        Logger.Error(userMessage);
                    }

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

            // TODO: For now, LoadPorts are working only with FOUP and OC Adapter carrier types.
            switch (e.Status.CarrierType)
            {
                case CarrierType.FOUP:
                case CarrierType.CarrierOnAdapter:
                case CarrierType.NotIdentified: // When no carrier is placed.
                    break;
                case CarrierType.Foup1Slot:
                case CarrierType.Special:
                    {
                        var userMessage = $"Carrier type is {e.Status.CarrierType}. "
                                          + "However, the system might not manage such carriers.";
                        OnUserWarningRaised(userMessage);
                        Logger.Warning(userMessage);
                    }

                    break;
                case CarrierType.FOSB:
                case CarrierType.OpenCassette:
                case CarrierType.Auto:
                    {
                        var userMessage = $"Carrier type is {e.Status.CarrierType}. "
                                          + "However, the system does not manage such carriers.";
                        OnUserErrorRaised(userMessage);
                        Logger.Error(userMessage);
                    }

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

        private void Driver_InfoPadsInputReceived(
            object sender,
            StatusEventArgs<InfoPadsInputStatus> e)
        {
            // TODO: see if that data could be useful (redundant with GPIO)
            Logger.Warning(
                "Not managed info pads data received: "
                + $"A:{e.Status.InfoPadAPresence}"
                + $"B:{e.Status.InfoPadBPresence}"
                + $"C:{e.Status.InfoPadCPresence}"
                + $"D:{e.Status.InfoPadDPresence}");
        }

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
                        RV201Command.InitializeCommunication);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        RV201Command.GetStatuses);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetVersion(); },
                        RV201Command.GetVersion);
                    for (var iCarrierType = 0; iCarrierType <= 63; iCarrierType++)
                    {
                        DriverWrapper.RunCommand(
                            delegate
                            {
                                Driver.GetDataPerCarrierType(
                                    (uint)iCarrierType,
                                    CarrierDataProperty.CarrierIdentificationCharacters);
                            },
                            RV201Command.GetSystemDataConfig);
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
                    DriverWrapper.InterruptTask();
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

            if (Carrier != null && _lockCarrier && CurrentCommand != nameof(ILoadPort.Initialize))
            {
                return;
            }

            Carrier = new Carrier(string.Empty, capacity, DetermineSubstrateSize());
            Carrier.SetLocation(
                IsDocked
                    ? DockedLocation
                    : UndockedLocation);
            _lockCarrier = true;
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

            _data = data;
        }

        private void UpdateDefaultConfig(string[] data)
        {
            var currentValue = int.Parse(data.FirstOrDefault() ?? string.Empty);
            var newSoftwareSwitchValue = currentValue | Constants.ExternalIoMask;
            DriverWrapper.RunCommand(
                delegate
                {
                    Driver.SetSystemDataConfig(
                        SystemDataProperty.SoftwareSwitch,
                        newSoftwareSwitchValue);
                },
                RV201Command.SetSystemDataConfig);
        }

        private void UpdateE84Config(string[] data, LoadingType loadingType)
        {
            var currentValue = int.Parse(data.FirstOrDefault() ?? string.Empty);
            int newSoftwareSwitchValue;
            int newSignalOutputSettingValue;
            if (loadingType == LoadingType.Manual)
            {
                newSoftwareSwitchValue = currentValue & ~Constants.E84RorzeLoadPortMask;
                newSignalOutputSettingValue = 0;
            }
            else
            {
                newSoftwareSwitchValue = currentValue | Constants.E84RorzeLoadPortMask;
                newSignalOutputSettingValue = 1;
            }

            try
            {
                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.SetSystemDataConfig(
                            SystemDataProperty.SoftwareSwitch,
                            newSoftwareSwitchValue);
                    },
                    RV201Command.SetSystemDataConfig);
                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.SetSystemDataConfig(
                            SystemDataProperty.SignalOutputSetting,
                            newSignalOutputSettingValue);
                    },
                    RV201Command.SetSystemDataConfig);
                AccessMode = loadingType;
                if (AccessMode == LoadingType.Auto)
                {
                    InternalSetE84Timeouts(
                        Configuration.E84Configuration.Tp1,
                        Configuration.E84Configuration.Tp2,
                        Configuration.E84Configuration.Tp3,
                        Configuration.E84Configuration.Tp4,
                        Configuration.E84Configuration.Tp5);
                }
            }
            catch (Exception ex)
            {
                MarkExecutionAsFailed(ex);
            }
        }

        private SampleDimension DetermineSubstrateSize()
        {
            if (I_Adapter)
            {
                // OC Adapter is plugged-in, use open cassette sensors to determine the size
                if (!I_CstCommon)
                {
                    // Not expected to happen: if we require size, cassette should be present so the common sensor should be on
                    Logger.Warning(
                        "Failed to determine substrate size; Adapter is present but common sensor is off");
                    return SampleDimension.NoDimension;
                }

                if (I_Cst150mm && !I_Cst200mm)
                {
                    return SampleDimension.S150mm;
                }

                if (I_Cst150mm && I_Cst200mm)
                {
                    return SampleDimension.S200mm;
                }

                const SampleDimension defaultSizeCassette = SampleDimension.S200mm;
                Logger.Warning(
                    new TraceParam(
                        new StringBuilder().AppendLine($"150mm CST = {I_Cst150mm}")
                            .AppendLine($"200mm CST = {I_Cst200mm}")
                            .ToString()),
                    $"Unexpected sensors state; default size is used: {defaultSizeCassette}");
                return defaultSizeCassette;
            }

            // OC Adapter NOT plugged-in, consider material is FOUP
            //if (CarrierPresence != CassettePresence.Correctly)
            //{
            //    // Not expected to happen: if we require size, carrier should be correctly placed
            //    Logger.Warning(
            //        "Failed to determine substrate size; carrier is not correctly placed.");
            //    return SampleDimension.NoDimension;
            //}
            // TODO we should probably have an InfoPad config to determine wafer size
            return RorzeConstants.SubstrateDimension;
        }

        private void UpdateDeviceState()
        {
            if (!IsCommunicating)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (ErrorDescription != LoadPort.RV201.Driver.Enums.ErrorCode.None)
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
                if (E84TransferInProgress)
                {
                    //We need to remain IDLE if E84Load/Unload is requested
                    //because the load port is in operation status moving
                    return;
                }

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
                                RV201Command.Stop);
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
                    Driver.CarrierIdReceived -= Driver_CarrierIdReceived;
                    Driver.VersionReceived -= Driver_VersionReceived;
                    CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                    Driver = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable

        #region Private Methods

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
                        RV201Command.SetSystemDataConfig);
                    break;
                case CarrierIDAcquisitionType.CodeReader:
                    DriverWrapper.RunCommand(
                        delegate
                        {
                            Driver.SetSystemDataConfig(
                                SystemDataProperty.Rs232cSetting,
                                (int)Rs232Settings.Keyence);
                        },
                        RV201Command.SetSystemDataConfig);
                    break;
            }
        }

        public override bool NeedsInitAfterE84Error()
        {
            switch (ErrorCode)
            {
                case Constants.E84Errors.Tp1Timeout:
                case Constants.E84Errors.Tp2Timeout:
                case Constants.E84Errors.Tp3Timeout_Load:
                case Constants.E84Errors.Tp3Timeout_Unload:
                case Constants.E84Errors.Tp4Timeout:
                case Constants.E84Errors.Tp5Timeout:
                case Constants.E84Errors.Tp6Timeout:
                case Constants.E84Errors.E84SignalError_TrReq:
                case Constants.E84Errors.E84SignalError_Busy:
                case Constants.E84Errors.E84SignalError_Placement:
                case Constants.E84Errors.E84SignalError_Complete:
                case Constants.E84Errors.E84SignalError_Valid:
                case Constants.E84Errors.E84SignalError_Cs0:
                case Constants.E84Errors.CarrierImproperlyPlaced:
                case Constants.E84Errors.CarrierImproperlyTaken:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}
