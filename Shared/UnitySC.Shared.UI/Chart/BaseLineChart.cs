using System;
using System.Diagnostics;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.UI.Chart
{
    public abstract class BaseLineChart : ObservableObject, IDisposable
    {
        private LightningChart _chart;

        public LightningChart Chart
        {
            get => _chart;
            protected set
            {
                _chart = value;
                if (_chart != null)
                {
                    _chart.ChartRenderOptions.UpdateOnResize = false;
                    _chart.ChartRenderOptions.UpdateOnResizeTimeInterval = 100;
                    _chart.ChartRenderOptions.UpdateType = ChartUpdateTypes.Async;
                    _chart.ChartMessage += Chart_ChartMessage;
                }
            }
        }

        private void Chart_ChartMessage(ChartMessageInfo info)
        {
            // TODO A LightningChart ticket is in progress to correct the license error.
            if (info.Description != "License error")
            {
                Debug.WriteLine(info);
            }
        }

        protected void SetupChartTheme()
        {
            Chart.ColorTheme = ColorTheme.LightGray;
            Chart.ChartBackground = new Fill
            {
                GradientFill = GradientFill.Solid,
                Color = Colors.White,
                GradientColor = Colors.White,
                Style = RectFillStyle.ColorOnly
            };

            Chart.ViewXY.GraphBackground = new Fill
            {
                Color = Colors.White,
                GradientFill = GradientFill.Solid,
                Style = RectFillStyle.ColorOnly
            };

            Chart.ViewXY.AxisLayout.YAxisTitleAutoPlacement = false;

            foreach (var xAxis in Chart.ViewXY.XAxes)
            {
                xAxis.Title.AllowUserInteraction = false;
                xAxis.Title.Color = Colors.DimGray;
                xAxis.Title.Shadow = new TextShadow
                {
                    Style = TextShadowStyle.Off
                };
            }

            foreach (var yAxis in Chart.ViewXY.YAxes)
            {
                yAxis.Title.AllowUserInteraction = false;
                yAxis.Title.HorizontalAlign = YAxisTitleAlignmentHorizontal.Left;
                yAxis.Title.Color = Colors.DimGray;
                yAxis.Title.Shadow = new TextShadow
                {
                    Style = TextShadowStyle.Off
                };
            }
        }

        private bool _isUpdating;

        /// <summary>
        /// Encapsulates the action between <see cref="LightningChart.BeginUpdate"/> and <see cref="LightningChart.EndUpdate"/>.
        /// Prevents redundant <see cref="LightningChart.BeginUpdate"/> calls in case the chart is already being updated.
        /// </summary>
        /// <param name="action"><see cref="Action"/> to encapsulate</param>
        protected void UpdateChart(Action action)
        {
            if (_isUpdating)
            {
                action();
            }
            else
            {
                _isUpdating = true;
                Chart.BeginUpdate();
                action();
                Chart.EndUpdate();
                _isUpdating = false;
            }
        }

        public static SeriesMarkerPointShapeStyle GetSelectionSymbol()
        {
            return new SeriesMarkerPointShapeStyle
            {
                Color1 = Colors.Transparent,
                Color2 = Colors.Yellow,
                Shape = SeriesMarkerPointShape.CrossAim,
                Width = 45,
                Height = 45,
                BodyThickness = 9,
                BorderColor = Colors.Blue,
                BorderWidth = 3
            };
        }

        protected static AxisXTitle CreateAxisXTitle(string name, double fontsize = 14.0)
        {
            return new AxisXTitle
            {
                AllowUserInteraction = false,
                Text = name,
                Font = new WpfFont(fontsize),
                Visible = !string.IsNullOrEmpty(name),
                Color = Colors.DimGray,
                Shadow = new TextShadow
                {
                    Style = TextShadowStyle.Off
                }
            };
        }

        protected static AxisYTitle CreateAxisYTitle(string name, double fontsize = 14.0)
        {
            return new AxisYTitle
            {
                DistanceToAxis = 35,
                HorizontalAlign = YAxisTitleAlignmentHorizontal.Left,
                AllowUserInteraction = false,
                Text = name,
                Font = new WpfFont(fontsize),
                Visible = !string.IsNullOrEmpty(name),
                Color = Colors.DimGray,
                Shadow = new TextShadow
                {
                    Style = TextShadowStyle.Off
                }
            };
        }

        protected static AxisYTitle CreateAxisYUnit(string name, double fontsize = 10.0)
        {
            return new AxisYTitle
            {
                VerticalAlign = YAxisTitleAlignmentVertical.Top,
                Angle = 0,
                DistanceToAxis = 50,

                HorizontalAlign = YAxisTitleAlignmentHorizontal.Left,
                AllowUserInteraction = false,
                Text = name,
                Font = new WpfFont(fontsize) { Bold = true },
                Visible = !string.IsNullOrEmpty(name),
                Color = Colors.Black,
                Shadow = new TextShadow
                {
                    Style = TextShadowStyle.Off
                }
            };
        }

        public static AxisY CreateAxisY(ViewXY owner, string title, double fontsize = 14.0, string unit = null, double unitfontsize = 10.0)
        {
            return new AxisY(owner)
            {
                AllowUserInteraction = false,
                MajorDivTickStyle = new AxisTickStyle
                {
                    Color = Color.FromRgb(55, 55, 55),
                },
                MinorDivTickStyle = new AxisTickStyle
                {
                    Color = Color.FromRgb(155, 155, 155)
                },
                MajorGrid = new GridOptions
                {
                    Color = Color.FromRgb(204, 204, 204),
                    Pattern = LinePattern.Dash,
                    PatternScale = 1
                },
                ScaleNibs = new AxisDragNib
                {
                    Size = new SizeDoubleXY(0, 0)
                },
                AxisColor = Colors.White,
                Title = CreateAxisYTitle(title, fontsize),
                Units = CreateAxisYUnit(unit, unitfontsize),
                LabelsColor = Color.FromRgb(50, 50, 50)
            };
        }

        #region IDisposable

        public virtual void Dispose()
        {
            Dispose(Chart != null);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_chart != null)
                {
                    _chart.ChartMessage -= Chart_ChartMessage;
                    _chart.Dispose();
                    _chart = null;
                }
            }
        }

        #endregion IDisposable
    }
}
