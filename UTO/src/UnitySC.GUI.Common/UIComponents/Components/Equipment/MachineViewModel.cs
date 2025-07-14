using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Aligner;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.LoadPort;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.ProcessModule;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Robot;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment
{
    public sealed class MachineViewModel : Notifier, IDisposable
    {
        #region Fields

        private readonly BusinessPanel _owner;

        #endregion

        #region Properties

        public List<SelectableMachineModuleViewModel> Modules { get; } = [];

        public EfemEquipmentManager EfemEquipmentManager { get; }

        private IMaterialLocationContainer _selectedSource;

        public IMaterialLocationContainer SelectedSource
        {
            get => _selectedSource;
            private set => SetAndRaiseIfChanged(ref _selectedSource, value);
        }

        private byte _selectedSlotSource;

        public byte SelectedSlotSource
        {
            get => _selectedSlotSource;
            private set => SetAndRaiseIfChanged(ref _selectedSlotSource, value);
        }

        private IMaterialLocationContainer _selectedDestination;

        public IMaterialLocationContainer SelectedDestination
        {
            get => _selectedDestination;
            private set => SetAndRaiseIfChanged(ref _selectedDestination, value);
        }

        private byte _selectedSlotDestination;

        public byte SelectedSlotDestination
        {
            get => _selectedSlotDestination;
            private set => SetAndRaiseIfChanged(ref _selectedSlotDestination, value);
        }

        public RobotModuleViewModel RobotModule { get; }

        #endregion

        #region Events

        public event EventHandler SourceDestinationChanged;

        #endregion

        #region Constructors

        public MachineViewModel(EfemEquipmentManager efemEquipmentManager, UnityDeviceUiManagerService factory, BusinessPanel owner)
        {
            _owner = owner;
            EfemEquipmentManager = efemEquipmentManager;

            foreach (var device in EfemEquipmentManager.Equipment.AllDevices())
            {
                var module = factory.GetModuleViewModel(device);
                switch (module)
                {
                    case null:
                        continue;
                    case RobotModuleViewModel robotModule:
                        RobotModule = robotModule;
                        continue;
                    case SelectableMachineModuleViewModel selectableMachine:
                        var pos = EfemEquipmentManager.Efem.GetPosition(device);
                        selectableMachine.Position = pos.Item1;
                        selectableMachine.Order = pos.Item2;
                        selectableMachine.ToggleSelectionStateRequested += OnModuleToggleSelectionStateRequested;
                        Modules.Add(selectableMachine);
                        continue;
                    default:
                        throw new InvalidOperationException($"Unexpected module type received from factory {nameof(UnityDeviceUiFactory)}: {module.GetType().Name} (only robot is allowed to not inherit from {nameof(SelectableMachineModuleViewModel)})");
                }
            }

            EfemEquipmentManager.Robot.CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;
        }

        #endregion

        #region Select source destination

        private void ToggleSelectionState(SelectableMachineModuleViewModel module)
        {
            switch (module)
            {
                case ProcessModuleViewModel processModule:
                    if (processModule.Module is { IsOutOfService: false })
                    {
                        InternalToggleSelectionState(processModule.Module.Location.Substrate == null);
                    }
                    break;
                case LoadPortModuleViewModel loadPort:
                    if (loadPort.SelectionState is SelectionState.Source or SelectionState.Destination)
                    {
                        InternalToggleSelectionState(loadPort.SelectionState == SelectionState.Destination);
                        loadPort.SelectedSlotNumber = 0;
                    }
                    else
                    {
                        SelectLoadPort(loadPort);
                    }
                    break;
                case AlignerModuleViewModel aligner:
                    InternalToggleSelectionState(aligner.Module.Location.Substrate == null);
                    break;
            }

            UpdateSelectionStates();

            void InternalToggleSelectionState(bool isDestination)
            {
                if (isDestination)
                {
                    SelectedDestination = ReferenceEquals(SelectedDestination, module.MaterialLocation) ? null : module.MaterialLocation;
                    SelectedSlotDestination = 1;
                }
                else
                {
                    SelectedSource = ReferenceEquals(SelectedSource, module.MaterialLocation) ? null : module.MaterialLocation;
                    SelectedSlotSource = 1;
                }
            }
        }

        #endregion

        #region Event handler

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (e.NewState != ExecutionState.Success)
            {
                return;
            }

            switch (e.Execution.Context.Command.Name)
            {
                case nameof(IRobot.Pick):
                    DeselectSource();
                    break;

                case nameof(IRobot.Place):
                    DeselectDestination();
                    break;
            }
        }

        private void OnModuleToggleSelectionStateRequested(object sender, EventArgs e)
        {
            if (sender is SelectableMachineModuleViewModel module)
            {
                ToggleSelectionState(module);
            }
        }

        #endregion

        #region Private methods

        private void DeselectSource()
        {
            SelectedSource = null;
            SelectedSlotSource = 1;

            UpdateSelectionStates();
        }

        private void DeselectDestination()
        {
            SelectedDestination = null;
            SelectedSlotDestination = 1;

            UpdateSelectionStates();
        }

        private void UpdateSelectionStates()
        {
            foreach (var module in Modules)
            {
                if (ReferenceEquals(SelectedDestination, module.MaterialLocation))
                {
                    module.SelectionState = SelectionState.Destination;
                }
                else if (ReferenceEquals(SelectedSource, module.MaterialLocation))
                {
                    module.SelectionState = SelectionState.Source;
                }
                else
                {
                    module.SelectionState = SelectionState.NotSelected;
                }
            }

            RaiseSourceDestinationChanged();
        }

        private void SelectLoadPort(LoadPortModuleViewModel viewModel)
        {
            var loadPort = viewModel.Module;
            if (loadPort?.Carrier == null)
            {
                return;
            }

            if (!loadPort.IsInService)
            {
                return;
            }

            var indexSlots = new ObservableCollection<IndexedSlotState>();
            if (loadPort.Carrier?.MappingTable != null)
            {
                for (var i = loadPort.Carrier.MappingTable.Count - 1; i >= 0; i--)
                {
                    Substrate substrate = null;
                    if (loadPort.Carrier.MaterialLocations[i].Material is Substrate s)
                    {
                        substrate = s;
                    }

                    indexSlots.Add(
                        new IndexedSlotState(
                            loadPort.Carrier.MappingTable[i],
                            substrate,
                            i + 1));
                }
            }

            var idPopup = new SelectSlotPopupViewModel(indexSlots);
            var popup = new Agileo.GUI.Services.Popups.Popup(loadPort.Name)
            {
                Content = idPopup,
                Commands =
                {
                    new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)),
                    new PopupCommand(
                        LocalizationManager.GetString(nameof(EquipmentResources.EQUIPMENT_SELECT)),
                        new DelegateCommand(
                            () => SelectLoadPortSlot(idPopup.SelectedSlot, viewModel),
                            () => !idPopup.HasErrors && idPopup.SelectedSlot != null))
                }

            };

            _owner.Popups.Show(popup);
        }

        private void SelectLoadPortSlot(IndexedSlotState selectedSlot, LoadPortModuleViewModel loadPort)
        {
            // No slot is selected -> do nothing
            if (selectedSlot == null)
            {
                return;
            }

            switch (selectedSlot.State)
            {
                case SlotState.HasWafer:
                    {
                        SelectedSource = loadPort.Module;
                        SelectedSlotSource = (byte)selectedSlot.Index;
                        break;
                    }
                case SlotState.NoWafer:
                    {
                        SelectedDestination = loadPort.Module;
                        SelectedSlotDestination = (byte)selectedSlot.Index;
                        break;
                    }
                default:
                    return;
            }

            loadPort.SelectedSlotNumber = selectedSlot.Index;
            
            UpdateSelectionStates();
        }

        private void RaiseSourceDestinationChanged() => SourceDestinationChanged?.Invoke(this, EventArgs.Empty);

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var module in Modules)
            {
                module.ToggleSelectionStateRequested -= OnModuleToggleSelectionStateRequested;
            }

            EfemEquipmentManager.Robot.CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;
        }

        #endregion
    }
}
