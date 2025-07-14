using System.Windows.Controls;

namespace UnitySC.UTO.Controller.Views.Tools.TerminalMessages
{
    public partial class TerminalMessagesToolView
    {
        public TerminalMessagesToolView()
        {
            InitializeComponent();
        }

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer))
            {
                return;
            }

            ScrollViewer scrollViewer = (ScrollViewer)sender;

            if (e.ExtentHeightChange != 0)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }
    }
}
