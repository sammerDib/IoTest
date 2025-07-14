using System.Windows;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Diagnostic.TraceViewer
{
    public partial class TraceViewerPanelView
    {
        public TraceViewerPanelView()
        {
            InitializeComponent();
        }

        private void OnUserControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FullScreenAttachmentWidth = e.NewSize.Width - ExpanderHeader.ActualWidth;
        }

        public static readonly DependencyProperty FullScreenAttachmentWidthProperty = DependencyProperty.Register(
            nameof(FullScreenAttachmentWidth),
            typeof(double),
            typeof(TraceViewerPanelView),
            new PropertyMetadata(default(double)));

        public double FullScreenAttachmentWidth
        {
            get { return (double)GetValue(FullScreenAttachmentWidthProperty); }
            set { SetValue(FullScreenAttachmentWidthProperty, value); }
        }
    }
}
