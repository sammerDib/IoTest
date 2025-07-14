using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Xml;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Resources;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

using Carrier = UnitySC.Equipment.Abstractions.Material.Carrier;
using CarrierEventArgs = UnitySC.Equipment.Abstractions.Material.CarrierEventArgs;
using GenericDeviceMessages =
    UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    public partial class LoadPort : IExtendedConfigurableDevice
    {
        #region Fields

        /// <summary>Used to automatically clamp when timer elapsed</summary>
        protected Timer _autoHandOffTimer;

        public const string UnknownCarrierName = "unknown";

        #endregion

        #region Properties

        public bool IsAutoHandOffInProgress { get; private set; }
        public bool IsAutoHandOffEnabled { get; private set; }
        public List<string> AvailableCarrierTypes { get; } = new();

        #endregion

        #region IMaterialLocationContainer

        public CarrierLocation UndockedLocation { get; protected set; }
        public CarrierLocation DockedLocation { get; protected set; }

        /// <inheritdoc />
        public OneToManyComposition<MaterialLocation> MaterialLocations { get; private set; }

        public SampleDimension GetMaterialDimension(byte slot = 1)
            => Carrier?.SampleSize ?? SampleDimension.NoDimension;

        public virtual bool IsReadyForTransfer(
            EffectorType effector,
            out List<string> errorMessages,
            Agileo.EquipmentModeling.Material armMaterial = null,
            byte slot = 1)
        {
            errorMessages = new List<string>();

            // Check we're not already busy with something else
            if (State is OperatingModes.Executing or OperatingModes.Initialization)
            {
                errorMessages.Add(GenericDeviceMessages.AlreadyBusy);
            }

            // Check that carrier is present and mapped
            if (Carrier == null)
            {
                errorMessages.Add(Messages.CarrierNotCorrectlyPlaced);
                return false;
            }

            if (Carrier.MappingTable == null || Carrier.MappingTable.Count <= 0)
            {
                errorMessages.Add(Messages.CarrierNotMapped);
                return false;
            }

            // Check that carrier is docked and opened
            if (!IsDocked)
            {
                errorMessages.Add(Messages.CarrierNotDocked);
            }

            if (!IsDoorOpen)
            {
                errorMessages.Add(Messages.DoorNotOpened);
            }

            // Check that slot exists
            if (!(1 <= slot && slot <= Carrier.MaterialLocations.Count))
            {
                // Special case when only one slot
                // (would be weird to say "not in range 1..1")
                if (Carrier.MaterialLocations.Count == 1)
                {
                    errorMessages.Add(Messages.CarrierHaveOnlyOneSlot);
                }
                else
                {
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.SlotNotInRange,
                            slot,
                            Carrier.MaterialLocations.Count));
                }

                return false;
            }

            // If there is a material on arm, check it can fit into the carrier
            if (armMaterial != null)
            {
                if (armMaterial is not Substrate substrate)
                {
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.MaterialNotAllowed,
                            armMaterial.GetType().Name));
                    return false;
                }

                if (substrate.MaterialDimension != Carrier.SampleSize)
                {
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.SubstrateSizeNotAllowed,
                            substrate.MaterialDimension));
                }

                if (Carrier.MaterialLocations[slot - 1].Material != null)
                {
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.SlotAlreadyHaveMaterial,
                            slot));
                }
            }
            else
            {
                // If there is no material on arm, check carrier can provide one
                if (Carrier.MaterialLocations[slot - 1].Material == null)
                {
                    errorMessages.Add(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.SlotHaveNoMaterial,
                            slot));
                }
            }

            // Check that slot state is ok (empty or correct)
            var slotState = Carrier.MappingTable[slot - 1];
            if (slotState != SlotState.NoWafer && slotState != SlotState.HasWafer)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.SlotHasInvalidState,
                        slot,
                        slotState));
            }

            return errorMessages.Count == 0;
        }

        #endregion IMaterialLocationContainer

        #region Carrier

        private Carrier _carrier;

        public Carrier Carrier
        {
            get => _carrier;
            protected set
            {
                if (_carrier == value)
                {
                    return;
                }

                // Carrier is removed
                if (value == null)
                {
                    _carrier.SetLocation(null);
                    OnCarrierRemoved(new CarrierEventArgs(_carrier));
                    _carrier.Dispose();
                    _carrier = null;
                    return;
                }

                // Carrier instance changes
                // If we already had one (normally no) notify its been removed
                if (_carrier != null)
                {
                    _carrier.SetLocation(null);
                    OnCarrierRemoved(new CarrierEventArgs(_carrier));
                    _carrier.Dispose();
                    _carrier = null;
                }

                // New carrier placed
                _carrier = value;
                _carrier.SetLocation(UndockedLocation);
                OnCarrierPlaced(new CarrierEventArgs(_carrier));
            }
        }

        /// <summary>Occurs when a carrier is removed from the load port.</summary>
        public event EventHandler<CarrierEventArgs> CarrierRemoved;

        /// <summary>Occurs when a carrier is correctly placed on the load port.</summary>
        public event EventHandler<CarrierEventArgs> CarrierPlaced;

        /// <summary>Occurs when carrier identifier changed.</summary>
        public event EventHandler<CarrierIdChangedEventArgs> CarrierIdChanged;

        protected virtual void OnCarrierRemoved(CarrierEventArgs args)
        {
            try
            {
                CarrierRemoved?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        protected virtual void OnCarrierPlaced(CarrierEventArgs args)
        {
            try
            {
                CarrierPlaced?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        protected virtual void OnCarrierIDChanged(CarrierIdChangedEventArgs args)
        {
            try
            {
                CarrierIdChanged?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion Carrier

        #region Setup

        private void InstanceInitialization()
        {
            CarrierPresence = CassettePresence.Absent;
            PhysicalState = LoadPortState.Unclamped;

            // Create reference to materialLocations
            MaterialLocations =
                ReferenceFactory.OneToManyComposition<MaterialLocation>(
                    nameof(MaterialLocations),
                    this);

            // Create static locations
            UndockedLocation = new CarrierLocation("Undocked Location");
            DockedLocation = new CarrierLocation("Docked Location");
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    LoadConfiguration();
                    IsInService = Configuration.IsInService;
                    StatusValueChanged += LoadPort_StatusValueChanged;
                    CreateAutoHandOffTimer(Configuration.AutoHandOffTimeout * 1000);
                    if (Configuration.IsManualCarrierType)
                    {
                        CarrierTypeNumber = 0;
                        CarrierTypeName = UnknownCarrierName;
                        CarrierTypeDescription = UnknownCarrierName;
                    }

                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Simulated)
                    {
                        SetUpSimulatedMode();
                    }

                    // MaterialLocation here (after configuration is read) in case number of locations depend on configuration settings.
                    MaterialLocations.Add(UndockedLocation);
                    MaterialLocations.Add(DockedLocation);
                    DeviceType.AllCommands().First(x => x.Name == nameof(Initialize)).Timeout =
                        Duration.FromSeconds(Configuration.InitializationTimeout);
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region Event Handlers

        protected virtual void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            // Does not update Status in case Load Port is not in service.
            if (e == null || (!IsInService && e.Status.Name != nameof(IsInService)))
            {
                return;
            }

            switch (e.Status.Name)
            {
                case nameof(CarrierPresence):
                    switch ((CassettePresence)e.NewValue)
                    {
                        case CassettePresence.Absent:
                            Carrier = null;
                            Logger.Info("Carrier Disposed");
                            break;
                        case CassettePresence.PresentNoPlacement:
                        case CassettePresence.NoPresentPlacement:
                            if (Configuration.IsManualCarrierType)
                            {
                                CarrierTypeNumber = 0;
                                CarrierTypeName = UnknownCarrierName;
                                CarrierTypeDescription = UnknownCarrierName;
                            }

                            StopAutoHandOffTimer();
                            if (Configuration.HandOffType == HandOffType.Button)
                            {
                                UpdateHandOffButtonLight(LightState.Off);
                            }

                            break;
                        case CassettePresence.Correctly:
                            if (AccessMode == LoadingType.Auto
                                && Configuration.IsE84Enabled
                                && ExecutionMode == ExecutionMode.Simulated)
                            {
                                //In simulation directly clamp the carrier to simulate E84 behavior when auto mode is selected
                                ClampAsync();
                            }

                            if (IsInService
                                && (AccessMode == LoadingType.Manual || !Configuration.IsE84Enabled)
                                && Configuration.HandOffType == HandOffType.Button)
                            {
                                UpdateHandOffButtonLight(LightState.Flashing);
                            }

                            /*
                             * We can't create the carrier here because we need it's capacity and sample size
                             * Concrete implementations need to determine when it's best to create the carrier
                             *
                             */
                            break;
                        default:
                            Logger.Debug("Driver's CarrierPresenceChanged event IGNORED.");
                            break;
                    }

                    if (State == OperatingModes.Initialization
                        || State == OperatingModes.Maintenance)
                    {
                        return;
                    }

                    LaunchAutoHandOffTimer();
                    break;
                case nameof(IsClamped):
                case nameof(IsDocked):
                case nameof(IsDoorOpen):
                    UpdatePhysicalState();
                    if (e.Status.Name.Equals(nameof(IsDocked)) && Carrier != null)
                    {
                        Carrier.SetLocation(
                            (bool)e.NewValue
                                ? DockedLocation
                                : UndockedLocation);
                    }

                    break;
                case nameof(PhysicalState):
                    if (IsClamped && Configuration.HandOffType == HandOffType.Button)
                    {
                        UpdateHandOffButtonLight(LightState.Off);
                    }

                    break;
                case nameof(IsInService):
                    if (!IsInService)
                    {
                        SetState(OperatingModes.Maintenance);
                    }

                    break;
                case nameof(State):
                    if (State == OperatingModes.Maintenance
                        && HandOffLightState != LightState.Off
                        && Configuration.HandOffType == HandOffType.Button)
                    {
                        UpdateHandOffButtonLight(LightState.Off);
                    }

                    break;
            }
        }

        protected virtual void AutoHandOffTimer_Elapsed(object sender, EventArgs e)
        {
            // This conditions are used to prevent clamp/dock in inappropriate state
            StopAutoHandOffTimer();
            if (IsInService
                && State == OperatingModes.Idle
                && AccessMode == LoadingType.Manual
                && CarrierPresence == CassettePresence.Correctly)
            {
                Logger.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - {1} - Automatic handOff timer elapsed, carrier will be clamped.",
                        Name,
                        MethodBase.GetCurrentMethod()?.Name));
                Clamp();
            }
        }

        #endregion Event Handlers

        #region Public LoadPort Commands

        protected abstract void InternalGetStatuses();

        protected abstract void InternalClamp();

        protected abstract void InternalUnclamp();

        protected abstract void InternalDock();

        protected abstract void InternalUndock();

        protected abstract void InternalOpen(bool performMapping);

        protected abstract void InternalClose();

        protected abstract void InternalMap();

        protected virtual void InternalReadCarrierId()
        {
            try
            {
                switch (Configuration.CarrierIdentificationConfig.CarrierIdAcquisition)
                {
                    case CarrierIDAcquisitionType.Generate:
                        if (!string.IsNullOrWhiteSpace(Carrier.Id))
                        {
                            Logger.Debug(
                                $"{Name} - {nameof(InternalReadCarrierId)} - Skipped CarrierId generation because CarrierId already present.");
                            OnCarrierIDChanged(
                                new CarrierIdChangedEventArgs(Carrier.Id, CommandStatusCode.Ok));
                            return;
                        }

                        if (!GenerateCarrierId())
                        {
                            throw new InvalidOperationException("CarrierID generation failed.");
                        }

                        break;
                    case CarrierIDAcquisitionType.CodeReader:
                    case CarrierIDAcquisitionType.TagReader:
                        var retryNumber = 1;
                        while (retryNumber
                               <= Configuration.CarrierIdentificationConfig.MaxNumberOfRetry)
                        {
                            try
                            {
                                if (Configuration.CarrierIdentificationConfig.CarrierIdAcquisition
                                    == CarrierIDAcquisitionType.CodeReader)
                                {
                                    RequestCarrierIdFromBarcodeReader();
                                }
                                else
                                {
                                    RequestCarrierIdFromTagReader();
                                }

                                break;
                            }
                            catch (Exception)
                            {
                                if (retryNumber
                                    == Configuration.CarrierIdentificationConfig.MaxNumberOfRetry)
                                {
                                    throw;
                                }
                            }
                            finally
                            {
                                retryNumber++;
                            }
                        }

                        break;
                    case CarrierIDAcquisitionType.EnterByOperator:
                        RequestCarrierIdFromOperator();
                        break;
                }
            }
            catch (Exception)
            {
                OnCarrierIDChanged(
                    new CarrierIdChangedEventArgs(
                        null,
                        CommandStatusCode.Error,
                        "Carrier ID read failed"));

                //Do not switch to idle in case of error on ReadCarrierId command
                throw;
            }
        }

        protected abstract void InternalReleaseCarrier();

        protected abstract void InternalSetLight(LoadPortLightRoleType role, LightState lightState);

        protected abstract void InternalSetDateAndTime();

        protected abstract void InternalSetAccessMode(LoadingType accessMode);

        protected abstract void InternalRequestLoad();

        protected abstract void InternalRequestUnload();

        protected abstract void InternalSetCarrierType(uint carrierType);

        protected virtual void InternalPrepareForTransfer()
        {
            if (Configuration.CloseDoorAfterRobotAction && !IsDoorOpen)
            {
                InternalOpen(false);
            }
        }

        protected virtual void InternalPostTransfer()
        {
            if (Configuration.CloseDoorAfterRobotAction && IsDoorOpen)
            {
                InternalClose();
            }
        }

        #endregion Public LoadPort Commands

        #region E84

        protected abstract void InternalEnableE84();

        protected abstract void InternalDisableE84();

        protected abstract void InternalSetE84Timeouts(int tp1, int tp2, int tp3, int tp4, int tp5);

        protected abstract void InternalManageEsSignal(bool isActive);

        public event EventHandler<E84ErrorOccurredEventArgs> E84ErrorOccurred;

        protected virtual void OnE84ErrorOccurred(E84ErrorOccurredEventArgs args)
        {
            try
            {
                E84ErrorOccurred?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public abstract bool NeedsInitAfterE84Error();

        #endregion E84

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public LoadPortConfiguration Configuration
            => ConfigManager.Current.Cast<LoadPortConfiguration>();

        /// <inheritdoc />
        public abstract string RelativeConfigurationDir { get; }

        /// <inheritdoc />
        public abstract void LoadConfiguration(string deviceConfigRootPath = "");

        /// <inheritdoc />
        public void SetExecutionMode(ExecutionMode executionMode) => ExecutionMode = executionMode;

        #endregion IConfigurableDevice

        #region Overrides

        protected override void HandleCommandExecutionStateChanged(CommandExecutionEventArgs e)
        {
            if (e.Execution.Context.Command.Name == nameof(ReadCarrierId)
                && e.NewState == ExecutionState.Failed)
            {
                CurrentCommand = string.Empty;
                switch (State)
                {
                    case OperatingModes.Maintenance:
                    case OperatingModes.Idle:
                        //Do nothing
                        break;
                    case OperatingModes.Initialization:
                        SetState(
                            e.NewState == ExecutionState.Success
                                ? OperatingModes.Idle
                                : OperatingModes.Maintenance);
                        break;
                    case OperatingModes.Executing:
                        SetState(
                            PreviousState == OperatingModes.Maintenance
                                ? OperatingModes.Maintenance
                                : OperatingModes.Idle);
                        break;
                }

                return;
            }

            if (!IsInService)
            {
                SetState(OperatingModes.Maintenance);
            }

            base.HandleCommandExecutionStateChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_autoHandOffTimer != null)
                {
                    _autoHandOffTimer.Elapsed -= AutoHandOffTimer_Elapsed;
                }

                if (SimulationData != null)
                {
                    DisposeSimulatedMode();
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Protected Methods

        protected void UpdatePhysicalState()
        {
            // In case LoadPort is not in service, force the lights to OFF and physical state to "Unknown"
            if (!IsInService)
            {
                PhysicalState = LoadPortState.Unknown;
                return;
            }

            var newState = LoadPortState.Unknown;
            if (!IsClamped && !IsDocked && !IsDoorOpen)
            {
                newState = LoadPortState.Unclamped;
            }
            else if (IsClamped && !IsDocked && !IsDoorOpen)
            {
                switch (PhysicalState)
                {
                    case LoadPortState.Unknown:
                    case LoadPortState.Clamped:
                    case LoadPortState.Unclamped:
                        newState = LoadPortState.Clamped;
                        break;
                    case LoadPortState.Docked:
                    case LoadPortState.Undocked:
                    case LoadPortState.Open:
                    case LoadPortState.Closed:
                        newState = LoadPortState.Undocked;
                        break;
                }
            }
            else if (IsClamped && IsDocked && !IsDoorOpen)
            {
                switch (PhysicalState)
                {
                    case LoadPortState.Unknown:
                    case LoadPortState.Clamped:
                    case LoadPortState.Unclamped:
                    case LoadPortState.Docked:
                    case LoadPortState.Undocked:
                        newState = LoadPortState.Docked;
                        break;
                    case LoadPortState.Open:
                    case LoadPortState.Closed:
                        newState = LoadPortState.Closed;
                        break;
                }
            }
            else if (IsClamped && IsDocked && IsDoorOpen)
            {
                newState = LoadPortState.Open;
            }

            PhysicalState = newState;
        }

        protected virtual bool GenerateCarrierId()
        {
            if (CarrierPresence == CassettePresence.Correctly)
            {
                SetCarrierId($"{InstanceId}-{Carrier.PutTimeStamp:yyyyMMddTHHmmss}");
                return true;
            }

            var userMessage =
                $"{Name} - {nameof(GenerateCarrierId)} - Trying to generate Carrier ID while carrier is in state {CarrierPresence}.";
            OnUserErrorRaised(userMessage);
            Logger.Error(userMessage);
            return false;
        }

        protected abstract void RequestCarrierIdFromBarcodeReader();

        protected abstract void RequestCarrierIdFromTagReader();

        protected virtual void ApplyCarrierIdTreatment(string carrierId)
        {
            if (Configuration.CarrierIdentificationConfig.CarrierIdStartIndex >= 0
                || Configuration.CarrierIdentificationConfig.CarrierIdStopIndex >= 0)
            {
                var startIndex = Configuration.CarrierIdentificationConfig.CarrierIdStartIndex >= 0
                    ? Configuration.CarrierIdentificationConfig.CarrierIdStartIndex
                    : 0;
                var stopIndex = Configuration.CarrierIdentificationConfig.CarrierIdStopIndex >= 0
                    ? Configuration.CarrierIdentificationConfig.CarrierIdStopIndex
                    : carrierId.Length - 1;
                if (stopIndex < startIndex)
                {
                    throw new InvalidOperationException(
                        "Carrier Id stop index lower than start index");
                }

                SetCarrierId(carrierId.Substring(startIndex, stopIndex - startIndex + 1));
            }
            else
            {
                SetCarrierId(carrierId);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>Method is used to initialize Clamp timer for load port</summary>
        /// <param name="interval">Clamp timer interval in milliseconds</param>
        private void CreateAutoHandOffTimer(double interval)
        {
            IsAutoHandOffEnabled = Configuration.HandOffType == HandOffType.Automatic;
            _autoHandOffTimer = new Timer(interval);
            _autoHandOffTimer.AutoReset = false;
            _autoHandOffTimer.Elapsed += AutoHandOffTimer_Elapsed;
            Logger.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} - {1} - Automatic Clamp timer created.",
                    Name,
                    MethodBase.GetCurrentMethod()?.Name));
        }

        protected void StartAutoHandOffTimer()
        {
            if (_autoHandOffTimer != null)
            {
                IsAutoHandOffInProgress = true;
                _autoHandOffTimer.Start();
            }
        }

        protected void StopAutoHandOffTimer()
        {
            if (Configuration.HandOffType == HandOffType.Automatic)
            {
                IsAutoHandOffInProgress = false;
                if (_autoHandOffTimer != null)
                {
                    _autoHandOffTimer.Stop();
                }
            }
        }

        protected virtual void LaunchAutoHandOffTimer()
        {
            if (IsInService
                && AccessMode == LoadingType.Manual
                && IsAutoHandOffEnabled
                && CarrierPresence == CassettePresence.Correctly
                && State == OperatingModes.Idle)
            {
                StopAutoHandOffTimer();
                StartAutoHandOffTimer();
                Logger.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - {1} - Automatic Clamp timer started.",
                        Name,
                        MethodBase.GetCurrentMethod()?.Name));
            }
        }

        public void SetCarrierId(string carrierId)
        {
            if (Carrier == null)
            {
                var userMessage =
                    $"{Name} - {nameof(SetCarrierId)} - Can't set carrier ID - Carrier data is null.";
                OnUserErrorRaised(userMessage);
                Logger.Error(userMessage);
                return;
            }

            carrierId = RemoveWhitespaces(carrierId);
            carrierId = RemoveInvalidXmlChars(carrierId);
            Carrier.Id = carrierId;
            Logger.Debug($"{Name} - {nameof(SetCarrierId)} - Carrier ID changed to {carrierId}.");
            OnCarrierIDChanged(new CarrierIdChangedEventArgs(carrierId, CommandStatusCode.Ok));
        }

        public string RemoveWhitespaces(string source)
            => new(source.Where(c => !char.IsWhiteSpace(c)).ToArray());

        private string RemoveInvalidXmlChars(string text)
        {
            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }

        public void SetIsAutoHandOffEnabled(bool value = true) => IsAutoHandOffEnabled = value;

        public void ResetIsAutoHandOffEnabled()
            => IsAutoHandOffEnabled = Configuration.HandOffType == HandOffType.Automatic;

        public void SetIsInService(bool value)
        {
            if (IsInService != value)
            {
                IsInService = value;
                ((LoadPortConfiguration)ConfigManager.Modified).IsInService = IsInService;
                ConfigManager.Apply(true);
                UpdatePhysicalState();
                if (!IsInService)
                {
                    SetState(OperatingModes.Maintenance);
                }
            }
        }

        protected void RequestCarrierIdFromOperator()
        {
            //Do not request the ID if it has already been entered
            if (!string.IsNullOrWhiteSpace(Carrier.Id))
            {
                SetCarrierId(Carrier.Id);
                return;
            }

            OnCarrierIdRequestedFromOperator();
        }

        protected void UpdateHandOffButtonLight(LightState state)
            => SetLightAsync(LoadPortLightRoleType.HandOffButton, state);

        #endregion

        #region CarrierID from Operator

        public event EventHandler<EventArgs> CarrierIdRequestedFromOperator;

        protected void OnCarrierIdRequestedFromOperator()
        {
            try
            {
                CarrierIdRequestedFromOperator?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    FormattableString.Invariant(
                        $"Exception occurred when sending event {nameof(CarrierIdRequestedFromOperator)}."),
                    ex);
            }
        }

        #endregion
    }
}
