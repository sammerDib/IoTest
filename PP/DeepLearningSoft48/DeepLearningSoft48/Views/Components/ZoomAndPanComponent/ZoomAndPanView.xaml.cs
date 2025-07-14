using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DeepLearningSoft48.ViewModels;

namespace DeepLearningSoft48.Views.Components.ZoomAndPanComponent
{
    /// <summary>
    /// Interaction logic for ZoomAndPanView.xaml
    /// </summary>
    public partial class ZoomAndPanView : UserControl
    {

        /// <summary>
        /// Gets or sets additional content for the UserControl
        /// </summary>
        public object AdditionalContent
        {
            get { return (object)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }
        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(ZoomAndPanView), new PropertyMetadata(null));

        /// <summary>
        /// Rectangle visible, utilisé pour assurer que ce rectangle est bien visible
        /// </summary>
        public Rect VisibleRect
        {
            get { return (Rect)GetValue(VisibleRectProperty); }
            set { SetValue(VisibleRectProperty, value); }
        }

        public static readonly DependencyProperty VisibleRectProperty =
            DependencyProperty.Register("VisibleRect", typeof(Rect), typeof(ZoomAndPanView), new UIPropertyMetadata(Rect.Empty, OnVisibleRectChanged));

        private static void OnVisibleRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomAndPanView graphview = d as ZoomAndPanView;
            graphview.OnVisibleRectChanged(e);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ZoomAndPanView()
        {
            InitializeComponent();
        }

        private AnnotateWaferLayerViewModel viewModel
        {
            get
            {
                return DataContext as AnnotateWaferLayerViewModel;
            }
        }

        private void contentPresenter_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (viewModel != null)
            {
                viewModel.XCoordinate = (int)e.GetPosition(parentGrid).X;
                viewModel.YCoordinate = (int)e.GetPosition(parentGrid).Y;
            }
        }
    }
}
