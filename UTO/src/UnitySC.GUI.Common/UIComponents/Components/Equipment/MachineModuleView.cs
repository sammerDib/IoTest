using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment
{
    public class MachineModuleView : UserControl
    {
        #region Dependency properties

        public static readonly DependencyProperty IsLocationSelectableProperty = DependencyProperty.Register(
            nameof(IsLocationSelectable),
            typeof(bool),
            typeof(MachineModuleView),
            new PropertyMetadata(true));

        /// <summary>
        /// Define if the UserControl is selectable
        /// </summary>
        [Category("Main")]
        public bool IsLocationSelectable
        {
            get => (bool)GetValue(IsLocationSelectableProperty);
            set => SetValue(IsLocationSelectableProperty, value);
        }

        public static readonly DependencyProperty SelectionStateProperty = DependencyProperty.Register(
            nameof(SelectionState),
            typeof(SelectionState),
            typeof(MachineModuleView),
            new PropertyMetadata(SelectionState.NotSelected));

        [Category("Main")]
        public SelectionState SelectionState
        {
            get => (SelectionState)GetValue(SelectionStateProperty);
            set => SetValue(SelectionStateProperty, value);
        }

        #endregion

        #region Constructor

        static MachineModuleView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MachineModuleView), new FrameworkPropertyMetadata(typeof(MachineModuleView)));
        }

        public MachineModuleView()
        {
            MouseUp += OnMouseUp;
        }

        #endregion

        #region Event handler

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsLocationSelectable)
            {
                return;
            }

            if (DataContext is SelectableMachineModuleViewModel viewModel)
            {
                viewModel.RaiseToggleSelectionStateRequested();
            }
        }

        #endregion
    }
}
