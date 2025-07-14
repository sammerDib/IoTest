using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Maps;
using LightningChartLib.WPF.Charting.SeriesXY;

using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.Shared.UI.Chart;
using UnitySC.Shared.UI.Helper;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.XY
{
    public class XYCalibResultHeatMapVM : BaseHeatMapChartVM
    {
        private const int HeatMapXYSize = 100;

        private CancellationTokenSource _tokenSource = null;
        private double _waferRadius;
        private string _title;
        private List<LineIntensityResult> _intensityResults;
        private ShiftToDisplay _shiftToDisplay;

        #region Private Classes

        private class PointIntensityResult
        {
            public int IntensityGridY { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Value { get; set; }
        }

        private class LineIntensityResult
        {
            public int IntensityGridX { get; set; }

            public List<PointIntensityResult> IntensityResults { get; } = new List<PointIntensityResult>();
        }

        #endregion Private Classes

        public enum ShiftToDisplay
        { X, Y };

        private XYCalibrationData _xyCalibdata;

        public XYCalibResultHeatMapVM(ShiftToDisplay shiftToDisplay)
            :base(HeatMapXYSize)
        {
            _title = shiftToDisplay.ToString();
            _shiftToDisplay = shiftToDisplay;
        }

        public void Update(XYCalibrationData xyCalibData, double specMin, double specMax)
        {
            _xyCalibdata = xyCalibData;
            _waferRadius = _xyCalibdata.WaferCalibrationDiameter.Millimeters * 0.5;

            if (!xyCalibData.IsInterpReady)
                XYCalibrationCalcul.PreCompute(xyCalibData);

            SpecValueMin = specMin;
            SpecValueMax = specMax;

            UpdateViewerType(HeatMapPaletteType.SpecValues);
        }

        private void FilterWaferShape(double waferRadius)
        {
            int circlePointCount = 2 * HeatMapSide;
            double angleStep = -Math.PI * 2.0 / circlePointCount;
            var circlePoints = new PointDouble2D[circlePointCount];

            for (int i = 0; i < circlePointCount; ++i)
            {
                circlePoints[i].X = waferRadius * Math.Cos(i * angleStep);
                circlePoints[i].Y = waferRadius * Math.Sin(i * angleStep);
            }

            var circle = new PolygonSeries(Chart.ViewXY, Chart.ViewXY.XAxes[0], Chart.ViewXY.YAxes[0])
            {
                ShowInLegendBox = false,
                AllowUserInteraction = false,
                Points = circlePoints,
                Fill =
                {
                    Style = RectFillStyle.None
                },
                Border =
                {
                    Color = Color.FromArgb(0xff, 0x29, 0x3D, 0x29),
                    Width = 2
                }
            };

            Chart.ViewXY.PolygonSeries.Clear();
            Chart.ViewXY.PolygonSeries.Add(circle);

            // Filter HeatMap with wafer shape
            var stencilArea = new StencilArea(HeatMap.Stencil);
            stencilArea.AddPolygon(circlePoints);
            HeatMap.Stencil.AdditiveAreas.Clear();
            HeatMap.Stencil.AdditiveAreas.Add(stencilArea);
        }

        protected static double EuclideanDistance(double x1, double y1, double x2, double y2)
        {
            double xDist = x1 - x2;
            double yDist = y1 - y2;
            return Math.Sqrt(xDist * xDist + yDist * yDist);
        }

        protected void UpdateViewerType(HeatMapPaletteType paletteType)
        {
            SetPaletteType(paletteType);
            UpdateChartData();
        }

        protected  void UpdateChartData()
        {
            _tokenSource?.Cancel();
            double heatMapToWaferRatio = _waferRadius * 2.0 / HeatMapSide;

            CurrentMinValue = double.MaxValue;
            CurrentMaxValue = double.MinValue;

            IsBusy = true;

            UpdateChart(() =>
            {
                HeatMap.SetRangesXY(-_waferRadius, _waferRadius, -_waferRadius, _waferRadius);
                HeatMap.Title.Text = _title;
                FilterWaferShape(_waferRadius);
                Chart.ViewXY.ZoomToFit();
            });

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            TaskHelper.DoAsyncOnSystemIdle(() =>
            {
                FeedDataMap(heatMapToWaferRatio, token);
            }, () =>
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                OnFeedDataDone();
            });
        }

        private void FeedDataMap(double heatMapToWaferRatio, CancellationToken token)
        {
            var tasks = new List<Task<LineIntensityResult>>();

            for (int i = 0; i < HeatMapSide; ++i)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                int intensityGridX = i;

                var task = new Task<LineIntensityResult>(() =>
                {
                    if (token.IsCancellationRequested) return null;

                    var lineInterpolation = new LineIntensityResult
                    {
                        IntensityGridX = intensityGridX
                    };

                    for (int j = 0; j < HeatMapSide; ++j)
                    {
                        if (token.IsCancellationRequested) return null;

                        double x = intensityGridX * heatMapToWaferRatio - _waferRadius;
                        double y = j * heatMapToWaferRatio - _waferRadius;

                        var pointInterpolation = new PointIntensityResult
                        {
                            IntensityGridY = j,
                            X = x,
                            Y = y
                        };

                        lineInterpolation.IntensityResults.Add(pointInterpolation);

                        // Exclude data points that are not in the wafer circle + a security margin
                        if (EuclideanDistance(x, y, 0, 0) > (_waferRadius + 1.0))
                        {
                            pointInterpolation.Value = double.NaN;
                            continue;
                        }

                        var Shifts = XYCalibrationHelper.ComputeCorrection(x, y, _xyCalibdata);

                        double value = 0.0;
                        switch (_shiftToDisplay)
                        {
                            case ShiftToDisplay.X:
                                value = Shifts.Item1.GetValueAs(XYCalibrationData.CorrectionUnit);
                                   break;

                            case ShiftToDisplay.Y:
                                value = Shifts.Item2.GetValueAs(XYCalibrationData.CorrectionUnit);
                                break;

                            default:
                                throw new InvalidOperationException("Unknow shift to display");
                        }
                        pointInterpolation.Value = value;

                        if (value < CurrentMinValue) CurrentMinValue = value;
                        if (value > CurrentMaxValue) CurrentMaxValue = value;
                    }

                    return lineInterpolation;
                });

                tasks.Add(task);
                task.Start(TaskScheduler.Current);
            }

            // Wait for all the tasks to finish.
            Task.WaitAll(tasks.Cast<Task>().ToArray());
            _intensityResults = tasks.Select(task => task.Result).ToList();
        }

        private void OnFeedDataDone()
        {
            UpdatePalette();
            ApplyHeatMapIntensities();
            IsBusy = false;
        }

        private void ApplyHeatMapIntensities()
        {
            if (_intensityResults == null) return;

            foreach (var line in _intensityResults)
            {
                foreach (var point in line.IntensityResults)
                {
                    HeatMap.Data[line.IntensityGridX, point.IntensityGridY].X = point.X;
                    HeatMap.Data[line.IntensityGridX, point.IntensityGridY].Y = point.Y;
                    HeatMap.Data[line.IntensityGridX, point.IntensityGridY].Value = point.Value;
                }
            }

            // Notify chart about updated data.
            HeatMap.InvalidateValuesDataOnly();

            _intensityResults = null;

        }

        protected override void OnMarkerClicked(object sender, MouseEventArgs e)
        {
            // nothing to do yet
        }
    }
}
