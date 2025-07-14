using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures.MeasuresSettings;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Extensions;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class ThicknessSettingsVM : MeasureSettingsVM
    {
        public const string WaferThicknessName = "Wafer Thickness";
        private IDialogOwnerService _dialogService;
        private DataAccess.Dto.Step _step;

        public ThicknessSettingsVM(RecipeMeasureVM recipeMeasure)
        {
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            RecipeMeasure = recipeMeasure;
            RecipeMeasure.IsCenteredRoi = false;//ROI not centered for Thickness

            AutoFocusSettings = new AutoFocusSettingsVM();

            var toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            var step = toolService.Invoke(x => x.GetStep(RecipeMeasure.EditedRecipe.Step.Id));
            _step = step;
            int colorIndex = 0;
            foreach (var layer in step.Layers)
            {
                var newLayer = new LayerViewModel(step) { Name = layer.Name, Thickness = new Length(layer.Thickness, LengthUnit.Micrometer), RefractiveIndex = layer.RefractiveIndex, LayerColor = Colors.Red };

                newLayer.LayerColor = LayersEditorViewModel.GetLayerColor(null, colorIndex);
                colorIndex++;
                Layers.Add(newLayer);
            }

            UpdateLayersToMeasure();
        }

        #region Properties

        public RecipeMeasureVM RecipeMeasure { get; set; }

        private TrulyObservableCollection<LayerViewModel> _layers;

        public TrulyObservableCollection<LayerViewModel> Layers
        {
            get
            {
                if (_layers is null)
                {
                    _layers = new TrulyObservableCollection<LayerViewModel>();
                    _layers.ItemPropertyChanged += _layers_ItemPropertyChanged;
                }
                return _layers;
            }
            set { if (_layers != value) { _layers = value; OnPropertyChanged(); } }
        }

        private void _layers_ItemPropertyChanged(IList sourceList, object item, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LayerViewModel.IsGroupedWithNext))
            {
                Application.Current.Dispatcher.Invoke(() => { UpdateLayersToMeasure(); });
            }
        }

        private TrulyObservableCollection<ThicknessLayerToMeasureViewModel> _layersToMeasure;

        public TrulyObservableCollection<ThicknessLayerToMeasureViewModel> LayersToMeasure
        {
            get
            {
                if (_layersToMeasure is null)
                {
                    _layersToMeasure = new TrulyObservableCollection<ThicknessLayerToMeasureViewModel>();
                    _layersToMeasure.ItemPropertyChanged += LayersToMeasure_CollectionItemChanged;
                }
                return _layersToMeasure;
            }
            set { if (_layersToMeasure != value) { _layersToMeasure = value; OnPropertyChanged(); } }
        }

        private void LayersToMeasure_CollectionItemChanged(IList collection, object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var layer = sender as ThicknessLayerToMeasureViewModel;

            if (IsPropertyASettingsModification(e.PropertyName))
            {
                IsModified = true;
            }

            if (e.PropertyName == nameof(ThicknessLayerToMeasureViewModel.IsMeasured)
                && layer != null && layer.IsMeasured)
            {
                Application.Current.Dispatcher.Invoke(() => { UpdateLayersToMeasure(); });
            }

            if (e.PropertyName == nameof(ThicknessLayerToMeasureViewModel.RefractiveIndex)
                && layer != null && layer.HasMultipleLayers)
            {
                Application.Current.Dispatcher.Invoke(() => { UpdateLayersToMeasure(); });
            }

            if (e.PropertyName == nameof(ThicknessLayerToMeasureViewModel.IsEditing))
            {
                if (layer != null && layer.IsEditing)
                {
                    IsEditingALayerToMeasure = true;
                }
                else
                {
                    IsEditingALayerToMeasure = false;
                }
            }
        }

        // returns true if the property corresponds to a settings modification for the layer to measure
        private bool IsPropertyASettingsModification(string propertyName)
        {
            return (propertyName == nameof(ThicknessLayerToMeasureViewModel.IsWaferThickness)
                || propertyName == nameof(ThicknessLayerToMeasureViewModel.IsMeasured)
                || propertyName == nameof(ThicknessLayerToMeasureViewModel.IsEditing)
                || propertyName == nameof(ThicknessLayerToMeasureViewModel.Layers)
                || propertyName == nameof(ThicknessLayerToMeasureViewModel.RefractiveIndex)
                || propertyName == nameof(ThicknessLayerToMeasureViewModel.Thickness)
                || propertyName == nameof(ThicknessLayerToMeasureViewModel.ThicknessOffset)
                || propertyName == nameof(ThicknessLayerToMeasureViewModel.ThicknessTolerance));
        }

        private void UpdateCompatibleMeasureTools()
        {
            var tools = (ThicknessMeasureTools)Proxy.ServiceLocator.MeasureSupervisor.GetMeasureTools(GetSettingsWithoutPoints())?.Result;
            if (tools != null)
            {
                foreach (var tool in tools.MeasureToolsForLayers)
                {
                    var layerToMeasure = LayersToMeasure.FirstOrDefault(l => l.Name == tool.NameLayerToMeasure);
                    if (!(layerToMeasure is null))
                    {
                        layerToMeasure.MeasureTools = tool;
                        if (tool.UpProbes.IsEmpty() && tool.DownProbes.IsEmpty() && tool.DualProbes.IsEmpty())
                        {
                            layerToMeasure.IsMeasured = false;
                            _dialogService.ShowMessageBox($"Layer \'{layerToMeasure.Name}\' can't be measured : No compatible probe found.",
                                "Probe Compatibility", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }

                // WaferThickness
                var waferThicknessMeasureTools = tools.MeasureToolsForLayers.FirstOrDefault(mt => mt.NameLayerToMeasure == WaferThicknessName);
                if (!(waferThicknessMeasureTools is null))
                {
                    WaferThicknessToMeasure.MeasureTools = waferThicknessMeasureTools;
                }
            }
        }

        private ThicknessLayerToMeasureViewModel _waferThicknessToMeasure;

        public ThicknessLayerToMeasureViewModel WaferThicknessToMeasure
        {
            get
            {
                if (_waferThicknessToMeasure is null)
                {
                    _waferThicknessToMeasure = new ThicknessLayerToMeasureViewModel() { IsWaferThickness = true };
                    _waferThicknessToMeasure.PropertyChanged += WaferThicknessToMeasure_PropertyChanged;
                    _waferThicknessToMeasure.Layers = Layers;
                }
                return _waferThicknessToMeasure;
            }
            set { if (_waferThicknessToMeasure != value) { _waferThicknessToMeasure = value; OnPropertyChanged(); } }
        }

        private void WaferThicknessToMeasure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (IsPropertyASettingsModification(e.PropertyName))
            {
                IsModified = true;
            }

            if (e.PropertyName == nameof(ThicknessLayerToMeasureViewModel.IsMeasured)
                && sender is ThicknessLayerToMeasureViewModel layer && layer.IsMeasured)
            {
                UpdateCompatibleMeasureTools();
            }

            OnPropertyChanged(nameof(CanMeasureWarp));
            if (!CanMeasureWarp)
                IsWarpMeasured = false;
        }

        private bool _isEditingALayerToMeasure;

        public bool IsEditingALayerToMeasure
        {
            get => _isEditingALayerToMeasure;
            set
            {
                if (_isEditingALayerToMeasure != value)
                {
                    _isEditingALayerToMeasure = value;
                    IsModified = true;
                    if (_isEditingALayerToMeasure)
                    {
                        // We disable all the settings for the other layers
                        foreach (var layerToMeasure in LayersToMeasure)
                        {
                            if (!layerToMeasure.IsEditing)
                                layerToMeasure.IsEnabled = false;
                        }
                    }
                    else
                    {
                        foreach (var layerToMeasure in LayersToMeasure)
                        {
                            layerToMeasure.IsEnabled = true;
                        }
                    }

                    OnPropertyChanged();
                }
            }
        }

        private bool _isWarpMeasured;

        public bool IsWarpMeasured
        {
            get => _isWarpMeasured; set { if (_isWarpMeasured != value) { _isWarpMeasured = value; IsModified = true; OnPropertyChanged(); } }
        }

        public bool CanMeasureWarp => WaferThicknessToMeasure.IsMeasured;

        private Length _warpTargetMax = 20.Micrometers();

        public Length WarpTargetMax
        {
            get => _warpTargetMax; set { if (_warpTargetMax != value) { _warpTargetMax = value; IsModified = true; OnPropertyChanged(); } }
        }

        public override bool ArePositionsOnDie => !(RecipeMeasure.WaferMap is null);

        private AutoFocusSettingsVM _autoFocusSettings;

        public AutoFocusSettingsVM AutoFocusSettings
        {
            get => _autoFocusSettings;
            set
            {
                if (_autoFocusSettings != value)
                {
                    if (!(_autoFocusSettings is null))
                    {
                        _autoFocusSettings.AutoFocusSettingsModified -= AutoFocusSettings_Modified;
                    }
                    _autoFocusSettings = value;
                    if (!(_autoFocusSettings is null))
                    {
                        _autoFocusSettings.AutoFocusSettingsModified += AutoFocusSettings_Modified;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private void AutoFocusSettings_Modified(object sender, EventArgs e)
        {
            IsModified = true;
        }

        #endregion Properties

        #region Public methods

        public void UpdateLayersToMeasure()
        {
            // We check that all the layers in the layersToMeasure still exist
            foreach (var layerToMeasure in LayersToMeasure.ToList())
            {
                foreach (var layer in layerToMeasure.Layers.ToList())
                {
                    if (!Layers.Contains(layer))
                        layerToMeasure.Layers.Remove(layer);
                }
                if (layerToMeasure.Layers.Count == 0)
                    LayersToMeasure.Remove(layerToMeasure);
            }
            //Save the layers to be measured in dictionary
            var layersToBeMeasureDictionary = new Dictionary<string, bool>();
            foreach (var item in LayersToMeasure)
            {
                layersToBeMeasureDictionary.Add(item.Name, item.IsMeasured);
            }

            int i = 0;
            int layerToMeasureIndex = 0;
            while (i < Layers.Count)
            {
                var layerToMeasure = GetLayerToMeasureThatContainsALayer(Layers[i]);
                if (layerToMeasure is null)
                {
                    layerToMeasure = new ThicknessLayerToMeasureViewModel() { RefractiveIndex = 0.25F };
                    layerToMeasure.Layers.Add(Layers[i]);
                    LayersToMeasure.Insert(layerToMeasureIndex, layerToMeasure);
                }
                else
                {
                    // we check that there is no other layers in the layer to measure
                    layerToMeasure.Layers.Clear();
                    layerToMeasure.Layers.Add(Layers[i]);
                }

                while (Layers[i].IsGroupedWithNext)
                {
                    var newLayerToMeasure = GetLayerToMeasureThatContainsALayer(Layers[i + 1]);
                    if (!(newLayerToMeasure is null))
                    {
                        newLayerToMeasure.Layers.Remove(Layers[i + 1]);
                        if (newLayerToMeasure.Layers.Count == 0)
                            LayersToMeasure.Remove(newLayerToMeasure);
                    }
                    layerToMeasure.Layers.Add(Layers[i + 1]);
                    i++;
                }

                i++;
                layerToMeasureIndex++;
            }
            //Update the layers to be measured
            foreach (var item in LayersToMeasure)
            {
                if (layersToBeMeasureDictionary.ContainsKey(item.Name))
                {
                    item.IsMeasured = layersToBeMeasureDictionary[item.Name];
                }
                else
                {
                    item.IsMeasured = false;
                }
            }

            UpdateCompatibleMeasureTools();
        }

        private ThicknessLayerToMeasureViewModel GetLayerToMeasureThatContainsALayer(LayerViewModel layerToSearch)
        {
            return LayersToMeasure.FirstOrDefault(l => !(l.Layers.FirstOrDefault(sl => sl.Name == layerToSearch.Name) is null));
        }

        #endregion Public methods

        #region Commands

        private AutoRelayCommand<LayerViewModel> _editLayers;

        public AutoRelayCommand<LayerViewModel> EditLayers
        {
            get
            {
                return _editLayers ?? (_editLayers = new AutoRelayCommand<LayerViewModel>(
                    (thicknessLayer) =>
                    {
                        var layerEditorVM = new ThicknessLayersEditorViewModel();

                        // We clone the layers collection
                        var layersEdited = new ObservableCollection<LayerViewModel>();
                        foreach (var layer in Layers)
                        {
                            layersEdited.Add((LayerViewModel)layer.Clone());
                        }

                        layerEditorVM.LayersEditor.Layers = layersEdited;

                        if (ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<ThicknessLayersEditorView>(layerEditorVM) == true)
                        {
                            IsModified = true;
                            bool needResetGroups = false;
                            // If the number of layers change, we will reset the groups configuration
                            if (layerEditorVM.LayersEditor.Layers.Count != Layers.Count)
                                needResetGroups = true;

                            for (int i = 0; i < layerEditorVM.LayersEditor.Layers.Count; i++)
                            {
                                if (i < Layers.Count)
                                    CopyLayerInfoToLayer(layerEditorVM.LayersEditor.Layers[i], Layers[i]);
                                else
                                    Layers.Add(layerEditorVM.LayersEditor.Layers[i]);
                            }

                            if (Layers.Count > layerEditorVM.LayersEditor.Layers.Count)
                            {
                                for (int i = Layers.Count - 1; i >= layerEditorVM.LayersEditor.Layers.Count; i--)
                                {
                                    Layers.RemoveAt(i);
                                }
                            }

                            if (needResetGroups)
                            {
                                foreach (var layer in Layers)
                                {
                                    layer.IsGroupedWithNext = false;
                                }
                            }
                        }
                        UpdateLayersToMeasure();
                    },
                    (thicknessLayer) => { return true; }
                ));
            }
        }

        private void CopyLayerInfoToLayer(LayerViewModel layerFrom, LayerViewModel layerTo)
        {
            layerTo.Name = layerFrom.Name;
            layerTo.IsRefractiveIndexUnknown = layerFrom.IsRefractiveIndexUnknown;
            if (layerFrom.IsRefractiveIndexUnknown)
                layerTo.RefractiveIndex = float.NaN;
            else
                layerTo.RefractiveIndex = layerFrom.RefractiveIndex;
            layerTo.Thickness = layerFrom.Thickness;
        }

        #endregion Commands

        public override void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            if (!(result is ThicknessPointResult thicknessPointResult))
                return;

            var thicknessResultDisplayVM = new ThicknessResultDisplayVM(thicknessPointResult, (ThicknessSettings)GetSettingsWithoutPoints(), resultFolderPath, dieIndex);

            ServiceLocator.DialogService.ShowDialog<ThicknessResultDisplay>(thicknessResultDisplayVM);
        }

        public override void Dispose()
        {
            foreach (var layerToMeasure in LayersToMeasure)
            {
                layerToMeasure.Dispose();
            }

            WaferThicknessToMeasure.Dispose();

            if (!(AutoFocusSettings is null))
            {
                _autoFocusSettings.AutoFocusSettingsModified -= AutoFocusSettings_Modified;
                AutoFocusSettings.Dispose();
            }
        }

        public override MeasureSettingsBase GetSettingsWithoutPoints()
        {
            var newThicknessSettings = new ThicknessSettings
            {
                NbOfRepeat = 1,
                PhysicalLayers = new List<LayerSettings>()
            };
            foreach (var layer in Layers)
            {
                var newLayer = new LayerSettings
                {
                    Name = layer.Name,
                    Thickness = layer.Thickness,
                    RefractiveIndex = layer.RefractiveIndex,
                    LayerColor = layer.LayerColor
                };
                newThicknessSettings.PhysicalLayers.Add(newLayer);
            }

            newThicknessSettings.LayersToMeasure = new List<Layer>();

            foreach (var layerToMeasure in LayersToMeasure)
            {
                if (!layerToMeasure.IsMeasured)
                {
                    continue;
                }
                var newLayerToMeasure = GetLayerToMeasureFromViewModel(layerToMeasure);
                newThicknessSettings.LayersToMeasure.Add(newLayerToMeasure);
            }

            if (WaferThicknessToMeasure.IsMeasured)
            {
                var newWaferThicknessToMeasure = GetLayerToMeasureFromViewModel(WaferThicknessToMeasure);
                newThicknessSettings.LayersToMeasure.Add(newWaferThicknessToMeasure);
            }

            newThicknessSettings.HasWarpMeasure = IsWarpMeasured;
            if (IsWarpMeasured)
            {
                newThicknessSettings.WarpTargetMax = WarpTargetMax;
            }

            if ((!(AutoFocusSettings is null)) && AutoFocusSettings.IsAutoFocusEnabled)
            {
                newThicknessSettings.AutoFocusSettings = AutoFocusSettings.GetAutoFocusSettings();
            }

            return newThicknessSettings;
        }

        private Layer GetLayerToMeasureFromViewModel(ThicknessLayerToMeasureViewModel layerToMeasure)
        {
            var newLayerToMeasure = new Layer();
            if (!(layerToMeasure.RefractiveIndex is null))
                newLayerToMeasure.RefractiveIndex = (double)layerToMeasure.RefractiveIndex;
            newLayerToMeasure.MultipleLayersOffset = layerToMeasure.ThicknessOffset;
            newLayerToMeasure.PhysicalLayers = new List<LayerSettings>();
            foreach (var layer in layerToMeasure.Layers)
            {
                var newLayer = new LayerSettings();
                newLayer.Name = layer.Name;
                newLayer.Thickness = layer.Thickness;
                newLayer.RefractiveIndex = layer.RefractiveIndex;

                newLayerToMeasure.PhysicalLayers.Add(newLayer);
            }
            newLayerToMeasure.IsWaferTotalThickness = layerToMeasure.IsWaferThickness;
            newLayerToMeasure.Name = layerToMeasure.Name;
            newLayerToMeasure.ThicknessTolerance = layerToMeasure.ThicknessTolerance;
            newLayerToMeasure.LayerColor = layerToMeasure.LayerColor;

            newLayerToMeasure.ProbeSettings = layerToMeasure.ProbeSelector.GetSelectedProbeSettings();
            return newLayerToMeasure;
        }

        private void UpdateLayerToMeasureVmFromSettings(Layer layerToMeasureSettings)
        {
            ThicknessLayerToMeasureViewModel layerToMeasureToUpdate;

            if (layerToMeasureSettings.IsWaferTotalThickness)
                layerToMeasureToUpdate = WaferThicknessToMeasure;
            else
            {
                layerToMeasureToUpdate = LayersToMeasure.FirstOrDefault(l => l.Name == layerToMeasureSettings.Name);
                if (layerToMeasureToUpdate is null)
                    return;
            }

            layerToMeasureToUpdate.IsMeasured = true;
            layerToMeasureToUpdate.RefractiveIndex = layerToMeasureSettings.RefractiveIndex;
            layerToMeasureToUpdate.ThicknessOffset = layerToMeasureSettings.MultipleLayersOffset;

            layerToMeasureToUpdate.IsWaferThickness = layerToMeasureSettings.IsWaferTotalThickness;
            layerToMeasureToUpdate.Name = layerToMeasureSettings.Name;
            layerToMeasureToUpdate.ThicknessTolerance = layerToMeasureSettings.ThicknessTolerance;

            layerToMeasureToUpdate.ProbeSelector.SelectedProbeId = layerToMeasureSettings.ProbeSettings.ProbeId;
            layerToMeasureToUpdate.ProbeSelector.SetProbeSettings(layerToMeasureSettings.ProbeSettings);
        }

        public override void Hide()
        {
            AutoFocusSettings.IsEditing = false;
        }

        public override async Task LoadSettingsAsync(MeasureSettingsBase measureSettings)
        {
            // If a loading was already in progress, we wait
            if (!SpinWait.SpinUntil(() => !IsLoading, 20000))
            {
                ClassLocator.Default.GetInstance<ILogger>().Error("Thickness Loading failed");
                return;
            }

            if (!(measureSettings is ThicknessSettings thicknessSettings))
            {
                return;
            }

            IsLoading = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Layers.Clear();
            });

            foreach (var layer in thicknessSettings.PhysicalLayers)
            {
                var newLayerVm = new LayerViewModel(_step);
                newLayerVm.Name = layer.Name;
                newLayerVm.Thickness = layer.Thickness;
                newLayerVm.RefractiveIndex = (float)layer.RefractiveIndex;
                newLayerVm.LayerColor = layer.LayerColor;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Layers.Add(newLayerVm);
                });
            }

            foreach (var layerTomeasure in thicknessSettings.LayersToMeasure)
            {
                if (layerTomeasure.IsWaferTotalThickness)
                {
                    continue;
                }

                if (layerTomeasure.PhysicalLayers.Count > 1)
                {
                    for (int i = 0; i < layerTomeasure.PhysicalLayers.Count - 1; i++)
                    {
                        var layer = Layers.FirstOrDefault(l => l.Name == layerTomeasure.PhysicalLayers[i].Name);
                        layer.IsGroupedWithNext = true;
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateLayersToMeasure();
            });

            foreach (var layerTomeasure in thicknessSettings.LayersToMeasure)
            {
                UpdateLayerToMeasureVmFromSettings(layerTomeasure);
            }

            if (thicknessSettings.AutoFocusSettings is null)
            {
                AutoFocusSettings = new AutoFocusSettingsVM();
            }
            else
            {
                AutoFocusSettings = AutoFocusSettingsVM.CreateFromAutoFocusSettings(thicknessSettings.AutoFocusSettings);
                AutoFocusSettings.EnableWithoutEditing();
            }

            AutoFocusSettings.AreSettingsVisible = false;

            await Task.Delay(5);
            IsLoading = false;
        }

        public override async Task PrepareToDisplayAsync()
        {
            await Task.Run(() =>
            {
                // Enable Lights
                Proxy.ServiceLocator.LightsSupervisor.LightsAreLocked = false;

                Proxy.ServiceLocator.CamerasSupervisor.Objective = Proxy.ServiceLocator.CamerasSupervisor.MainObjective;

                // we set the probelise up as the current probe
                Proxy.ServiceLocator.ProbesSupervisor.SetCurrentProbe(Proxy.ServiceLocator.ProbesSupervisor.ProbeLiseUp.DeviceID);
            });
        }

        public override bool AreSettingsValid(ObservableCollection<MeasurePointVM> measurePoints, bool forTestOnPoint = false)
        {
            if (IsEditingALayerToMeasure)
            {
                ValidationErrorMessage = "Layer settings must be validated";
                return false;
            }

            // If there is nothing to measure
            if (!(LayersToMeasure.Any(l => l.IsMeasured)) && !WaferThicknessToMeasure.IsMeasured)
            {
                ValidationErrorMessage = "At least one layer must be measured";
                return false;
            }

            if ((!forTestOnPoint) && (measurePoints == null || measurePoints.Count == 0))
            {
                ValidationErrorMessage = "Measure points are not defined";
                return false;
            }

            if (_autoFocusSettings.IsEditing)
            {
                ValidationErrorMessage = "Autofocus must be validated";
                return false;
            }

            if ((!forTestOnPoint) && IsWarpMeasured && measurePoints.Count < 9)
            {
                ValidationErrorMessage = "To measure the warp at least 9 points are required";
                return false;
            }

            if ((!forTestOnPoint) && IsWarpMeasured && WarpTargetMax.Value <= 0d)
            {
                ValidationErrorMessage = "The max warp must be higher than 0";
                return false;
            }

            ValidationErrorMessage = string.Empty;
            return true;
        }

        public override void DisplaySettingsTab()
        {
        }

        public override void HideSettingsTab()
        {
        }

        #region PatternRec objectives

        public override ObjectiveConfig GetObjectiveUsedByMeasure()
        {
            ThicknessLayerToMeasureViewModel layerUsedForObjectiveSelection;
            if (WaferThicknessToMeasure.IsMeasured)
            {
                layerUsedForObjectiveSelection = WaferThicknessToMeasure;
            }
            else
            {
                layerUsedForObjectiveSelection = LayersToMeasure?.FirstOrDefault(layer => layer.IsMeasured);
            }

            if (layerUsedForObjectiveSelection != null)
            {
                var probeVM = layerUsedForObjectiveSelection.ProbeSelector.SelectedProbe;
                if (probeVM is SelectableDualLiseVM dualLiseVM)
                {
                    return dualLiseVM.SelectedObjectiveDualUp;
                }

                return layerUsedForObjectiveSelection.ProbeSelector.SelectedProbe?.SelectedObjective;
            }

            return null;
        }

        #endregion
    }

    public class IsLastItemInContainerConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DependencyObject item = (DependencyObject)values[0];
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            if (ic is null)
                return false;

            return ic.ItemContainerGenerator.IndexFromContainer(item)
                    == ic.Items.Count - 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
