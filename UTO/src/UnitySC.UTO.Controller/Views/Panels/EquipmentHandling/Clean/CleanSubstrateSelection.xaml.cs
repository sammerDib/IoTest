using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.GUI.Common.Equipment.LoadPort;

namespace UnitySC.UTO.Controller.Views.Panels.EquipmentHandling.Clean
{
    /// <summary>
    /// Interaction logic for CleanSubstrateSelection.xaml
    /// </summary>
    public partial class CleanSubstrateSelection
    {
        public CleanSubstrateSelection()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SlotsProperty = DependencyProperty.Register(
            nameof(Slots),
            typeof(ObservableCollection<IndexedSlotState>),
            typeof(CleanSubstrateSelection),
            new PropertyMetadata(default(ObservableCollection<IndexedSlotState>)));

        public ObservableCollection<IndexedSlotState> Slots
        {
            get => (ObservableCollection<IndexedSlotState>)GetValue(SlotsProperty);
            set => SetValue(SlotsProperty, value);
        }

        public static readonly DependencyProperty LoadPortProperty = DependencyProperty.Register(
            nameof(LoadPort),
            typeof(LoadPort),
            typeof(CleanSubstrateSelection),
            new PropertyMetadata(default(LoadPort)));

        public LoadPort LoadPort
        {
            get => (LoadPort)GetValue(LoadPortProperty);
            set => SetValue(LoadPortProperty, value);
        }

        public static readonly DependencyProperty SelectedLoadPortProperty = DependencyProperty.Register(
            nameof(SelectedLoadPort),
            typeof(LoadPort),
            typeof(CleanSubstrateSelection),
            new PropertyMetadata(default(LoadPort)));

        public LoadPort SelectedLoadPort
        {
            get => (LoadPort)GetValue(SelectedLoadPortProperty);
            set => SetValue(SelectedLoadPortProperty, value);
        }

        public static readonly DependencyProperty SelectedSlotProperty = DependencyProperty.Register(
            nameof(SelectedSlot),
            typeof(IndexedSlotState),
            typeof(CleanSubstrateSelection),
            new PropertyMetadata(default(IndexedSlotState)));

        public IndexedSlotState SelectedSlot
        {
            get => (IndexedSlotState)GetValue(SelectedSlotProperty);
            set => SetValue(SelectedSlotProperty, value);
        }

        [Category("Behavior")]
        public event SelectionChangedEventHandler SelectionChanged;

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedLoadPort = LoadPort;
            SelectedSlot = SlotList.SelectedItems.OfType<IndexedSlotState>().FirstOrDefault();
            SelectionChanged?.Invoke(this, e);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var maxItemsCount = Slots != null ? Math.Max(Slots.Count, 25) : 25;
            ItemHeight = SlotListContainer is { ActualHeight: > 0 } ? (SlotListContainer.ActualHeight - 1) / maxItemsCount : 0;
        }

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            nameof(ItemHeight),
            typeof(double),
            typeof(CleanSubstrateSelection),
            new PropertyMetadata(default(double)));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
    }
}
