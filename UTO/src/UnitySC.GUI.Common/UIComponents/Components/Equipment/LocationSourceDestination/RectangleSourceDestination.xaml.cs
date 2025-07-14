using System.ComponentModel;
using System.Windows;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment.LocationSourceDestination
{
    /// <summary>
    /// Interaction logic for RectangleSourceDestination.xaml
    /// </summary>
    public partial class RectangleSourceDestination
    {
        public RectangleSourceDestination()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectionStateProperty = DependencyProperty.Register(
            nameof(SelectionState),
            typeof(SelectionState),
            typeof(RectangleSourceDestination),
            new PropertyMetadata(SelectionState.NotSelected));

        [Category("Main")]
        public SelectionState SelectionState
        {
            get => (SelectionState)GetValue(SelectionStateProperty);
            set => SetValue(SelectionStateProperty, value);
        }
    }
}
