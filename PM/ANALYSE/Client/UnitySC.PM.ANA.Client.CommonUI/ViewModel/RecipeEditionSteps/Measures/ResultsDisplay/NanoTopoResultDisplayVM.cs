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
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class NanoTopoResultDisplayVM : ResultDisplayWithRepetaVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public NanoTopoResultDisplayVM(NanoTopoPointResult nanoTopoPointResult, NanoTopoSettings nanoSettings, string resultFolderPath, DieIndex dieIndex)
        {
            NanoTopoSettings = nanoSettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;
            NanoTopoResult = nanoTopoPointResult;
        }

        private NanoTopoPointResult _nanoTopoResult = null;

        public NanoTopoPointResult NanoTopoResult
        {
            get => _nanoTopoResult;
            set
            {
                if (_nanoTopoResult != value)
                {
                    _nanoTopoResult = value;
                    UpdateNanoTopoDetailResult();
                    OnPropertyChanged();
                }
            }
        }

        private bool _resultAvailable;
        public bool ResultAvailable
        {
            get => _resultAvailable; set { if (_resultAvailable != value) { _resultAvailable = value; OnPropertyChanged(); } }
        }

        private void UpdateNanoTopoDetailResult()
        {
            ExternalProcessingResults = null;
            if ((_nanoTopoSettings is null) || (NanoTopoResult is null) || string.IsNullOrEmpty(_resultFolderPath))
                return;

            NanoTopoDetailResult = new NanotopoDetailMeasureInfoVM();
            NanoTopoDetailResult.Digits = 3;
            NanoTopoDetailResult.Settings = new NanoTopoResultSettings();
            NanoTopoDetailResult.Settings.RoughnessTolerance = _nanoTopoSettings.RoughnessTolerance;
            NanoTopoDetailResult.Settings.RoughnessTarget = _nanoTopoSettings.RoughnessTarget;
            NanoTopoDetailResult.Settings.StepHeightTolerance = _nanoTopoSettings.StepHeightTolerance;
            NanoTopoDetailResult.Settings.StepHeightTarget = _nanoTopoSettings.StepHeightTarget;
            NanoTopoDetailResult.Settings.ExternalProcessingOutputs = new List<ExternalProcessingOutput>();
            if (_nanoTopoSettings.PostProcessingSettings != null && _nanoTopoSettings.PostProcessingSettings.IsEnabled)
            {
                foreach (var output in _nanoTopoSettings.PostProcessingSettings.Outputs.Where(x => x.IsUsed))
                {
                    var resultOutput = new ExternalProcessingOutput
                    {
                        Name = output.Name,
                        OutputTarget = output.Target,
                        OutputTolerance = new Tolerance() { Unit = ToleranceUnit.AbsoluteValue, Value = output.Tolerance }
                    };
                    NanoTopoDetailResult.Settings.ExternalProcessingOutputs.Add(resultOutput);
                }
            }
            var result = new NanoTopoResult();
            result.Settings = NanoTopoDetailResult.Settings;
            result.Points = new List<MeasurePointResult>
            {
                NanoTopoResult
            };
            result.FillNonSerializedProperties(true, true);

            if (NanoTopoResult.State != MeasureState.NotMeasured)
            {
                NanoTopoResult.GenerateStats();
                NanoTopoDetailResult.Point = NanoTopoResult;
                if (!(_dieIndex is null))
                    NanoTopoDetailResult.DieIndex = $"Column  {_dieIndex.Column}    Row  {_dieIndex.Row}";
                if (NanoTopoDetailResult.Point.Datas.Count > 0)
                {
                    var fileName = ((NanoTopoPointData)NanoTopoDetailResult.Point.Datas[0]).ResultImageFileName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Topo = new MatrixThumbnailViewerVM(new NanotopoPointSelector(), point => point is NanoTopoPointData nanoTopoPoint ? nanoTopoPoint.ResultImageFileName : null);
                        Topo.LoadFile(Path.Combine(_resultFolderPath, fileName));
                    }

                    ExternalProcessingResults = ((NanoTopoPointData)NanoTopoDetailResult.Point.Datas[0]).ExternalProcessingResults;
                    if (ExternalProcessingResults?.Count > 0)
                        NanoTopoDetailResult.Output = ExternalProcessingResults.First().Name;
                    else
                        NanoTopoDetailResult.Output = "";
                }
                else
                {
                    NanoTopoDetailResult.Output = "";
                }

                ResultAvailable = true;
            }
            else
            {
                ResultAvailable = false;
            }
            NanoTopoDetailResult.Point = NanoTopoResult;
            OnPropertyChanged(nameof(NanoTopoDetailResult));
        }

        private NanoTopoSettings _nanoTopoSettings;
        public NanoTopoSettings NanoTopoSettings
        {
            get => _nanoTopoSettings; private set { if (_nanoTopoSettings != value) { _nanoTopoSettings = value; OnPropertyChanged(); } }
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

        private NanotopoDetailMeasureInfoVM _nanoTopoDetailResult = null;
        public NanotopoDetailMeasureInfoVM NanoTopoDetailResult
        {
            get => _nanoTopoDetailResult; private set { if (_nanoTopoDetailResult != value) { _nanoTopoDetailResult = value; OnPropertyChanged(); } }
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

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        // Todo display Thumbnail3DChart Cf. viewer

        /* protected override void LoadFile(string filePath)
         {
             Task.Factory.StartNew(() =>
             {
                 using (var format3daFile = new MatrixFloatFile(filePath, -1))
                 {
                     _matrix = MatrixFloatFile.AggregateChunks(format3daFile.GetChunkStatus(), format3daFile);
                     _width = format3daFile.Header.Width;
                     _height = format3daFile.Header.Height;
                     _matrixUnit = format3daFile.Header.AdditionnalHeaderData.UnitLabelZ;
                     Thumbnail3DChart.SetData(_width, _height, _matrix);
                     GenerateBitmap();
                 }

                 RaiseCommandsCanExecute();
             });
         }*/

        #region Static Repeta

        private readonly NanoTopoPointResult _statsMeasurePointResult = new NanoTopoPointResult();

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
                new Point(NanoTopoResult.XPosition, NanoTopoResult.YPosition),
                repeatIndex + 1,
                nbRepeat,
                true);
        }

        protected override void StartStaticRepetaForMeasure(int nbRepeats)
        {
            var measurePoint = new MeasurePoint(0, NanoTopoResult.XPosition, NanoTopoResult.YPosition, false);
            ServiceLocator.MeasureSupervisor.StartStaticRepetaMeasure(NanoTopoSettings, measurePoint, nbRepeats);
        }

        protected override void BuildStaticRepetaMeasurePointResultStats(List<MeasurePointDataResultBase> dataResults)
        {
            _statsMeasurePointResult.Datas.AddRange(dataResults);
            _statsMeasurePointResult.GenerateStats();

            // Fill StaticRepetaAvailableStatTypes
            var staticRepetaAvailableStatTypes = new List<string>();
            if (_statsMeasurePointResult.Datas.Count > 0)
            {
                if (_statsMeasurePointResult.RoughnessStat != null && _statsMeasurePointResult.RoughnessStat.Mean != null)
                {
                    staticRepetaAvailableStatTypes.Add("Roughness");
                }

                if (_statsMeasurePointResult.StepHeightStat != null && _statsMeasurePointResult.StepHeightStat.Mean != null)
                {
                    staticRepetaAvailableStatTypes.Add("StepHeight");
                }

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
                case "Roughness":
                    staticRepetaSelectedStat = _statsMeasurePointResult.RoughnessStat;
                    _selectedStatUnit = Length.GetUnitSymbol(LengthUnit.Nanometer);
                    break;
                case "StepHeight":
                    staticRepetaSelectedStat = _statsMeasurePointResult.StepHeightStat;
                    _selectedStatUnit = Length.GetUnitSymbol(LengthUnit.Nanometer);
                    break;
                case "":
                    staticRepetaSelectedStat = null;
                    break;
                default:
                    staticRepetaSelectedStat = _statsMeasurePointResult.ExternalProcessingStats[_staticRepetaStatTypeSelected];
                    var statsMeasurePointData = _statsMeasurePointResult.Datas[0] as NanoTopoPointData;
                    _selectedStatUnit = statsMeasurePointData.ExternalProcessingResults.FirstOrDefault(e => e.Name == _staticRepetaStatTypeSelected)?.Unit;
                    break;
            }

            StaticRepetaStatMean = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.Mean, _selectedStatUnit);
            StaticRepetaStatStdDev = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.StdDev, _selectedStatUnit);
            StaticRepetaStatMin = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.Min, _selectedStatUnit);
            StaticRepetaStatMax = GetStatQuantityFromIStatObjet(staticRepetaSelectedStat?.Max, _selectedStatUnit);
        }

        protected override string GetStaticRepetaMeasureExportTitle()
        {
            return "NanoTopo Static repeta export";
        }

        protected override CSVStringBuilder BuildStaticRepetaCSVExportStringBuilder()
        {
            var sbCSV = new CSVStringBuilder();
            if (!string.IsNullOrEmpty(_staticRepetaStatTypeSelected))
            {
                sbCSV.Append("Index");
                sbCSV.AppendLineWithoutFinalDelim($"{_staticRepetaStatTypeSelected} ({_selectedStatUnit})");

                _statsMeasurePointResult.Datas.OfType<NanoTopoPointData>().ToList().ForEach(data =>
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

        private void AppendCSVLineForOneData(ref CSVStringBuilder sbCSV, NanoTopoPointData data)
        {
            string dataValue;
            switch (_staticRepetaStatTypeSelected)
            {
                case "Roughness":
                    dataValue = $"{data.Roughness.Micrometers}";
                    break;
                case "StepHeight":
                    dataValue = $"{data.StepHeight.Micrometers}";
                    break;
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
            sbCSV.AppendLineWithoutFinalDelim(statsTitle, $"{statLength?.Micrometers}");
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
