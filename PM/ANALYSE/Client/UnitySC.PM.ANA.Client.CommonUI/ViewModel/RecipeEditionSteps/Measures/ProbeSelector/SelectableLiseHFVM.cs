using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures.MeasuresSettings;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector
{
    public class SelectableLiseHFVM : SelectableProbeVM, IDisposable
    {
        private ProbeLiseHFVM _probeLiseHF;
        private PositionBase _positionBeforeCalibration = null;

        public SelectableLiseHFVM()
        {
            IsCalibrationRequiredForSignal = true;

            PropertyChanged += SelectableLiseHFVM_PropertyChanged;
        }

        private void SelectableLiseHFVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedObjective))
            {
                if (ProbeInputParameters != null)
                    ProbeInputParameters.ObjectiveId = SelectedObjective?.DeviceID;
            }
        }

        private ProbeWithObjectivesMaterial _liseHFMaterial => ProbeMaterial as ProbeWithObjectivesMaterial;

        protected override void UpdateMaterial()
        {
            foreach (var compatibleObjectiveId in _liseHFMaterial.CompatibleObjectives)
            {
                var objectiveConfig = ServiceLocator.ProbesSupervisor.GetOjectiveConfig(compatibleObjectiveId);

                if (!(objectiveConfig is null))
                    Objectives.Add(objectiveConfig);
            }

            if (SelectedObjective is null)
                SelectedObjective = Objectives.FirstOrDefault();

            ProbeInputParameters = (ServiceLocator.ProbesSupervisor.Probes.FirstOrDefault(p => p.DeviceID == _liseHFMaterial.ProbeId) as ProbeLiseHFVM).InputParametersLiseHF;
            ProbeInputParameters.Init();
            ProbeInputParameters.ObjectiveId = SelectedObjective?.DeviceID;
            _probeLiseHF = (ServiceLocator.ProbesSupervisor.Probes.FirstOrDefault(p => p.DeviceID == _liseHFMaterial.ProbeId) as ProbeLiseHFVM);
        }

        public override void SetProbeSettings(ProbeSettings probeSettings)
        {
            if (probeSettings is LiseHFSettings liseHFSettings)
            {
                if (liseHFSettings.ProbeObjectiveContext != null)
                    SelectedObjective = Objectives.FirstOrDefault(o => o.DeviceID == liseHFSettings.ProbeObjectiveContext.ObjectiveId);
                ProbeInputParameters.ObjectiveId = SelectedObjective?.DeviceID;
                ProbeInputParameters.IsLowIlluminationPower = liseHFSettings.IsLowIlluminationPower;
                ProbeInputParameters.IntegrationTimems = liseHFSettings.IntegrationTimems;
                ProbeInputParameters.IntensityFactor = liseHFSettings.IntensityFactor;
                ProbeInputParameters.NbMeasuresAverage = liseHFSettings.NbMeasuresAverage;
                ProbeInputParameters.Threshold = liseHFSettings.Threshold;
                ProbeInputParameters.ThresholdPeak = liseHFSettings.ThresholdPeak;
                ProbeInputParameters.SaveFFTSignal = liseHFSettings.SaveFFTSignal;
                ProbeInputParameters.CalibrationFreq = liseHFSettings.CalibrationFreq;
                ProbeInputParameters.Layers.Clear();
                foreach (var layer in liseHFSettings.Layers)
                {
                    var newLayer = new LayerViewModel(null)
                    {
                        Name = layer.Name,
                        Thickness = layer.Thickness,
                        ThicknessTolerance = layer.ThicknessTolerance,
                        RefractiveIndex = (float?)layer.RefractiveIndex,
                        LayerColor = layer.LayerColor
                    };

                    ProbeInputParameters.Layers.Add(newLayer);
                }
            }
        }

        public override ProbeSettings GetProbeSettings()
        {
            var newProbeSettings = new LiseHFSettings();
            newProbeSettings.ProbeId = ProbeMaterial.ProbeId;
            newProbeSettings.ProbeObjectiveContext = new ObjectiveContext
            {
                ObjectiveId = SelectedObjective?.DeviceID,
            };
            newProbeSettings.IsLowIlluminationPower = ProbeInputParameters.IsLowIlluminationPower;
            newProbeSettings.IntegrationTimems = ProbeInputParameters.IntegrationTimems;
            newProbeSettings.IntensityFactor = ProbeInputParameters.IntensityFactor;
            newProbeSettings.NbMeasuresAverage = ProbeInputParameters.NbMeasuresAverage;
            newProbeSettings.Threshold = ProbeInputParameters.Threshold;
            newProbeSettings.ThresholdPeak = ProbeInputParameters.ThresholdPeak;
            newProbeSettings.SaveFFTSignal = ProbeInputParameters.SaveFFTSignal;
            newProbeSettings.CalibrationFreq = ProbeInputParameters.CalibrationFreq;
            newProbeSettings.Layers = ProbeInputParameters.GetLayersWithToleranceSettings();
            return newProbeSettings;
        }

        public override void SetAsCurrentProbe()
        {
        }

        public override void UnsetAsCurrentProbe()
        {
        }

        public override void StartCalibration()
        {
            _positionBeforeCalibration = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result;

            IsCalibrationInProgress = true;
            _probeLiseHF.CalibrationTerminated += ProbeLiseHF_CalibrationTerminated;
            var liseHFCalibParam = new LiseHFCalibParams();

            var res = ServiceLocator.ProbesSupervisor.StartCalibration(_liseHFMaterial.ProbeId, liseHFCalibParam, ProbeInputParameters.GetLiseHFInputParams())?.Result;
            if (res == null || res == false)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("The LiseHF calibration failed", "LiseHF", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
            }
        }

        private void ProbeLiseHF_CalibrationTerminated(ProbeCalibResultsBase probeCalibResults)
        {
            if (_positionBeforeCalibration != null)
                ServiceLocator.AxesSupervisor.GotoPosition(_positionBeforeCalibration, PM.Shared.Hardware.Service.Interface.Axes.AxisSpeed.Fast);

            IsCalibrationInProgress = false;
            _probeLiseHF.CalibrationTerminated -= ProbeLiseHF_CalibrationTerminated;
            if (probeCalibResults is ProbeLiseHFCalibResult liseHFCalibResults)
            {
                if (!liseHFCalibResults.Success)
                {
                    Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The LiseHF calibration failed. \n{probeCalibResults.ErrorMessage}", "LiseHF calibration", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
                else
                {
                    _probeLiseHF.IsCalibrated = true;

                    Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Just to restart the continuous acquisition because the calibration stops it.
                        ServiceLocator.ProbesSupervisor.IsEditingProbe = false;
                        ServiceLocator.ProbesSupervisor.IsEditingProbe = true;
                    }));
                }
            }
        }

        public override void CancelCalibration()
        {
            if (_positionBeforeCalibration != null)
                ServiceLocator.AxesSupervisor.GotoPosition(_positionBeforeCalibration, PM.Shared.Hardware.Service.Interface.Axes.AxisSpeed.Fast);
            ServiceLocator.ProbesSupervisor.CancelCalibration(_liseHFMaterial.ProbeId);
            _probeLiseHF.CalibrationTerminated -= ProbeLiseHF_CalibrationTerminated;
            IsCalibrationInProgress = false;
        }

        public override List<ObjectiveConfig> GetTopObjectives()
        {
            return Objectives.ToList();
        }

        public override void Dispose()
        {
            base.Dispose();
            PropertyChanged -= SelectableLiseHFVM_PropertyChanged;
        }

        private ObservableCollection<ObjectiveConfig> _objectives;

        public ObservableCollection<ObjectiveConfig> Objectives
        {
            get
            {
                if (_objectives is null)
                    _objectives = new ObservableCollection<ObjectiveConfig>();
                return _objectives;
            }
            set
            {
                if (_objectives == value)
                {
                    return;
                }
                _objectives = value;
                OnPropertyChanged();
            }
        }

        private List<int> _averagesList = new List<int>() { 1, 2, 4, 8, 16, 32 };

        public List<int> AveragesList
        {
            get => _averagesList; set { if (_averagesList != value) { _averagesList = value; OnPropertyChanged(); } }
        }

        private ProbeInputParametersLiseHFVM _probeInputParameters = null;

        public ProbeInputParametersLiseHFVM ProbeInputParameters
        {
            get => _probeInputParameters; set { if (_probeInputParameters != value) { _probeInputParameters = value; OnPropertyChanged(); } }
        }

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
                        foreach (var layer in ProbeInputParameters.Layers)
                        {
                            layersEdited.Add((LayerViewModel)layer.Clone());
                        }

                        layerEditorVM.LayersEditor.Layers = layersEdited;
                        layerEditorVM.LayersEditor.EditLayersThicknessTolerance = true;

                        if (ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<ThicknessLayersEditorView>(layerEditorVM) == true)
                        {
                            ProbeInputParameters.Layers.Clear();
                            for (int i = 0; i < layerEditorVM.LayersEditor.Layers.Count; i++)
                            {
                                ProbeInputParameters.Layers.Add(layerEditorVM.LayersEditor.Layers[i]);
                            }
                        }
                    },
                    (thicknessLayer) => { return true; }
                ));
            }
        }
    }
}
