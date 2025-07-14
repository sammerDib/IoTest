using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.PointLocation
{
    public class PointsLocationVM : ObservableObject, IDisposable
    {
        private readonly PointSelectorBase _pointSelector;

        #region Properties

        public ObservableCollection<PointLocationVM> PointsLocationCollection { get; } = new ObservableCollection<PointLocationVM>();

        #endregion

        #region Ctor

        public PointsLocationVM(PointSelectorBase pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
        }

        #endregion

        #region Public Methods

        public void PopulatePointsLocationCollection()
        {
            PointsLocationCollection.Clear();
            var die = _pointSelector.Dies.FirstOrDefault();
            if (die == null) return;

            var diePoints = die.Points;
            foreach (var point in diePoints)
            {
                var pl = PointsLocationCollection.SingleOrDefault(s => s.SiteId == point.SiteId);
                if (pl is null)
                {
                    // garantee SiteID Unicity
                    PointsLocationCollection.Add(new PointLocationVM(this, point.XPosition, point.YPosition, point.SiteId, $"SiteID {point.SiteId}"));
                }
            }

            foreach (var point in _pointSelector.AllPoints)
            {
                var pl = PointsLocationCollection.SingleOrDefault(s => s.SiteId == point.SiteId);
                pl?.AddPoint(point);
            }

            OnPropertyChanged(nameof(PointsLocationCollection));
        }

        public void UpdateCheckedPoints(IEnumerable<MeasurePointResult> points, bool? show)
        {
            switch (show)
            {
                case true:
                    _pointSelector.CheckPoints(this, points);
                    _pointSelector.SelectFirstCheckedPointIfNoneSelected();
                    break;
                case false:
                    _pointSelector.UncheckPoints(this, points);
                    _pointSelector.SelectFirstCheckedPointIfNoneSelected();
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            RefreshSelectedPointLocation();
        }

        private void RefreshSelectedPointLocation()
        {
            var pointsChecked = _pointSelector.CheckedPoints;

            foreach (var pointLocationVM in PointsLocationCollection)
            {
                int count = pointLocationVM.Points.Count(point => pointsChecked.Contains(point));

                if (count == 0)
                {
                    pointLocationVM.SelectWithoutNotification(false);
                }
                else if (count < pointLocationVM.Points.Count)
                {
                    pointLocationVM.SelectWithoutNotification(null);
                }
                else
                {
                    pointLocationVM.SelectWithoutNotification(true);
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
