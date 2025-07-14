using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer
{
    /// <summary>
    /// Interaction logic for LogViewerView.xaml
    /// </summary>
    public partial class LogViewerView : UserControl
    {
        private LogViewerViewModel ViewModel => DataContext as LogViewerViewModel;

        public LogViewerView()
        {
            InitializeComponent();
            var backgroundRenderer = new HighlightCurrentLineBackgroundRenderer(LogViewer);
            LogViewer.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);
        }

        private void LogViewerView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (!handle) return;
            if (e.Delta > 0) viewModel.IncreaseFont();
            else viewModel.DecreaseFont();
            e.Handled = true;
        }
    }
}
