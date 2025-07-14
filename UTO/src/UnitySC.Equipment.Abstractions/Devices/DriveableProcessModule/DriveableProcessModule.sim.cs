using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.Simulation;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Shared.Data.Enum;

using Timer = System.Timers.Timer;
using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;

namespace UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule
{
    public partial class DriveableProcessModule : ISimDevice
    {
        #region Fields

        protected internal DriveableProcessModuleSimulationData SimulationData { get; private set; }

        private Timer _recipeTimer;

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new DriveableProcessModuleSimulationView() { DataContext = SimulationData };

        #endregion

        #region Commands

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            UpdateProcessModuleState(ProcessModuleState.Initializing);
            tempomat.Sleep(Duration.FromSeconds(5));
            UpdateProcessModuleState(ProcessModuleState.Idle);
            if (WaferPresence == WaferPresence.Unknown)
            {
                WaferPresence = WaferPresence.Absent;
            }

            SimulationData.IsDoorOpen = false;
            SimulationData.IsReadyToLoadUnload = false;
            TransferState = WaferPresence == WaferPresence.Present
                ? EnumPMTransferState.ReadyToUnload_SlitDoorClosed
                : EnumPMTransferState.ReadyToLoad_SlitDoorClosed;
        }

        protected virtual void InternalSimulatePrepareTransfer(
            TransferType transferType,
            RobotArm arm,
            MaterialType materialType,
            SampleDimension dimension,
            Tempomat tempomat)
        {
            if (ProcessModuleState == ProcessModuleState.Idle)
            {
                UpdateProcessModuleState(ProcessModuleState.Active);
                TransferState = EnumPMTransferState.NotReady;
                tempomat.Sleep(Duration.FromMilliseconds(1000));
                SimulationData.IsDoorOpen = true;
                UpdateProcessModuleState(ProcessModuleState.Idle);
                SimulationData.IsReadyToLoadUnload = true;
                TransferState = WaferPresence == WaferPresence.Present
                    ? EnumPMTransferState.ReadyToUnload_SlitDoorOpened
                    : EnumPMTransferState.ReadyToLoad_SlitDoorOpened;
                Task.Factory.StartNew(
                    () =>
                    {
                        Thread.Sleep(1000);
                        OnReadyToTransfer();
                    });
            }
        }

        protected virtual void InternalSimulatePostTransfer(Tempomat tempomat)
        {
            WaferPresence = Location.Wafer != null
                ? WaferPresence.Present
                : WaferPresence.Absent;
            UpdateProcessModuleState(ProcessModuleState.Active);
            TransferState = EnumPMTransferState.NotReady;
            tempomat.Sleep(Duration.FromMilliseconds(1000));
            SimulationData.IsDoorOpen = false;
            SimulationData.IsReadyToLoadUnload = false;
            UpdateProcessModuleState(ProcessModuleState.Idle);
            TransferState = WaferPresence == WaferPresence.Present
                ? EnumPMTransferState.ReadyToUnload_SlitDoorClosed
                : EnumPMTransferState.ReadyToLoad_SlitDoorClosed;
        }

        protected virtual void InternalSimulateSelectRecipe(Wafer wafer, Tempomat tempomat)
        {
            SelectedRecipe = $"Recipe{InstanceId}";
            Logger.Debug($"Select recipe process module {InstanceId}");
        }

        protected virtual void InternalSimulateStartRecipe(Tempomat tempomat)
        {
            UpdateProcessModuleState(ProcessModuleState.Active);
            SelectedRecipe = $"Recipe{InstanceId}";
            Logger.Debug($"Start recipe process module {InstanceId}");
            _recipeTimer.Start();
        }

        protected virtual void InternalSimulateAbortRecipe(Tempomat tempomat)
        {
            _recipeTimer.Stop();
            UpdateProcessModuleState(ProcessModuleState.Error);
        }

        protected virtual void InternalSimulateResetSmokeDetectorAlarm(Tempomat tempomat)
        {
            //Do nothing
        }

        #endregion

        #region Private Methods

        protected virtual void SetupSimulatedMode()
        {
            SimulationData = new DriveableProcessModuleSimulationData(this);
            SimulationData.PropertyChanged += SimulationData_PropertyChanged;
            UpdateProcessModuleState(ProcessModuleState.Unknown);
            SimulationData.Is100mmSupported = true;
            SimulationData.Is150mmSupported = true;
            SimulationData.Is200mmSupported = true;
            SimulationData.Is300mmSupported = true;
            SimulationData.Is450mmSupported = true;
            SimulationData.IsDoorOpen = false;
            SimulationData.IsReadyToLoadUnload = false;
            SimulationData.TransferValidationState = true;
            _recipeTimer = new Timer(5000);
            _recipeTimer.Elapsed += Timer_Elapsed;
        }

        protected virtual void DisposeSimulatedMode()
        {
            if (SimulationData != null)
            {
                SimulationData.PropertyChanged -= SimulationData_PropertyChanged;
                SimulationData = null;
            }

            if (_recipeTimer != null)
            {
                _recipeTimer.Elapsed -= Timer_Elapsed;
                _recipeTimer.Stop();
                _recipeTimer.Dispose();
            }
        }

        #endregion

        #region Event Handlers

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _recipeTimer.Stop();
            UpdateProcessModuleState(ProcessModuleState.Idle);
            Logger.Debug($"Recipe finished on process module {InstanceId}");
            Task.Factory.StartNew(OnReadyToTransfer);
        }

        private void SimulationData_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SimulationData.Is100mmSupported):
                    UpdateSampleDimension(SimulationData.Is100mmSupported, SampleDimension.S100mm);
                    break;
                case nameof(SimulationData.Is150mmSupported):
                    UpdateSampleDimension(SimulationData.Is150mmSupported, SampleDimension.S150mm);
                    break;
                case nameof(SimulationData.Is200mmSupported):
                    UpdateSampleDimension(SimulationData.Is200mmSupported, SampleDimension.S200mm);
                    break;
                case nameof(SimulationData.Is300mmSupported):
                    UpdateSampleDimension(SimulationData.Is300mmSupported, SampleDimension.S300mm);
                    break;
                case nameof(SimulationData.Is450mmSupported):
                    UpdateSampleDimension(SimulationData.Is450mmSupported, SampleDimension.S450mm);
                    break;
                case nameof(SimulationData.IsDoorOpen):
                    IsDoorOpen = SimulationData.IsDoorOpen;
                    break;
                case nameof(SimulationData.IsReadyToLoadUnload):
                    IsReadyToLoadUnload = SimulationData.IsReadyToLoadUnload;
                    break;
                case nameof(SimulationData.TransferValidationState):
                    TransferValidationState = SimulationData.TransferValidationState;
                    break;
            }
        }

        #endregion

        #region private

        private void UpdateSampleDimension(bool isSupported, SampleDimension dimension)
        {
            if (isSupported)
            {
                if (!SupportedSampleDimensions.Contains(dimension))
                {
                    SupportedSampleDimensions.Add(dimension);
                }
            }
            else
            {
                if (SupportedSampleDimensions.Contains(dimension))
                {
                    SupportedSampleDimensions.Remove(dimension);
                }
            }
        }

        #endregion
    }
}
