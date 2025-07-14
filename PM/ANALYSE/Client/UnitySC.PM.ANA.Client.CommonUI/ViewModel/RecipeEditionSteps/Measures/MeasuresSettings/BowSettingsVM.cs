using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class BowSettingsVM : MeasureSettingsVM
    {
        public BowSettingsVM(RecipeMeasureVM recipeMeasure)
        {
            RecipeMeasure = recipeMeasure;
      
            UpdateCompatibleProbes();

            recipeMeasure.CanMeasurePositionsBeOnDie = false;
        }

        private void UpdateSubMeasuresPositions(List<XYPosition> refSubMeasuresPositions, List<int> subMeasurePoints)
        {
            if (subMeasurePoints is null)
            {
                subMeasurePoints = new List<int>();
            }

            if (subMeasurePoints.Count == 0)
            {
                AddMeasurePoint(subMeasurePoints, 0, 0);
                foreach (var refPlanePosition in refSubMeasuresPositions)
                {
                    AddMeasurePoint(subMeasurePoints, refPlanePosition.X, refPlanePosition.Y);
                }
            }
        }

        private void AddMeasurePoint(List<int> subMeasurePoints, double X, double Y)
        {
            var siteId = RecipeMeasure.MeasurePoints.GetNextPointId();
            subMeasurePoints.Add(siteId);
            var newMeasurePoint = new MeasurePointVM(RecipeMeasure.MeasurePoints,
                new XYZTopZBottomPosition(new WaferReferential(), X, Y, 0, 0),
                siteId,
                false
            );
            newMeasurePoint.IsSubMeasurePoint = true;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                RecipeMeasure.MeasurePoints.Points.Add(newMeasurePoint);
            }));
        }

        public override async Task LoadSettingsAsync(MeasureSettingsBase measureSettings)
        {
            // If a loading was already in progress, we wait
            if (!SpinWait.SpinUntil(() => !IsLoading, 20000))
            {
                ClassLocator.Default.GetInstance<ILogger>().Error("BowSettings Loading failed");
                return;
            }

            if (!(measureSettings is BowSettings))
            {
                return;
            }

            IsLoading = true;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                RecipeMeasure.MeasurePoints.Points.Clear();
                var bowSettings = measureSettings as BowSettings;
                BowMax = bowSettings.BowMax;
                BowMin = bowSettings.BowMin;

                var measureTools = (ServiceLocator.MeasureSupervisor.GetMeasureTools(bowSettings)?.Result as BowMeasureTools);
                if (measureTools != null)
                {
                    UpdateSubMeasuresPositions(measureTools.DefaultReferencePlanePositions, bowSettings.SubMeasurePoints);
                }

                ProbeSelector.SelectedProbeId = bowSettings.ProbeSettings?.ProbeId;
                ProbeSelector.SetProbeSettings(bowSettings.ProbeSettings);

                CharacteristicsChanged = false;
            }));

            // Just to avoid a warning on the async
            await Task.Delay(5);
            IsLoading = false;
        }

        #region Properties

        public RecipeMeasureVM RecipeMeasure { get; set; }

        public IEnumerable<string> ToleranceUnits { get; private set; }

        private Length _bowMax = 100.Micrometers();

        public Length BowMax
        {
            get => _bowMax; set { if (_bowMax != value) { _bowMax = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private Length _bowMin = -50.Micrometers();

        public Length BowMin
        {
            get => _bowMin; set { if (_bowMin != value) { _bowMin = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private bool _characteristicsChanged = true;

        public bool CharacteristicsChanged
        {
            get => _characteristicsChanged; set { if (_characteristicsChanged != value) { _characteristicsChanged = value; OnPropertyChanged(); } }
        }

        private ProbeSelectorVM _probeSelector = new ProbeSelectorVM();

        public ProbeSelectorVM ProbeSelector
        {
            get => _probeSelector; set { if (_probeSelector != value) { _probeSelector = value; OnPropertyChanged(); } }
        }

        private bool _isEditingHardware = false;

        public bool IsEditingHardware
        {
            get => _isEditingHardware; set { if (_isEditingHardware != value) { _isEditingHardware = value; OnPropertyChanged(); } }
        }

        public override bool IsMeasureWithSubMeasurePoints { get; } = true;

        #endregion Properties

        #region Commands

        private AutoRelayCommand _submitCharacteristics;

        public AutoRelayCommand SubmitCharacteristics
        {
            get
            {
                return _submitCharacteristics ?? (_submitCharacteristics = new AutoRelayCommand(
                    () =>
                    {
                        ProbeSelector.IsEditing = false;
                        UpdateCompatibleProbes();
                        CharacteristicsChanged = false;
                        IsModified = true;
                    },
                    () => { return CharacteristicsChanged; }
                ));
            }
        }

        private AutoRelayCommand _startEditHardware;

        public AutoRelayCommand StartEditHardware
        {
            get
            {
                return _startEditHardware ?? (_startEditHardware = new AutoRelayCommand(
                    () =>
                    {
                        IsEditingHardware = true;
                        ProbeSelector.IsEditing = true;
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _submitHardware;

        public AutoRelayCommand SubmitHardware
        {
            get
            {
                return _submitHardware ?? (_submitHardware = new AutoRelayCommand(
                    () =>
                    {
                        IsEditingHardware = false;

                        ProbeSelector.IsEditing = false;
                        IsModified = true;
                    },
                    () => { return true; }
                ));
            }
        }

        #endregion Commands

        private void UpdateCompatibleProbes()
        {
            var newBowConfiguration = GetSettingsWithoutPoints();
            var measureTools = ServiceLocator.MeasureSupervisor.GetMeasureTools(newBowConfiguration)?.Result as BowMeasureTools;

            // Add the new probes
            foreach (var compatibleProbe in measureTools?.CompatibleProbes)
            {
                ProbeSelector.AddProbe(compatibleProbe);
            }

            // Remove the probes that are not anymore compatible
            foreach (var probe in ProbeSelector.Probes.ToList())
            {
                if (!measureTools.CompatibleProbes.Any(p => p.ProbeId == probe.ProbeMaterial.ProbeId))
                {
                    ProbeSelector.Probes.Remove(probe);
                }
            }
        }

        public override MeasureSettingsBase GetSettingsWithoutPoints()
        {
            var newBowSettings = new BowSettings();
            newBowSettings.NbOfRepeat = 1;
            newBowSettings.BowMax = BowMax;
            newBowSettings.BowMin = BowMin;
            newBowSettings.WaferCharacteristic = RecipeMeasure.EditedRecipe.WaferDimentionalCharacteristic;
            newBowSettings.ProbeSettings = ProbeSelector.GetSelectedProbeSettings() ?? new ProbeSettings();
            newBowSettings.SubMeasurePoints = new List<int>();
            newBowSettings.IsMeasureWithSubMeasurePoints = IsMeasureWithSubMeasurePoints;

            foreach (var point in RecipeMeasure.MeasurePoints.Points)
            {
                newBowSettings.SubMeasurePoints.Add(point.Id.Value);
            }

            return newBowSettings;
        }

        public override void Dispose()
        {
        }

        public override async Task PrepareToDisplayAsync()
        {
            // Enable Lights
            ServiceLocator.LightsSupervisor.LightsAreLocked = false;

            if (ProbeSelector.SelectedProbe?.SelectedObjective != null)
            {
                ServiceLocator.CamerasSupervisor.Objective = ProbeSelector.SelectedProbe.SelectedObjective;
            }
            else
            {
                ServiceLocator.CamerasSupervisor.Objective = ServiceLocator.CamerasSupervisor.MainObjective;
            }

            if (ProbeSelector.SelectedProbe?.ProbeMaterial?.ProbeId != null)
            {
                ServiceLocator.ProbesSupervisor.SetCurrentProbe(ProbeSelector.SelectedProbe.ProbeMaterial.ProbeId);
            }
            else
            {
                ServiceLocator.ProbesSupervisor.SetCurrentProbe(ServiceLocator.ProbesSupervisor.ProbeLiseUp.DeviceID);
            }

            if (RecipeMeasure.MeasurePoints.Points.Count == 0)
            {
                var newBowSettings = GetSettingsWithoutPoints();
                var measureTools = (ServiceLocator.MeasureSupervisor.GetMeasureTools(newBowSettings)?.Result as BowMeasureTools);
                if (measureTools != null)
                {
                    UpdateSubMeasuresPositions(measureTools.DefaultReferencePlanePositions, newBowSettings.SubMeasurePoints);
                }
            }

            await Task.Delay(5);
        }

        public override void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            if (!(result is BowPointResult bowPointResult))
            {
                return;
            }

            var bowResultDisplayVM = new BowResultDisplayVM(bowPointResult, (BowSettings)GetSettingsWithoutPoints(), resultFolderPath, dieIndex);

            ServiceLocator.DialogService.ShowDialog<BowResultDisplay>(bowResultDisplayVM);
        }

        public override void Hide()
        {
        }

        public override bool AreSettingsValid(ObservableCollection<MeasurePointVM> measurePoints, bool forTestOnPoint = false)
        {

            if ((!forTestOnPoint) && (measurePoints == null || measurePoints.Count == 0))
            {
                ValidationErrorMessage = "Measure points are not defined";
                return false;
            }

            ValidationErrorMessage = string.Empty;
    
            return true;
        }

        public override void DisplaySettingsTab()
        {
            RecipeMeasure.DisplayROI = false;
        }

        public override void HideSettingsTab()
        {
            
        }

        #region PatternRec objectives

        public override ObjectiveConfig GetObjectiveUsedByMeasure()
        {
            return ProbeSelector.SelectedProbe?.SelectedObjective;
        }

        #endregion
    }
}
