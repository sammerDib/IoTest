using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Tsv.Copla;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv.Copla
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="BestFitPlaneView"/>
    /// </summary>
    public class BestFitPlaneViewModel : ObservableObject, IDisposable
    {
        private readonly PointSelectorBase _pointSelector;

        private BestFitPlan _waferBestFitPlan;
        private BestFitPlan _dieBestFitPlan;

        public BestFitPlaneViewModel(PointSelectorBase pointSelector)
        {
            _pointSelector = pointSelector;
            _pointSelector.SelectedPointChanged += SelectedPointChanged;
        }

        private void SelectedPointChanged(object sender, EventArgs e)
        {
            IsDieMode = _pointSelector.Dies.Count > 0;
            if (!IsDieMode) SelectedBestFitPlanMode = 0;
            if (_pointSelector.SingleSelectedPoint != null)
            {
                if (_pointSelector.GetDieFromPoint(_pointSelector.SingleSelectedPoint) is TSVDieResult die)
                {
                    _dieBestFitPlan = die.BestFitPlan;
                }
                else
                {
                    _dieBestFitPlan = null;
                }
                OnPropertyChanged(nameof(BestFitPlan));
            }
            else
            {
                _dieBestFitPlan = null;
                OnPropertyChanged(nameof(BestFitPlan));
            }
        }

        public void Update(BestFitPlan bestFitPlan)
        {
            _waferBestFitPlan = bestFitPlan;
            OnPropertyChanged(nameof(BestFitPlan));
        }

        public List<int> BestFitPlanModes { get; } = new List<int> { 0, 1 };

        private int _selectedBestFitPlanMode;

        public int SelectedBestFitPlanMode
        {
            get { return _selectedBestFitPlanMode; }
            set
            {
                SetProperty(ref _selectedBestFitPlanMode, value);
                OnPropertyChanged(nameof(BestFitPlan));
            }
        }

        private bool _isDieMode;

        public bool IsDieMode
        {
            get { return _isDieMode; }
            set { SetProperty(ref _isDieMode, value); }
        }

        public BestFitPlan BestFitPlan => SelectedBestFitPlanMode == 0 ? _waferBestFitPlan : _dieBestFitPlan;

        #region IDisposable

        public void Dispose()
        {
            _pointSelector.SelectedPointChanged -= SelectedPointChanged;
        }

        #endregion
    }
}

