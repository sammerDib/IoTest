using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public abstract class GlobalStatsVM : ObservableObject, IDisposable
    {
        private readonly PointSelectorBase _pointSelector;

        protected GlobalStatsVM(PointSelectorBase pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;
        }
        
        private IStatsContainer _stats;

        public IStatsContainer Stats
        {
            get { return _stats; }
            protected set { SetProperty(ref _stats, value); }
        }

        private double _qualityScore;

        public double QualityScore
        {
            get { return _qualityScore; }
            protected set { SetProperty(ref _qualityScore, value); }
        }

        protected abstract void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e);

        #region IDisposable

        public virtual void Dispose()
        {
            _pointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
        }

        #endregion
    }
}
