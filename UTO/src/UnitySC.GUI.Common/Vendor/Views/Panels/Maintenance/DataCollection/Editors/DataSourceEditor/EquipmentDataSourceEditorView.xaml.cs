using System.Windows;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Editors.DataSourceEditor
{
    /// <summary>
    /// Interaction logic for AddDcpSourcePopupView.xaml
    /// </summary>
    public partial class EquipmentDataSourceEditorView
    {
        public EquipmentDataSourceEditorView()
        {
            InitializeComponent();
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedDevice = e.NewValue as Device;
            if (selectedDevice == null) return;
            ((EquipmentDataSourceEditor)DataContext).SelectedDevice = selectedDevice;
        }

        /// <summary>
        /// Useful to keep treeview selected element details when popup is closed and re opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSourceEditorView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataContext = DataContext as EquipmentDataSourceEditor;

            if ((bool)e.NewValue && dataContext != null)
            {
                dataContext.SelectedDevice = EquipmentTreeView?.SelectedItem as Device;
            }
        }
    }
}
