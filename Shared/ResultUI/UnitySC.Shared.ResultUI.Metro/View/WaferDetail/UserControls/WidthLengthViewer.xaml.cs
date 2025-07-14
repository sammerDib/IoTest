using System.Windows;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.UserControls
{
    /// <summary>
    /// Interaction logic for WidthLengthViewer.xaml
    /// </summary>
    public partial class WidthLengthViewer
    {
        public WidthLengthViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty WidthValueProperty = DependencyProperty.Register(
            nameof(WidthValue), typeof(double), typeof(WidthLengthViewer), new PropertyMetadata(10.0));

        public double WidthValue
        {
            get { return (double)GetValue(WidthValueProperty); }
            set { SetValue(WidthValueProperty, value); }
        }

        public static readonly DependencyProperty LengthValueProperty = DependencyProperty.Register(
            nameof(LengthValue), typeof(double), typeof(WidthLengthViewer), new PropertyMetadata(10.0));

        public double LengthValue
        {
            get { return (double)GetValue(LengthValueProperty); }
            set { SetValue(LengthValueProperty, value); }
        }

        public static readonly DependencyProperty IsRectShapedProperty = DependencyProperty.Register(
            nameof(IsRectShaped), typeof(bool), typeof(WidthLengthViewer), new PropertyMetadata(false));

        public bool IsRectShaped
        {
            get { return (bool)GetValue(IsRectShapedProperty); }
            set { SetValue(IsRectShapedProperty, value); }
        }

        public static readonly DependencyProperty IsCircleShapedProperty = DependencyProperty.Register(
        nameof(IsCircleShaped), typeof(bool), typeof(WidthLengthViewer), new PropertyMetadata(false));

        public bool IsCircleShaped
        {
            get { return (bool)GetValue(IsCircleShapedProperty); }
            set { SetValue(IsCircleShapedProperty, value); }
        }
    }
}
