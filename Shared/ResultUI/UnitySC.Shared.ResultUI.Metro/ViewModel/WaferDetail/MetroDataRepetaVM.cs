using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public abstract class MetroDataRepetaVM<T> : ObservableObject, IDisposable where T : MeasurePointDataResultBase
    {
        #region Properties

        #region Notifiables

        private string _max = "-";

        public string Max
        {
            get { return _max; }
            protected set { SetProperty(ref _max, value); }
        }

        private string _min = "-";

        public string Min
        {
            get { return _min; }
            protected set { SetProperty(ref _min, value); }
        }

        private string _delta = "-";

        public string Delta
        {
            get { return _delta; }
            protected set { SetProperty(ref _delta, value); }
        }

        private string _mean = "-";

        public string Mean
        {
            get { return _mean; }
            protected set { SetProperty(ref _mean, value); }
        }

        private string _sigma3 = "-";

        public string Sigma3
        {
            get { return _sigma3; }
            protected set { SetProperty(ref _sigma3, value); }
        }

        private T _selectedRepetaPoint;

        public T SelectedRepetaPoint
        {
            get { return _selectedRepetaPoint; }
            set
            {
                if (_isUpdating) return;
                if (SetProperty(ref _selectedRepetaPoint, value))
                {
                    PointSelector.SelectRepetaPoint(this, value);
                }
            }
        }

        private int _digits;

        public int Digits
        {
            get { return _digits; }
            set
            {
                if (SetProperty(ref _digits, value))
                {
                    UpdateValues();
                }
            }
        }

        #endregion

        #region Sort Definitions

        public SortDefinition SortByIndex { get; }
        public SortDefinition SortByState { get; }
        public SortDefinition SortByScore { get; }

        #endregion

        public DataTableSource<T> RepetaSource { get; }
        public Func<MeasurePointDataResultBase, string> DataToIndex { get; }

        #endregion

        private bool _isUpdating;

        protected readonly PointSelectorBase PointSelector;

        protected MetroDataRepetaVM(PointSelectorBase pointSelector)
        {
            RepetaSource = new DataTableSource<T>();

            SortByIndex = RepetaSource.Sort.AddSortDefinition(data => data.IndexRepeta + 1);
            SortByState = RepetaSource.Sort.AddSortDefinition(data => data.State);
            SortByScore = RepetaSource.Sort.AddSortDefinition(data => data.QualityScore);

            DataToIndex = data => $"{data.IndexRepeta + 1}";

            PointSelector = pointSelector;
            PointSelector.SelectedPointChanged += OnSelectedPointChanged;
            PointSelector.SelectedRepetaPointChanged += OnSelectedRepetaPointChanged;
        }

        private void OnSelectedRepetaPointChanged(object sender, EventArgs e)
        {
            if (sender == this) return;
            SetProperty(ref _selectedRepetaPoint, PointSelector.SelectedRepetaPoint as T, nameof(SelectedRepetaPoint));
        }

        private void OnSelectedPointChanged(object sender, EventArgs e) => UpdateValues();

        protected void UpdateValues()
        {
            _isUpdating = true;

            try
            {
                var point = PointSelector.SingleSelectedPoint;
                var dataPoints = point?.Datas?.OfType<T>().ToList();
                RepetaSource.Reset(dataPoints ?? new List<T>());

                InternalUpdateValues(point);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        protected abstract void InternalUpdateValues(MeasurePointResult point);

        #region IDisposable

        public virtual void Dispose()
        {
            PointSelector.SelectedPointChanged -= OnSelectedPointChanged;
            PointSelector.SelectedRepetaPointChanged -= OnSelectedRepetaPointChanged;
        }

        #endregion
    }
}
