using System.Windows;
using System.Windows.Controls;

using Agileo.Semi.Gem300.Abstractions.E87;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment
{
    /// <summary>
    /// Interaction logic for SlotMapView.xaml
    /// </summary>
    public partial class SlotMapView : UserControl
    {
        public SlotMapView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ShowBorderProperty = DependencyProperty.Register(
            nameof(ShowBorder), typeof(bool), typeof(SlotMapView), new PropertyMetadata(true));

        public bool ShowBorder
        {
            get => (bool)GetValue(ShowBorderProperty);
            set => SetValue(ShowBorderProperty, value);
        }

    }
}
