using System.ComponentModel;
using System.Windows;

namespace UnitySC.GUI.Common.Equipment.LoadPort
{
    /// <summary>
    /// Logique d'interaction pour SimplifiedSlotMapView.xaml
    /// </summary>
    public partial class SimplifiedSlotMapView
    {
        public SimplifiedSlotMapView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight),
            typeof(double),
            typeof(SimplifiedSlotMapView),
            new PropertyMetadata(7.0));

        [Category("Main")]
        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

    }

}
