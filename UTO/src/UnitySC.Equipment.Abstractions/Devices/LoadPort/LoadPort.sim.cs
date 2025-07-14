using System;
using System.ComponentModel;
using System.Linq;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    public partial class LoadPort : ISimDevice
    {
        #region ISimDevice

        /// <inheritdoc />
        public ISimDeviceView SimDeviceView
        {
            get
            {
                LoadPortViewModel ??= new LoadPortSimulationViewModel(this);
                return new LoadPortSimulationView(LoadPortViewModel);
            }
        }

        #endregion ISimDevice

        #region Simulation Data

        public LoadPortSimulationData SimulationData { get; private set; }

        protected LoadPortSimulationViewModel LoadPortViewModel;

        #endregion Simulation Data

        #region Commands

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            if (State == OperatingModes.Idle && !mustForceInit)
            {
                return;
            }

            base.InternalSimulateInitialize(mustForceInit, tempomat);
            IsDoorOpen = false;
            IsDocked = false;
            IsClamped = false;
            UpdatePhysicalState();
            LoadPortViewModel.LoadPortControlViewModel.PropertyChanged +=
                LoadPortControlViewModel_PropertyChanged;
        }

        protected virtual void InternalSimulateGetStatuses(Tempomat tempomat)
        {
            //Do nothing
        }

        protected virtual void InternalSimulateClamp(Tempomat tempomat)
        {
            if (IsClamped)
            {
                return;
            }

            tempomat.Sleep(Duration.FromMilliseconds(200));
            IsClamped = true;
            UpdatePhysicalState();
        }

        protected virtual void InternalSimulateUnclamp(Tempomat tempomat)
        {
            if (!IsClamped)
            {
                return;
            }

            if (IsDocked)
            {
                InternalSimulateUndock(tempomat);
            }

            tempomat.Sleep(Duration.FromMilliseconds(200));
            IsClamped = false;
            UpdatePhysicalState();
        }

        protected virtual void InternalSimulateDock(Tempomat tempomat)
        {
            if (IsDocked)
            {
                return;
            }

            if (!IsClamped)
            {
                InternalSimulateClamp(tempomat);
            }

            tempomat.Sleep(Duration.FromMilliseconds(500));
            IsDocked = true;
            UpdatePhysicalState();
        }

        protected virtual void InternalSimulateUndock(Tempomat tempomat)
        {
            if (!IsDocked)
            {
                return;
            }

            if (IsDoorOpen)
            {
                InternalSimulateClose(tempomat);
            }

            tempomat.Sleep(Duration.FromMilliseconds(500));
            IsDocked = false;
            UpdatePhysicalState();
        }

        protected virtual void InternalSimulateOpen(bool performMapping, Tempomat tempomat)
        {
            if (IsDoorOpen)
            {
                return;
            }

            if (!IsDocked)
            {
                InternalSimulateDock(tempomat);
            }

            tempomat.Sleep(Duration.FromMilliseconds(3000));
            IsDoorOpen = true;
            UpdatePhysicalState();
            if (performMapping)
            {
                Carrier.SetSlotMap(
                    SimulationData.Mapping,
                    (byte)InstanceId,
                    MaterialType.SiliconWithNotch);
            }
        }

        protected virtual void InternalSimulateClose(Tempomat tempomat)
        {
            if (!IsDoorOpen)
            {
                return;
            }

            tempomat.Sleep(Duration.FromMilliseconds(2000));
            IsDoorOpen = false;
            UpdatePhysicalState();
        }

        protected virtual void InternalSimulateMap(Tempomat tempomat)
        {
            if (IsDoorOpen)
            {
                // Close door
                InternalSimulateClose(tempomat);
            }

            InternalSimulateOpen(true, tempomat);
            Carrier.SetSlotMap(
                SimulationData.Mapping,
                (byte)InstanceId,
                MaterialType.SiliconWithNotch);
        }

        protected virtual void InternalSimulateReadCarrierId(Tempomat tempomat)
        {
            try
            {
                switch (Configuration.CarrierIdentificationConfig.CarrierIdAcquisition)
                {
                    case CarrierIDAcquisitionType.Generate:
                        var isGenerationSucceeded = true;
                        if (string.IsNullOrWhiteSpace(Carrier.Id))
                        {
                            isGenerationSucceeded = GenerateCarrierId();
                        }
                        else
                        {
                            SetCarrierId(Carrier.Id);
                        }

                        if (!isGenerationSucceeded)
                        {
                            MarkExecutionAsFailed("CarrierID generation failed.");
                        }

                        break;
                    case CarrierIDAcquisitionType.TagReader:
                        if (SimulationData.IsCommandExecutionFailed)
                        {
                            MarkExecutionAsFailed("Command simulation failed.");
                            return;
                        }

                        if (Carrier == null || CarrierPresence != CassettePresence.Correctly)
                        {
                            MarkExecutionAsFailed("No correctly placed carrier detected.");
                            return;
                        }

                        if (SimulationData.IsTagReadEnabled)
                        {
                            tempomat.Sleep(Duration.FromMilliseconds(500));
                        }
                        else
                        {
                            MarkExecutionAsFailed("Function set as disable was requested.");
                            return;
                        }

                        if (SimulationData.IsReadWriteFailed)
                        {
                            OnCarrierIDChanged(
                                new CarrierIdChangedEventArgs(
                                    null,
                                    CommandStatusCode.Error,
                                    "Carrier ID read failed"));
                            return;
                        }

                        ApplyCarrierIdTreatment(SimulationData.CarrierIdRead);
                        break;
                    case CarrierIDAcquisitionType.EnterByOperator:
                        OnCarrierIdRequestedFromOperator();
                        break;
                }
            }
            catch
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

        protected virtual void InternalSimulateReleaseCarrier(Tempomat tempomat)
            => InternalSimulateUnclamp(tempomat);

        protected virtual void InternalSimulateSetLight(
            LoadPortLightRoleType role,
            LightState lightState,
            Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromMilliseconds(200));
            switch (role)
            {
                case LoadPortLightRoleType.LoadReady:
                    LoadLightState = lightState;
                    break;
                case LoadPortLightRoleType.UnloadReady:
                    UnloadLightState = lightState;
                    break;
                case LoadPortLightRoleType.AccessModeManual:
                    ManualModeLightState = lightState;
                    break;
                case LoadPortLightRoleType.AccessModeAuto:
                    AutoModeLightState = lightState;
                    break;
                case LoadPortLightRoleType.Reserve:
                    ReservedLightState = lightState;
                    break;
                case LoadPortLightRoleType.Alarm:
                    ErrorLightState = lightState;
                    break;
                case LoadPortLightRoleType.HandOffButton:
                    HandOffLightState = lightState;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }

        protected virtual void InternalSimulateSetDateAndTime(Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateEnableE84(Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateDisableE84(Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateSetE84Timeouts(
            int tp1,
            int tp2,
            int tp3,
            int tp4,
            int tp5,
            Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateManageEsSignal(bool isActive, Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateSetAccessMode(
            LoadingType accessMode,
            Tempomat tempomat)
        {
            AccessMode = accessMode;
            InternalSimulateSetLight(
                LoadPortLightRoleType.AccessModeManual,
                accessMode == LoadingType.Manual
                    ? LightState.On
                    : LightState.Off,
                tempomat);
        }

        protected virtual void InternalSimulateRequestLoad(Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateRequestUnload(Tempomat tempomat)
            => tempomat.Sleep(Duration.FromSeconds(2));

        protected virtual void InternalSimulateSetCarrierType(uint carrierType, Tempomat tempomat)
        {
            var configCarrierType =
                Configuration.CarrierTypes.FirstOrDefault(c => c.Id == carrierType);
            if (configCarrierType != null && !string.IsNullOrWhiteSpace(configCarrierType.Name))
            {
                CarrierTypeNumber = carrierType;
                CarrierTypeName = configCarrierType.Name;
                CarrierTypeDescription = configCarrierType.Description;
            }
            else
            {
                CarrierTypeNumber = 0;
                CarrierTypeName = UnknownCarrierName;
                CarrierTypeDescription = UnknownCarrierName;
            }
        }

        protected virtual void InternalSimulatePrepareForTransfer(Tempomat tempomat)
        {
            if (Configuration.CloseDoorAfterRobotAction && !IsDoorOpen)
            {
                InternalSimulateOpen(false,tempomat);
            }
        }

        protected virtual void InternalSimulatePostTransfer(Tempomat tempomat)
        {
            if (Configuration.CloseDoorAfterRobotAction && IsDoorOpen)
            {
                InternalSimulateClose(tempomat);
            }
        }

        #endregion

        #region Private Methods

        protected virtual void SetUpSimulatedMode()
        {
            SimulationData = new LoadPortSimulationData();
            SimulationData.IsTagReadEnabled = Configuration.IsCarrierIdSupported;
            AccessMode = Configuration.IsE84Enabled
                ? LoadingType.Auto
                : LoadingType.Manual;
        }

        protected virtual void DisposeSimulatedMode()
            => LoadPortViewModel.LoadPortControlViewModel.PropertyChanged -=
                LoadPortControlViewModel_PropertyChanged;

        protected virtual void UpdateLoadPortStatus(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(LoadPortSimulationViewModel.LoadPortControlViewModel.IsClamped):
                    IsClamped = LoadPortViewModel.LoadPortControlViewModel.IsClamped;
                    UpdatePhysicalState();
                    break;
                case nameof(LoadPortSimulationViewModel.LoadPortControlViewModel.IsDocked):
                    IsDocked = LoadPortViewModel.LoadPortControlViewModel.IsDocked;
                    UpdatePhysicalState();
                    break;
                case nameof(LoadPortSimulationViewModel.LoadPortControlViewModel.IsDoorClosed):
                    IsDoorOpen = !LoadPortViewModel.LoadPortControlViewModel.IsDoorClosed;
                    UpdatePhysicalState();
                    break;
                case nameof(LoadPortSimulationViewModel.LoadPortControlViewModel
                    .IsCarrierPlacementOk):
                case nameof(LoadPortSimulationViewModel.LoadPortControlViewModel.IsCarrierPresent):
                    if (LoadPortViewModel.LoadPortControlViewModel.IsCarrierPresent
                        && LoadPortViewModel.LoadPortControlViewModel.IsCarrierPlacementOk)
                    {
                        Carrier ??= new Carrier(
                            string.Empty,
                            (byte)SimulationData.Mapping.Count,
                            SimulationData.CarrierConfiguration.Type);
                    }

                    break;
            }
        }

        #endregion

        #region Event Handlers

        private void LoadPortControlViewModel_PropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
            => UpdateLoadPortStatus(e.PropertyName);

        #endregion
    }
}
