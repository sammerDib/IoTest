using System;
using System.Windows;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.UserControls
{
    /// <summary>
    /// Interaction logic for BowViewer.xaml
    /// </summary>
    public partial class BowViewer
    {
        public BowViewer()
        {
            InitializeComponent();
            CalculateGridLength();
        }

        #region Properties

        public static readonly DependencyProperty BowValueProperty = DependencyProperty.Register(
            nameof(BowValue), typeof(double), typeof(BowViewer), new PropertyMetadata(default(double), CalculateGridLength));

        public double BowValue
        {
            get { return (double)GetValue(BowValueProperty); }
            set { SetValue(BowValueProperty, value); }
        }

        public static readonly DependencyProperty BowTargetMinValueProperty = DependencyProperty.Register(
            nameof(BowTargetMinValue), typeof(double), typeof(BowViewer), new PropertyMetadata(default(double), CalculateGridLength));

        public double BowTargetMinValue
        {
            get { return (double)GetValue(BowTargetMinValueProperty); }
            set { SetValue(BowTargetMinValueProperty, value); }
        }

        public static readonly DependencyProperty BowTargetMaxValueProperty = DependencyProperty.Register(
    nameof(BowTargetMaxValue), typeof(double), typeof(BowViewer), new PropertyMetadata(default(double), CalculateGridLength));

        public double BowTargetMaxValue
        {
            get { return (double)GetValue(BowTargetMaxValueProperty); }
            set { SetValue(BowTargetMaxValueProperty, value); }
        }

        public static readonly DependencyProperty BowCurveTopMarginProperty = DependencyProperty.Register(
            nameof(BowCurveTopMargin), typeof(string), typeof(BowViewer), new PropertyMetadata(default(string)));

        public string BowCurveTopMargin
        {
            get { return (string)GetValue(BowCurveTopMarginProperty); }
            set { SetValue(BowCurveTopMarginProperty, value); }
        }

        public static readonly DependencyProperty Point2ValueProperty = DependencyProperty.Register(
            nameof(Point2Value), typeof(string), typeof(BowViewer), new PropertyMetadata(default(string)));

        public string Point2Value
        {
            get { return (string)GetValue(Point2ValueProperty); }
            set { SetValue(Point2ValueProperty, value); }
        }

        public static readonly DependencyProperty BowTargetMinTopMarginProperty = DependencyProperty.Register(
            nameof(BowTargetMinTopMargin), typeof(string), typeof(BowViewer), new PropertyMetadata(default(string)));

        public string BowTargetMinTopMargin
        {
            get { return (string)GetValue(BowTargetMinTopMarginProperty); }
            set { SetValue(BowTargetMinTopMarginProperty, value); }
        }

        public static readonly DependencyProperty BowTargetMaxTopMarginProperty = DependencyProperty.Register(
    nameof(BowTargetMaxTopMargin), typeof(string), typeof(BowViewer), new PropertyMetadata(default(string)));

        public string BowTargetMaxTopMargin
        {
            get { return (string)GetValue(BowTargetMaxTopMarginProperty); }
            set { SetValue(BowTargetMaxTopMarginProperty, value); }
        }

        #endregion Properties

        #region Overrides of FrameworkElement

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CalculateGridLength();
        }

        #endregion Overrides of FrameworkElement

        private static void CalculateGridLength(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BowViewer self)
            {
                self.CalculateGridLength();
            }
        }

        private void CalculateGridLength()
        {
            Point2Value = "0,0";
            BowTargetMaxTopMargin = "0,0,0,0";
            BowTargetMinTopMargin = "0,0,0,0";
            BowCurveTopMargin = "0,0,0,0";

            if (BowValue < 0) // curve -> :)
            {
                Point2Value = "100, 100";

                if (Math.Truncate(BowValue) < Math.Truncate(BowTargetMinValue))
                {
                    BowTargetMinTopMargin = "0,-35,0,0";
                    BowCurveTopMargin = "0,0,0,0";
                }
                else if (Math.Truncate(BowValue) == Math.Truncate(BowTargetMinValue))
                {
                    BowTargetMinTopMargin = "0,0,0,0";
                    BowCurveTopMargin = "0,5,0,0";
                }
                else if (Math.Truncate(BowValue) > Math.Truncate(BowTargetMinValue))
                {
                    BowTargetMinTopMargin = "0,25,0,0";
                    BowCurveTopMargin = "0,0,0,0";
                }
            }
            else if (BowValue > 0) // curve -> :(
            {
                Point2Value = "100, -100";

                if (Math.Truncate(BowValue) < Math.Truncate(BowTargetMaxValue))
                {
                    BowTargetMaxTopMargin = "0,-5,0,0";
                    BowCurveTopMargin = "0,5,0,0";
                }
                else if (Math.Truncate(BowValue) == Math.Truncate(BowTargetMaxValue))
                {
                    BowTargetMaxTopMargin = "0,0,0,0";
                    BowCurveTopMargin = "0,0,0,0";
                }
                else if (Math.Truncate(BowValue) > Math.Truncate(BowTargetMaxValue))
                {
                    BowTargetMaxTopMargin = "0,25,0,0";
                    BowCurveTopMargin = "0,0,0,0";
                }
            }
        }
    }
}
