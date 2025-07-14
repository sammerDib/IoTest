using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common
{
    public abstract class MetroDieStatsVM<T> : ObservableObject, IDisposable where T : MeasurePointResult
    {
        private readonly PointSelectorBase _pointSelector;

        private MeasureDieResult _currentDie;

        #region Properties

        private IStatsContainer _stats;

        public IStatsContainer Stats
        {
            get { return _stats; }
            private set { SetProperty(ref _stats, value); }
        }

        #endregion

        protected MetroDieStatsVM(PointSelectorBase pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedPointChanged += PointSelectorSelectedPointChanged;
        }

        private void PointSelectorSelectedPointChanged(object sender, EventArgs e)
        {
            var selectedPoint = _pointSelector.SingleSelectedPoint;

            if (selectedPoint == null)
            {
                Clear();
                return;
            }

            var selectedDie = _pointSelector.Dies.SingleOrDefault(die => die.Points.Contains(selectedPoint));
            if (selectedDie == null)
            {
                Clear();
                return;
            }

            if (_currentDie == selectedDie)
            {
                return;
            }

            _currentDie = selectedDie;
            GenerateDieStats();
        }

        protected void GenerateDieStats()
        {
            if (_currentDie == null) return;

            var diePoints = _currentDie.Points.OfType<T>().ToList();
            Stats = GetStats(diePoints);
        }

        protected abstract IStatsContainer GetStats(List<T> points);

        #region Protected Methods

        private void Clear()
        {
            _currentDie = null;
            Stats = null;
        }

        #endregion

        public virtual void Dispose()
        {
            _pointSelector.SelectedPointChanged -= PointSelectorSelectedPointChanged;
        }
    }
}
