using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper.Internal;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.TestResults
{
    public abstract class ResultDisplayWithRepetaVM : StepBaseVM
    {
        private bool _useRepeta = false;

        public bool UseRepeta
        {
            get => _useRepeta;
            set => SetProperty(ref _useRepeta, value);
        }

        private int _nbRepeats = 2;

        public int NbRepeats
        {
            get => _nbRepeats;
            set => SetProperty(ref _nbRepeats, value);
        }

        private AutoRelayCommand _startStaticRepetaCommand;
        public AutoRelayCommand StartStaticRepetaCommand => _startStaticRepetaCommand ?? (_startStaticRepetaCommand = new AutoRelayCommand(StartStaticRepeta));

        private AutoRelayCommand _stopStaticRepetaCmmand;
        public AutoRelayCommand StopStaticRepetaCommand => _stopStaticRepetaCmmand ?? (_stopStaticRepetaCmmand = new AutoRelayCommand(StopStaticRepeta));

        private AutoRelayCommand _exportCsvCommand;
        public AutoRelayCommand ExportCsvCommand => _exportCsvCommand ?? (_exportCsvCommand = new AutoRelayCommand(ExportCSV));

        private bool _isStaticRepetaInProgress = false;

        public bool IsStaticRepetaInProgress
        {
            get => _isStaticRepetaInProgress;
            set => SetProperty(ref _isStaticRepetaInProgress, value);
        }

        private bool _showRepetaResults = false;

        public bool ShowRepetaResults
        {
            get => _showRepetaResults;
            set => SetProperty(ref _showRepetaResults, value);
        }

        // Stats results might be a Length or and Angle (in case of Mountains results)
        private dynamic _staticRepetaStatMin;

        public dynamic StaticRepetaStatMin
        {
            get => _staticRepetaStatMin;
            set => SetProperty(ref _staticRepetaStatMin, value);
        }

        private dynamic _staticRepetaStatMax;

        public dynamic StaticRepetaStatMax
        {
            get => _staticRepetaStatMax;
            set => SetProperty(ref _staticRepetaStatMax, value);
        }

        private dynamic _staticRepetaStatMean;

        public dynamic StaticRepetaStatMean
        {
            get => _staticRepetaStatMean;
            set => SetProperty(ref _staticRepetaStatMean, value);
        }

        private dynamic _staticRepetaStatStdDev;

        public dynamic StaticRepetaStatStdDev
        {
            get => _staticRepetaStatStdDev;
            set => SetProperty(ref _staticRepetaStatStdDev, value);
        }

        public ObservableCollection<MeasurePointResultVM> MeasurePointsResults { get; set; } = new ObservableCollection<MeasurePointResultVM>();

        protected abstract void StartStaticRepetaForMeasure(int nbRepeats);

        protected abstract MeasurePointResultVM GetStaticRepetaNewMeasurePointResultVM(int repeatIndex, int nbRepeat);

        protected abstract void BuildStaticRepetaMeasurePointResultStats(List<MeasurePointDataResultBase> dataResults);

        protected abstract void UpdateStaticRepetaSelectedStats();

        protected abstract string GetStaticRepetaMeasureExportTitle();

        protected abstract CSVStringBuilder BuildStaticRepetaCSVExportStringBuilder();

        protected dynamic GetStatQuantityFromIStatObjet(object statValue, string unitForDouble = "")
        {
            try
            {
                if (statValue is null)
                {
                    return null;
                }

                if (statValue is Length lengthStat)
                {
                    return lengthStat;
                }

                if (statValue is Angle angleStat)
                {
                    return angleStat;
                }

                if ((double.TryParse(statValue.ToString(), out double statValueDouble)) && !double.IsNaN(statValueDouble))
                {
                    if (Length.TryGetLengthUnit(unitForDouble, out var lengthUnit))
                    {
                        return new Length(statValueDouble, lengthUnit);
                    }

                    if (Angle.TryGetAngleUnit(unitForDouble, out var angleUnit))
                    {
                        return new Angle(statValueDouble, angleUnit);
                    }
                }

                return statValue;
            }
            catch 
            { 
                return statValue;
            }
        }

        private void StartStaticRepeta()
        {
            try
            {
                IsStaticRepetaInProgress = true;
                ShowRepetaResults = false;

                if (MeasurePointsResults.Count != 0)
                {
                    MeasurePointsResults.Clear();
                }

                ServiceLocator.MeasureSupervisor.StaticMeasureResultStartedEvent += StartStaticRepetaResult;
                ServiceLocator.MeasureSupervisor.StaticMeasureResultChangedEvent += UpdateStaticRepetaResult;

                Task.Run(() =>
                {
                    StartStaticRepetaForMeasure(NbRepeats);
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on StartStaticRepeta");
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, $"Error on StartStaticRepeta {ex.Message}");
            }
        }

        private void StopStaticRepeta()
        {
            ServiceLocator.MeasureSupervisor.StopStaticRepetaMeasure();
            MeasurePointsResults.ForAll(m => { if (m.IsInProgress) m.IsInProgress = false; });
            IsStaticRepetaInProgress = false;
            RemoveStaticRepetaEvents();
        }

        private void StartStaticRepetaResult(int repeatIndex)
        {
            var measurePointResult = GetStaticRepetaNewMeasurePointResultVM(repeatIndex, NbRepeats);
            Application.Current.Dispatcher.Invoke(() =>
            {
                MeasurePointsResults.Add(measurePointResult);
            });
        }

        private void UpdateStaticRepetaResult(MeasurePointResult res, int repeatIndex)
        {
            var measurePointResult = MeasurePointsResults.Find(x => x.RepeatIndex == (repeatIndex + 1));
            measurePointResult.Result = res;
            measurePointResult.IsInProgress = false;

            if (NbRepeats == repeatIndex + 1)
            {
                FinishStaticRepeatForAllResults();
            }
        }

        private void FinishStaticRepeatForAllResults()
        {
            Task.Run(() =>
            {
                IsStaticRepetaInProgress = false;
                RemoveStaticRepetaEvents();
                GenerateStaticRepeatStats();
                ShowRepetaResults = true;
            });
        }

        private void RemoveStaticRepetaEvents()
        {
            ServiceLocator.MeasureSupervisor.StaticMeasureResultStartedEvent -= StartStaticRepetaResult;
            ServiceLocator.MeasureSupervisor.StaticMeasureResultChangedEvent -= UpdateStaticRepetaResult;
        }

        private void GenerateStaticRepeatStats()
        {
            BuildStaticRepetaMeasurePointResultStats(MeasurePointsResults.Select(m => m.Result.Datas[0])?.ToList());
            UpdateStaticRepetaSelectedStats();
        }

        private void ExportCSV()
        {
            string exportTitle = GetStaticRepetaMeasureExportTitle();;
            string csvFileName = GetExportCSVFileName(exportTitle);
            if (string.IsNullOrEmpty(csvFileName))
            {
                return;
            }

            var sbCSV = BuildStaticRepetaCSVExportStringBuilder();

            try
            {
                using (var file = new StreamWriter(csvFileName, false, Encoding.UTF8))
                {
                    file.Write(sbCSV.ToString());
                }
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Static repeta results have been exported with success.", exportTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Failed to export the static repeta results", exportTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                var logger = ClassLocator.Default.GetInstance<ILogger>();
                logger.Error($"Failed to export the static repeta results {ex.Message}");
                return;
            }
        }

        private string GetExportCSVFileName(string exportTitle)
        {
            string csvFileName = string.Empty;
            var settings = new SaveFileDialogSettings
            {
                Title = exportTitle + ".csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "csv file (*.csv) | *.csv;",
                CheckFileExists = false
            };
            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

            bool? rep = dialogService.ShowSaveFileDialog(settings);
            if (rep.HasValue && rep.Value)
            {
                csvFileName = settings.FileName;
            }

            return csvFileName;
        }
    }
}
