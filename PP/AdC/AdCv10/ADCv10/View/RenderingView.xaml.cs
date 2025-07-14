using System.Windows;
using System.Windows.Controls;

using ADC.ViewModel;

namespace ADC.View
{
    /// <summary>
    /// Interaction logic for Rendering.xaml
    /// </summary>
    public partial class RenderingView : Window
    {
        public RenderingView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool closed = ((RecipeViewModel)DataContext).RenderingView_Closing();
            e.Cancel = !closed;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
                overflowGrid.Visibility = Visibility.Collapsed;

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
                mainPanelBorder.Margin = new Thickness();
        }

    }
}
