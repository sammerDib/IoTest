using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Popup
{
    /// <summary>
    /// Interaction logic for StartProcessJobPopupView.xaml
    /// </summary>
    public partial class LoadPmPopupView
    {
        public LoadPmPopupView()
        {
            InitializeComponent();
        }

        #region EventHandler

        private void LpSubstrateSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            foreach (var item in ItemsControl.Items)
            {
                if (ItemsControl.ItemContainerGenerator.ContainerFromItem(item) is ContentPresenter contentPresenter
                    && VisualTreeHelper.GetChild(contentPresenter,0) is LoadPmSubstrateSelectionView parentObject
                    && parentObject != sender)
                {
                    parentObject.DeselectAllCommand.Execute();
                }
            }
        }

        #endregion
    }
}
