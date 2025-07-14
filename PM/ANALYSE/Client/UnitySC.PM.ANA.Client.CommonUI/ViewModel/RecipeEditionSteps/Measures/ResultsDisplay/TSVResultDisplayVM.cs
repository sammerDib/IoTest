using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

using MvvmDialogs;

using UnitySC.PM.ANA.Client.CommonUI.Helpers;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.TestResults;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Common.ViewModel.Dialogs;
using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class TSVResultDisplayVM : ResultDisplayWithRepetaVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public TSVResultDisplayVM(TSVPointResult tsvPointResult, TSVSettings tsvSettings, string resultFolderPath, DieIndex dieIndex)
        {
            TSVResult = tsvPointResult;
            TSVSettings = tsvSettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;

            StaticRepetaAvailableStatTypes.Clear();
            StaticRepetaAvailableStatTypes.Add(TSVStatType.Depth);

            if (TSVResult.TSVDatas[0].Width != null && TSVResult.TSVDatas[0].Length != null)
            {
                if (TSVResult.TSVDatas[0].Width.Value == TSVResult.TSVDatas[0].Length.Value)
                {
                    StaticRepetaAvailableStatTypes.Add(TSVStatType.Diameter);
                }
                else
                {
                    StaticRepetaAvailableStatTypes.Add(TSVStatType.Width);
                    StaticRepetaAvailableStatTypes.Add(TSVStatType.Length);
                }
            }
            else
            {
                if (TSVResult.TSVDatas[0].Width != null)
                {
                    StaticRepetaAvailableStatTypes.Add(TSVStatType.Width);
                }
                else if (TSVResult.TSVDatas[0].Length != null)
                {
                    StaticRepetaAvailableStatTypes.Add(TSVStatType.Length);
                }
            }

            UpdateTSVDetailResult();
        }

        private TSVPointResult _tsvResult = null;

        public TSVPointResult TSVResult
        {
            get => _tsvResult;
            private set
            {
                if (_tsvResult != value)
                {
                    _tsvResult = value;
                    UpdateTSVDetailResult();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateTSVDetailResult()
        {
            ResultImage = null;

            if ((TSVSettings is null) || (TSVResult is null) || string.IsNullOrEmpty(ResultFolderPath))
            {
                return;
            }

            TSVDetailResult = new TsvDetailMeasureInfoVM();
            TSVDetailResult.Settings = new TSVResultSettings();
            TSVDetailResult.Settings.WidthTarget = TSVSettings.WidthTarget;
            TSVDetailResult.Settings.WidthTolerance = TSVSettings.WidthTolerance;
            TSVDetailResult.Settings.DepthTarget = TSVSettings.DepthTarget;
            TSVDetailResult.Settings.DepthTolerance = TSVSettings.DepthTolerance;
            TSVDetailResult.Settings.LengthTarget = TSVSettings.LengthTarget;
            TSVDetailResult.Settings.LengthTolerance = TSVSettings.LengthTolerance;
            TSVDetailResult.Settings.Shape = TSVSettings.Shape;
            TSVDetailResult.Digits = 3;
            TSVResult.GenerateStats();
            TSVDetailResult.Point = TSVResult;

            if (!(DieIndex is null))
            {
                TSVDetailResult.DieIndex = $"Column  {DieIndex.Column}    Row  {DieIndex.Row}";
            }

            if (TSVDetailResult.Point.Datas.Count > 0)
            {
                var fileName = ((TSVPointData)TSVDetailResult.Point.Datas[0]).ResultImageFileName;
                if (!String.IsNullOrEmpty(fileName))
                {
                    var resultImagePath = Path.Combine(ResultFolderPath, fileName);
                    _imagePath = resultImagePath;
                    ResultImage = new BitmapImage(new Uri(resultImagePath));
                }
            }

            OnPropertyChanged(nameof(TSVDetailResult));
        }

        private TSVSettings _tsvSettings = null;

        public TSVSettings TSVSettings
        {
            get => _tsvSettings;
            private set => SetProperty(ref _tsvSettings, value);
        }

        private string _resultFolderPath;

        public string ResultFolderPath
        {
            get => _resultFolderPath;
            private set => SetProperty(ref _resultFolderPath, value);
        }

        private BitmapImage _resultImage;

        public BitmapImage ResultImage
        {
            get => _resultImage;
            private set { if (_resultImage != value) { _resultImage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasImage)); } }
        }

        private DieIndex _dieIndex;

        public DieIndex DieIndex
        {
            get => _dieIndex;
            private set => SetProperty(ref _dieIndex, value);
        }

        private TsvDetailMeasureInfoVM _tsvDetailResult = null;

        public TsvDetailMeasureInfoVM TSVDetailResult
        {
            get => _tsvDetailResult;
            private set => SetProperty(ref _tsvDetailResult, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        public bool HasImage => ResultImage != null;

        private string _imagePath;

        protected bool OpenImageViewerCommandCanExecute() => !string.IsNullOrWhiteSpace(_imagePath);

        protected void OpenImageViewerCommandExecute()
        {
            if (string.IsNullOrWhiteSpace(_imagePath)) return;

            string extension = Path.GetExtension(_imagePath);
            string fileName = Path.GetFileName(_imagePath);
            var image = new BitmapImage(new Uri(_imagePath));

            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

            void ExportAction(string destFileName)
            {
                try
                {
                    File.Copy(_imagePath, destFileName);
                }
                catch (Exception)
                {
                    // cannot copy image exception
                    // do not display a modal message box here. either make a log warning or notify it via our notifier
                    Logger.Warning($"Cannot Export TSV Test Thumbnail in <{destFileName}>");
                }
            }

            var imageViewer = new ImageViewerViewModel(image, ExportAction, extension, fileName, false);
            dialogService.Show(this, new GenericMvvmDialogViewModel("Thumbnail Viewer", imageViewer));
        }

        private AutoRelayCommand _openImageViewerCommand;
        public AutoRelayCommand OpenImageViewerCommand => _openImageViewerCommand ?? (_openImageViewerCommand = new AutoRelayCommand(OpenImageViewerCommandExecute, OpenImageViewerCommandCanExecute));

        #region Static Repeta

        public enum TSVStatType
        {
            Depth,
            Width,
            Length,
            Diameter
        }

        private ObservableCollection<TSVStatType> _staticRepetaAvailableStatTypes = new ObservableCollection<TSVStatType>();

        public ObservableCollection<TSVStatType> StaticRepetaAvailableStatTypes
        {
            get => _staticRepetaAvailableStatTypes;
            set => SetProperty(ref _staticRepetaAvailableStatTypes, value);
        }

        private TSVStatType _staticRepetaStatTypeSelected = TSVStatType.Depth;

        public TSVStatType StaticRepetaStatTypeSelected
        {
            get => _staticRepetaStatTypeSelected;
            set
            {
                if (_staticRepetaStatTypeSelected != value)
                {
                    _staticRepetaStatTypeSelected = value;
                    UpdateStaticRepetaSelectedStats();
                    OnPropertyChanged();
                }
            }
        }

        private readonly TSVPointResult _statsMeasurePointResult = new TSVPointResult();

        protected override MeasurePointResultVM GetStaticRepetaNewMeasurePointResultVM(int repeatIndex, int nbRepeat)
        {
            return new MeasurePointResultVM(
                ResourceHelper.GetMeasureName(MeasureType.TSV),
                -1,
                null,
                new Point(TSVResult.XPosition, TSVResult.YPosition),
                repeatIndex + 1,
                nbRepeat,
                true);
        }

        protected override void StartStaticRepetaForMeasure(int nbRepeats)
        {
            var measurePoint = new MeasurePoint(0, TSVResult.XPosition, TSVResult.YPosition, false);
            ServiceLocator.MeasureSupervisor.StartStaticRepetaMeasure(TSVSettings, measurePoint, nbRepeats);
        }

        protected override void BuildStaticRepetaMeasurePointResultStats(List<MeasurePointDataResultBase> dataResults)
        {
            _statsMeasurePointResult.Datas.AddRange(dataResults);
            _statsMeasurePointResult.GenerateStats();
        }

        protected override void UpdateStaticRepetaSelectedStats()
        {
            MetroStatsContainer metroStatSelected = null;
            switch (StaticRepetaStatTypeSelected)
            {
                case TSVStatType.Depth:
                    metroStatSelected = _statsMeasurePointResult.DepthTsvStat;
                    break;
                case TSVStatType.Width:
                case TSVStatType.Diameter:
                    metroStatSelected = _statsMeasurePointResult.WidthTsvStat;
                    break;
                case TSVStatType.Length:
                    metroStatSelected = _statsMeasurePointResult.LengthTsvStat;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Missing stat type = {StaticRepetaStatTypeSelected}");
            }

            StaticRepetaStatMin = metroStatSelected.Min;
            StaticRepetaStatMax = metroStatSelected.Max;
            StaticRepetaStatMean = metroStatSelected.Mean;
            StaticRepetaStatStdDev = metroStatSelected.StdDev;
        }

        protected override string GetStaticRepetaMeasureExportTitle()
        {
            return "TSV Static repeta export";
        }

        protected override CSVStringBuilder BuildStaticRepetaCSVExportStringBuilder()
        {
            var sbCSV = new CSVStringBuilder();
            bool withDiameter = StaticRepetaAvailableStatTypes.Any(s => s == TSVStatType.Diameter);
            sbCSV.Append("Index", "Depth (µm)");
            sbCSV.AppendLineWithoutFinalDelim(withDiameter ? "Diameter (µm)" : "Width (µm), Length (µm)");

            _statsMeasurePointResult.TSVDatas.ForEach(data =>
            {
                AppendCSVLineForOneData(ref sbCSV, data, withDiameter);
            });

            sbCSV.AppendLine_NoDelim(string.Empty);

            AppendCSVLineForOneStat(ref sbCSV, "Min", withDiameter, _statsMeasurePointResult.DepthTsvStat.Min, _statsMeasurePointResult.WidthTsvStat.Min, _statsMeasurePointResult.LengthTsvStat.Min);
            AppendCSVLineForOneStat(ref sbCSV, "Max", withDiameter, _statsMeasurePointResult.DepthTsvStat.Max, _statsMeasurePointResult.WidthTsvStat.Max, _statsMeasurePointResult.LengthTsvStat.Max);
            AppendCSVLineForOneStat(ref sbCSV, "Avg", withDiameter, _statsMeasurePointResult.DepthTsvStat.Mean, _statsMeasurePointResult.WidthTsvStat.Mean, _statsMeasurePointResult.LengthTsvStat.Mean);
            AppendCSVLineForOneStat(ref sbCSV, "Std Dev", withDiameter, _statsMeasurePointResult.DepthTsvStat.StdDev, _statsMeasurePointResult.WidthTsvStat.StdDev, _statsMeasurePointResult.LengthTsvStat.StdDev);

            return sbCSV;
        }

        private void AppendCSVLineForOneData(ref CSVStringBuilder sbCSV, TSVPointData data, bool withDiameter)
        {
            sbCSV.Append($"{data.IndexRepeta + 1}, {data.Depth?.Micrometers}");
            sbCSV.AppendLineWithoutFinalDelim(withDiameter ? $"{data.Width?.Micrometers}" : $"{data.Width?.Micrometers}, {data.Length?.Micrometers}");
        }

        private void AppendCSVLineForOneStat(ref CSVStringBuilder sbCSV, string statsTitle, bool withDiameter, Length depth, Length width, Length length)
        {
            sbCSV.Append(statsTitle, $"{depth?.Micrometers}");
            sbCSV.AppendLineWithoutFinalDelim(withDiameter ? $"{width?.Micrometers}" : $"{width?.Micrometers}, {length?.Micrometers}");
        }

        #endregion
    }
}
