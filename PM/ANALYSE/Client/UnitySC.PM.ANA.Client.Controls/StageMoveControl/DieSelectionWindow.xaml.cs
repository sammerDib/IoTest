using System;
using System.Windows;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Controls.StageMoveControl
{
    public partial class DieSelectionWindow : Window
    {
        public DieSelectionWindow(WaferMapResult waferMap, WaferDimensionalCharacteristic waferDimentionalCharac)
        {
            InitializeComponent();
            this.DataContext = this;
            WaferMap = waferMap;
            WaferDimensions = waferDimentionalCharac;
        }

        public WaferDimensionalCharacteristic WaferDimensions { get; internal set; }
        public WaferMapResult WaferMap { get; internal set; }

        public int SelectedDieRow
        {
            get { return (int)GetValue(SelectedDieRowProperty); }
            set { SetValue(SelectedDieRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDieRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDieRowProperty =
            DependencyProperty.Register(nameof(SelectedDieRow), typeof(int), typeof(DieSelectionWindow), new PropertyMetadata(int.MaxValue, SelectedDieRowChanged));

        private static void SelectedDieRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dieSelectionWindow = d as DieSelectionWindow;
            int selectedRowFromDieReference = DieIndexConverter.ConvertRowFromDieReference(dieSelectionWindow.SelectedDieRow, dieSelectionWindow.WaferMap.DieReference);
            if ((dieSelectionWindow.SelectedDie is null) || (selectedRowFromDieReference != dieSelectionWindow.SelectedDie.Row))
                dieSelectionWindow.SelectedDie = new DieIndex(dieSelectionWindow.SelectedDie?.Column ?? 0, selectedRowFromDieReference);
        }

        public int SelectedDieColumn
        {
            get { return (int)GetValue(SelectedDieColumnProperty); }
            set { SetValue(SelectedDieColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDieRow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDieColumnProperty =
            DependencyProperty.Register(nameof(SelectedDieColumn), typeof(int), typeof(DieSelectionWindow), new PropertyMetadata(int.MaxValue, SelectedDieColumnChanged));

        private static void SelectedDieColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dieSelectionWindow = d as DieSelectionWindow;
            int selectedColumnFromDieReference = DieIndexConverter.ConvertColumnFromDieReference(dieSelectionWindow.SelectedDieColumn, dieSelectionWindow.WaferMap.DieReference);
            if ((dieSelectionWindow.SelectedDie is null) || (selectedColumnFromDieReference != dieSelectionWindow.SelectedDie.Column))
                dieSelectionWindow.SelectedDie = new DieIndex(selectedColumnFromDieReference, dieSelectionWindow.SelectedDie?.Row ?? 0);
        }

        // Real index not user
        public DieIndex SelectedDie
        {
            get { return (DieIndex)GetValue(SelectedDieProperty); }
            set { SetValue(SelectedDieProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDie.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDieProperty =
            DependencyProperty.Register(nameof(SelectedDie), typeof(DieIndex), typeof(DieSelectionWindow), new PropertyMetadata(null, SelectedDieChanged));

        private static void SelectedDieChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dieSelectionWindow = d as DieSelectionWindow;
            DieIndex selectedDieInDieReference = dieSelectionWindow.SelectedDie.ToDieReference(dieSelectionWindow.WaferMap.DieReference);
            if (dieSelectionWindow.SelectedDieColumn != selectedDieInDieReference.Column)
                dieSelectionWindow.SelectedDieColumn = selectedDieInDieReference.Column;
            if (dieSelectionWindow.SelectedDieRow != selectedDieInDieReference.Row)
                dieSelectionWindow.SelectedDieRow = selectedDieInDieReference.Row;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
