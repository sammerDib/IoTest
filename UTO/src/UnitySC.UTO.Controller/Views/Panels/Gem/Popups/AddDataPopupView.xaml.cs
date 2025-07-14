using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Popups
{
    /// <summary>
    /// Logique d'interaction pour AddPauseEventPopup.xaml
    /// </summary>
    public partial class AddDataPopupView
    {
        public AddDataPopupView()
        {
            InitializeComponent();
        }

        #region Event

        [Category("Behavior")]
        public event SelectionChangedEventHandler SelectionChanged;

        #endregion

        #region Methods

        #region Select all

        private SafeDelegateCommand _selectAllCommand;

        public SafeDelegateCommand SelectAllCommand
            => _selectAllCommand ??= new SafeDelegateCommand(SelectAllCommandExecute, () => true);

        private void SelectAllCommandExecute()
        {
            ItemList.SelectAll();
        }

        #endregion

        #region Deselect all

        private SafeDelegateCommand _deselectAllCommand;

        public SafeDelegateCommand DeselectAllCommand
            => _deselectAllCommand ??=
                new SafeDelegateCommand(DeselectAllCommandExecute, () => true);

        private void DeselectAllCommandExecute()
        {
            ItemList.UnselectAll();
        }

        #endregion

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView {DataContext: AddDataPopupViewModel popModel})
            {
                popModel.SelectedData.Clear();
                popModel.SelectedData.AddRange(ItemList.SelectedItems.OfType<string>());
                SelectionChanged?.Invoke(this, e);
            }
        }

        #endregion
    }
}
