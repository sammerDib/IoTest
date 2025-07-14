using System.Linq;
using System.Windows.Controls;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.Carriers
{
    /// <summary>
    /// Logique d'interaction pour CarrierNotificationView.xaml
    /// </summary>
    public partial class CarrierEditionPopupView : UserControl
    {
        public CarrierEditionPopupView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is CarrierEditionPopup viewModel)
            {
                viewModel.UpdateSelectedSlot(SlotList.SelectedItems.OfType<Slot>());
            }
        }

    }
}
