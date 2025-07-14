using System.ComponentModel;
using System.Windows;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment.LocationSourceDestination
{
    /// <summary>
    /// Interaction logic for LocationSourceDestination.xaml
    /// </summary>
    public partial class LocationSourceDestination
    {
        public LocationSourceDestination()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectionStateProperty = DependencyProperty.Register(
            nameof(SelectionState),
            typeof(SelectionState),
            typeof(LocationSourceDestination),
            new PropertyMetadata(SelectionState.NotSelected));

        [Category("Main")]
        public SelectionState SelectionState
        {
            get => (SelectionState)GetValue(SelectionStateProperty);
            set => SetValue(SelectionStateProperty, value);
        }

        public static readonly DependencyProperty IsSlotNumberDisplayedProperty = DependencyProperty.Register(
            nameof(IsSlotNumberDisplayed),
            typeof(bool),
            typeof(LocationSourceDestination),
            new PropertyMetadata(false));

        [Category("Main")]
        public bool IsSlotNumberDisplayed
        {
            get => (bool)GetValue(IsSlotNumberDisplayedProperty);
            set => SetValue(IsSlotNumberDisplayedProperty, value);
        }

        public static readonly DependencyProperty SlotNumberProperty = DependencyProperty.Register(
            nameof(SlotNumber),
            typeof(int),
            typeof(LocationSourceDestination),
            new PropertyMetadata(0));

        [Category("Main")]
        public int SlotNumber
        {
            get => (int)GetValue(SlotNumberProperty);
            set => SetValue(SlotNumberProperty, value);
        }
    }
}
