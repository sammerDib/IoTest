using System.Windows;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.UserControls
{
    /// <summary>
    /// Interaction logic for DepthViewer.xaml
    /// </summary>
    public partial class ScanDirectionViewer
    {
        public ScanDirectionViewer()
        {
            InitializeComponent();
        }

        #region Overrides of FrameworkElement

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            SetInterfaceAngle();
        }

        #endregion

        protected static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle), typeof(double), typeof(ScanDirectionViewer), new PropertyMetadata(default(double)));

        protected double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty TargetAngleProperty = DependencyProperty.Register(
            nameof(TargetAngle), typeof(double), typeof(ScanDirectionViewer), new PropertyMetadata(default(double), SetInterfaceAngle));

        public double TargetAngle
        {
            get { return (double)GetValue(TargetAngleProperty); }
            set { SetValue(TargetAngleProperty, value); }
        }

        private static void SetInterfaceAngle(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScanDirectionViewer self)
            {
                self.SetInterfaceAngle();
            }
        }

        private void SetInterfaceAngle()
        {
            Angle = TargetAngle - 90;
        }
    }
}
