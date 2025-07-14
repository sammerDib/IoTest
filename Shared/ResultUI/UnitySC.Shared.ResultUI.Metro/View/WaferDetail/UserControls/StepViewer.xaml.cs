using System.Windows;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.UserControls
{
    /// <summary>
    /// Interaction logic for DepthViewer.xaml
    /// </summary>
    public partial class StepViewer
    {
        public StepViewer()
        {
            InitializeComponent();
        }

        #region Overrides of FrameworkElement

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            SetHeight();
        }

        #endregion

        public static readonly DependencyProperty TargetValueProperty = DependencyProperty.Register(
            nameof(TargetValue), typeof(double), typeof(StepViewer), new PropertyMetadata(default(double), SetHeight));

        public double TargetValue
        {
            get { return (double)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }

        public static readonly DependencyProperty TargetHeightProperty = DependencyProperty.Register(
            nameof(TargetHeight), typeof(double), typeof(StepViewer), new PropertyMetadata(default(double)));

        public double TargetHeight
        {
            get { return (double)GetValue(TargetHeightProperty); }
            set { SetValue(TargetHeightProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            nameof(Target), typeof(double), typeof(StepViewer), new PropertyMetadata(default(double)));

        public double Target
        {
            get { return (double)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty IsStepUpProperty = DependencyProperty.Register(
            nameof(IsStepUp), typeof(bool), typeof(StepViewer), new PropertyMetadata(default(bool)));

        public bool IsStepUp
        {
            get { return (bool)GetValue(IsStepUpProperty); }
            set { SetValue(IsStepUpProperty, value); }
        }

        private static void SetHeight(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StepViewer self)
            {
                self.SetHeight();
            }
        }

        private void SetHeight()
        {
            if (Target < 2)
            {
                TargetHeight = TargetValue * 50;
            }
            else if (Target < 10)
            {
                TargetHeight = TargetValue * 5;
            }
            else if (Target > 100)
            {
                double divisor = (Target / 100) * 2;

                TargetHeight = TargetValue / divisor;
            }
            else
            {
                TargetHeight = TargetValue;
            }

            if (TargetHeight < 20)
            {
                TargetHeight = 20;
            }
        }
    }
}
