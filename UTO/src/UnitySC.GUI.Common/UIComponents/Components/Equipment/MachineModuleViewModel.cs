using System;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment
{
    public abstract class MachineModuleViewModel : Notifier
    {
        public abstract IMaterialLocationContainer MaterialLocation { get; }

        private protected MachineModuleViewModel()
        {
        }
    }

    public abstract class SelectableMachineModuleViewModel : MachineModuleViewModel
    {
        public event EventHandler ToggleSelectionStateRequested;

        public DevicePosition Position { get; set; }

        public int Order { get; set; }

        public int Index
        {
            get;
        }

        private SelectionState _selectionState;
        public SelectionState SelectionState
        {
            get => _selectionState;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectionState, value) &&
                    value == SelectionState.NotSelected)
                {
                    OnSelectionStateDeselected();
                }
            }
        }

        #region Constructor

        private protected SelectableMachineModuleViewModel(int index)
        {
            Index = index;
        }

        #endregion

        #region Virtual methods

        protected virtual void OnSelectionStateDeselected()
        {
        }

        #endregion

        public void RaiseToggleSelectionStateRequested()
        {
            ToggleSelectionStateRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    public abstract class SelectableMachineModuleViewModel<T> : SelectableMachineModuleViewModel where T : Device, IMaterialLocationContainer
    {
        public T Module { get; }

        public override IMaterialLocationContainer MaterialLocation => Module;

        #region Constructor

        private protected SelectableMachineModuleViewModel(T module) : base(module.InstanceId)
        {
            Module = module;
        }

        #endregion
    }
}
