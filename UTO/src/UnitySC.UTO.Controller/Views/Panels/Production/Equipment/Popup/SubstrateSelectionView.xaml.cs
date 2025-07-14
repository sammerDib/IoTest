using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Popup
{
    /// <summary>
    /// Interaction logic for SubstrateSelectionView.xaml
    /// </summary>
    public partial class SubstrateSelectionView
    {
        public SubstrateSelectionView()
        {
            InitializeComponent();
        }

        #region Select all

        private SafeDelegateCommand _selectAllCommand;

        public SafeDelegateCommand SelectAllCommand
            => _selectAllCommand ??= new SafeDelegateCommand(SelectAllCommandExecute);

        private void SelectAllCommandExecute()
        {
            if (SlotList.ItemsSource.OfType<IndexedSlotState>().Any(i => i.State != SlotState.HasWafer)) 
            {
                SlotList.SelectedItems.Clear();
                foreach (var indexedSlotState in SlotList.ItemsSource.OfType<IndexedSlotState>().Where(i => i.State == SlotState.HasWafer).ToList())
                {
                    SlotList.SelectedItems.Add(indexedSlotState);
                }
            }
            else
            {
                SlotList.SelectAll();
            }
        }

        #endregion

        #region Deselect all

        private SafeDelegateCommand _deselectAllCommand;

        public SafeDelegateCommand DeselectAllCommand
            => _deselectAllCommand ??= new SafeDelegateCommand(DeselectAllCommandExecute);

        private void DeselectAllCommandExecute() => SlotList.UnselectAll();

        #endregion

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not SubstrateSelectionViewModel model)
            {
                return;
            }

            model.LpSelectedSlots.Clear();
            model.LpSelectedSlots.AddRange(SlotList.SelectedItems.OfType<IndexedSlotState>());
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var maxItemsCount = SlotList.ItemsSource?.OfType<IndexedSlotState>() != null ? Math.Max(SlotList.ItemsSource.OfType<IndexedSlotState>().Count(), 25) : 25;
            ItemHeight = SlotListContainer is { ActualHeight: > 0 } ? (SlotListContainer.ActualHeight - 1) / maxItemsCount : 0;
        }

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight),
            typeof(double),
            typeof(SubstrateSelectionView),
            new PropertyMetadata(default(double)));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
    }
}
