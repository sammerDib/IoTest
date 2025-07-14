using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Agileo.GUI.Commands;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts
{
    /// <summary>
    /// Interaction logic for E87LoadPortsViewerPanelView.xaml
    /// </summary>
    public partial class LoadPortsViewerPanelView
    {
        public LoadPortsViewerPanelView()
        {
            InitializeComponent();
            ScrollToRightCommand = new DelegateCommand(ScrollToRightExecute, ScrollToRightCanExecute);
            ScrollToLeftCommand = new DelegateCommand(ScrollToLeftExecute, ScrollToLeftCanExecute);
        }

        private bool ScrollToLeftCanExecute()
        {
            if (ScrollViewer == null) return false;
            return ScrollViewer.HorizontalOffset != 0;
        }

        private bool ScrollToRightCanExecute()
        {
            if (ScrollViewer == null) return false;
            return Math.Abs(ScrollViewer.HorizontalOffset - ScrollViewer.ScrollableWidth) > 0.1;
        }

        private void ScrollToRightExecute()
        {
            ScrollViewer.LineRight();
        }

        private void ScrollToLeftExecute()
        {
            ScrollViewer.LineLeft();
        }

        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register(
            nameof(ScrollViewer), typeof(ScrollViewer), typeof(LoadPortsViewerPanelView), new PropertyMetadata(default(ScrollViewer)));

        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        private void FrameworkElement_OnInitialized(object sender, EventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                ScrollViewer = scrollViewer;
            }
        }

        public static readonly DependencyProperty ScrollToRightCommandProperty = DependencyProperty.Register(
            nameof(ScrollToRightCommand), typeof(ICommand), typeof(LoadPortsViewerPanelView), new PropertyMetadata(default(ICommand)));

        public ICommand ScrollToRightCommand
        {
            get { return (ICommand)GetValue(ScrollToRightCommandProperty); }
            set { SetValue(ScrollToRightCommandProperty, value); }
        }

        public static readonly DependencyProperty ScrollToLeftCommandProperty = DependencyProperty.Register(
            nameof(ScrollToLeftCommand), typeof(ICommand), typeof(LoadPortsViewerPanelView), new PropertyMetadata(default(ICommand)));

        public ICommand ScrollToLeftCommand
        {
            get { return (ICommand)GetValue(ScrollToLeftCommandProperty); }
            set { SetValue(ScrollToLeftCommandProperty, value); }
        }
    }
}
