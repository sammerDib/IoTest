using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Appearance
{
    public partial class AppearancePanelView : UserControl
    {
        public AppearancePanelView()
        {
            InitializeComponent();
            DataContextChanged += AppearancePanelView_DataContextChanged;
        }

        private void AppearancePanelView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var model = DataContext as AppearancePanelViewModel;
            if (model != null)
            {
                model.PreviewElement = PreviewElement;
            }
        }
    }
}
