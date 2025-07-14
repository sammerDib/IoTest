using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector;
using UnitySC.PM.ANA.Client.Proxy.Helpers;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class WarpSettingsVM : MeasureSettingsVM
    {
        private readonly List<LayerViewModel> _layers = new List<LayerViewModel>();

        private readonly Length _theoricalWaferThickness = new Length(0d, LengthUnit.Micrometer);

        public WarpSettingsVM(RecipeMeasureVM recipeMeasure)
        {
            RecipeMeasure = recipeMeasure;
            var toolService = new ServiceInvoker<IToolService>("ToolService",
                ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(),
                ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress()
            );
            var step = toolService.Invoke(x => x.GetStep(RecipeMeasure.EditedRecipe.Step.Id));
            int colorIndex = 0;
            double totalLayersThickness = 0;
            foreach (var layer in step.Layers)
            {
                var newLayer = new LayerViewModel(step)
                { 
                    Name = layer.Name,
                    Thickness = new Length(layer.Thickness, LengthUnit.Micrometer),
                    RefractiveIndex = layer.RefractiveIndex,
                    LayerColor = Colors.Red
                };
                newLayer.LayerColor = LayersEditorViewModel.GetLayerColor(null, colorIndex);
                colorIndex++;
                _theoricalWaferThickness += newLayer.Thickness;
                _layers.Add(newLayer);
            }

            UpdateCompatibleProbes();
            recipeMeasure.CanMeasurePositionsBeOnDie = false;

            recipeMeasure.MeasurePoints.CustomPointsManagement = new WarpCustomPointsManagementVM(recipeMeasure.MeasurePoints);
        }

        private void UpdateSubMeasuresPositions(List<XYPosition> refSubMeasuresPositions, List<int> subMeasurePoints)
        {
            if (subMeasurePoints is null)
            {
                subMeasurePoints = new List<int>();
            }

            if (subMeasurePoints.Count == 0)
            {
                // TODO Warp : temporary add 0.0 point + 8 reference points according to angles
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
                ClassLocator.Default.GetInstance<ILogger>().Error("WarpSettings Loading failed");
                return;
            }

            if (!(measureSettings is WarpSettings))
            {
                return;
            }

            IsLoading = true;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                RecipeMeasure.MeasurePoints.Points.Clear();
                var warpSettings = measureSettings as WarpSettings;
                IsSurfaceWarp = warpSettings.IsSurfaceWarp;
                WarpMax = warpSettings.WarpMax;
                TotalThicknessTolerance = warpSettings.TotalThicknessTolerance;

                var measureTools = (ServiceLocator.MeasureSupervisor.GetMeasureTools(warpSettings)?.Result as WarpMeasureTools);
                if (measureTools != null)
                {
                    UpdateSubMeasuresPositions(measureTools.DefaultReferencePlanePositions, warpSettings.SubMeasurePoints);
                }

                ProbeSelector.SelectedProbeId = warpSettings.ProbeSettings?.ProbeId;
                ProbeSelector.SetProbeSettings(warpSettings.ProbeSettings);

                CharacteristicsChanged = false;

                UpdateCompatibleProbes();
            }));

            // Just to avoid a warning on the async
            await Task.Delay(5);
            IsLoading = false;
        }

        #region Properties

        public RecipeMeasureVM RecipeMeasure { get; set; }

        public IEnumerable<string> ToleranceUnits { get; private set; }

        private bool _isSurfaceWarp = false;

        public bool IsSurfaceWarp
        {
            get => _isSurfaceWarp;
            set { if (_isSurfaceWarp != value) { _isSurfaceWarp = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private Length _warpMax = 20.Micrometers();

        public Length WarpMax
        {
            get => _warpMax;
            set { if (_warpMax != value) { _warpMax = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private LengthTolerance _totalThicknessTolerance = new LengthTolerance(50.0, LengthToleranceUnit.Micrometer);

        public LengthTolerance TotalThicknessTolerance
        {
            get => _totalThicknessTolerance;
            set { if (_totalThicknessTolerance != value) { _totalThicknessTolerance = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private bool _characteristicsChanged = true;

        public bool CharacteristicsChanged
        {
            get => _characteristicsChanged;
            set => SetProperty(ref _characteristicsChanged, value);
        }

        private ProbeSelectorVM _probeSelector = new ProbeSelectorVM();

        public ProbeSelectorVM ProbeSelector
        {
            get => _probeSelector;
            set => SetProperty(ref _probeSelector, value);
        }

        private bool _isEditingHardware = false;

        public bool IsEditingHardware
        {
            get => _isEditingHardware;
            set => SetProperty(ref _isEditingHardware, value);
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
            var newWarpConfiguration = GetSettingsWithoutPoints();
            var measureTools = ServiceLocator.MeasureSupervisor.GetMeasureTools(newWarpConfiguration)?.Result as WarpMeasureTools;

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
                    // if the probe to remove is the selected one, we try to select another one
                    if (ProbeSelector.SelectedProbeId == probe.ProbeMaterial.ProbeId)
                    {
                        ProbeSelector.SelectedProbe = ProbeSelector.Probes.First(p => p.ProbeMaterial.ProbeId != ProbeSelector.SelectedProbeId);
                    }
                    ProbeSelector.Probes.Remove(probe);
                }
            }
        }

        internal override void EnsureMeasurePointsZPositions(ObservableCollection<MeasurePointVM> measurePoints)
        {
            // To ensure that Z top and Z bottom are the same for all points
            // We truncate them to allow max 3 digits
            // As in AreSettingsValid we compare Z values with an epsilon equal to 0.0005
            foreach (var measurePoint in measurePoints)
            {
                measurePoint.PointPosition.ZTop = Math.Round(measurePoint.PointPosition.ZTop * 1000d, 0) / 1000d;
                measurePoint.PointPosition.ZBottom = Math.Round(measurePoint.PointPosition.ZBottom * 1000d, 0) / 1000d;
            }
        }

        public override MeasureSettingsBase GetSettingsWithoutPoints()
        {
            var newWarpSettings = new WarpSettings();
            newWarpSettings.NbOfRepeat = 1;
            newWarpSettings.IsSurfaceWarp = IsSurfaceWarp;
            newWarpSettings.WarpMax = WarpMax;
            newWarpSettings.TotalThicknessTolerance = TotalThicknessTolerance;
            // TODO Warp : check how to verify that wafer is transparent
            // For V1, it is user responsability to know which probe can be used, so we consider that wafer is transparent in order to allow all probes
            newWarpSettings.IsWaferTransparent = true; // !_layers.Any(layer => layer.IsRefractiveIndexUnknown);
            newWarpSettings.WaferCharacteristic = RecipeMeasure.EditedRecipe.WaferDimentionalCharacteristic;
            newWarpSettings.ProbeSettings = ProbeSelector.GetSelectedProbeSettings() ?? new ProbeSettings();
            newWarpSettings.SubMeasurePoints = new List<int>();
            newWarpSettings.IsMeasureWithSubMeasurePoints = IsMeasureWithSubMeasurePoints;
            newWarpSettings.TheoricalWaferThickness = _theoricalWaferThickness;
            newWarpSettings.PhysicalLayers = LayersHelper.GetPhysicalLayers(RecipeMeasure.EditedRecipe.Step.Id);

            foreach (var point in RecipeMeasure.MeasurePoints.Points)
            {
                newWarpSettings.SubMeasurePoints.Add(point.Id.Value);
            }

            return newWarpSettings;
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
                var newWarpSettings = GetSettingsWithoutPoints();
                var measureTools = (ServiceLocator.MeasureSupervisor.GetMeasureTools(newWarpSettings)?.Result as WarpMeasureTools);
                if (measureTools != null)
                {
                    UpdateSubMeasuresPositions(measureTools.DefaultReferencePlanePositions, newWarpSettings.SubMeasurePoints);
                }
            }

            await Task.Delay(5);
        }

        public override void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            if (!(result is WarpPointResult warpPointResult))
            {
                return;
            }

            var warpResultDisplayVM = new WarpResultDisplayVM(warpPointResult, (WarpSettings)GetSettingsWithoutPoints(), resultFolderPath, dieIndex);
            ServiceLocator.DialogService.ShowDialog<WarpResultDisplay>(warpResultDisplayVM);
        }

        public override void Hide()
        {
        }

        public override bool AreSettingsValid(ObservableCollection<MeasurePointVM> measurePoints, bool forTestOnPoint = false)
        {
            if (!forTestOnPoint)
            {
                if (measurePoints == null || measurePoints.Count == 0)
                {
                    ValidationErrorMessage = "Measure points are not defined";
                    return false;
                }

                if (measurePoints.Count < 9)
                {
                    ValidationErrorMessage = "To measure the warp at least 9 points are required";
                    return false;
                }

                double epsilon = 0.0005;
                if (!measurePoints.Min(m => m.PointPosition.ZTop).Near(measurePoints.Max(m => m.PointPosition.ZTop), epsilon))
                {
                    ValidationErrorMessage = "To measure the warp all points must have the same Z top";
                    return false;
                }
                if (!measurePoints.Min(m => m.PointPosition.ZBottom).Near(measurePoints.Max(m => m.PointPosition.ZBottom), epsilon))
                {
                    ValidationErrorMessage = "To measure the warp all points must have the same Z bottom";
                    return false;
                }

                if (WarpMax.Value <= 0d)
                {
                    ValidationErrorMessage = "The max warp must be higher than 0";
                    return false;
                }

                if (TotalThicknessTolerance.Value <= 0d)
                {
                    ValidationErrorMessage = "Total thickness tolerance must be higher than 0";
                    return false;
                }
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
            var probeVM = ProbeSelector.SelectedProbe;
            if (probeVM is SelectableDualLiseVM dualLiseVM)
            {
                return dualLiseVM.SelectedObjectiveDualUp;
            }

            return ProbeSelector.SelectedProbe?.SelectedObjective;
        }

        #endregion
    }
}
