using System.Text;
using System.Windows;
using System.Windows.Controls;

using LightningChartLib.WPF.ChartingMVVM;
using LightningChartLib.WPF.ChartingMVVM.Annotations;
using LightningChartLib.WPF.ChartingMVVM.SeriesXY;
using LightningChartLib.WPF.ChartingMVVM.Views.ViewXY;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Controls
{
    /// <summary>
    /// Interaction logic for LiseHFChartControl.xaml
    /// </summary>
    public partial class LiseHFChartControl : UserControl
    {
        public LiseHFChartControl()
        {
            InitializeComponent();

            Unloaded += LiseHFChartControl_Unloaded;

            RawAcquisitionChart.SizeChanged += RawAcquisitionChart_SizeChanged;
            RawAcquisitionChart.ViewXY.Panned += ViewXY_Panned;
            RawAcquisitionChart.ViewXY.Zoomed += ViewXY_Zoomed;
            RawAcquisitionChart.ViewXY.PointLineSeries.Changed += PointLineSeries_Changed;
        }

        #region Event Methods
        private void LiseHFChartControl_Unloaded(object sender, RoutedEventArgs e)
        {
            RawAcquisitionChart.ViewXY.PointLineSeries.Changed -= PointLineSeries_Changed;
            RawAcquisitionChart.ViewXY.Panned -= ViewXY_Panned;
            RawAcquisitionChart.ViewXY.Zoomed -= ViewXY_Zoomed;
            RawAcquisitionChart.SizeChanged -= RawAcquisitionChart_SizeChanged;

            Unloaded -= LiseHFChartControl_Unloaded;
        }

        // when user zoom on sontrol

        private void ViewXY_Zoomed(object sender, ZoomedXYEventArgs e)
        {
            UpdateCursorsResult();
        }

        // when user pan on sontrol
        private void ViewXY_Panned(object sender, PannedXYEventArgs e)
        {
            UpdateCursorsResult();
        }

        //When size changed (resize window or ctrl)
        private void RawAcquisitionChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCursorsResult();
        }

        // when new data comes update current values of cursor
        private void PointLineSeries_Changed(object sender, System.EventArgs e)
        {
            UpdateCursorsResult();
        }
        #endregion

        #region Dependency Properties
        public double Threshold
        {
            get { return (double)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register(nameof(Threshold), typeof(double), typeof(LiseHFChartControl), new PropertyMetadata(0.0));

        public double BandBegin
        {
            get { return (double)GetValue(BandBeginProperty); }
            set { SetValue(BandBeginProperty, value); }
        }

        public static readonly DependencyProperty BandBeginProperty =
            DependencyProperty.Register(nameof(BandBegin), typeof(double), typeof(LiseHFChartControl), new PropertyMetadata(0.0));

        public double BandEnd
        {
            get { return (double)GetValue(BandEndProperty); }
            set { SetValue(BandEndProperty, value); }
        }

        public static readonly DependencyProperty BandEndProperty =
            DependencyProperty.Register(nameof(BandEnd), typeof(double), typeof(LiseHFChartControl), new PropertyMetadata(0.0));
        #endregion

        #region Tracked Cursors Properties & Methods 
        public double CursorPosition
        {
            get { return (double)GetValue(CursorPositionProperty); }
            set { SetValue(CursorPositionProperty, value); }
        }

        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register(nameof(CursorPosition), typeof(double), typeof(LiseHFChartControl), new PropertyMetadata(-0.1, CursorPositionChanged));

        public double CursorPosition2
        {
            get { return (double)GetValue(CursorPosition2Property); }
            set { SetValue(CursorPosition2Property, value); }
        }

        public static readonly DependencyProperty CursorPosition2Property =
            DependencyProperty.Register(nameof(CursorPosition2), typeof(double), typeof(LiseHFChartControl), new PropertyMetadata(-1.0, CursorPositionChanged));

        private static void CursorPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LiseHFChartControl)?.UpdateCursorsResult();
        }

        private void UpdateCursorsResult()
        {
            RawAcquisitionChart.BeginUpdate();

            bool showNextToCursor = true;
            // get cursor 1 ref and its annotation displays
            LineSeriesCursor cursor1 = RawAcquisitionChart.ViewXY.LineSeriesCursors[0];
            AnnotationXY cursorValueDisplay1 = RawAcquisitionChart.ViewXY.Annotations[0];

            //Get cursor2 target nd its annotation
            LineSeriesCursor cursor2 = RawAcquisitionChart.ViewXY.LineSeriesCursors[1];
            AnnotationXY cursorValueDisplay2 = RawAcquisitionChart.ViewXY.Annotations[1];

            //Get annotation Delta
            AnnotationXY cursorValueDisplayDelta = RawAcquisitionChart.ViewXY.Annotations[2];

            //Set annotation target. The location is relative to target. 
            float fTargetYCoordTop = (float)RawAcquisitionChart.ViewXY.GetMarginsRect().Top;
            float fTargetYCoordBottom = (float)RawAcquisitionChart.ViewXY.GetMarginsRect().Bottom;
            RawAcquisitionChart.ViewXY.YAxes[0].CoordToValue(fTargetYCoordTop, out double yTop);
            RawAcquisitionChart.ViewXY.YAxes[0].CoordToValue(fTargetYCoordBottom, out double yBot);

            double valueCursorX2 = cursor2.ValueAtXAxis;
            cursorValueDisplay2.TargetAxisValues.X = valueCursorX2;
            cursorValueDisplay2.TargetAxisValues.Y = yTop;

            double valueCursorX1 = cursor1.ValueAtXAxis;
            cursorValueDisplay1.TargetAxisValues.X = valueCursorX1;
            cursorValueDisplay1.TargetAxisValues.Y = yBot;

            double seriesYValue1;
            double seriesYValue2;
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sbDelta = new StringBuilder();

            string channelStringFormat = "{0:F3} : {1:F3}";
            string channelDeltaStringFormat = "Delta : {0:F3} : {1:F3}";
            bool labelVisible1 = false;
            bool labelVisible2 = false;

            var pointLineSeries = RawAcquisitionChart.ViewXY.PointLineSeries;

            foreach (PointLineSeries series in pointLineSeries)
            {
                //show series titles and cursor2 values in them, on the right side of the chart, 
                //if cursor2 values are not shown next to the cursor2 in an annotation
                bool resolvedOK = false;
                bool resolvedOK2 = false;
                
                string value = string.Empty;

                resolvedOK = SolveValueAccurate(series, valueCursorX1, out seriesYValue1);
                resolvedOK2 = SolveValueAccurate(series, valueCursorX2, out seriesYValue2);

                if (resolvedOK)
                {
                    labelVisible1 = true;
                    value = string.Format(channelStringFormat, valueCursorX1, seriesYValue1);
                }
                else
                {
                    value = string.Format(channelStringFormat, "-", "-");
                }

                if (sb1.Length != 0)
                    sb1.AppendLine();
                sb1.Append(value);

                if (resolvedOK2)
                {
                    labelVisible2 = true;
                    value = string.Format(channelStringFormat, valueCursorX2, seriesYValue2);
                }
                else
                {
                    value = string.Format(channelStringFormat, "-", "-");
                }
                if (sb2.Length != 0)
                    sb2.AppendLine();
                sb2.Append(value);


                if (resolvedOK && resolvedOK2)
                    value = string.Format(channelDeltaStringFormat, valueCursorX2 - valueCursorX1, seriesYValue2 - seriesYValue1);
                else
                    value = string.Empty;

                if (sbDelta.Length != 0)
                    sbDelta.AppendLine();
                sbDelta.Append(value);

            }

            //Set text
            cursorValueDisplay1.Text = sb1.ToString();
            //Show the label only if it selected to be shown
            cursorValueDisplay1.Visible = labelVisible1 && showNextToCursor;

            //Set text
            cursorValueDisplay2.Text = sb2.ToString();
            //Show the label only if it selected to be shown
            cursorValueDisplay2.Visible = labelVisible2 && showNextToCursor;

            cursorValueDisplayDelta.Text = sbDelta.ToString();
            cursorValueDisplayDelta.Visible = sbDelta.Length != 0;

            RawAcquisitionChart.EndUpdate();
        }

        private bool SolveValueAccurate(PointLineSeries series, double xValue, out double yValue)
        {
            yValue = 0;

            var result = series.SolveYValueAtXValue(xValue);
            if (result.SolveStatus == LineSeriesSolveStatus.OK)
            {
                //PointLineSeries may have two or more points at same X value. If so, center it between min and max 
                yValue = (result.YMax + result.YMin) * 0.5;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Commands
        private AutoRelayCommand _zoomToFitCommand;

        public AutoRelayCommand ZoomToFitCommand
        {
            get
            {
                return _zoomToFitCommand ?? (_zoomToFitCommand = new AutoRelayCommand(
              () =>
              {
                  RawAcquisitionChart?.ViewXY.ZoomToFit();
                 
                  // Reset Cursors Position
                  CursorPosition2 = -1.0;
                  CursorPosition = -0.5;

                  if (RawAcquisitionChart != null)
                  {
                      // hide annotation for a better look
                      RawAcquisitionChart.ViewXY.Annotations[0].Visible = false;
                      RawAcquisitionChart.ViewXY.Annotations[1].Visible = false;
                      RawAcquisitionChart.ViewXY.Annotations[2].Visible = false;
                  }
              },
              () => { return true; }));
            }
        }
        #endregion

    }
}
