using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;
using UnitySC.Shared.ResultUI.Metro.PointSelector;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public abstract class MetroPointsListVM : ObservableObject, IDisposable
    {
        #region Fields

        private List<MeasurePointResult> _selectedItems;

        #endregion

        #region Properties

        public DataTableSource<MeasurePointResult> SortedPoints { get; } = new DataTableSource<MeasurePointResult>();

        public PointSelectorBase PointSelector { get; }

        public MeasurePointResult SingleSelectedItem
        {
            get { return PointSelector.SingleSelectedPoint; }
            set { PointSelector.SetSelectedPoint(this, value); }
        }

        public IReadOnlyList<MeasurePointResult> SelectedMeasurePoints => PointSelector.CheckedPoints;

        public bool? SelectionState
        {
            get
            {
                if (SelectedMeasurePoints.Count == SortedPoints.Count)
                {
                    return true;
                }

                if (SelectedMeasurePoints.Count == 0)
                {
                    return false;
                }

                return null;
            }
        }

        private bool _syncListViewFlag;

        public bool SyncListViewFlag
        {
            get { return _syncListViewFlag; }
            private set { SetProperty(ref _syncListViewFlag, value); }
        }

        private bool _hideRepetaColumns;

        public bool HideRepetaColumns
        {
            get { return _hideRepetaColumns; }
            set { SetProperty(ref _hideRepetaColumns, value); }
        }

        private bool _hideDieIndex;

        public bool HideDieIndex
        {
            get { return _hideDieIndex; }
            set { SetProperty(ref _hideDieIndex, value); }
        }

        private bool _hideQualityColumns;

        public bool HideQualityColumns
        {
            get { return _hideQualityColumns; }
            set { SetProperty(ref _hideQualityColumns, value); }
        }

        private bool _hideSiteIdColumns;

        public bool HideSiteIdColumns
        {
            get { return _hideSiteIdColumns; }
            set { SetProperty(ref _hideSiteIdColumns, value); }
        }

        private int _digits;

        public int Digits
        {
            get { return _digits; }
            set { SetProperty(ref _digits, value); }
        }

        public Func<MeasurePointResult, string> MeasurePointResultToStringPos { get; }

        public Func<MeasurePointResult, string> MeasurePointResultToDieIndex { get; }

        public Func<MeasurePointResult, bool, bool> MeasurePointSelectionToBool { get; }

        #endregion

        #region Sort Definitions

        public SortDefinition SortByIndex { get; }
        public SortDefinition SortBySiteId { get; }
        public SortDefinition SortByState { get; }
        public SortDefinition SortByDieIndex { get; }
        public SortDefinition SortByX { get; }
        public SortDefinition SortByY { get; }

        public SortDefinition SortByQualityScore { get; }

        #endregion

        protected MetroPointsListVM(PointSelectorBase pointSelector)
        {
            PointSelector = pointSelector;

            PointSelector.SelectedPointChanged += PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged += PointSelectorOnCheckedPointsChanged;

            MeasurePointResultToStringPos = result => $"{PointSelector.PointToIndex[result] + 1}";
            MeasurePointResultToDieIndex = result =>
            {
                var die = PointSelector.GetDieFromPoint(result);
                return die == null ? string.Empty : $"[{die.ColumnIndex};{die.RowIndex}]";
            };
            MeasurePointSelectionToBool = (point, flag) => PointSelector.CheckedPoints.Contains(point);

            // Sorting
            SortByIndex = SortedPoints.Sort.AddSortDefinition(result => PointSelector.PointToIndex[result]);
            SortBySiteId = SortedPoints.Sort.AddSortDefinition(result => result.SiteId);
            SortByState = SortedPoints.Sort.AddSortDefinition(result => result.State);
            SortByDieIndex = SortedPoints.Sort.AddSortDefinition(result =>
            {
                var die = PointSelector.GetDieFromPoint(result);
                if (die == null) return double.MinValue;
                return (double)die.ColumnIndex * 100000 + die.RowIndex;
            });
            SortByX = SortedPoints.Sort.AddSortDefinition(result => result.XPosition);
            SortByY = SortedPoints.Sort.AddSortDefinition(result => result.YPosition);

            SortByQualityScore = SortedPoints.Sort.AddSortDefinition(result => result.QualityScore);
        }

        #region Event Handlers

        private void PointSelectorOnSelectedPointChanged(object sender, EventArgs e)
        {
            if (_selectedItems == null || _selectedItems.Count <= 1)
            {
                OnPropertyChanged(nameof(SingleSelectedItem));
            }

            SyncListView();
            OnSelectedMeasurePointsChanged();
        }

        private void PointSelectorOnCheckedPointsChanged(object sender, EventArgs e)
        {
            if (sender == this) return;

            SyncListView();
            OnSelectedMeasurePointsChanged();
        }

        #endregion Event Handlers

        #region Protected Methods

        /// <summary>
        /// Notifies the view that the selection of points has changed.
        /// </summary>
        protected void SyncListView()
        {
            SyncListViewFlag = !SyncListViewFlag;
        }

        protected void OnSelectedMeasurePointsChanged()
        {
            OnPropertyChanged(nameof(SelectionState));
        }

        #endregion

        #region Public Methods

        public virtual void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            SortedPoints.Reset(sourcePoint);
            HideRepetaColumns = !showRepetaColumns;
            HideDieIndex = !showDieIndex;
            HideQualityColumns = !showQualityScore;
            HideSiteIdColumns = !showSiteID;
            OnSelectedMeasurePointsChanged();
        }

        public void OnKeyDown(KeyEventArgs e, ListView listView)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                e.Handled = true;

                var selectedItems = listView.SelectedItems.OfType<MeasurePointResult>().ToList();

                var notCheckedPoint = selectedItems.Where(point => !PointSelector.CheckedPoints.Contains(point)).ToList();

                if (notCheckedPoint.Any())
                {
                    // Check all selected points
                    PointSelector.CheckPoints(this, notCheckedPoint);
                }
                else
                {
                    // Uncheck all selected points
                    PointSelector.UncheckPoints(this, selectedItems);
                }

                OnSelectedMeasurePointsChanged();
                SyncListView();
            }
        }

        public void SynchronizeSelectedItems(ListView listView) => _selectedItems = listView.SelectedItems.OfType<MeasurePointResult>().ToList();

        #endregion

        #region Private Methods

        private void SetSelectedPoints(IEnumerable<MeasurePointResult> selectedPoints)
        {
            PointSelector.SetCheckedPoints(this, selectedPoints);

            SyncListView();
            OnSelectedMeasurePointsChanged();
        }

        #endregion

        #region Commands

        private ICommand _previousMeasureCommand;

        public ICommand PreviousMeasureCommand => _previousMeasureCommand ?? (_previousMeasureCommand = new AutoRelayCommand(PreviousMeasureCommandExecute, PreviousMeasureCommandCanExecute));

        private bool PreviousMeasureCommandCanExecute()
        {
            if (PointSelector.SingleSelectedPoint == null)
                return PointSelector.PointToIndex.Count > 0;

            return PointSelector.SortedIndexToPoint.Values.First() != PointSelector.SingleSelectedPoint;
        }

        private void PreviousMeasureCommandExecute()
        {
            if (PointSelector.SingleSelectedPoint == null)
            {
                PointSelector.SetSelectedPoint(this, SortedPoints.LastOrDefault());
            }
            else
            {
                var orderedPoints = PointSelector.SortedIndexToPoint.Values.ToList();
                int currentPointIndexInList = orderedPoints.IndexOf(PointSelector.SingleSelectedPoint);
                PointSelector.SetSelectedPoint(this, orderedPoints[currentPointIndexInList - 1]);
            }

            SyncListView();
            OnSelectedMeasurePointsChanged();
        }

        private ICommand _nextMeasureCommand;

        public ICommand NextMeasureCommand => _nextMeasureCommand ?? (_nextMeasureCommand = new AutoRelayCommand(NextMeasureCommandExecute, NextMeasureCommandCanExecute));

        private bool NextMeasureCommandCanExecute()
        {
            if (PointSelector.SingleSelectedPoint == null)
                return PointSelector.PointToIndex.Count > 0;

            return PointSelector.SortedIndexToPoint.Values.Last() != PointSelector.SingleSelectedPoint;
        }

        private void NextMeasureCommandExecute()
        {
            if (PointSelector.SingleSelectedPoint == null)
            {
                PointSelector.SetSelectedPoint(this, SortedPoints.FirstOrDefault());
            }
            else
            {
                var orderedPoints = PointSelector.SortedIndexToPoint.Values.ToList();
                int currentPointIndexInList = orderedPoints.IndexOf(PointSelector.SingleSelectedPoint);
                PointSelector.SetSelectedPoint(this, orderedPoints[currentPointIndexInList + 1]);
            }

            SyncListView();
            OnSelectedMeasurePointsChanged();
        }

        private ICommand _toggleSelectionCommand;

        public ICommand ToggleSelectionCommand => _toggleSelectionCommand ?? (_toggleSelectionCommand = new AutoRelayCommand(ToggleSelectionCommandExecute));

        private void ToggleSelectionCommandExecute()
        {
            SetSelectedPoints(SelectedMeasurePoints.Count == SortedPoints.Count ? new List<MeasurePointResult>() : SortedPoints.ToList());
        }

        private ICommand _togglePointSelectionCommand;

        public ICommand TogglePointSelectionCommand => _togglePointSelectionCommand ?? (_togglePointSelectionCommand = new AutoRelayCommand<MeasurePointResult>(TogglePointSelectionCommandExecute));

        private void TogglePointSelectionCommandExecute(MeasurePointResult point)
        {
            // On multi selection
            if (_selectedItems != null && _selectedItems.Count > 1)
            {
                if (_selectedItems.Any(p => PointSelector.CheckedPoints.Contains(p)))
                {
                    PointSelector.UncheckPoints(this, _selectedItems);
                }
                else
                {
                    PointSelector.CheckPoints(this, _selectedItems);
                }

                SyncListView();
            }
            // On normal selection
            else
            {
                if (PointSelector.CheckedPoints.Contains(point))
                {
                    PointSelector.UncheckPoint(this, point);
                }
                else
                {
                    PointSelector.CheckPoint(this, point);
                }
            }

            OnSelectedMeasurePointsChanged();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            PointSelector.SelectedPointChanged -= PointSelectorOnSelectedPointChanged;
            PointSelector.CheckedPointsChanged -= PointSelectorOnCheckedPointsChanged;
        }

        #endregion
    }
}
