using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MvvmDialogs;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class ThicknessResultDisplayVM : StepBaseVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public ThicknessResultDisplayVM(ThicknessPointResult thicknessPointResult, ThicknessSettings thicknessSettings, string resultFolderPath, DieIndex dieIndex)
        {
            ThicknessResult = thicknessPointResult;
            ThicknessSettings = thicknessSettings;
            ResultFolderPath = resultFolderPath;
            DieIndex = dieIndex;
            UpdateThicknessDetailResult();
        }

        private ThicknessPointResult _thicknessResult = null;

        public ThicknessPointResult ThicknessResult
        {
            get => _thicknessResult;
            private set
            {
                if (_thicknessResult != value)
                {
                    _thicknessResult = value;
                    UpdateThicknessDetailResult();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateThicknessDetailResult()
        {
            if ((ThicknessSettings is null) || (ThicknessResult is null) || string.IsNullOrEmpty(ResultFolderPath))
                return;

            var pointSelector = new ThicknessPointSelector();
            ThicknessDetailResult = new ThicknessDetailMeasureInfoVM(layer => Color.FromArgb(255, 0, 255, 128));
            ThicknessDetailResult.Digits = 3;
            ThicknessDetailResult.Layer = ThicknessResultVM.TotalLayerName;

            var thicknessResultSettings = new ThicknessResultSettings();
            thicknessResultSettings.ThicknessLayers = new List<ThicknessLengthSettings>();
            var layersUnit = new Dictionary<string, LengthUnit>();
            Length totalThickness = 0.Micrometers();
            foreach (var waferLayer in ThicknessSettings.PhysicalLayers)
            {
                bool isMeasured = false;

                Layer correspondingLayerToMeasure = null;
                // Is it a measured layer
                foreach (var layerToMeasure in ThicknessSettings.LayersToMeasure)
                {
                    // If the layer to measure is not the wafer thickness and it includes the current layer we say that it is a measured layer
                    if ((!layerToMeasure.IsWaferTotalThickness) && (layerToMeasure.PhysicalLayers.Any(l => l.Name == waferLayer.Name)))
                    {
                        correspondingLayerToMeasure = layerToMeasure;
                        isMeasured = true;
                        break;
                    }
                }

                ThicknessLengthSettings thicknessLengthSettings = null;
                if (isMeasured)
                {
                    // We check if it has already been added, it will be the case for the second layer of a group for example
                    if (!thicknessResultSettings.ThicknessLayers.Any(l => l.Name == correspondingLayerToMeasure.Name))
                    {
                        thicknessLengthSettings = new ThicknessLengthSettings() { IsMeasured = true, Name = correspondingLayerToMeasure.Name };
                        var totalLayerThickness = 0.Micrometers();
                        foreach (var layer in correspondingLayerToMeasure.PhysicalLayers)
                        {
                            totalLayerThickness += layer.Thickness;
                        }
                        thicknessLengthSettings.Target = totalLayerThickness;
                        thicknessLengthSettings.Tolerance = correspondingLayerToMeasure.ThicknessTolerance;
                        thicknessLengthSettings.LayerColor = correspondingLayerToMeasure.LayerColor;
                    }
                }
                else
                {
                    thicknessLengthSettings = new ThicknessLengthSettings() { IsMeasured = false, Name = waferLayer.Name };
                    thicknessLengthSettings.Target = waferLayer.Thickness;
                    thicknessLengthSettings.LayerColor = waferLayer.LayerColor;
                }
                if (!(thicknessLengthSettings is null))
                {
                    thicknessResultSettings.ThicknessLayers.Add(thicknessLengthSettings);
                    layersUnit.Add(thicknessLengthSettings.Name, thicknessLengthSettings.Target.Unit);
                }
            }

            // we calculate the total thickness target
            var totalThicknessTarget = 0.Micrometers();
            foreach (var layer in thicknessResultSettings.ThicknessLayers)
            {
                totalThicknessTarget += layer.Target;
            }

            thicknessResultSettings.TotalTarget = totalThicknessTarget;
            // Has WaferThickness ?
            var waferThickness = ThicknessSettings.LayersToMeasure.FirstOrDefault(l => l.IsWaferTotalThickness);
            if (!(waferThickness is null))
            {
                thicknessResultSettings.HasWaferThicknesss = true;
                thicknessResultSettings.TotalTolerance = waferThickness.ThicknessTolerance;
            }
            else
            {
                // We set the tolerance because the tolerance unit is used for the display
                thicknessResultSettings.TotalTolerance = new LengthTolerance(5, LengthToleranceUnit.Micrometer);
            }
            layersUnit.Add("Total", totalThicknessTarget.Unit);
            thicknessResultSettings.ComputeNotMeasuredLayers();
            ThicknessResult.ComputeWaferRelativePosition();

            foreach (var data in ThicknessResult.Datas)
            {
                (data as ThicknessPointData).ComputeTotalThickness(thicknessResultSettings);
                (data as ThicknessPointData).TotalState = MeasureState.Success;
            }

            ThicknessResult.GenerateStats();
            ThicknessDetailResult.SetLayersUnit(layersUnit);
            ThicknessDetailResult.Point = ThicknessResult;

            thicknessResultSettings.HasWarpMeasure = ThicknessSettings.HasWarpMeasure;
            thicknessResultSettings.WarpTargetMax = ThicknessSettings.WarpTargetMax;


            ThicknessDetailResult.Settings = thicknessResultSettings;

            if (!(DieIndex is null))
                ThicknessDetailResult.DieIndex = $"Column  {DieIndex.Column}    Row  {DieIndex.Row}";

            OnPropertyChanged(nameof(ThicknessDetailResult));
        }

        private ThicknessSettings _thicknessSettings = null;

        public ThicknessSettings ThicknessSettings
        {
            get => _thicknessSettings; private set { if (_thicknessSettings != value) { _thicknessSettings = value; OnPropertyChanged(); } }
        }

        private string _resultFolderPath;

        public string ResultFolderPath
        {
            get => _resultFolderPath; private set { if (_resultFolderPath != value) { _resultFolderPath = value; OnPropertyChanged(); } }
        }

        private BitmapImage _resultImage;

        public BitmapImage ResultImage
        {
            get => _resultImage; private set { if (_resultImage != value) { _resultImage = value; OnPropertyChanged(); } }
        }

        private DieIndex _dieIndex;

        public DieIndex DieIndex
        {
            get => _dieIndex; private set { if (_dieIndex != value) { _dieIndex = value; OnPropertyChanged(); } }
        }

        private ThicknessDetailMeasureInfoVM _thicknessDetailResult = null;

        public ThicknessDetailMeasureInfoVM ThicknessDetailResult
        {
            get => _thicknessDetailResult; private set { if (_thicknessDetailResult != value) { _thicknessDetailResult = value; OnPropertyChanged(); } }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
