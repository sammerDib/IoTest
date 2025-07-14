using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort.Helpers;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort
{
    public partial class LayingPlanLoadPort : IConfigurableDevice<LoadPortConfiguration>
    {
        #region Fields

        private ILayingPlanLoadPortIos _ioModule;
        private const int _waferProtrusionErrorCode = 1154;

        private TaskCompletionSource<bool> _tcsMapping;

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            AccessMode = LoadingType.Manual;
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
                        var ioModule = this.GetTopDeviceContainer()
                            .AllDevices()
                            .FirstOrDefault(d => d is ILayingPlanLoadPortIos);
                        if (ioModule is not ILayingPlanLoadPortIos layingPlanLoadPortIos)
                        {
                            throw new InvalidOperationException(
                                $"Mandatory device of type {nameof(ILayingPlanLoadPortIos)} is not found in equipment model tree.");
                        }

                        _ioModule = layingPlanLoadPortIos;
                        _ioModule.StatusValueChanged += IoModule_StatusValueChanged;
                    }
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region Overrides

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            //Used to unlock mapping command if in progress
            _tcsMapping?.TrySetResult(false);

            base.InternalInterrupt(interruption, interruptedExecution);
        }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            base.InternalInitialize(mustForceInit);
            IsDoorOpen = false;
            IsDocked = false;
            IsClamped = false;
            MappingRequested = false;
            CarrierTypeNumber = 0;
            CarrierTypeName = UnknownCarrierName;
            CarrierTypeDescription = UnknownCarrierName;
            if (Carrier != null)
            {
                //Simulate that carrier has been unloaded and new mapping is required
                PhysicalState = LoadPortState.Unclamped;
                Carrier = new Carrier(Carrier.Name, Carrier.Capacity, Carrier.SampleSize);
            }
            CheckWaferProtrudeError();
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            if (!_ioModule.IsCommunicationStarted)
            {
                _ioModule.StartCommunication();
            }
        }

        protected override void InternalStopCommunication()
        {
            _ioModule.StopCommunication();
        }

        #endregion ICommunicatingDevice Commands

        #region ILoadPort Commands

        protected override void InternalGetStatuses()
        {
            //Do nothing in this load port
        }

        protected override void InternalClamp()
        {
            IsClamped = true;
        }

        protected override void InternalUnclamp()
        {
            IsClamped = false;
        }

        protected override void InternalDock()
        {
            IsClamped = true;
            IsDocked = true;
        }

        protected override void InternalUndock()
        {
            IsDoorOpen = false;
            IsDocked = false;
            IsClamped = false;
        }

        protected override void InternalOpen(bool performMapping)
        {
            IsClamped = true;
            IsDocked = true;
            IsDoorOpen = true;
            if (performMapping)
            {
                InternalMap();
            }
        }

        protected override void InternalClose()
        {
            IsDoorOpen = false;
        }

        protected override void InternalMap()
        {
            try
            {
                _tcsMapping = new TaskCompletionSource<bool>();
                MappingRequested = true;
                _tcsMapping.Task.Wait();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            finally
            {
                MappingRequested = false;
                CheckWaferProtrudeError();
            }
        }

        protected override void InternalReleaseCarrier()
        {
            IsDoorOpen = false;
            IsDocked = false;
            IsClamped = false;
        }

        protected override void InternalSetLight(LoadPortLightRoleType role, LightState lightState)
        {
            //Do nothing, no light to handle
        }

        protected override void InternalSetDateAndTime()
        {
            //Do nothing, not available with this load port
        }

        protected override void InternalSetAccessMode(LoadingType accessMode)
        {
            if (accessMode != LoadingType.Manual)
            {
                throw new InvalidOperationException("E84 not supported in this load port");
            }
        }

        protected override void InternalRequestLoad()
        {
            throw new InvalidOperationException("E84 not supported in this load port");
        }

        protected override void InternalRequestUnload()
        {
            throw new InvalidOperationException("E84 not supported in this load port");
        }

        protected override void InternalSetCarrierType(uint carrierType)
        {
            CarrierTypeIndex = carrierType;
        }

        protected override void InternalEnableE84()
        {
            throw new InvalidOperationException("E84 not supported in this load port");
        }

        protected override void InternalDisableE84()
        {
            //Do nothing in case of this load port
        }

        protected override void InternalSetE84Timeouts(int tp1, int tp2, int tp3, int tp4, int tp5)
        {
            throw new InvalidOperationException("E84 not supported in this load port");
        }

        protected override void InternalManageEsSignal(bool isActive)
        {
            throw new InvalidOperationException("E84 not supported in this load port");
        }

        public override bool NeedsInitAfterE84Error()
        {
            return false;
        }

        protected override void RequestCarrierIdFromBarcodeReader()
        {
            throw new InvalidOperationException("Code reader not supported");
        }

        protected override void RequestCarrierIdFromTagReader()
        {
            throw new InvalidOperationException("Tag reader not supported");
        }

        #endregion

        #region Public Methods

        public void SetMapping(Collection<RR75xSlotState> slotsState)
        {
            Carrier.SetSlotMap(
                slotsState.Select(Converters.ToAbstractionSlotState).ToList(),
                (byte)InstanceId,
                MaterialType.SiliconWithNotch);
            _tcsMapping?.TrySetResult(true);
        }

        public void CheckWaferProtrudeError()
        {
            //Check wafer protrusion and raise alarm if detected
            //Otherwise clear the alarm
            if (Carrier != null
                && ((Carrier.SampleSize == SampleDimension.S200mm && WaferProtrudeSensor3)
                    || (Carrier.SampleSize == SampleDimension.S150mm
                        && WaferProtrudeSensor2
                        && WaferProtrudeSensor3)
                    || (Carrier.SampleSize == SampleDimension.S100mm
                        && WaferProtrudeSensor1
                        && WaferProtrudeSensor2
                        && WaferProtrudeSensor3)))
            {
                SetAlarmById(_waferProtrusionErrorCode.ToString());
            }
            else
            {
                ClearAlarmById(_waferProtrusionErrorCode.ToString());
            }
        }

        #endregion

        #region Private Methods

        private void UpdateStatuses()
        {
            if (PlacementSensorC && PlacementSensorD)
            {
                //8" Carrier
                CarrierPresence = CassettePresence.Correctly;
                if (Carrier == null)
                {
                    Carrier = new Carrier(string.Empty, 25, SampleDimension.S200mm);
                }
            }
            else if (PlacementSensorA && PlacementSensorC)
            {
                //6" Carrier
                CarrierPresence = CassettePresence.Correctly;
                if (Carrier == null)
                {
                    Carrier = new Carrier(string.Empty, 25, SampleDimension.S150mm);
                }
            }
            else if (PlacementSensorB && PlacementSensorC)
            {
                //4" Carrier
                CarrierPresence = CassettePresence.Correctly;
                if (Carrier == null)
                {
                    Carrier = new Carrier(string.Empty, 25, SampleDimension.S100mm);
                }
            }
            else if (PlacementSensorA || PlacementSensorB || PlacementSensorC || PlacementSensorD)
            {
                //Incorrectly placed carrier
                CarrierPresence = CassettePresence.NoPresentPlacement;
            }
            else
            {
                //No carrier
                CarrierPresence = CassettePresence.Absent;
                if (Carrier != null)
                {
                    Carrier = null;
                    Logger.Info("Carrier Disposed");
                }
            }
        }

        private void UpdateState()
        {
            if (State is OperatingModes.Maintenance or OperatingModes.Idle)
            {
                SetState(
                    !IsCommunicating || _ioModule.State == OperatingModes.Maintenance
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
        }

        #endregion

        #region IConfigurableDevice

        public LoadPortConfiguration CreateDefaultConfiguration()
        {
            return new LoadPortConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(LoadPort)}/{nameof(LayingPlanLoadPort)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region Event Handlers

        private void IoModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                #region Load Port 1

                case nameof(ILayingPlanLoadPortIos.PlacementSensorALoadPort1):
                    if (InstanceId != 0 && InstanceId != 1)
                    {
                        return;
                    }

                    PlacementSensorA = _ioModule.PlacementSensorALoadPort1;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorBLoadPort1):
                    if (InstanceId != 0 && InstanceId != 1)
                    {
                        return;
                    }

                    PlacementSensorB = _ioModule.PlacementSensorBLoadPort1;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorCLoadPort1):
                    if (InstanceId != 0 && InstanceId != 1)
                    {
                        return;
                    }

                    PlacementSensorC = _ioModule.PlacementSensorCLoadPort1;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorDLoadPort1):
                    if (InstanceId != 0 && InstanceId != 1)
                    {
                        return;
                    }

                    PlacementSensorD = _ioModule.PlacementSensorDLoadPort1;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor1LoadPort1):
                    if (InstanceId != 0 && InstanceId != 1)
                    {
                        return;
                    }

                    WaferProtrudeSensor1 = _ioModule.WaferProtrudeSensor1LoadPort1;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor2LoadPort1):
                    if (InstanceId != 0 && InstanceId != 1)
                    {
                        return;
                    }

                    WaferProtrudeSensor2 = _ioModule.WaferProtrudeSensor2LoadPort1;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor3LoadPort1):
                    if (InstanceId != 0 && InstanceId != 1)
                    {
                        return;
                    }

                    WaferProtrudeSensor3 = _ioModule.WaferProtrudeSensor3LoadPort1;
                    break;

                #endregion

                #region Load Port 2

                case nameof(ILayingPlanLoadPortIos.PlacementSensorALoadPort2):
                    if (InstanceId != 2)
                    {
                        return;
                    }

                    PlacementSensorA = _ioModule.PlacementSensorALoadPort2;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorBLoadPort2):
                    if (InstanceId != 2)
                    {
                        return;
                    }

                    PlacementSensorB = _ioModule.PlacementSensorBLoadPort2;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorCLoadPort2):
                    if (InstanceId != 2)
                    {
                        return;
                    }

                    PlacementSensorC = _ioModule.PlacementSensorCLoadPort2;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorDLoadPort2):
                    if (InstanceId != 2)
                    {
                        return;
                    }

                    PlacementSensorD = _ioModule.PlacementSensorDLoadPort2;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor1LoadPort2):
                    if (InstanceId != 2)
                    {
                        return;
                    }

                    WaferProtrudeSensor1 = _ioModule.WaferProtrudeSensor1LoadPort2;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor2LoadPort2):
                    if (InstanceId != 2)
                    {
                        return;
                    }

                    WaferProtrudeSensor2 = _ioModule.WaferProtrudeSensor2LoadPort2;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor3LoadPort2):
                    if (InstanceId != 2)
                    {
                        return;
                    }

                    WaferProtrudeSensor3 = _ioModule.WaferProtrudeSensor3LoadPort2;
                    break;

                #endregion

                #region Load Port 3

                case nameof(ILayingPlanLoadPortIos.PlacementSensorALoadPort3):
                    if (InstanceId != 3)
                    {
                        return;
                    }

                    PlacementSensorA = _ioModule.PlacementSensorALoadPort3;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorBLoadPort3):
                    if (InstanceId != 3)
                    {
                        return;
                    }

                    PlacementSensorB = _ioModule.PlacementSensorBLoadPort3;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorCLoadPort3):
                    if (InstanceId != 3)
                    {
                        return;
                    }

                    PlacementSensorC = _ioModule.PlacementSensorCLoadPort3;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorDLoadPort3):
                    if (InstanceId != 3)
                    {
                        return;
                    }

                    PlacementSensorD = _ioModule.PlacementSensorDLoadPort3;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor1LoadPort3):
                    if (InstanceId != 3)
                    {
                        return;
                    }

                    WaferProtrudeSensor1 = _ioModule.WaferProtrudeSensor1LoadPort3;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor2LoadPort3):
                    if (InstanceId != 3)
                    {
                        return;
                    }

                    WaferProtrudeSensor2 = _ioModule.WaferProtrudeSensor2LoadPort3;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor3LoadPort3):
                    if (InstanceId != 3)
                    {
                        return;
                    }

                    WaferProtrudeSensor3 = _ioModule.WaferProtrudeSensor3LoadPort3;
                    break;

                #endregion

                #region Load Port 4

                case nameof(ILayingPlanLoadPortIos.PlacementSensorALoadPort4):
                    if (InstanceId != 4)
                    {
                        return;
                    }

                    PlacementSensorA = _ioModule.PlacementSensorALoadPort4;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorBLoadPort4):
                    if (InstanceId != 4)
                    {
                        return;
                    }

                    PlacementSensorB = _ioModule.PlacementSensorBLoadPort4;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorCLoadPort4):
                    if (InstanceId != 4)
                    {
                        return;
                    }

                    PlacementSensorC = _ioModule.PlacementSensorCLoadPort4;
                    break;
                case nameof(ILayingPlanLoadPortIos.PlacementSensorDLoadPort4):
                    if (InstanceId != 4)
                    {
                        return;
                    }

                    PlacementSensorD = _ioModule.PlacementSensorDLoadPort4;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor1LoadPort4):
                    if (InstanceId != 4)
                    {
                        return;
                    }

                    WaferProtrudeSensor1 = _ioModule.WaferProtrudeSensor1LoadPort4;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor2LoadPort4):
                    if (InstanceId != 4)
                    {
                        return;
                    }

                    WaferProtrudeSensor2 = _ioModule.WaferProtrudeSensor2LoadPort4;
                    break;
                case nameof(ILayingPlanLoadPortIos.WaferProtrudeSensor3LoadPort4):
                    if (InstanceId != 4)
                    {
                        return;
                    }

                    WaferProtrudeSensor3 = _ioModule.WaferProtrudeSensor3LoadPort4;
                    break;

                #endregion

                case nameof(ILayingPlanLoadPortIos.IsCommunicating):
                    IsCommunicating = _ioModule.IsCommunicating;
                    UpdateState();
                    break;
                case nameof(ILayingPlanLoadPortIos.IsCommunicationStarted):
                    IsCommunicationStarted = _ioModule.IsCommunicationStarted;
                    break;
                case nameof(ILayingPlanLoadPort.State):
                    UpdateState();
                    break;
            }

            UpdateStatuses();
        }

        #endregion Event Handlers

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing && _ioModule != null)
            {
                _ioModule.StatusValueChanged -= IoModule_StatusValueChanged;
                _ioModule = null;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
