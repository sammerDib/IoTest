using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeInputParametersLiseHFVM : ObservableObject, ILiseHFInputParams
    {
        public ProbeInputParametersLiseHFVM()
        {
            
        }

        public void Init()
        {
            var toolService = new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());
            var step = toolService.Invoke(x => x.GetStep(ClassLocator.Default.GetInstance<IRecipeManager>().EditedRecipe.Step.Id));
            int colorIndex = 0;
            Layers.Clear();
            foreach (var layer in step.Layers)
            {
                var newLayer = new LayerViewModel(step) { Name = layer.Name, Thickness = new Length(layer.Thickness, LengthUnit.Micrometer), RefractiveIndex = layer.RefractiveIndex, LayerColor = Colors.Red };

                newLayer.LayerColor = LayersEditorViewModel.GetLayerColor(null, colorIndex);
                colorIndex++;
                Layers.Add(newLayer);
            }
        }

        private Length _depthTarget = 0.Micrometers();

        public Length DepthTarget
        {
            get => _depthTarget; set { if (_depthTarget != value) { _depthTarget = value; OnPropertyChanged(); } }
        }

        private LengthTolerance _depthTolerance = new LengthTolerance(0, LengthToleranceUnit.Micrometer);

        public LengthTolerance DepthTolerance
        {
            get => _depthTolerance; set { if (_depthTolerance != value) { _depthTolerance = value; OnPropertyChanged(); } }
        }

        private bool _isLowIlluminationPower = false;

        public bool IsLowIlluminationPower
        {
            get => _isLowIlluminationPower; set { if (_isLowIlluminationPower != value) { _isLowIlluminationPower = value; OnPropertyChanged(); } }
        }

        private double _integrationTimems = 10.0; // move from int to double [02/09/2024]

        public double IntegrationTimems
        {
            get => _integrationTimems; set { if (_integrationTimems != value) { _integrationTimems = value; OnPropertyChanged(); } }
        }

        private double _intensityFactor = 1.0;

        public double IntensityFactor
        {
            get => _intensityFactor; set { if (_intensityFactor != value) { _intensityFactor = value; OnPropertyChanged(); } }
        }

        private int _nbMeasuresAverage = 8;

        public int NbMeasuresAverage

        {
            get => _nbMeasuresAverage; set { if (_nbMeasuresAverage != value) { _nbMeasuresAverage = value; OnPropertyChanged(); } }
        }

        private double _threshold = 0.005;

        public double Threshold
        {
            get => _threshold; set { if (_threshold != value) { _threshold = value; OnPropertyChanged(); } }
        }

        private double _thresholdPeak = 0.5;

        public double ThresholdPeak
        {
            get => _thresholdPeak; set { if (_thresholdPeak != value) { _thresholdPeak = value; OnPropertyChanged(); } }
        }

        private CalibrationFrequency _calibrationFreq = CalibrationFrequency.Wafer;

        public CalibrationFrequency CalibrationFreq
        {
            get => _calibrationFreq; set { if (_calibrationFreq != value) { _calibrationFreq = value; OnPropertyChanged(); } }
        }

        private bool _saveFFTSignal = false;

        public bool SaveFFTSignal
        {
            get => _saveFFTSignal; set { if (_saveFFTSignal != value) { _saveFFTSignal = value; OnPropertyChanged(); } }
        }


        private string _objectiveId;

        public string ObjectiveId
        {
            get => _objectiveId; set { if (_objectiveId != value) { _objectiveId = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<LayerViewModel> _layers;

        public ObservableCollection<LayerViewModel> Layers
        {
            get
            {
                if (_layers is null)
                {
                    _layers = new ObservableCollection<LayerViewModel>();
                }
                return _layers;
            }
            set { if (_layers != value) { _layers = value; OnPropertyChanged(); } }
        }

        public LiseHFInputParams GetLiseHFInputParams()
        {
            var mapper = ClassLocator.Default.GetInstance<Mapper>();
            var liseHFInputParams = mapper.AutoMap.Map<LiseHFInputParams>(this);
            //liseHFInputParams.PhysicalLayers= LayersHelper.GetPhysicalLayers(ClassLocator.Default.GetInstance<IRecipeManager>().EditedRecipe.Step.Id);
            liseHFInputParams.PhysicalLayers = GetLayersWithToleranceSettings();

            return liseHFInputParams;
        }

        public List<LayerWithToleranceSettings> GetLayersWithToleranceSettings()
        {
            var layersWithToleranceSettings = new List<LayerWithToleranceSettings>();
            foreach (var layer in Layers)
            {
                var layerSettings = new LayerWithToleranceSettings()
                {
                    Name = layer.Name,
                    Thickness = layer.Thickness,
                    ThicknessTolerance = layer.ThicknessTolerance,
                    MaterialName = layer.LayerMaterial?.Name ?? "",
                    RefractiveIndex = layer.RefractiveIndex,
                    LayerColor = layer.LayerColor
                };
                layersWithToleranceSettings.Add(layerSettings);
            }
            return layersWithToleranceSettings;
        }


    }
}
