using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Modules.TestMeasure.ViewModel
{
    internal class ThicknessMeasureVM : ObservableObject, IWizardNavigationItem, IDisposable
    {
        public string Name { get; set; } = "Thickness";
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = false;
        public bool IsValidated { get; set; } = false;

        private TestMeasureVM _testMeasureVM;

        public ThicknessMeasureVM(TestMeasureVM testMeasureVM)
        {
            _testMeasureVM = testMeasureVM;
        }

        public void Dispose()
        {
            // Nothing
        }

        private AutoRelayCommand _startThicknessCommand;

        private static LayerSettings CreateLayer143(int id)
        {
            double reflexionIndex = 3.47;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 143.71.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static LayerSettings CreateLayerSilicium134(int id)
        {
            double reflexionIndex = 3.68;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 134.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static Layer Create1LayerToMeasureSilisium(SingleLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 3.68;
            var layers = new List<LayerSettings>() { CreateLayerSilicium134(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static Layer Create1LayerToMeasureSilisium(DualLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 3.68;
            var layers = new List<LayerSettings>() { CreateLayerSilicium134(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static LayerSettings CreateLayerGlue57(int id)
        {
            double reflexionIndex = 1.5;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 57.5.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static Layer Create1LayerToMeasureGlue(SingleLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 1.5;
            var layers = new List<LayerSettings>() { CreateLayerGlue57(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static Layer Create1LayerToMeasureGlue(DualLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 1.5;
            var layers = new List<LayerSettings>() { CreateLayerGlue57(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static LayerSettings CreateLayerGlass712(int id)
        {
            double reflexionIndex = 1.4621;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 712.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static Layer Create1LayerToMeasureGlass(SingleLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 1.4621;
            var layers = new List<LayerSettings>() { CreateLayerGlass712(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static Layer Create1LayerToMeasureGlass(DualLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 1.4621;
            var layers = new List<LayerSettings>() { CreateLayerGlass712(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layers,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static LayerSettings CreateLayer200(int id)
        {
            double reflexionIndex = 3.47;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 200.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static LayerSettings CreateLayer43(int id)
        {
            double reflexionIndex = 3.47;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 43.71.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static LayerSettings CreateLayer183(int id)
        {
            double reflexionIndex = 3.47;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 183.75.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static LayerSettings CreateLayer25(int id)
        {
            double reflexionIndex = 3.47;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 25.37.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static Layer Create1LayerToMeasure25(SingleLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 3.47;
            var layersListFor200 = new List<LayerSettings>() { CreateLayer25(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layersListFor200,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static Layer Create1LayerToMeasure143(SingleLiseSettings probeSettings, int id)
        {
            double reflexionIndex = 3.47;
            var layersListFor550 = new List<LayerSettings>() { CreateLayer143(id), };
            return new Layer()
            {
                Name = $"MeasurableLayers number {id}",
                PhysicalLayers = layersListFor550,
                ProbeSettings = probeSettings,
                ThicknessTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                IsWaferTotalThickness = false,
                RefractiveIndex = reflexionIndex,
            };
        }

        private static SingleLiseSettings CreateProbeSettingsLiseUp(double gain = 1.8, string objective = "ObjectiveSelector01")
        {
            return new SingleLiseSettings()
            {
                ProbeId = "ProbeLiseUp",
                LiseGain = gain,
                ProbeObjectiveContext = new ObjectiveContext
                {
                    ObjectiveId = objective
                }
            };
        }

        private static ThicknessSettings CreateThicknessSettings(List<LayerSettings> layers, List<Layer> layerGroupsToMeasure)
        {
            return new ThicknessSettings()
            {
                Name = "Thickness Test",
                IsActive = true,
                MeasurePoints = new List<int> { 1, 20, 30 },
                NbOfRepeat = 1,
                PhysicalLayers = layers,
                LayersToMeasure = layerGroupsToMeasure,

                Strategy = AcquisitionStrategy.Standard,
            };
        }

        private static DualLiseSettings CreateProbeSettingsDualLise(double gain = 1.8)
        {
            var probeLiseUpSettings = new SingleLiseSettings()
            {
                ProbeId = "ProbeLiseUp",
                LiseGain = gain,
                ProbeObjectiveContext = new ObjectiveContext
                {
                    ObjectiveId = "ObjectiveSelector02"
                }
            };
            var probeLiseBottomSettings = new SingleLiseSettings()
            {
                ProbeId = "ProbeLiseBottom",
                LiseGain = gain,
                ProbeObjectiveContext = new ObjectiveContext
                {
                    ObjectiveId = "ObjectiveSelector02"
                }
            };
            return new DualLiseSettings()
            {
                ProbeId = "ProbeLiseDouble",
                LiseUp = probeLiseUpSettings,
                LiseDown = probeLiseBottomSettings
            };
        }

        private static LayerSettings CreateUnknowLayer(int id)
        {
            double reflexionIndex = 3.47;
            return new LayerSettings()
            {
                Name = $"MeasurableLayers number {id}",
                Thickness = 0.Micrometers(),
                RefractiveIndex = reflexionIndex
            };
        }

        private static SingleLiseSettings CreateProbeSettingsLiseBottom(double gain = 1.8, string objective = "ObjectiveSelector02")
        {
            return new SingleLiseSettings()
            {
                ProbeId = "ProbeLiseDown",
                LiseGain = gain,
                ProbeObjectiveContext = new ObjectiveContext
                {
                    ObjectiveId = objective
                }
            };
        }

        private ThicknessSettings CreateThicknessSettingsDualLise()
        {
            var layers = new List<LayerSettings>() { CreateLayerSilicium134(1), CreateUnknowLayer(2), CreateLayerGlass712(3) };

            var layerSilicium = Create1LayerToMeasureSilisium(CreateProbeSettingsDualLise(), 1);
            var layerGlue = Create1LayerToMeasureGlue(CreateProbeSettingsDualLise(), 2);
            var layerGlass = Create1LayerToMeasureGlass(CreateProbeSettingsDualLise(), 3);

            var layersToMeasure = new List<Layer>
            {
                layerSilicium,
                layerGlue,
                layerGlass
            };

            var layerGroups = layersToMeasure;

            var thicknessSettings = CreateThicknessSettings(layers, layersToMeasure);
            return thicknessSettings;
        }

        public AutoRelayCommand StartThicknessCommand
        {
            get
            {
                return _startThicknessCommand ?? (_startThicknessCommand = new AutoRelayCommand(
              () =>
              {
                  _testMeasureVM.StartMeasure(CreateThicknessSettingsDualLise());
              },
              () => { return true; }));
            }
        }
    }
}
