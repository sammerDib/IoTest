using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;

namespace UnitySC.Shared.ResultUI.Metro.PointSelector
{
    public class PointSelectorBase : ObservableObject
    {
        #region Fields

        private readonly Dictionary<MeasurePointResult, int> _pointToIndex = new Dictionary<MeasurePointResult, int>();

        private readonly SortedDictionary<int, MeasurePointResult> _indexToPoint = new SortedDictionary<int, MeasurePointResult>();

        private readonly SortedDictionary<int, MeasurePointResult> _checkedPointsDictionary = new SortedDictionary<int, MeasurePointResult>();

        private readonly SortedDictionary<int, MeasurePointDataResultBase> _currentRepetaPointsDictionary = new SortedDictionary<int, MeasurePointDataResultBase>();

        private readonly List<MeasureDieResult> _dies = new List<MeasureDieResult>();

        private readonly Dictionary<MeasurePointResult, MeasureDieResult> _pointToDie = new Dictionary<MeasurePointResult, MeasureDieResult>();

        #endregion Fields

        #region Properties
        
        public IReadOnlyDictionary<MeasurePointResult, int> PointToIndex => new ReadOnlyDictionary<MeasurePointResult, int>(_pointToIndex);

        public IReadOnlyDictionary<int, MeasurePointResult> SortedIndexToPoint => new ReadOnlyDictionary<int, MeasurePointResult>(_indexToPoint);

        public IReadOnlyCollection<MeasurePointResult> AllPoints => _pointToIndex.Keys.ToList().AsReadOnly();

        public MeasurePointResult SingleSelectedPoint { get; private set; }

        public MeasurePointDataResultBase SelectedRepetaPoint { get; private set; }

        public IReadOnlyList<MeasurePointResult> CheckedPoints => _checkedPointsDictionary.Values.ToList();

        public IReadOnlyList<MeasurePointDataResultBase> CurrentRepetaPoints => _currentRepetaPointsDictionary.Values.ToList();

        public bool AllIsChecked => _pointToIndex.Count == _checkedPointsDictionary.Count;

        public IReadOnlyList<MeasureDieResult> Dies => _dies.AsReadOnly();

        #endregion Properties

        #region Event Handlers

        public event EventHandler SelectedPointChanged;

        public event EventHandler CheckedPointsChanged;

        public event EventHandler CurrentRepetaPointsChanged;

        public event EventHandler SelectedRepetaPointChanged;

        #endregion Event Handlers

        #region Public Methods

        public void ResetPointsList(object sender, List<MeasurePointResult> points, List<MeasureDieResult> dies)
        {
            // Clear selection
            SingleSelectedPoint = null;
            SelectedRepetaPoint = null;
            _checkedPointsDictionary.Clear();
            _currentRepetaPointsDictionary.Clear();

            // Dies
            _dies.Clear();
            _dies.AddRange(dies);

            _pointToIndex.Clear();
            _indexToPoint.Clear();
            for (int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                _pointToIndex.Add(points[pointIndex], pointIndex);
                _indexToPoint.Add(pointIndex, points[pointIndex]);
            }

            _pointToDie.Clear();
            foreach (var die in dies)
            {
                foreach (var point in die.Points)
                {
                    _pointToDie.Add(point, die);
                }
            }

            SelectedPointChanged?.Invoke(sender, EventArgs.Empty);
            CheckedPointsChanged?.Invoke(sender, EventArgs.Empty);
            CurrentRepetaPointsChanged?.Invoke(sender, EventArgs.Empty);
            SelectedRepetaPointChanged?.Invoke(sender, EventArgs.Empty);
        }

        #region Selection

        public void SetSelectedPoint(object sender, MeasurePointResult point)
        {
            if (point == SingleSelectedPoint) return;

            _currentRepetaPointsDictionary.Clear();
            SelectedRepetaPoint = null;

            SingleSelectedPoint = point;

            if (point != null)
            {
                // If the point is not part of the list of checked points, automatically add it
                if (!_checkedPointsDictionary.ContainsKey(_pointToIndex[point]))
                {
                    _checkedPointsDictionary.Add(_pointToIndex[point], point);
                    CheckedPointsChanged?.Invoke(sender, EventArgs.Empty);
                }

                foreach (var pointOccurrence in point.Datas)
                {
                    _currentRepetaPointsDictionary.Add(pointOccurrence.IndexRepeta, pointOccurrence);
                }

                SelectedRepetaPoint = point.Datas.FirstOrDefault();
            }

            SelectedPointChanged?.Invoke(sender, EventArgs.Empty);
            CurrentRepetaPointsChanged?.Invoke(sender, EventArgs.Empty);
            SelectedRepetaPointChanged?.Invoke(sender, EventArgs.Empty);
        }

        public void SelectFirstCheckedPointIfNoneSelected()
        {
            if (CheckedPoints.Any() && SingleSelectedPoint == null)
            {
                SetSelectedPoint(this, CheckedPoints.FirstOrDefault());
            }
        }

        #endregion

        #region Check

        public void CheckAllPoints(object sender)
        {
            if (!AllIsChecked)
            {
                SetCheckedPoints(sender, _pointToIndex.Keys);
            }
        }

        public void SetCheckedPoints(object sender, IEnumerable<MeasurePointResult> points)
        {
            _checkedPointsDictionary.Clear();

            foreach (var point in points)
            {
                _checkedPointsDictionary.Add(_pointToIndex[point], point);
            }

            UnSelectPointIfNotChecked(sender);

            CheckedPointsChanged?.Invoke(sender, EventArgs.Empty);
        }

        private void UnSelectPointIfNotChecked(object sender)
        {
            if (SingleSelectedPoint != null && !_checkedPointsDictionary.ContainsKey(_pointToIndex[SingleSelectedPoint]))
            {
                SingleSelectedPoint = null;
                _currentRepetaPointsDictionary.Clear();
                SelectedRepetaPoint = null;

                SelectedPointChanged?.Invoke(sender, EventArgs.Empty);
                CurrentRepetaPointsChanged?.Invoke(sender, EventArgs.Empty);
                SelectedRepetaPointChanged?.Invoke(sender, EventArgs.Empty);
            }
        }

        public void CheckPoint(object sender, MeasurePointResult point)
        {
            if (_checkedPointsDictionary.ContainsKey(_pointToIndex[point])) return;

            _checkedPointsDictionary.Add(_pointToIndex[point], point);
            CheckedPointsChanged?.Invoke(sender, EventArgs.Empty);
            SelectSinglePointIfOnlyOneSelected(sender);
        }

        public void CheckPoints(object sender, IEnumerable<MeasurePointResult> points)
        {
            foreach (var point in points)
            {
                if (_checkedPointsDictionary.ContainsKey(_pointToIndex[point])) continue;
                _checkedPointsDictionary.Add(_pointToIndex[point], point);
            }
            CheckedPointsChanged?.Invoke(sender, EventArgs.Empty);
        }

        public void UncheckPoint(object sender, MeasurePointResult point)
        {
            if (!_checkedPointsDictionary.ContainsKey(_pointToIndex[point])) return;

            _checkedPointsDictionary.Remove(_pointToIndex[point]);

            CheckedPointsChanged?.Invoke(sender, EventArgs.Empty);

            if (SingleSelectedPoint != point) return;

            SingleSelectedPoint = null;
            _currentRepetaPointsDictionary.Clear();
            SelectedRepetaPoint = null;

            SelectedPointChanged?.Invoke(sender, EventArgs.Empty);
            CurrentRepetaPointsChanged?.Invoke(sender, EventArgs.Empty);
            SelectedRepetaPointChanged?.Invoke(sender, EventArgs.Empty);

            SelectSinglePointIfOnlyOneSelected(sender);
        }

        public void UncheckPoints(object sender, IEnumerable<MeasurePointResult> points)
        {
            foreach (var point in points)
            {
                if (!_checkedPointsDictionary.ContainsKey(_pointToIndex[point])) continue;
                _checkedPointsDictionary.Remove(_pointToIndex[point]);
            }

            UnSelectPointIfNotChecked(sender);
            CheckedPointsChanged?.Invoke(sender, EventArgs.Empty);
        }

        #endregion

        #region Repeta

        public void SelectRepetaPoint(object sender, MeasurePointDataResultBase point)
        {
            if (point != null && !_currentRepetaPointsDictionary.ContainsKey(point.IndexRepeta)) return;

            SelectedRepetaPoint = point;
            SelectedRepetaPointChanged?.Invoke(sender, EventArgs.Empty);
        }
        
        #endregion

        #region Die

        public MeasureDieResult GetDieFromPoint(MeasurePointResult point)
        {
            if (point == null) return null;
            return _pointToDie.TryGetValue(point, out var die) ? die : null;
        }

        #endregion

        #endregion Public Methods

        #region Private Methods

        private void SelectSinglePointIfOnlyOneSelected(object sender)
        {
            var point = _checkedPointsDictionary.Values.FirstOrDefault();
            
            if (_checkedPointsDictionary.Count != 1 || point == null || SingleSelectedPoint == point)
                return;

            SingleSelectedPoint = point;

            foreach (var pointOccurrence in point.Datas)
            {
                _currentRepetaPointsDictionary.Add(pointOccurrence.IndexRepeta, pointOccurrence);
            }

            SelectedRepetaPoint = point.Datas.FirstOrDefault();

            SelectedPointChanged?.Invoke(sender, EventArgs.Empty);
            CurrentRepetaPointsChanged?.Invoke(sender, EventArgs.Empty);
            SelectedRepetaPointChanged?.Invoke(sender, EventArgs.Empty);
        }

        #endregion Private Methods
    }
}
