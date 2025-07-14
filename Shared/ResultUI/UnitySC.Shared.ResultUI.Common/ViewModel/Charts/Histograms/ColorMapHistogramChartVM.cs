using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Annotations;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.UI.Chart;

using GradientFill = LightningChartLib.WPF.Charting.GradientFill;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Histograms
{
    public class ColorMapHistogramChartVM : BaseLineChart
    {
        private float[] _sortedMatrix;
        private float _minimumValue;
        private float _maximumValue;

        private ColorMap _colorMap;

        #region Properties

        private float _minimumLimit;

        public float MinimumLimit
        {
            get => _referenceValue.HasValue ? _minimumLimit - _referenceValue.Value : _minimumLimit;
            set
            {
                if (_referenceValue.HasValue) value += _referenceValue.Value;
                SetProperty(ref _minimumLimit, value);
                Refresh();
            }
        }

        private float _maximumLimit = 200;

        public float MaximumLimit
        {
            get => _referenceValue.HasValue ? _maximumLimit - _referenceValue.Value : _maximumLimit;
            set
            {
                if (_referenceValue.HasValue) value += _referenceValue.Value;
                SetProperty(ref _maximumLimit, value);
                Refresh();
            }
        }

        private int _stepNumber = 100;

        public int StepNumber
        {
            get { return _stepNumber; }
            set
            {
                SetProperty(ref _stepNumber, value);
                Refresh();
            }
        }

        private bool _hideOutOfRangeValues;

        public bool HideOutOfRangeValues
        {
            get { return _hideOutOfRangeValues; }
            set
            {
                SetProperty(ref _hideOutOfRangeValues, value);
                Refresh();
            }
        }

        private ICommand _setDefaultSettingsCommand;

        public ICommand SetDefaultSettingsCommand => _setDefaultSettingsCommand ?? (_setDefaultSettingsCommand = new AutoRelayCommand(SetDefaultSettingsCommandExecute));

        private void SetDefaultSettingsCommandExecute()
        {
            SetProperty(ref _minimumLimit, _minimumValue, nameof(MinimumLimit));
            SetProperty(ref _maximumLimit, _maximumValue, nameof(MaximumLimit));
            SetProperty(ref _stepNumber, 100, nameof(StepNumber));
            Refresh();
        }

        #endregion

        private float _currentStepValue;

        private BarSeriesList _barSeriesCollection;

        private readonly AxisX _xAxis;
        private readonly AxisY _yAxis;

        private readonly AnnotationXY _tooltip;

        public ColorMapHistogramChartVM(string xAxis)
        {
            #region Axis

            _xAxis = new AxisX
            {
                AutoFormatLabels = false,
                CustomTicksEnabled = true,
                ValueType = AxisValueType.Number,
                Visible = true,
                Title = CreateAxisXTitle(xAxis)
            };

            _yAxis = new AxisY
            {
                Visible = true,
                Title = CreateAxisYTitle("Count")
            };

            _yAxis.Title.DistanceToAxis = 50;

            #endregion

            #region Chart

            Chart = new LightningChart
            {
                ViewXY =
                {
                    DropOldSeriesData = false,
                    XAxes = new AxisXList { _xAxis },
                    YAxes = new AxisYList { _yAxis },
                    AxisLayout = new AxisLayout
                    {
                        AxisGridStrips = XYAxisGridStrips.X
                    },
                    BarViewOptions = new BarViewOptions
                    {
                        BarSpacing = 0,
                        Grouping = BarsGrouping.ByLocation,
                        IndexGroupingFitGroupDistance = 0,
                        IndexGroupingFitSideMargins = 10
                    },
                    ZoomPanOptions = new ZoomPanOptions
                    {
                        ViewFitYMarginPixels = 10,
                        AutoYFit = new AutoYFit
                        {
                            Enabled = true
                        }
                    }
                },
                Title = new ChartTitle
                {
                    Text = "Histogram",
                    Visible = false
                }
            };

            SetupChartTheme();

            #endregion

            #region Tooltip

            _tooltip = new AnnotationXY
            {
                Visible = false,
                LocationCoordinateSystem = CoordinateSystem.ScreenCoordinates,
                AllowUserInteraction = false,
                Text = "",
                RenderBehindAxes = true,
                Style = AnnotationStyle.Rectangle
            };

            Chart.ViewXY.Annotations.Add(_tooltip);

            #endregion
        }

        public void ResetChart(float[] matrix, ColorMap colorMap, float min, float max)
        {
            _sortedMatrix = matrix;
            _minimumValue = min;
            _maximumValue = max;
            _colorMap = colorMap;

            SetProperty(ref _minimumLimit, _minimumValue, nameof(MinimumLimit));
            SetProperty(ref _maximumLimit, _maximumValue, nameof(MaximumLimit));

            Refresh();
        }

        private void Refresh()
        {
            UpdateChart(() =>
            {
                if (_barSeriesCollection != null)
                {
                    foreach (var barSeries in _barSeriesCollection)
                    {
                        barSeries.MouseOverOn -= BarSeriesMouseOverOn;
                        barSeries.MouseOverOff -= BarSeriesOnMouseOverOff;
                    }
                }

                _barSeriesCollection = GenerateBarSeries();
                Chart.ViewXY.BarSeries = _barSeriesCollection;
                Chart.ViewXY.LegendBoxes.Clear();

                UpdateView();
            });
        }

        public void UpdateXAxisTitle(string xAxis)
        {
            UpdateChart(() =>
            {
                _xAxis.Title.Text = xAxis;
            });
        }

        private void UpdateView()
        {
            UpdateChart(() =>
            {
                Chart.ViewXY.ZoomToFit();
            });
        }

        private BarSeriesList GenerateBarSeries()
        {
            if (_sortedMatrix == null) return new BarSeriesList();

            var colors = _colorMap.Colors;
            int colorLength = colors.Length;
            float fColorMapLenght = colorLength;

            float a = fColorMapLenght / (_maximumValue - _minimumValue);
            float b = -_minimumValue * fColorMapLenght / (_maximumValue - _minimumValue);

            float stepval = (_maximumLimit - _minimumLimit) / (StepNumber - 1.0f);
            float stepvalHistoCalc = (_maximumLimit - _minimumLimit) / (StepNumber - 2.0f);

            _currentStepValue = (_maximumLimit - _minimumLimit) / StepNumber;


            //Change x position and create custom tickmark for each octave
            var customAxisXTicks = new CustomAxisTickList();

            //Use starting frequency as starting position for octaves
            double labelTickValue = _minimumLimit;

            int stepTick = (int)Math.Ceiling(stepval / 30.0);
            if (stepTick > 0.0)
            {
                for (int bar = 0; bar < StepNumber; bar += stepTick)
                {
                    double xPos = _minimumLimit + (double)(bar * stepval);

                    string labelText = _referenceValue.HasValue ? (labelTickValue - _referenceValue.Value).ToString("0.0") : labelTickValue.ToString("0.0");

                    customAxisXTicks.Add(new CustomAxisTick { AxisValue = xPos, LabelText = labelText });
                    labelTickValue += stepTick * stepval;
                }
            }

            _xAxis.CustomTicks = customAxisXTicks;
            _xAxis.InvalidateCustomTicks();

            var barSeriesValues = new List<BarInput>();

            for (int currentStep = 0; currentStep < StepNumber; currentStep++)
            {
                float currentMin = _minimumLimit + currentStep * stepval;
                float currentMax = currentMin + stepval;

                //Get series fill color
                float fVal = (_minimumLimit + (currentStep + 0.5f) * stepval) * a + b;
                int nVal = (int)Math.Round(fVal);
                if (nVal < 0) nVal = 0;
                else if (nVal >= colorLength) nVal = colorLength - 1;
                var color = colors[nVal];

                var barSeries = new BarSeries
                {
                    Tag = new Tuple<float, float>(currentMin, currentMax),
                    Fill =
                    {
                        Color = new Color
                        {
                            A = color.A,
                            R = color.R,
                            G = color.G,
                            B = color.B
                        },
                        Style = RectFillStyle.ColorOnly,
                        GradientFill = GradientFill.Solid
                    },
                    BorderWidth = 0,
                    BarThickness = 5,
                    LabelStyle =
                    {
                        Visible = false,
                        Angle = 0,
                        VerticalAlign = BarsTitleVerticalAlign.BarTop
                    },
                    Shadow =
                    {
                        Visible = false
                    },
                    //Assign the location
                    Values = new[]
                    {
                        new BarSeriesValue { Location = _minimumLimit + currentStep * stepval }
                    }
                };

                float referenceValue = _referenceValue ?? 0;
                if (currentStep == 0 && !HideOutOfRangeValues)
                {
                    barSeries.Title.Text = $"<{_minimumLimit - referenceValue}";
                }
                else if (currentStep == _sortedMatrix.Length - 1 && !HideOutOfRangeValues)
                {
                    barSeries.Title.Text = $">{_maximumLimit - referenceValue}";
                }
                else
                {
                    float displayMin = _minimumLimit + (currentStep - 1) * stepvalHistoCalc;
                    float displayMax = _minimumLimit + currentStep * stepvalHistoCalc;
                    barSeries.Title.Text = $"{displayMin - referenceValue}...{displayMax - referenceValue}";
                }

                barSeries.MouseOverOn += BarSeriesMouseOverOn;
                barSeries.MouseOverOff += BarSeriesOnMouseOverOff;

                float maxValue = currentStep == StepNumber - 1 && !HideOutOfRangeValues ? float.MaxValue : currentMax;
                float minValue = currentStep == 0 && !HideOutOfRangeValues ? float.MinValue : currentMin;

                barSeriesValues.Add(new BarInput
                {
                    BarSeries = barSeries,
                    Max = maxValue,
                    Min = minValue
                });
            }

            int curclassid = 0;
            int maxid = StepNumber;
            float curclassval = _minimumValue;
            foreach (float val in _sortedMatrix)
            {
                while ((val > curclassval) && (curclassid < maxid - 1))
                {
                    curclassval += stepvalHistoCalc;
                    curclassid++;
                }
                barSeriesValues[curclassid]++;
            }

            foreach (var barSeriesValue in barSeriesValues)
            {
                barSeriesValue.BarSeries.Values[0].Value = (double)barSeriesValue.Count;
                barSeriesValue.BarSeries.Values[0].Text = barSeriesValue.Count.ToString(CultureInfo.InvariantCulture);
            }

            var barserieList = new BarSeriesList();
            barserieList.AddRange(barSeriesValues.Select(input => input.BarSeries).ToList());
            return barserieList;
         
        }

        public void UpdateColors(ColorMap colorMap)
        {
            if (Chart == null) return;

            var colors = colorMap.Colors;
            int colorLength = colors.Length;
            float fColorMapLenght = colorLength;

            float a = fColorMapLenght / (_maximumValue - _minimumValue);
            float b = -_minimumValue * fColorMapLenght / (_maximumValue - _minimumValue);

            float stepval = _currentStepValue;

            UpdateChart(() =>
            {
                int bar = 0;

                foreach (var barSeries in Chart.ViewXY.BarSeries)
                {
                    float fval = (_minimumLimit + (bar + 0.5f) * stepval) * a + b;
                    // X values
                    int nVal = (int)Math.Round(fval);
                    if (nVal < 0) nVal = 0;
                    else if (nVal >= colorLength) nVal = colorLength - 1;

                    var color = colors[nVal];
                    barSeries.Fill.Color = new Color
                    {
                        A = color.A,
                        R = color.R,
                        G = color.G,
                        B = color.B
                    };

                    bar++;
                }
            });
        }

        #region Handlers

        private void BarSeriesOnMouseOverOff(object sender, MouseEventArgs e)
        {
            _tooltip.Visible = false;
        }

        private void BarSeriesMouseOverOn(object sender, MouseEventArgs e)
        {
            if (!(sender is BarSeries bs)) return;

            string text = $"Value: [{bs.Title.Text}] = {bs.Values[0].Value:N0}";

            UpdateChart(() =>
            {
                _tooltip.Visible = true;
                _tooltip.Text = text;

                float posX = (float)e.GetPosition(Chart).X;
                float posY = (float)e.GetPosition(Chart).Y;
                SetLocationInBounds(_tooltip, Chart, posX, posY);
            });
        }

        /// <summary>
        /// Place the annotation by limiting the values inside the chart
        /// </summary>
        private static void SetLocationInBounds(AnnotationXY toolTip, LightningChart chart, float posX, float posY)
        {
            float maxX = (float)chart.ActualWidth - toolTip.Text.Length * 4;
            float minX = (float)toolTip.Text.Length * 4 + 50;
            if (posX > maxX) posX = maxX;
            else if (posX < minX) posX = minX;
            float minY = 35;
            posY -= 30;
            if (posY < minY) posY = minY;
            toolTip.LocationScreenCoords.SetValues(posX, posY);
        }

        #endregion

        private class BarInput
        {
            public float Min { get; set; }

            public float Max { get; set; }

            private int _cnt = 0;
            public int Count { get { return _cnt; } set { _cnt = value; } }

            public BarSeries BarSeries { get; set; }

            public static BarInput operator ++(BarInput bar)
            {
                Interlocked.Increment(ref bar._cnt);
                return bar;
            }
        }


        private float? _referenceValue;

        public void SetReferenceValue(float? referenceValue)
        {
            _referenceValue = referenceValue;
            OnPropertyChanged(nameof(MaximumLimit));
            OnPropertyChanged(nameof(MinimumLimit));
            Refresh();
        }
    }
}
