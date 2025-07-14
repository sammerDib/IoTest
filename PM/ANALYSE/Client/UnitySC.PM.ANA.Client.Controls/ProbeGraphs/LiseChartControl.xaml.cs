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
    /// Interaction logic for LiseChartControl.xaml
    /// </summary>
    public partial class LiseChartControl : UserControl
    {
        public LiseChartControl()
        {
            InitializeComponent();

            Unloaded += LiseHFChartControl_Unloaded;

            RawAcquisitionChartUp.SizeChanged += RawAcquisitionChart_SizeChanged;
            RawAcquisitionChartUp.ViewXY.Panned += ViewXY_Panned;
            RawAcquisitionChartUp.ViewXY.Zoomed += ViewXY_Zoomed;
            RawAcquisitionChartUp.ViewXY.PointLineSeries.Changed += PointLineSeries_Changed;
        }

        #region Event Methods
        private void LiseHFChartControl_Unloaded(object sender, RoutedEventArgs e)
        {
            RawAcquisitionChartUp.ViewXY.PointLineSeries.Changed -= PointLineSeries_Changed;
            RawAcquisitionChartUp.ViewXY.Panned -= ViewXY_Panned;
            RawAcquisitionChartUp.ViewXY.Zoomed -= ViewXY_Zoomed;
            RawAcquisitionChartUp.SizeChanged -= RawAcquisitionChart_SizeChanged;

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

     
        #region Tracked Cursors Properties & Methods 
        public double CursorPosition
        {
            get { return (double)GetValue(CursorPositionProperty); }
            set { SetValue(CursorPositionProperty, value); }
        }

        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register(nameof(CursorPosition), typeof(double), typeof(LiseChartControl), new PropertyMetadata(-1.0, CursorPositionChanged));

        public double CursorPosition2
        {
            get { return (double)GetValue(CursorPosition2Property); }
            set { SetValue(CursorPosition2Property, value); }
        }

        public static readonly DependencyProperty CursorPosition2Property =
            DependencyProperty.Register(nameof(CursorPosition2), typeof(double), typeof(LiseChartControl), new PropertyMetadata(-10.0, CursorPositionChanged));

        private static void CursorPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LiseChartControl)?.UpdateCursorsResult();
        }

        private void UpdateCursorsResult()
        {
            RawAcquisitionChartUp.BeginUpdate();

            bool showNextToCursor = true;
            // get cursor 1 ref and its annotation displays
            LineSeriesCursor cursor1 = RawAcquisitionChartUp.ViewXY.LineSeriesCursors[0];
            AnnotationXY cursorValueDisplay1 = RawAcquisitionChartUp.ViewXY.Annotations[0];

            //Get cursor2 target nd its annotation
            LineSeriesCursor cursor2 = RawAcquisitionChartUp.ViewXY.LineSeriesCursors[1];
            AnnotationXY cursorValueDisplay2 = RawAcquisitionChartUp.ViewXY.Annotations[1];

            //Get annotation Delta
            AnnotationXY cursorValueDisplayDelta = RawAcquisitionChartUp.ViewXY.Annotations[2];

            //Set annotation target. The location is relative to target. 
            float fTargetYCoordTop = (float)RawAcquisitionChartUp.ViewXY.GetMarginsRect().Top;
            float fTargetYCoordBottom = (float)RawAcquisitionChartUp.ViewXY.GetMarginsRect().Bottom;
            RawAcquisitionChartUp.ViewXY.YAxes[0].CoordToValue(fTargetYCoordTop, out double yTop);
            RawAcquisitionChartUp.ViewXY.YAxes[0].CoordToValue(fTargetYCoordBottom, out double yBot);

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

            string channelStringFormat = "{0:F2} : {1:F1}";
            string channelDeltaStringFormat = "Delta : {0:F2} : {1:F1}";
            bool labelVisible1 = false;
            bool labelVisible2 = false;

            var pointLineSeries = RawAcquisitionChartUp.ViewXY.PointLineSeries;
            PointLineSeries series = pointLineSeries[0];
            if(series != null)
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

            RawAcquisitionChartUp.EndUpdate();
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
                  RawAcquisitionChartUp?.ViewXY.ZoomToFit();
                 
                  // Reset Cursors Position
                  CursorPosition2 = -10.0;
                  CursorPosition = -1.0;

                  if (RawAcquisitionChartUp != null)
                  {
                      // hide annotation for a better look
                      RawAcquisitionChartUp.ViewXY.Annotations[0].Visible = false;
                      RawAcquisitionChartUp.ViewXY.Annotations[1].Visible = false;
                      RawAcquisitionChartUp.ViewXY.Annotations[2].Visible = false;
                  }
              },
              () => { return true; }));
            }
        }
        #endregion

    }
}
