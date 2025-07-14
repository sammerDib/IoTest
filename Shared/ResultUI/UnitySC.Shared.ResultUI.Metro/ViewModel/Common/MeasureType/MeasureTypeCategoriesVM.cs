using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.MeasureType
{
    public class MeasureTypeCategoriesVM : ObservableObject, IDisposable
    {
        private readonly PointSelectorBase _pointSelector;

        #region Ctor

        public MeasureTypeCategoriesVM(PointSelectorBase pointSelector)
        {
            _pointSelector = pointSelector;

            var states = (MeasureState[])Enum.GetValues(typeof(MeasureState));
            foreach (var state in states)
            {
                _measureTypes.Add(state, new MeasureTypeCategoryVM(this, state));
            }

            _pointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
        }

        #endregion

        #region Properties

        private readonly Dictionary<MeasureState, MeasureTypeCategoryVM> _measureTypes = new Dictionary<MeasureState, MeasureTypeCategoryVM>();

        public int TotalCountSelected => _pointSelector.CheckedPoints.Count;

        public bool ClassesAreSelected
        {
            get => !_measureTypes.Values.Any(s => s.IsEnabled && (s.IsSelected == false || s.IsSelected == null));
            set
            {
                if (value)
                {
                    _pointSelector.CheckAllPoints(this);
                }
                else
                {
                    _pointSelector.SetCheckedPoints(this, new List<MeasurePointResult>());
                }
            }
        }

        public List<MeasureTypeCategoryVM> MeasureTypeCategories => _measureTypes.Values.ToList();

        #endregion

        #region Public Methods

        public void OnChangeCategorySelection(MeasureTypeCategoryVM mtc)
        {
            switch (mtc.IsSelected)
            {
                // update selected categories
                case true:
                    {
                        var statePoints = _pointSelector.AllPoints.Where(p => p.State == mtc.MeasureState);
                        _pointSelector.CheckPoints(this, statePoints);
                        _pointSelector.SelectFirstCheckedPointIfNoneSelected();
                        break;
                    }
                case false:
                    {
                        var statePoints = _pointSelector.AllPoints.Where(p => p.State == mtc.MeasureState);
                        _pointSelector.UncheckPoints(this, statePoints);
                        _pointSelector.SelectFirstCheckedPointIfNoneSelected();
                        break;
                    }
            }
        }

        public void UpdateCategories(List<MeasurePointResult> points)
        {
            foreach (var measureType in _measureTypes)
            {
                measureType.Value.PointsNumber = 0;
            }

            foreach (var measurePointResult in points)
            {
                _measureTypes[measurePointResult.State].PointsNumber++;
            }

            OnPropertyChanged(nameof(MeasureTypeCategories));
        }

        #endregion

        #region Private Methods

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            UpdateMeasureTypesState();
            OnPropertyChanged(nameof(ClassesAreSelected));
            OnPropertyChanged(nameof(TotalCountSelected));
        }

        private void UpdateMeasureTypesState()
        {
            var pointsChecked = _pointSelector.CheckedPoints;

            foreach (var measureType in _measureTypes)
            {
                measureType.Value.CurrentCount = 0;
            }

            foreach (var pointResult in pointsChecked)
            {
                _measureTypes[pointResult.State].CurrentCount++;
            }

            foreach (var measureType in _measureTypes)
            {
                if (measureType.Value.CurrentCount > 0 && measureType.Value.CurrentCount < measureType.Value.PointsNumber)
                {
                    //null
                    measureType.Value.SelectWithoutNotification(null);
                }
                else if (measureType.Value.CurrentCount == 0)
                {
                    //false
                    measureType.Value.SelectWithoutNotification(false);
                }
                else
                {
                    //true
                    measureType.Value.SelectWithoutNotification(true);
                }
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _pointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
        }

        #endregion
    }
}
