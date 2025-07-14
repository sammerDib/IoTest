using System.Windows;

using Agileo.Semi.Gem300.Abstractions.E87;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Views
{
    /// <summary>
    /// Interaction logic for LoadPortViewerView.xaml
    /// </summary>
    public partial class SimplifiedLoadPortViewerView
    {
        public SimplifiedLoadPortViewerView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CarrierProperty = DependencyProperty.Register(
            "Carrier",
            typeof(Carrier),
            typeof(SimplifiedLoadPortViewerView),
            new PropertyMetadata(default(Carrier)));

        public Carrier Carrier
        {
            get => (Carrier)GetValue(CarrierProperty);
            set => SetValue(CarrierProperty, value);
        }
    }
}
