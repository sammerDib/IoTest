using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Annotations;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Format.HAZE;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.HAZE.ViewModel.WaferDetails
{
    public class HazeHistogramChartVM : BaseLineChart
    {
        private float _currentStepValue;

        private BarSeriesList _barSeriesCollection;

        private readonly AxisX _xAxis;
        private readonly AxisY _yAxis;

        private readonly AnnotationXY _tooltip;
        
        public HazeHistogramChartVM()
        {
            #region Axis

            _xAxis = new AxisX
            {
                AutoFormatLabels = false,
                CustomTicksEnabled = true,
                ValueType = AxisValueType.Number,
                Visible = true,
                Title = CreateAxisXTitle(string.Empty)
            };

            _yAxis = new AxisY
            {
                Visible = true,
                Title = CreateAxisYTitle(string.Empty)
            };

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

        public void ResetChart(float maxValue, float minValue, HazeMap hazeMap, ColorMap colorMap)
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

                _barSeriesCollection = GenerateBarSeries(maxValue, minValue, hazeMap, colorMap);
                Chart.ViewXY.BarSeries = _barSeriesCollection;
                Chart.ViewXY.LegendBoxes.Clear();


                UpdateView();
            });
        }

        private void UpdateView()
        {
            UpdateChart(() =>
            {
                Chart.ViewXY.ZoomToFit();
            });
        }

        private BarSeriesList GenerateBarSeries(float maxValue, float minValue, HazeMap hazeMap, ColorMap colorMap)
        {
            float limitMax = hazeMap.HistLimitMax;
            float limitMin = hazeMap.HistLimitMin;
            float stepNumber = hazeMap.HistNbStep;
            uint[] histogramData = hazeMap.Histo;

            var colors = colorMap.Colors;
            int colorLength = colors.Length;
            float fColorMapLenght = colorLength;

            float a = fColorMapLenght / (maxValue - minValue);
            float b = -minValue * fColorMapLenght / (maxValue - minValue);

            float stepval = (limitMax - limitMin) / (stepNumber - 1.0f);
            float stepvalHistoCalc = (limitMax - limitMin) / (stepNumber - 2.0f);

            _currentStepValue = (limitMax - limitMin) / stepNumber;
            

            //Change x position and create custom tickmark for each octave
            var customAxisXTicks = new CustomAxisTickList();

            //Use starting frequency as starting position for octaves
            double labelTickValue = limitMin;

            int stepTick = (int)Math.Ceiling(histogramData.Length / 30.0);
            for (int bar = 0; bar < histogramData.Length; bar += stepTick)
            {
                double xPos = limitMin + (double)(bar * stepval);
                customAxisXTicks.Add(new CustomAxisTick
                {
                    AxisValue = xPos,
                    LabelText = labelTickValue.ToString("0.0")
                });
                labelTickValue += stepTick * stepval;
            }

            _xAxis.CustomTicks = customAxisXTicks;

            var barSeriesValues = new BarSeriesList();

            for (int bar = 0; bar < histogramData.Length; bar++)
            {
                //Get series fill color
                float fVal = (limitMin + (bar + 0.5f) * stepval) * a + b;
                int nVal = (int)Math.Round(fVal);
                if (nVal < 0) nVal = 0;
                else if (nVal >= colorLength) nVal = colorLength - 1;
                var color = colors[nVal];
                
                var barSeries = new BarSeries
                {
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
                    //Assign the value
                    Values = new[]
                    {
                        new BarSeriesValue
                        {
                            Location = limitMin + bar * stepval,
                            Value = histogramData[bar],
                            Text = histogramData[bar].ToString("0")
                        }
                    }
                };

                if (bar == 0) barSeries.Title.Text = $"<{limitMin}";
                else if (bar == histogramData.Length - 1) barSeries.Title.Text = $">{limitMax}";
                else barSeries.Title.Text = $"{limitMin + (bar - 1) * stepvalHistoCalc}...{limitMin + bar * stepvalHistoCalc}";

                barSeries.MouseOverOn += BarSeriesMouseOverOn;
                barSeries.MouseOverOff += BarSeriesOnMouseOverOff;

                barSeriesValues.Add(barSeries);
            }

            return barSeriesValues;
        }
        
        public void UpdateColors(float maxValue, float minValue, HazeMap hazeMap, ColorMap colorMap)
        {
            if (Chart == null) return;

            var colors = colorMap.Colors;
            int colorLength = colors.Length;
            float fColorMapLenght = colorLength;

            float a = fColorMapLenght / (maxValue - minValue);
            float b = -minValue * fColorMapLenght / (maxValue - minValue);

            float stepval = _currentStepValue;

            UpdateChart(() =>
            {
                int bar = 0;

                foreach (var barSeries in Chart.ViewXY.BarSeries)
                {
                    float fval = (hazeMap.HistLimitMin + (bar + 0.5f) * stepval) * a + b;
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
    }
}
