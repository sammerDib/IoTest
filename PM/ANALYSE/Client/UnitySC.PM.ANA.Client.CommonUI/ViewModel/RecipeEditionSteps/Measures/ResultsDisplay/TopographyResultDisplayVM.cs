using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

using MvvmDialogs;

using UnitySC.PM.ANA.Client.CommonUI.Helpers;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.TestResults;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class TopographyResultDisplayVM : ResultDisplayWithRepetaVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public TopographyResultDisplayVM(TopographyPointResult topographyPointResult, TopographySettings topographySettings, string resultFolderPath, DieIndex dieIndex)
        {
            TopographySettings = topographySettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;
            TopographyPointResult = topographyPointResult;
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
        private TopographyPointResult _topographyPointResult = null;

        public TopographyPointResult TopographyPointResult
        {
            get => _topographyPointResult;
            set
            {
                if (_topographyPointResult != value)
                {
                    _topographyPointResult = value;
                    UpdateTopographyDetailResult();
                    OnPropertyChanged();
                }
            }
        }
        private bool _resultAvailable;
        public bool ResultAvailable
        {
            get => _resultAvailable; set { if (_resultAvailable != value) { _resultAvailable = value; OnPropertyChanged(); } }
        }

        private void UpdateTopographyDetailResult()
        {
            ExternalProcessingResults = null;
            if ((_topographySettings is null) || (TopographyPointResult is null) || string.IsNullOrEmpty(_resultFolderPath))
                return;

            TopographyDetailResult = new TopographyDetailMeasureInfoVM();
            TopographyDetailResult.Digits = 3;
            TopographyDetailResult.Settings = new UnitySC.Shared.Format.Metro.Topography.TopographyResultSettings();            
            TopographyDetailResult.Settings.ExternalProcessingOutputs = new System.Collections.Generic.List<ExternalProcessingOutput>();
            if (_topographySettings.PostProcessingSettings != null && _topographySettings.PostProcessingSettings.IsEnabled)
            {
                foreach (var output in _topographySettings.PostProcessingSettings.Outputs.Where(x => x.IsUsed))
                {
                    var resultOutput = new ExternalProcessingOutput();
                    resultOutput.Name = output.Name;
                    resultOutput.OutputTarget = output.Target;
                    resultOutput.OutputTolerance = new Tolerance() { Unit = ToleranceUnit.AbsoluteValue, Value = output.Tolerance };
                    TopographyDetailResult.Settings.ExternalProcessingOutputs.Add(resultOutput);
                }
            }

            var result = new TopographyResult();
            result.Settings = TopographyDetailResult.Settings;
            result.Points = new List<MeasurePointResult>();
            result.Points.Add(TopographyPointResult);
            result.FillNonSerializedProperties(true, true);

            if (TopographyPointResult.State != MeasureState.NotMeasured)
            {
                TopographyPointResult.GenerateStats();
                TopographyDetailResult.Point = TopographyPointResult;
                if (!(_dieIndex is null))
                    TopographyDetailResult.DieIndex = $"Column  {_dieIndex.Column}    Row  {_dieIndex.Row}";
                if (TopographyDetailResult.Point.Datas.Count > 0)
                {
                    var fileName = ((TopographyPointData)TopographyDetailResult.Point.Datas[0]).ResultImageFileName;
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        Topo = new MatrixThumbnailViewerVM(new TopographyPointSelector(), point => point is TopographyPointData topographyPoint ? topographyPoint.ResultImageFileName : null);
                        Topo.LoadFile(Path.Combine(_resultFolderPath, fileName));
                    }

                    ExternalProcessingResults = ((TopographyPointData)TopographyDetailResult.Point.Datas[0]).ExternalProcessingResults;
                    if (ExternalProcessingResults?.Count > 0)
                        TopographyDetailResult.Output = ExternalProcessingResults.First().Name;
                    else
                        TopographyDetailResult.Output = "";

                }
                else
                {
                    TopographyDetailResult.Output = "";
                }

                ResultAvailable = true;
            }
            else
            {
                ResultAvailable = false;
            }
            TopographyDetailResult.Point = TopographyPointResult;
            OnPropertyChanged(nameof(TopographyDetailResult));

        }

        private TopographySettings _topographySettings;
        public TopographySettings TopographySettings
        {
            get => _topographySettings; private set { if (_topographySettings != value) { _topographySettings = value; OnPropertyChanged(); } }
        }

        private string _resultFolderPath;
        public string ResultFolderPath
        {
            get => _resultFolderPath; private set { if (_resultFolderPath != value) { _resultFolderPath = value; OnPropertyChanged(); } }
        }

        private DieIndex _dieIndex;
        public DieIndex DieIndex
        {
            get => _dieIndex; private set { if (_dieIndex != value) { _dieIndex = value; OnPropertyChanged(); } }
        }

        private TopographyDetailMeasureInfoVM _topographyDetailResult = null;
        public TopographyDetailMeasureInfoVM TopographyDetailResult
        {
            get => _topographyDetailResult; private set { if (_topographyDetailResult != value) { _topographyDetailResult = value; OnPropertyChanged(); } }
        }
        private MatrixThumbnailViewerVM _topo;
        public MatrixThumbnailViewerVM Topo
        {
            get => _topo; set { if (_topo != value) { _topo = value; OnPropertyChanged(); } }
        }

        private List<ExternalProcessingResult> _externalProcessingResults;
        public List<ExternalProcessingResult> ExternalProcessingResults
        {
            get => _externalProcessingResults; set { if (_externalProcessingResults != value) { _externalProcessingResults = value; OnPropertyChanged(); } }
        }

        #region Static Repeta

        private readonly TopographyPointResult _statsMeasurePointResult = new TopographyPointResult();

        private string _selectedStatUnit = "";

        private ObservableCollection<string> _staticRepetaAvailableStatTypes = new ObservableCollection<string>();

        public ObservableCollection<string> StaticRepetaAvailableStatTypes
        {
            get => _staticRepetaAvailableStatTypes;
            set => SetProperty(ref _staticRepetaAvailableStatTypes, value);
        }

        private string _staticRepetaStatTypeSelected = string.Empty;

        public string StaticRepetaStatTypeSelected
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

        protected override MeasurePointResultVM GetStaticRepetaNewMeasurePointResultVM(int repeatIndex, int nbRepeat)
        {
            return new MeasurePointResultVM(
                ResourceHelper.GetMeasureName(MeasureType.NanoTopo),
                -1,
                null,
                new Point(TopographyPointResult.XPosition, TopographyPointResult.YPosition),
                repeatIndex + 1,
                nbRepeat,
                true);
        }

        protected override void StartStaticRepetaForMeasure(int nbRepeats)
        {
            var measurePoint = new MeasurePoint(0, TopographyPointResult.XPosition, TopographyPointResult.YPosition, false);
            ServiceLocator.MeasureSupervisor.StartStaticRepetaMeasure(TopographySettings, measurePoint, nbRepeats);
        }

        protected override void BuildStaticRepetaMeasurePointResultStats(List<MeasurePointDataResultBase> dataResults)
        {
            _statsMeasurePointResult.Datas.AddRange(dataResults);
            _statsMeasurePointResult.GenerateStats();

            // Fill StaticRepetaAvailableStatTypes
            var staticRepetaAvailableStatTypes = new List<string>();
            if (_statsMeasurePointResult.Datas.Count > 0)
            {
                if (_statsMeasurePointResult.ExternalProcessingStats != null && _statsMeasurePointResult.ExternalProcessingStats.Count > 0)
                {
                    staticRepetaAvailableStatTypes.AddRange(_statsMeasurePointResult.ExternalProcessingStats.Keys);
                }
            }

            StaticRepetaAvailableStatTypes = new ObservableCollection<string>(staticRepetaAvailableStatTypes);

            StaticRepetaStatTypeSelected = StaticRepetaAvailableStatTypes.Count > 0 ? StaticRepetaAvailableStatTypes[0] : string.Empty;
        }

        protected override void UpdateStaticRepetaSelectedStats()
        {
            IStatsContainer staticRepetaSelectedStat = MetroStatsContainer.Empty;
            switch (_staticRepetaStatTypeSelected)
            {
                case "":
                    staticRepetaSelectedStat = null;
                    break;
                default:
                    staticRepetaSelectedStat = _statsMeasurePointResult.ExternalProcessingStats[_staticRepetaStatTypeSelected];
                    var statsMeasurePointData = _statsMeasurePointResult.Datas[0] as TopographyPointData;
                    _selectedStatUnit = statsMeasurePointData.ExternalProcessingResults.FirstOrDefault(e => e.Name == _staticRepetaStatTypeSelected).Unit;
                    break;
            }

            StaticRepetaStatMean = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.Mean, _selectedStatUnit);
            StaticRepetaStatStdDev = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.StdDev, _selectedStatUnit);
            StaticRepetaStatMin = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.Min, _selectedStatUnit);
            StaticRepetaStatMax = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.Max, _selectedStatUnit);
        }

        protected override string GetStaticRepetaMeasureExportTitle()
        {
            return "Topography Static repeta export";
        }

        protected override CSVStringBuilder BuildStaticRepetaCSVExportStringBuilder()
        {
            var sbCSV = new CSVStringBuilder();
            if (!string.IsNullOrEmpty(_staticRepetaStatTypeSelected))
            {
                sbCSV.Append("Index");
                sbCSV.AppendLineWithoutFinalDelim($"{_staticRepetaStatTypeSelected} ({_selectedStatUnit})");

                _statsMeasurePointResult.Datas.OfType<TopographyPointData>().ToList().ForEach(data =>
                {
                    AppendCSVLineForOneData(ref sbCSV, data);
                });

                sbCSV.AppendLine_NoDelim(string.Empty);

                AppendCSVLineForOneStat(ref sbCSV, "Min", StaticRepetaStatMin);
                AppendCSVLineForOneStat(ref sbCSV, "Max", StaticRepetaStatMax);
                AppendCSVLineForOneStat(ref sbCSV, "Avg", StaticRepetaStatMean);
                AppendCSVLineForOneStat(ref sbCSV, "Std Dev", StaticRepetaStatStdDev);
            }

            return sbCSV;
        }

        private void AppendCSVLineForOneData(ref CSVStringBuilder sbCSV, TopographyPointData data)
        {
            string dataValue;
            switch (_staticRepetaStatTypeSelected)
            {
                case "":
                    dataValue = "";
                    break;
                default:
                    dataValue = $"{data.ExternalProcessingResults.FirstOrDefault(r => r.Name == _staticRepetaStatTypeSelected)?.Value}";
                    break;
            }
            sbCSV.AppendLineWithoutFinalDelim($"{data.IndexRepeta + 1}, {dataValue}");
        }

        private void AppendCSVLineForOneStat(ref CSVStringBuilder sbCSV, string statsTitle, Length statLength)
        {
            sbCSV.AppendLineWithoutFinalDelim(statsTitle, $"{statLength?.Micrometers}, {statLength?.Micrometers}");
        }

        private void AppendCSVLineForOneStat(ref CSVStringBuilder sbCSV, string statsTitle, Angle statAngle)
        {
            sbCSV.AppendLineWithoutFinalDelim(statsTitle, $"{statAngle?.Degrees}");
        }

        private void AppendCSVLineForOneStat(ref CSVStringBuilder sbCSV, string statsTitle, double statValue)
        {
            sbCSV.AppendLineWithoutFinalDelim(statsTitle, $"{statValue}");
        }
        #endregion
    }
}
