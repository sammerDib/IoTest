using System;
using System.Collections.Generic;
using System.Windows;

using LightningChartLib.WPF.Charting;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.ResultUI.Common.Helpers;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Profile;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public class MatrixProfileVM : ObservableObject, IDisposable
    {
        private enum MatrixProfileType
        {
            None,
            LineProfile,
            CrossProfile
        }

        #region Fields

        private readonly string _unit;
        private readonly float[] _matrix;
        private readonly int _width;
        private readonly int _height;

        private int _crossProfileX;
        private int _crossProfileY;

        private int _startPointX;
        private int _startPointY;
        private int _endPointX;
        private int _endPointY;

        private MatrixProfileType _currentProfileType;

        private float? _referenceValue;

        /// <summary>
        /// Point attached to profile distance map
        /// </summary>
        private readonly List<Point> _profilePoints = new List<Point>();

        private readonly List<Point> _horizontalCrossProfilePoints = new List<Point>();
        private readonly List<Point> _verticalCrossProfilePoints = new List<Point>();

        #endregion

        #region Events

        public event EventHandler<Point?> LineMarkerPositionChanged;
        public event EventHandler<double?> HorizontalMarkerPositionChanged;
        public event EventHandler<double?> VerticalMarkerPositionChanged;

        #endregion

        private FloatStatsContainer _stats;

        public FloatStatsContainer Stats
        {
            get { return _stats; }
            private set { SetProperty(ref _stats, value); }
        }

        public ProfileChart ProfileChart { get; }

        public MatrixProfileVM(float[] matrix, int width, int height, string unit)
        {
            _matrix = matrix;
            _width = width;
            _height = height;
            _unit = unit;

            ProfileChart = new ProfileChart($"Value ({unit})");
            ProfileChart.MarkerPositionChanged += ProfileChart_MarkerPositionChanged;
        }

        private void ProfileChart_MarkerPositionChanged(object sender, MarkerPositionChangedEventArgs e)
        {
            switch (e.MarkerType)
            {
                case MarkerType.Line:
                    {
                        var point = e.Index > -1 && e.Index < _profilePoints.Count ? (Point?)_profilePoints[e.Index] : null;
                        LineMarkerPositionChanged?.Invoke(this, point);
                        ProfileChart.UpdateMarkerLabel(e.MarkerType, $"Coord: [{point?.X};{point?.Y}]{Environment.NewLine}Value: {e.Value:0.0} {_unit}");
                    }
                    break;
                case MarkerType.Horizontal:
                    {
                        double? xPos = e.Index > -1 && e.Index < _horizontalCrossProfilePoints.Count ? (double?)_horizontalCrossProfilePoints[e.Index].X : null;
                        HorizontalMarkerPositionChanged?.Invoke(this, xPos);
                        ProfileChart.UpdateMarkerLabel(e.MarkerType, $"Coord: [{xPos};{_crossProfileY}]{Environment.NewLine}Value: {e.Value:0.0} {_unit}");
                    }
                    break;
                case MarkerType.Vertical:
                    {
                        double? yPos = e.Index > -1 && e.Index < _verticalCrossProfilePoints.Count ? (double?)_verticalCrossProfilePoints[e.Index].Y : null;
                        VerticalMarkerPositionChanged?.Invoke(this, yPos);
                        ProfileChart.UpdateMarkerLabel(e.MarkerType, $"Coord: [{_crossProfileX};{yPos}]{Environment.NewLine}Value: {e.Value:0.0} {_unit}");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdateProfile(int startPointX, int startPointY, int endPointX, int endPointY)
        {
            _startPointX = startPointX;
            _startPointY = startPointY;
            _endPointX = endPointX;
            _endPointY = endPointY;

            _currentProfileType = MatrixProfileType.LineProfile;

            var line = LineHelper.InterBresenhamLine(startPointX, startPointY, endPointX, endPointY);

            _profilePoints.Clear();

            if (line.Count <= 0)
            {
                Clear();
                return;
            }

            int nFirstNoNan = -1;

            float currentDistance = 0.0f;

            var srcPoints = new SeriesPoint[line.Count];

            float minDist = 0;
            float maxDist = 0;
            var valueList = new List<float>();

            float zValue = GetValue(line[0].X, line[0].Y);

            _profilePoints.Add(line[0]);

            if (!float.IsNaN(zValue))
            {
                nFirstNoNan = 0;

                valueList.Add(zValue);
            }

            srcPoints[0] = new SeriesPoint
            {
                X = currentDistance,
                Y = nFirstNoNan != -1 ? zValue : double.NaN
            };

            for (int i = 1; i < line.Count; i++) // Loop through List with for
            {
                double distance = Math.Sqrt(Math.Pow(line[i].X - line[i - 1].X, 2) + Math.Pow(line[i].Y - line[i - 1].Y, 2));

                currentDistance += (float)distance;
                if (currentDistance < minDist) minDist = currentDistance;
                if (currentDistance > maxDist) maxDist = currentDistance;

                zValue = GetValue(line[i].X, line[i].Y);

                if (float.IsNaN(zValue) == false)
                {
                    if (nFirstNoNan == -1) nFirstNoNan = i;

                    valueList.Add(zValue);
                    
                    srcPoints[i] = new SeriesPoint
                    {
                        X = currentDistance,
                        Y = zValue
                    };
                }
                else
                {
                    srcPoints[i] = new SeriesPoint
                    {
                        X = currentDistance,
                        Y = double.NaN
                    };
                }

                _profilePoints.Add(line[i]);
            }

            Stats = FloatStatsContainer.GenerateFromFloats(valueList);
            ProfileChart.ResetChart(srcPoints);
        }

        public void UpdateCrossProfile(int crossProfileX, int crossProfileY)
        {
            _crossProfileX = crossProfileX;
            _crossProfileY = crossProfileY;

            _currentProfileType = MatrixProfileType.CrossProfile;

            _horizontalCrossProfilePoints.Clear();
            _verticalCrossProfilePoints.Clear();

            var lidxH = LineHelper.InterBresenhamLine(0, crossProfileY, _width, crossProfileY);

            float fDist = 0.0f;
            var valueList = new List<float>();

            SeriesPoint[] srcPoints = null;
            if (lidxH.Count > 0)
            {
                srcPoints = new SeriesPoint[lidxH.Count];

                float minDist = 0;
                float maxDist = 0;

                float zValue = GetValue(lidxH[0].X, lidxH[0].Y);

                _horizontalCrossProfilePoints.Add(lidxH[0]);

                if (float.IsNaN(zValue) == false)
                {
                    valueList.Add(zValue);
                }

                srcPoints[0] = new SeriesPoint
                {
                    X = fDist,
                    Y = zValue
                };

                for (int i = 1; i < lidxH.Count; i++) // Loop through List with for
                {

                    double distance = Math.Sqrt(Math.Pow(lidxH[i].X - lidxH[i - 1].X, 2) + Math.Pow(lidxH[i].Y - lidxH[i - 1].Y, 2));
                    fDist += (float)distance;

                    if (fDist < minDist) minDist = fDist;
                    if (fDist > maxDist) maxDist = fDist;

                    zValue = GetValue(lidxH[i].X, lidxH[i].Y);
                    if (float.IsNaN(zValue) == false)
                    {
                        valueList.Add(zValue);
                    }

                    srcPoints[i] = new SeriesPoint
                    {
                        X = fDist,
                        Y = zValue
                    };

                    _horizontalCrossProfilePoints.Add(lidxH[i]);
                }
            }

            var lidxV = LineHelper.InterBresenhamLine(crossProfileX, 0, crossProfileX, _height);
            fDist = 0.0f;
            SeriesPoint[] srcPoints2 = null;
            if (lidxV.Count > 0)
            {
                srcPoints2 = new SeriesPoint[lidxV.Count];

                float minDist = 0;
                float maxDist = 0;

                float zValue = GetValue(lidxV[0].X, lidxV[0].Y);
                if (float.IsNaN(zValue) == false)
                {
                    valueList.Add(zValue);
                }

                srcPoints2[0] = new SeriesPoint
                {
                    X = fDist,
                    Y = zValue
                };

                _verticalCrossProfilePoints.Add(lidxV[0]);

                for (int i = 1; i < lidxV.Count; i++) // Loop through List with for
                {
                    double distance = Math.Sqrt(Math.Pow(lidxV[i].X - lidxV[i - 1].X, 2) + Math.Pow(lidxV[i].Y - lidxV[i - 1].Y, 2));
                    fDist += (float)distance;

                    if (fDist < minDist) minDist = fDist;
                    if (fDist > maxDist) maxDist = fDist;

                    zValue = GetValue(lidxV[i].X, lidxV[i].Y);
                    if (float.IsNaN(zValue) == false)
                    {
                        valueList.Add(zValue);
                    }

                    srcPoints2[i] = new SeriesPoint
                    {
                        X = fDist,
                        Y = zValue
                    };

                    _verticalCrossProfilePoints.Add(lidxV[i]);
                }
            }

            Stats = FloatStatsContainer.GenerateFromFloats(valueList);
            ProfileChart.ResetChart(srcPoints, srcPoints2);
        }
        
        public void Clear()
        {
            Stats = null;
            ProfileChart.Clear();
        }

        public void SetReferenceValue(float? referenceValue)
        {
            _referenceValue = referenceValue;

            if (_referenceValue.HasValue)
            {
                ProfileChart.UpdateYAxisTitle($"Relative value ({_unit})");
            }
            else
            {
                ProfileChart.UpdateYAxisTitle($"Value ({_unit})");
            }

            switch (_currentProfileType)
            {
                case MatrixProfileType.None:
                    break;
                case MatrixProfileType.LineProfile:
                    UpdateProfile(_startPointX, _startPointY, _endPointX, _endPointY);
                    break;
                case MatrixProfileType.CrossProfile:
                    UpdateCrossProfile(_crossProfileX, _crossProfileY);
                    break;
            }
        }

        #region Private Methods
        
        private float GetValue(int x, int y)
        {
            if (_matrix == null || _matrix.Length == 0) return float.NaN;
            if (x < 0 || x >= _width) return float.NaN;
            if (y < 0 || y >= _height) return float.NaN;

            float value = _matrix[y * _width + x];
            return _referenceValue.HasValue ? value - _referenceValue.Value : value;
        }

        private float GetValue(double x, double y)
        {
            int ix = (int)Math.Round(x, 0);
            int iy = (int)Math.Round(y, 0);
            return GetValue(ix, iy);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            ProfileChart.MarkerPositionChanged -= ProfileChart_MarkerPositionChanged;
        }

        #endregion
    }
}
