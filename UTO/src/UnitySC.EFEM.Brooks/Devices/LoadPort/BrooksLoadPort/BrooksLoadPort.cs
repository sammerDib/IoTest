using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using BAI.General;
using BAI.Systems.Common;
using BAI.Systems.Common.Carriers;
using BAI.Systems.Common.Carriers.Controls;
using BAI.Systems.Devices.LoadPort;
using BAI.Systems.Modules.EFEM;

using UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort.Configuration;
using UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort.Resources;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

using ILoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.ILoadPort;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort
{
    public partial class BrooksLoadPort : IConfigurableDevice<BrooksLoadPortConfiguration>
    {
        #region Fields

        private LoadPortRemoteProxy _loadPortProxy;
        private EfemProxy _efemProxy;

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            BAI.CTC.ClientInit.ClientLibLoader.InitializeLoader();
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
                        _efemProxy = Helpers.Helper.GetEfemProxy(this);

                        var loadPort = _efemProxy.GetDevice(Configuration.BrooksLoadPortName);
                        if (loadPort is not LoadPortRemoteProxy loadPortProxy)
                        {
                            throw new InvalidOperationException(
                                Messages.LoadPortNotPresentInEfemConfig);
                        }

                        _loadPortProxy = loadPortProxy;
                        _loadPortProxy.CarrierClampStateChanged +=
                            LoadPortProxy_CarrierClampStateChanged;
                        _loadPortProxy.AutoHandoffActiveSignalChanged +=
                            LoadPortProxy_AutoHandoffActiveSignalChanged;
                        _loadPortProxy.AutoHandoffPassiveSignalChanged +=
                            LoadPortProxy_AutoHandoffPassiveSignalChanged;
                        _loadPortProxy.CarrierPresenceStateChanged +=
                            LoadPortProxy_CarrierPresenceStateChanged;
                        _loadPortProxy.CarrierPortLocationChanged +=
                            LoadPortProxy_CarrierPortLocationChanged;
                        _loadPortProxy.AlarmGenerated += LoadPortProxy_AlarmGenerated;
                        _loadPortProxy.CarrierDoorStateChanged +=
                            LoadPortProxy_CarrierDoorStateChanged;
                        _efemProxy.CarrierArrived += EfemProxy_CarrierArrived;
                        _efemProxy.CarrierRemoved += EfemProxy_CarrierRemoved;
                        _efemProxy.AutoHandoffTimedoutOnTP += EfemProxy_AutoHandoffTimedoutOnTP;
                        _efemProxy.AutoHandoffActiveSignalLost +=
                            EfemProxy_AutoHandoffActiveSignalLost;
                        _efemProxy.AutoHandoffActiveSignalUnexpected +=
                            EfemProxy_AutoHandoffActiveSignalUnexpected;
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                //Base init
                base.InternalInitialize(mustForceInit);
                CarrierTypeNumber = 0;
                CarrierTypeName = UnknownCarrierName;
                CarrierTypeDescription = UnknownCarrierName;

                //Device init
                _loadPortProxy.ClearAlarm();
                _loadPortProxy.Initialize();

                if (mustForceInit)
                {
                    _loadPortProxy.HomeLoadPort();
                }
                else
                {
                    InternalUnclamp();
                }

                //Status update
                InternalGetStatuses();
                InternalSetAccessMode(
                    Configuration.IsE84Enabled
                        ? AccessMode
                        : LoadingType.Manual);
                if (CarrierPresence == CassettePresence.Correctly)
                {
                    CreateCarrier();
                }

                //Check device ready
                if (!_loadPortProxy.IsOperable())
                {
                    throw new InvalidOperationException(Messages.LoadPortNotOperable);
                }

                if (_loadPortProxy.IsInMaintenance())
                {
                    throw new InvalidOperationException(Messages.LoadPortInMaintenance);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            try
            {
                if (!_loadPortProxy.Connected)
                {
                    _loadPortProxy.Connect();
                }

                IsCommunicationStarted = true;
                IsCommunicating = true;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStopCommunication()
        {
            try
            {
                if (_loadPortProxy.Connected)
                {
                    _loadPortProxy.Disconnect();
                }

                IsCommunicationStarted = false;
                IsCommunicating = false;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region ILoadPort Commands

        protected override void InternalGetStatuses()
        {
            try
            {
                //Presence status
                UpdatePresenceStatus(_loadPortProxy.GetCarrierPresenceState());

                //E84 Active signals
                I_VALID = _loadPortProxy.GetAutoHandoffActiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalAtoP.VALID);
                I_BUSY = _loadPortProxy.GetAutoHandoffActiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalAtoP.BUSY);
                I_COMPT = _loadPortProxy.GetAutoHandoffActiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalAtoP.COMPT);
                I_CONT = _loadPortProxy.GetAutoHandoffActiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalAtoP.CONT);
                I_CS_0 = _loadPortProxy.GetAutoHandoffActiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalAtoP.CS_0);
                I_CS_1 = _loadPortProxy.GetAutoHandoffActiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalAtoP.CS_1);
                I_TR_REQ = _loadPortProxy.GetAutoHandoffActiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalAtoP.TR_REQ);

                //E84 Passive signals
                O_ES = _loadPortProxy.GetAutoHandoffPassiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalPtoA.ES);
                O_HO_AVBL = _loadPortProxy.GetAutoHandoffPassiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalPtoA.HO_AVBL);
                O_L_REQ = _loadPortProxy.GetAutoHandoffPassiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalPtoA.L_REQ);
                O_U_REQ = _loadPortProxy.GetAutoHandoffPassiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalPtoA.U_REQ);
                O_READY = _loadPortProxy.GetAutoHandoffPassiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalPtoA.READY);
                IsClamped = _loadPortProxy.GetCarrierClampState() == LockState.Locked;
                IsDocked = _loadPortProxy.GetCarrierDockState() == CarrierDockState.Docked;
                IsDoorOpen = _loadPortProxy.GetCarrierDoorState() == GateState.Open;

                //Standard lights
                ErrorLightState = GetLightState(
                    _loadPortProxy.GetLoadPortStdLedState(LoadPortStdLed.Error));
                PlacementLightState = GetLightState(
                    _loadPortProxy.GetLoadPortStdLedState(LoadPortStdLed.Placement));
                PresenceLightState = GetLightState(
                    _loadPortProxy.GetLoadPortStdLedState(LoadPortStdLed.Presence));
                ReadyLightState = GetLightState(
                    _loadPortProxy.GetLoadPortStdLedState(LoadPortStdLed.Ready));

                //Custom lights
                Led1LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED1));
                Led2LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED2));
                Led3LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED3));
                Led4LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED4));
                Led5LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED5));
                Led6LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED6));
                Led7LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED7));
                Led8LightState = GetLightState(
                    _loadPortProxy.GetLoadPortCustomLedState(LoadPortCustomLed.LED8));

                //Infopads
                InfoPadA = _loadPortProxy.GetCarrierInfoPadsData()[CarrierInfoPad.A]
                           == HighLowState.High;
                InfoPadB = _loadPortProxy.GetCarrierInfoPadsData()[CarrierInfoPad.B]
                           == HighLowState.High;
                InfoPadC = _loadPortProxy.GetCarrierInfoPadsData()[CarrierInfoPad.C]
                           == HighLowState.High;
                InfoPadD = _loadPortProxy.GetCarrierInfoPadsData()[CarrierInfoPad.D]
                           == HighLowState.High;
                InfoPadE = _loadPortProxy.GetCarrierInfoPadsData()[CarrierInfoPad.E]
                           == HighLowState.High;
                InfoPadF = _loadPortProxy.GetCarrierInfoPadsData()[CarrierInfoPad.F]
                           == HighLowState.High;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalClamp()
        {
            try
            {
                _loadPortProxy.ClampCarrier();
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
                if (IsDoorOpen)
                {
                    _loadPortProxy.CloseCarrierDoor();
                }

                if (IsDocked)
                {
                    _loadPortProxy.UndockCarrier();
                }

                _loadPortProxy.UnclampCarrier();
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
                    _loadPortProxy.ClampCarrier();
                }

                _loadPortProxy.DockCarrier();
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
                if (IsDoorOpen)
                {
                    _loadPortProxy.CloseCarrierDoor();
                }

                _loadPortProxy.UndockCarrier();
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
                if (!IsClamped)
                {
                    _loadPortProxy.ClampCarrier();
                }

                if (!IsDocked)
                {
                    _loadPortProxy.DockCarrier();
                }

                if (performMapping)
                {
                    var mapping = _loadPortProxy.OpenDoorAndMapCarrier();
                    SetMapping(mapping);
                }
                else
                {
                    _loadPortProxy.OpenCarrierDoor();
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
                _loadPortProxy.CloseCarrierDoor();
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
                var mapping = _loadPortProxy.MapCarrier();
                SetMapping(mapping);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalReleaseCarrier()
        {
            try
            {
                if (IsDoorOpen)
                {
                    _loadPortProxy.CloseCarrierDoor();
                }

                if (IsDocked)
                {
                    _loadPortProxy.UndockCarrier();
                }

                if (IsClamped)
                {
                    _loadPortProxy.UnclampCarrier();
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetLight(LoadPortLightRoleType role, LightState lightState)
        {
            //Auto managed by Brooks
        }

        protected override void InternalSetDateAndTime()
        {
            try
            {
                _efemProxy.SetControllerLocalTime(DateTime.Now);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetAccessMode(LoadingType accessMode)
        {
            AccessMode = accessMode;
            if (accessMode == LoadingType.Manual)
            {
                try
                {
                    _efemProxy.SetAutoHandoffUnavailable(Configuration.BrooksLoadPortName);
                }
                catch (Exception e)
                {
                    MarkExecutionAsFailed(e);
                }
            }
        }

        protected override void InternalRequestLoad()
        {
            try
            {
                _efemProxy.SetAutoHandoffAvailable(Configuration.BrooksLoadPortName);
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
                _efemProxy.SetAutoHandoffAvailable(Configuration.BrooksLoadPortName);
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
                var carrierTypeFromConfig = Configuration.CarrierTypes.First(x => x.Id == carrierType);

                _efemProxy.SetActiveCarrierTypeOnHandoffPort(_loadPortProxy.DeviceName,carrierTypeFromConfig.Name);

                CarrierTypeNumber = carrierType;
                CarrierTypeName = carrierTypeFromConfig.Name;
                CarrierTypeDescription = carrierTypeFromConfig.Description;

                if (!string.IsNullOrWhiteSpace(carrierTypeFromConfig.Name)
                    && !AvailableCarrierTypes.Contains(carrierTypeFromConfig.Name))
                {
                    AvailableCarrierTypes.Add(carrierTypeFromConfig.Name);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalEnableE84()
        {
            InternalSetE84Timeouts(
                Configuration.E84Configuration.Tp1,
                Configuration.E84Configuration.Tp2,
                Configuration.E84Configuration.Tp3,
                Configuration.E84Configuration.Tp4,
                Configuration.E84Configuration.Tp5);
            InternalManageEsSignal(true);
        }

        protected override void InternalDisableE84()
        {
            try
            {
                InternalManageEsSignal(true);
                _loadPortProxy.SetAutoHandoffPassiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalPtoA.HO_AVBL,
                    false);
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
                _efemProxy.SetTpTimerSetpoint(
                    Configuration.BrooksLoadPortName,
                    Constants.TP1,
                    new IntWithUnit(tp1, Constants.SecondUnit));
                _efemProxy.SetTpTimerSetpoint(
                    Configuration.BrooksLoadPortName,
                    Constants.TP2,
                    new IntWithUnit(tp2, Constants.SecondUnit));
                _efemProxy.SetTpTimerSetpoint(
                    Configuration.BrooksLoadPortName,
                    Constants.TP3,
                    new IntWithUnit(tp3, Constants.SecondUnit));
                _efemProxy.SetTpTimerSetpoint(
                    Configuration.BrooksLoadPortName,
                    Constants.TP4,
                    new IntWithUnit(tp4, Constants.SecondUnit));
                _efemProxy.SetTpTimerSetpoint(
                    Configuration.BrooksLoadPortName,
                    Constants.TP5,
                    new IntWithUnit(tp5, Constants.SecondUnit));
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
                _loadPortProxy.SetAutoHandoffPassiveSignal(
                    AutoHandoffPio.Overhead,
                    AutoHandoffSignalPtoA.ES,
                    isActive);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        public override bool NeedsInitAfterE84Error()
        {
            return false;
        }

        #endregion

        #region IConfigurableDevice

        public new BrooksLoadPortConfiguration Configuration
            => ConfigurationExtension.Cast<BrooksLoadPortConfiguration>(base.Configuration);

        public BrooksLoadPortConfiguration CreateDefaultConfiguration()
        {
            return new BrooksLoadPortConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(LoadPort)}/{nameof(BrooksLoadPort)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region Overrides

        protected override void RequestCarrierIdFromBarcodeReader()
        {
            try
            {
                var carrierId = _efemProxy.ReadCarrierId(Configuration.BrooksLoadPortName);
                ApplyCarrierIdTreatment(carrierId);
            }
            catch (Exception)
            {
                OnCarrierIDChanged(
                    new CarrierIdChangedEventArgs(string.Empty, CommandStatusCode.Error));
            }
        }

        protected override void RequestCarrierIdFromTagReader()
        {
            try
            {
                var carrierId = _efemProxy.ReadCarrierId(Configuration.BrooksLoadPortName);
                ApplyCarrierIdTreatment(carrierId);
            }
            catch (Exception)
            {
                OnCarrierIDChanged(
                    new CarrierIdChangedEventArgs(string.Empty, CommandStatusCode.Error));
            }
        }

        #endregion

        #region Private Methods

        private void SetMapping(WaferPresenceAtHost[] mapping)
        {
            var slots = new List<SlotState>();
            foreach (var slot in mapping)
            {
                switch (slot.WaferPresence)
                {
                    case WaferPresenceState.Error:
                    case WaferPresenceState.Unknown:
                    case WaferPresenceState.Unavailable:
                    case WaferPresenceState.Tilt:
                    case WaferPresenceState.Shift:
                    case WaferPresenceState.TiltUp:
                    case WaferPresenceState.TiltDown:
                    case WaferPresenceState.WrongSize:
                        slots.Add(SlotState.Error);
                        break;
                    case WaferPresenceState.Absent:
                        slots.Add(SlotState.NoWafer);
                        break;
                    case WaferPresenceState.Present:
                        slots.Add(SlotState.HasWafer);
                        break;
                    case WaferPresenceState.Double:
                        slots.Add(SlotState.DoubleWafer);
                        break;
                    case WaferPresenceState.Cross:
                        slots.Add(SlotState.CrossWafer);
                        break;
                }
            }

            Carrier.SetSlotMap(slots, (byte)InstanceId, MaterialType.SiliconWithNotch);
        }

        private void UpdatePresenceStatus(PresenceState state)
        {
            switch (state)
            {
                case PresenceState.Error:
                case PresenceState.Unknown:
                case PresenceState.PodOnly:
                    CarrierPresence = CassettePresence.Unknown;
                    break;
                case PresenceState.Absent:
                    CarrierPresence = CassettePresence.Absent;
                    break;
                case PresenceState.Present:
                    CarrierPresence = CassettePresence.Correctly;
                    break;
            }
        }

        private LightState GetLightState(UiIndicatorState loadPortLight)
        {
            switch (loadPortLight)
            {
                case UiIndicatorState.Off:
                    return LightState.Off;
                case UiIndicatorState.On:
                    return LightState.On;
                case UiIndicatorState.Pulsating:
                    return LightState.Flashing;
                case UiIndicatorState.Strobe:
                    return LightState.FlashingFast;
            }

            return LightState.Undetermined;
        }

        private void CreateCarrier()
        {
            if (Carrier != null)
            {
                return;
            }

            UpdateCarrierType();
            var sampleDimension = GetSubstrateDimension();

            Carrier = new Carrier(
                string.Empty,
                (byte)_loadPortProxy.GetCarrierProperties().SlotCount,
                sampleDimension);
        }

        private void UpdateCarrierType()
        {
            if (!_efemProxy.ActiveCarrierTypeIsSetOnCarrierHandoffPort(_loadPortProxy.DeviceName))
            {
                var matchingCarrierTypes = _efemProxy.GetMatchingCarrierTypesOnHandoffPort(_loadPortProxy.DeviceName);
                if (matchingCarrierTypes.Length > 1)
                {
                    _efemProxy.SetActiveCarrierTypeOnHandoffPort(_loadPortProxy.DeviceName, matchingCarrierTypes.First()); // See what to do in this case   
                }
            }

            var carrierType = _efemProxy.GetActiveCarrierTypeOnHandoffPort(_loadPortProxy.DeviceName);
            var carrierTypeFromConfig = Configuration.CarrierTypes.FirstOrDefault(x => x.Name == carrierType);
            if (carrierTypeFromConfig == null)
            {
                return;
            }

            CarrierTypeName = carrierTypeFromConfig.Name;
            CarrierTypeNumber = carrierTypeFromConfig.Id;
            CarrierTypeDescription = carrierTypeFromConfig.Description ?? string.Empty;
        }

        private SampleDimension GetSubstrateDimension()
        {
            if (!_efemProxy.ActiveSubstrateTypeIsSetOnCarrierHandoffPort(_loadPortProxy.DeviceName))
            {
                var matchingSubstrateTypes = _efemProxy.GetMatchingSubstrateTypesOnHandoffPort(_loadPortProxy.DeviceName);
                if (matchingSubstrateTypes.Length > 1)
                {
                    _efemProxy.SetActiveSubstrateTypeOnHandoffPort(_loadPortProxy.DeviceName, matchingSubstrateTypes.First()); // See what to do in this case   
                }
            }

            var substrateType = _efemProxy.GetActiveSubstrateTypeOnHandoffPort(_loadPortProxy.DeviceName);
            var substrateInfo = _efemProxy.GetSubstrateTypeInfo(substrateType);
            return Helpers.Helper.ConvertDiameterInSampleDimension(substrateInfo.NominalDiameter.Number);
        }


        #endregion

        #region Event Handlers

        private void LoadPortProxy_CarrierDoorStateChanged(
            [BAI.Internal.DeviceName] string device,
            string before,
            string after)
        {
            IsDoorOpen = after.Equals(Constants.DoorOpen);
        }

        private void LoadPortProxy_CarrierPresenceStateChanged(
            [BAI.Internal.DeviceName] string device,
            PresenceState before,
            PresenceState after)
        {
            UpdatePresenceStatus(after);
        }

        private void LoadPortProxy_CarrierClampStateChanged(
            [BAI.Internal.DeviceName] string device,
            LockState before,
            LockState after)
        {
            switch (after)
            {
                case LockState.Locked:
                    IsClamped = true;
                    break;
                default:
                    IsClamped = false;
                    break;
            }
        }

        private void LoadPortProxy_AlarmGenerated(
            [BAI.Internal.DeviceName] string source,
            AlarmLevel level,
            string message)
        {
            if (_loadPortProxy.IsInMaintenance() || !_loadPortProxy.IsOperable())
            {
                Interrupt(InterruptionKind.Abort);
            }
        }

        private void LoadPortProxy_AutoHandoffPassiveSignalChanged(
            [BAI.Internal.DeviceName] string device,
            AutoHandoffPio pio,
            AutoHandoffSignalPtoA signal,
            bool after)
        {
            switch (signal)
            {
                case AutoHandoffSignalPtoA.L_REQ:
                    O_L_REQ = after;
                    break;
                case AutoHandoffSignalPtoA.U_REQ:
                    O_U_REQ = after;
                    break;
                case AutoHandoffSignalPtoA.READY:
                    O_READY = after;
                    break;
                case AutoHandoffSignalPtoA.HO_AVBL:
                    O_HO_AVBL = after;
                    break;
                case AutoHandoffSignalPtoA.ES:
                    O_ES = after;
                    break;
                case AutoHandoffSignalPtoA.VA:
                    break;
                case AutoHandoffSignalPtoA.VS_0:
                    break;
                case AutoHandoffSignalPtoA.VS_1:
                    break;
            }
        }

        private void LoadPortProxy_AutoHandoffActiveSignalChanged(
            [BAI.Internal.DeviceName] string device,
            AutoHandoffPio pio,
            AutoHandoffSignalAtoP signal,
            bool after)
        {
            switch (signal)
            {
                case AutoHandoffSignalAtoP.VALID:
                    I_VALID = after;
                    break;
                case AutoHandoffSignalAtoP.CS_0:
                    I_CS_0 = after;
                    break;
                case AutoHandoffSignalAtoP.CS_1:
                    I_CS_1 = after;
                    break;
                case AutoHandoffSignalAtoP.TR_REQ:
                    I_TR_REQ = after;
                    break;
                case AutoHandoffSignalAtoP.BUSY:
                    I_BUSY = after;
                    break;
                case AutoHandoffSignalAtoP.COMPT:
                    I_COMPT = after;
                    break;
                case AutoHandoffSignalAtoP.CONT:
                    I_CONT = after;
                    break;
                case AutoHandoffSignalAtoP.AM_AVBL:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(signal), signal, null);
            }
        }

        private void LoadPortProxy_CarrierPortLocationChanged(
            [BAI.Internal.DeviceName] string device,
            string before,
            string after)
        {
            IsDocked = after.Equals(Constants.LocationDocked);
            if (after == Constants.LocationDocked)
            {
                Carrier?.SetLocation(DockedLocation);
            }
            else if (after == Constants.LocationUnDocked)
            {
                Carrier?.SetLocation(UndockedLocation);
            }
        }

        private void EfemProxy_CarrierRemoved(
            string device,
            string location,
            PresenceState carrier,
            string data,
            string message)
        {
            Carrier = null;
        }

        private void EfemProxy_CarrierArrived(
            string device,
            string location,
            PresenceState carrier,
            string data,
            string message)
        {
            if (!device.Contains(Configuration.BrooksLoadPortName))
            {
                return;
            }

            CreateCarrier();
        }

        private void EfemProxy_AutoHandoffTimedoutOnTP(
            string device,
            AlarmLevel level,
            AutoHandoffPio pio,
            string timer,
            string span,
            string description,
            string message)
        {
            if (!device.Contains(Configuration.BrooksLoadPortName))
            {
                return;
            }

            switch (timer)
            {
                case Constants.TP1:
                    OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp1Timeout));
                    break;
                case Constants.TP2:
                    OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp2Timeout));
                    break;
                case Constants.TP3:
                    OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp3Timeout));
                    break;
                case Constants.TP4:
                    OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp4Timeout));
                    break;
                case Constants.TP5:
                    OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.Tp5Timeout));
                    break;
            }
        }

        private void EfemProxy_AutoHandoffActiveSignalUnexpected(
            string device,
            AlarmLevel level,
            AutoHandoffPio pio,
            AutoHandoffDevice signalSource,
            string signalName,
            bool stateAfter,
            string message)
        {
            if (!device.Contains(Configuration.BrooksLoadPortName))
            {
                return;
            }

            OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.SignalError));
        }

        private void EfemProxy_AutoHandoffActiveSignalLost(
            string device,
            AlarmLevel level,
            AutoHandoffPio pio,
            AutoHandoffDevice signalSource,
            string signalName,
            bool stateAfter,
            string message)
        {
            if (!device.Contains(Configuration.BrooksLoadPortName))
            {
                return;
            }

            OnE84ErrorOccurred(new E84ErrorOccurredEventArgs(E84Errors.SignalError));
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (_loadPortProxy != null)
                {
                    _loadPortProxy.CarrierClampStateChanged -=
                        LoadPortProxy_CarrierClampStateChanged;
                    _loadPortProxy.AutoHandoffActiveSignalChanged -=
                        LoadPortProxy_AutoHandoffActiveSignalChanged;
                    _loadPortProxy.AutoHandoffPassiveSignalChanged -=
                        LoadPortProxy_AutoHandoffPassiveSignalChanged;
                    _loadPortProxy.CarrierPresenceStateChanged -=
                        LoadPortProxy_CarrierPresenceStateChanged;
                    _loadPortProxy.CarrierPortLocationChanged -=
                        LoadPortProxy_CarrierPortLocationChanged;
                    _loadPortProxy.AlarmGenerated -= LoadPortProxy_AlarmGenerated;
                    _loadPortProxy.CarrierDoorStateChanged -= LoadPortProxy_CarrierDoorStateChanged;
                    _loadPortProxy.Dispose();
                }

                if (_efemProxy != null)
                {
                    _efemProxy.CarrierArrived -= EfemProxy_CarrierArrived;
                    _efemProxy.CarrierRemoved -= EfemProxy_CarrierRemoved;
                    _efemProxy.AutoHandoffTimedoutOnTP -= EfemProxy_AutoHandoffTimedoutOnTP;
                    _efemProxy.AutoHandoffActiveSignalLost -= EfemProxy_AutoHandoffActiveSignalLost;
                    _efemProxy.AutoHandoffActiveSignalUnexpected -=
                        EfemProxy_AutoHandoffActiveSignalUnexpected;
                    _efemProxy.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
