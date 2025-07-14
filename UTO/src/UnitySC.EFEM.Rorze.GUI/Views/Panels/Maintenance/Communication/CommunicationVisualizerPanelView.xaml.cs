using System.Linq;
using System.Windows;
using System.Windows.Input;

using UnitySC.EFEM.Rorze.GUI.Views.Tools.StatusComparer;
using UnitySC.GUI.Common;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Maintenance.Communication
{
    /// <summary>
    /// Interaction logic for CommunicationVisualizerPanelView.xaml
    /// </summary>
    public partial class CommunicationVisualizerPanelView
    {
        private StatusComparer _statusComparer;

        public CommunicationVisualizerPanelView()
        {
            InitializeComponent();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CommunicationVisualizerPanel viewModel)
                || !(sender is FrameworkElement frameworkElement))
            {
                return;
            }

            if (frameworkElement.DataContext is CommunicationTrace communicationTrace)
                viewModel.CopyContentCommand.Execute(communicationTrace);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Shortcut is available only when Ctrl down to avoid bad manipulations
            if (!Keyboard.IsKeyDown(Key.LeftCtrl)
                || !(sender is FrameworkElement frameworkElement)
                || !(frameworkElement.DataContext is CommunicationTrace communicationTrace))
                return;

            if (_statusComparer == null)
                _statusComparer = App.Instance.UserInterface.ToolManager.Tools.FirstOrDefault(tool => tool is StatusComparer) as StatusComparer;
            if (_statusComparer == null)
                return;

            // Get status only from selected communication trace
            // Message with a status always follows the format "<cmdType><DeviceId>[.<DevicePart>].<Command>:<Status>"
            var parsedMessage = communicationTrace.Content.Split(':');
            if (parsedMessage.Length < 2)
                return;

            // Make the older status in first
            _statusComparer.Status1 = _statusComparer.Status2;
            _statusComparer.Status2 = parsedMessage[1];
        }
    }
}
