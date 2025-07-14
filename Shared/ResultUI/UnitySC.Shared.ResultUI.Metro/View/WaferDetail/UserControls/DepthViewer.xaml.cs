using System.Windows;

using UnitySC.Shared.UI.Controls;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.UserControls
{
    /// <summary>
    /// Interaction logic for DepthViewer.xaml
    /// </summary>
    public partial class DepthViewer
    {
        public DepthViewer()
        {
            InitializeComponent();
        }

        #region Overrides of FrameworkElement

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CalculateGridLength();
        }

        #endregion

        public static readonly DependencyProperty DepthValueProperty = DependencyProperty.Register(
            nameof(DepthValue), typeof(double), typeof(DepthViewer), new PropertyMetadata(default(double), CalculateGridLength));

        public double DepthValue
        {
            get { return (double)GetValue(DepthValueProperty); }
            set { SetValue(DepthValueProperty, value); }
        }

        public static readonly DependencyProperty TargetValueProperty = DependencyProperty.Register(
            nameof(TargetValue), typeof(double), typeof(DepthViewer), new PropertyMetadata(default(double), CalculateGridLength));

        public double TargetValue
        {
            get { return (double)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }

        private static void CalculateGridLength(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DepthViewer self)
            {
                self.CalculateGridLength();
            }
        }

        private void CalculateGridLength()
        {
            if (TargetValue <= 0) return;

            if (DepthValue < 0)
            {
                DepthSize = 0;
                TargetSize = ActualHeight;
            }
            else if (DepthValue > TargetValue)
            {
                DepthSize = ActualHeight;
                TargetSize = TargetValue / DepthValue * ActualHeight;
            }
            else
            {
                DepthSize = DepthValue / TargetValue * ActualHeight;
                TargetSize = ActualHeight;
            }
        }

        public static readonly DependencyProperty DepthSizeProperty = DependencyProperty.Register(
            nameof(DepthSize), typeof(double), typeof(DepthViewer), new PropertyMetadata(default(double)));

        public double DepthSize
        {
            get { return (double)GetValue(DepthSizeProperty); }
            set { SetValue(DepthSizeProperty, value); }
        }

        public static readonly DependencyProperty TargetSizeProperty = DependencyProperty.Register(
            nameof(TargetSize), typeof(double), typeof(DepthViewer), new PropertyMetadata(default(double)));

        public double TargetSize
        {
            get { return (double)GetValue(TargetSizeProperty); }
            set { SetValue(TargetSizeProperty, value); }
        }

        public static readonly DependencyProperty ToleranceProperty = DependencyProperty.Register(
            nameof(Tolerance), typeof(Tolerance), typeof(DepthViewer), new PropertyMetadata(default(Tolerance)));

        public Tolerance Tolerance
        {
            get { return (Tolerance)GetValue(ToleranceProperty); }
            set { SetValue(ToleranceProperty, value); }
        }
    }
}
