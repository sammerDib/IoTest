using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Export;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.ResultUI.Metro.PointSelector;


namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    public abstract class MetroResultVM : ResultWaferVM
    {
        #region Properties

        // The '66' constant is the size the graph needs to display the heatmap. There is no way to get it dynamically which is why it is hard written.
        public Func<double, double> CalculateHeatmapWidthFunc { get; } = height =>
        {
            if (height > 0) return height + 48;
            return 448;
        };

        public MetroResult MetroResult => ResultDataObj as MetroResult;

        private bool _hasRepeta;

        public bool HasRepeta
        {
            get { return _hasRepeta; }
            set { SetProperty(ref _hasRepeta, value); }
        }

        private bool _isDieMode;

        public bool IsDieMode
        {
            get { return _isDieMode; }
            private set { SetProperty(ref _isDieMode, value); }
        }

        private bool _hasSiteID;

        public bool HasSiteID
        {
            get { return _hasSiteID; }
            set { SetProperty(ref _hasSiteID, value); }
        }

        private bool _hasQualityScore;

        public bool HasQualityScore
        {
            get { return _hasQualityScore; }
            set { SetProperty(ref _hasQualityScore, value); }
        }

        private bool _hasRawData;

        public bool HasRawData
        {
            get => _hasRawData;
            set
            {
                SetProperty(ref _hasRawData, value);
            }
        }

        private bool _hasThumbnail;

        public bool HasThumbnail
        {
            get => _hasThumbnail;
            set => SetProperty(ref _hasThumbnail, value);
        }

        private bool _hasReport;

        public bool HasReport
        {
            get => _hasReport;
            set => SetProperty(ref _hasReport, value);
        }

        public string SelectedMeasurePointResultIndex
        {
            get
            {
                var pointSelector = GetPointSelector();
                return pointSelector.SingleSelectedPoint != null ? $"{pointSelector.PointToIndex[pointSelector.SingleSelectedPoint] + 1} / {pointSelector.PointToIndex.Count}" : "-";
            }
        }

        private string _searchPointText;

        public string SearchPointText
        {
            get { return _searchPointText; }
            set
            {
                SetProperty(ref _searchPointText, value);
                ValidateSearchCommand.NotifyCanExecuteChanged();
            }
        }

        private int _digits = 8;

        public int Digits
        {
            get { return _digits; }
            set
            {
                SetProperty(ref _digits, value);
                DecreaseDigitsCommand.NotifyCanExecuteChanged();
                IncreaseDigitsCommand.NotifyCanExecuteChanged();
                OnDigitsChanged();
            }
        }
        
        public SummaryChart SummaryChart { get; }

        public ThumbnailViewerVM ThumbnailViewerVm { get; protected set; }

        public ReportVM ReportVm { get; protected set; }


        #region Abstracts

        public abstract MetroPointsListVM ResultPointsList { get; }

        #endregion

        #endregion

        protected MetroResultVM(IResultDisplay resDisplay) : base(resDisplay)
        {
            ExportResultVM.AdditionalEntries.Add(new ExportEntry(MetroExportResult.CsvExport));

            var stateToColorDict = new Dictionary<Enum, Color>
            {
                { MeasureState.Success, ToleranceDisplayer.GoodColorBrush.Color },
                { MeasureState.Partial, ToleranceDisplayer.WarningColorBrush.Color },
                { MeasureState.Error, ToleranceDisplayer.BadColorBrush.Color },
                { MeasureState.NotMeasured, ToleranceDisplayer.NotMeasuredColorBrush.Color }
            };

            SummaryChart = new SummaryChart("Total", stateToColorDict);
        }

        #region Abstract Methods

        protected abstract PointSelectorBase GetPointSelector();

        protected abstract void OnDigitsChanged();

        #endregion

        #region Overrides of ResultWaferVM

        public override void UpdateResData(IResultDataObject resdataObj)
        {
            base.UpdateResData(resdataObj);

            var pointSelector = GetPointSelector();

            List<MeasurePointResult> points;

            var metroResult = MetroResult?.MeasureResult;

            if (metroResult != null)
            {
                metroResult.FillNonSerializedProperties(true, true);

                if (metroResult.Points == null)
                    metroResult.Points = new List<MeasurePointResult>();
                if (metroResult.Dies == null)
                    metroResult.Dies = new List<MeasureDieResult>();

                points = metroResult.Points.Count != 0 ? metroResult.Points : metroResult.Dies.SelectMany(die => die.Points).ToList();

                HasRepeta = points.Any(point => point?.Datas?.Count > 1);
                IsDieMode = metroResult.Dies.Count > 0;
                HasQualityScore = points.Any(point => point.QualityScore != 0.0);
                HasSiteID = points.Any(point => point.SiteId != 0);
                MeasureLabelName = metroResult.Name;

                pointSelector.ResetPointsList(this, points, metroResult.Dies);

            }
            else
            {
                points = new List<MeasurePointResult>();

                HasRepeta = false;
                IsDieMode = false;
                HasQualityScore = false;
                HasSiteID = false;
                MeasureLabelName = null;

                pointSelector.ResetPointsList(this, points, new List<MeasureDieResult>());
            }

            if (ThumbnailViewerVm != null)
            {
                ThumbnailViewerVm.RootPath = MetroResult?.ResFilePath;
            }

            if (ReportVm != null)
            {
                ReportVm.RootPath = MetroResult?.ResFilePath;
            }

            ResultPointsList.UpdatePointsSource(points, HasRepeta, IsDieMode, HasQualityScore, HasSiteID);

            OnResDataChanged(points);

            SummaryChart.UpdateBars(points);

            OnPropertyChanged(nameof(MetroResult));
            pointSelector.CheckAllPoints(this);
            pointSelector.SelectFirstCheckedPointIfNoneSelected();
        }

        protected abstract void OnResDataChanged(List<MeasurePointResult> points);

        #endregion

        #region Private Methods

        private void ValidateSearch()
        {
            if (int.TryParse(SearchPointText, out int index))
            {
                var pointSelector = GetPointSelector();
                if (pointSelector.SortedIndexToPoint.TryGetValue(index - 1, out var point))
                {
                    pointSelector.SetSelectedPoint(this, point);
                }
            }

            SearchPointText = string.Empty;
        }

        #endregion

        #region Commands

        private AutoRelayCommand _validateSearchCommand;

        public AutoRelayCommand ValidateSearchCommand => _validateSearchCommand ?? (_validateSearchCommand = new AutoRelayCommand(ValidateSearch, ValidateSearchCommandCanExecute));

        private bool ValidateSearchCommandCanExecute()
        {
            return !string.IsNullOrWhiteSpace(SearchPointText);
        }

        private AutoRelayCommand _decreaseDigitsCommand;

        public AutoRelayCommand DecreaseDigitsCommand => _decreaseDigitsCommand ?? (_decreaseDigitsCommand = new AutoRelayCommand(DecreaseDigitsCommandExecute, DecreaseDigitsCommandCanExecute));

        private bool DecreaseDigitsCommandCanExecute() => Digits > 0;

        private void DecreaseDigitsCommandExecute() => Digits--;

        private AutoRelayCommand _increaseDigitsCommand;

        public AutoRelayCommand IncreaseDigitsCommand => _increaseDigitsCommand ?? (_increaseDigitsCommand = new AutoRelayCommand(IncreaseDigitsCommandExecute, IncreaseDigitsCommandCanExecute));

        private bool IncreaseDigitsCommandCanExecute() => Digits < 5;

        private void IncreaseDigitsCommandExecute() => Digits++;

        #endregion

        #region Overrides of ResultWaferVM

        public override void Dispose()
        {
            SummaryChart?.Dispose();
            ThumbnailViewerVm?.Dispose();
            ReportVm?.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
