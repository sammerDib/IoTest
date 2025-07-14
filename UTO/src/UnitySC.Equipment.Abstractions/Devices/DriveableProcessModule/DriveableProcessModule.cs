using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.Resources;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.Shared.TC.Shared.Data;

using Alarm = Agileo.AlarmModeling.Alarm;
using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;

namespace UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule
{
    public partial class DriveableProcessModule
    {
        #region Properties

        public List<SampleDimension> SupportedSampleDimensions { get; protected set; } = new();

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
                    DeviceType.AddPrecondition(
                        nameof(IDriveableProcessModule.PrepareTransfer),
                        new IsNotOffline(),
                        Logger);

                    LoadConfiguration();
                    if (ExecutionMode == ExecutionMode.Simulated)
                    {
                        SetupSimulatedMode();
                    }

                    break;
                case SetupPhase.SettingUp:
                    Location.PropertyChanged += Location_PropertyChanged;
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

        #region Events

        public event EventHandler<EventArgs> ReadyToTransfer;

        protected void OnReadyToTransfer()
        {
            ReadyToTransfer?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EquipmentConstantChangedEventArgs> EquipmentConstantChanged;

        protected void OnEquipmentConstantChanged(List<EquipmentConstant> equipmentConstants)
        {
            EquipmentConstantChanged?.Invoke(
                this,
                new EquipmentConstantChangedEventArgs(equipmentConstants));
        }

        public event EventHandler<StatusVariableChangedEventArgs> StatusVariableChanged;

        protected virtual void OnStatusVariableChanged(List<StatusVariable> statusVariables)
        {
            StatusVariableChanged?.Invoke(
                this,
                new StatusVariableChangedEventArgs(statusVariables));
        }

        public event EventHandler<CollectionEventEventArgs> CollectionEventRaised;

        protected void OnCollectionEventRaised(CommonEvent commonEvent)
        {
            CollectionEventRaised?.Invoke(this, new CollectionEventEventArgs(commonEvent.Name, commonEvent.DataVariables));
        }

        public event EventHandler<EventArgs> SmokeDetected;

        protected void OnSmokeDetected()
        {
            SmokeDetected?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region IMaterialLocationContainer

        public override bool IsReadyForTransfer(
            EffectorType effector,
            out List<string> errorMessages,
            Agileo.EquipmentModeling.Material armMaterial = null,
            byte slot = 1)
        {
            errorMessages = new List<string>();

            // Check that slot exists
            if (slot != 1)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.ProcessModuleHaveOnlyOneSlot,
                        slot));
            }

            if (State != OperatingModes.Idle || ProcessModuleState != ProcessModuleState.Idle)
            {
                errorMessages.Add(Messages.NotIdle);
            }

            if (!IsDoorOpen)
            {
                errorMessages.Add(
                    string.Format(CultureInfo.InvariantCulture, Messages.DoorClosed, slot));
            }

            if (!IsReadyToLoadUnload)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.NotReadyToLoadUnload,
                        slot));
            }

            if (!TransferValidationState)
            {
                errorMessages.Add(Messages.TransfertNotValidated);
            }

            if (armMaterial != null)
            {
                if (armMaterial is not Substrate substrate)
                {
                    errorMessages.Add(Messages.MaterialNotSupported);
                }
                else
                {
                    if (!SupportedSampleDimensions.Contains(substrate.MaterialDimension))
                    {
                        errorMessages.Add(Messages.WaferSizeNotSupported);
                    }
                }
            }

            //Check wafer presence has no conflict
            if (SubstrateDetectionError)
            {
                errorMessages.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Messages.SubstrateDetectionError,
                        slot));
            }

            return errorMessages.Count == 0;
        }

        #endregion IMaterialLocationContainer

        #region Commands

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            if (ProcessModuleState is ProcessModuleState.Active)
            {
                AbortRecipe();
            }

            if (ProcessModuleState is ProcessModuleState.Error)
            {
                SetState(OperatingModes.Maintenance);
            }

            base.InternalInterrupt(interruption, interruptedExecution);
        }

        #region Material

        protected abstract void InternalPrepareTransfer(
            TransferType transferType,
            RobotArm arm,
            MaterialType materialType,
            SampleDimension dimension);

        protected abstract void InternalPostTransfer();

        protected abstract void InternalSelectRecipe(Wafer wafer);

        protected abstract void InternalStartRecipe();

        protected abstract void InternalAbortRecipe();

        protected abstract void InternalResetSmokeDetectorAlarm();

        #endregion

        #endregion

        #region Public Methods

        public abstract IEnumerable<Alarm> GetActiveAlarms();

        public abstract IEnumerable<EquipmentConstant> GetEquipmentConstants(List<int> ids);

        public abstract bool SetEquipmentConstant(EquipmentConstant equipmentConstant);

        public abstract IEnumerable<StatusVariable> GetStatusVariables(List<int> ids);

        public abstract IEnumerable<CommonEvent> GetCollectionEvents();

        public abstract double GetAlignmentAngle();

        public abstract string GetMessagesConfigurationPath(string path);

        #endregion

        #region Protected Methods

        protected abstract void UpdateSupportedWaferDimensions();

        #endregion

        #region Private Methods

        private Wafer _currentWafer;

        private void Location_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Location.Material))
            {
                return;
            }

            if (Location.Substrate != null)
            {
                WaferDimension = Location.Substrate.MaterialDimension;
                SimplifiedWaferId = Location.Substrate.SimplifiedName;
                WaferStatus = Location.Substrate.Status;
                _currentWafer = Location.Substrate as Wafer;

                try
                {
                    LoadMaterial(_currentWafer);
                }
                catch (Exception exception)
                {
                  Logger.Error(exception);
                }
               
                Location.Substrate.PropertyChanged += Substrate_PropertyChanged;
            }
            else
            {
                WaferDimension = SampleDimension.NoDimension;
                SimplifiedWaferId = string.Empty;
                WaferStatus = WaferStatus.None;

                try
                {
                    UnloadMaterial(_currentWafer);
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }
               
                _currentWafer = null;
            }
        }

        private void Substrate_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Substrate.Status))
            {
                WaferStatus = Location.Substrate?.Status ?? WaferStatus.None;
            }
        }

        protected void UpdateProcessModuleState(ProcessModuleState state)
        {
            PreviousProcessModuleState = ProcessModuleState;
            ProcessModuleState = state;
        }

        protected abstract void LoadMaterial(Wafer wafer);

        protected abstract void UnloadMaterial(Wafer wafer);

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ExecutionMode == ExecutionMode.Simulated)
                {
                    DisposeSimulatedMode();
                }

                Location.PropertyChanged -= Location_PropertyChanged;
                if (Location.Substrate != null)
                {
                    Location.Substrate.PropertyChanged -= Substrate_PropertyChanged;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
