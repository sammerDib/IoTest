using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.Data.ColorMap;

namespace UnitySC.Shared.UI.Controls
{
    /// <summary>
    /// Interaction logic for ColorMapSelector.xaml
    /// </summary>
    public partial class ColorMapSelector : UserControl
    {
        public ColorMapSelector()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeColorMapToggleButton.IsChecked = false;
        }

        public static readonly DependencyProperty SelectedColorMapProperty = DependencyProperty.Register(
            nameof(SelectedColorMap), typeof(ColorMap), typeof(ColorMapSelector), new PropertyMetadata(default(ColorMap)));

        public ColorMap SelectedColorMap
        {
            get { return (ColorMap)GetValue(SelectedColorMapProperty); }
            set { SetValue(SelectedColorMapProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation), typeof(Orientation), typeof(ColorMapSelector), new PropertyMetadata(Orientation.Vertical));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
    }
}
