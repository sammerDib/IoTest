using System.Windows;
using System.Windows.Controls;

using Agileo.Semi.Gem300.Abstractions.E87;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Views
{
    /// <summary>
    /// Interaction logic for SlotMapView.xaml
    /// </summary>
    public partial class SimplifiedSlotMapView : UserControl
    {
        public SimplifiedSlotMapView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CarrierProperty = DependencyProperty.Register(
            nameof(Carrier), typeof(Carrier), typeof(SimplifiedSlotMapView), new PropertyMetadata(default(Carrier)));

        public Carrier Carrier
        {
            get { return (Carrier)GetValue(CarrierProperty); }
            set { SetValue(CarrierProperty, value); }
        }

        public string Substrates => ProductionEquipmentResources.EQUIPMENT_SUBSTRATES_SLOTMAP;
    }
}
