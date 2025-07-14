using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public class ThicknessDetailMeasureInfoVM : MetroDetailMeasureInfoVM<ThicknessPointResult>
    {
        private readonly Func<string, Color> _getLayerColor;

        private Dictionary<string, LengthUnit> _layersUnit = new Dictionary<string, LengthUnit>();

        #region Properties

        public ThicknessLayersInfoVM LayersInfo { get; } = new ThicknessLayersInfoVM();

        private string _layer;

        public string Layer
        {
            get { return _layer; }
            set
            {
                SetProperty(ref _layer, value);
                OnPointChanged();
            }
        }

        private ThicknessResultSettings _settings;

        public ThicknessResultSettings Settings
        {
            get { return _settings; }
            set
            {
                if (SetProperty(ref _settings, value))
                {
                    OnPointChanged();
                }
            }
        }

        #endregion

        #region Warp Result
        private static System.Drawing.Color s_validResultColor = System.Drawing.Color.FromArgb(8, 180, 8);
        private static System.Drawing.Color s_invalidResultColor = System.Drawing.Color.FromArgb(216, 18, 18);

        private MeasureState _globlaState;
        public MeasureState GlobalState
        {
            get => _globlaState;
            set => SetProperty(ref _globlaState, value);
        }
        
        private Length _warpResultLength;
        public Length WarpResultLength
        {
            get { return _warpResultLength; }
            set { SetProperty(ref _warpResultLength, value); }
        }
        
        private string _warpResult;
        public string WarpResult
        {
            get { return _warpResult; }
            set { SetProperty(ref _warpResult, value); }
        }

        private System.Drawing.Color _resultColor;
        public System.Drawing.Color ResultColor
        {
            get { return _resultColor; }
            set { SetProperty(ref _resultColor, value); }
        }
        #endregion

        public ThicknessDetailMeasureInfoVM()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                throw new InvalidOperationException("The default constructor can only be called by the designer.");
        }

        public ThicknessDetailMeasureInfoVM(Func<string, Color> getLayerColor)
        {
            _getLayerColor = getLayerColor;
        }


        #region Overrides of MetroDetailMeasureInfoVM

        protected override void OnPointChanged()
        {
            base.OnPointChanged();

            var layers = new List<LayerDetailInfoVM>();
            var warpMax = new Length(0.0, LengthUnit.Micrometer);
            WarpResult = string.Empty;

            if (Point != null && Settings != null)
            {
                switch (Layer)
                {
                    case ThicknessResultVM.TotalLayerName:
                    case ThicknessResultVM.CrossSectionModeName:
                        {
                            LayersInfo.Title = ThicknessResultVM.TotalLayerName;
                            _layersUnit.TryGetValue(ThicknessResultVM.TotalLayerName, out var totalUnit);

                            // All Layers
                            foreach (var layerSetting in Settings.ThicknessLayers)
                            {
                                Point.ThicknessLayerStats.TryGetValue(layerSetting.Name, out var stats);
                                _layersUnit.TryGetValue(layerSetting.Name, out var unit);
                                layers.Add(new LayerDetailInfoVM(layerSetting.Name, layerSetting, stats, Digits, layerSetting.LayerColor, unit));
                            }

                            // Total value & deviation
                            string stringTotalValue = LengthToStringConverter.ConvertToString(Point.TotalThicknessStat.Mean, Digits, true, "-", totalUnit);
                            var totalDelta = Point.TotalThicknessStat.Mean == null || Settings.TotalTarget == null ? null : Point.TotalThicknessStat.Mean - Settings.TotalTarget;
                            string stringTotalDelta = LengthToStringConverter.ConvertToString(totalDelta, Digits, true, "-", totalUnit);
                            LayersInfo.Value = $"Value = {Environment.NewLine}{stringTotalValue}{Environment.NewLine}{Environment.NewLine}Deviation = {Environment.NewLine}{stringTotalDelta}";
                            LayersInfo.ShowTotalArrow = true;

                            // Total target & tolerance
                            string formattedTarget = LengthToStringConverter.ConvertToString(Settings.TotalTarget, Digits, true, "-", totalUnit);
                            string formattedTolerance = LengthToleranceToStringConverter.ConvertToString(Settings.TotalTolerance, Digits, true, "-", totalUnit);
                            LayersInfo.LeftSideTooltip = $"{ThicknessResultVM.TotalLayerName}{Environment.NewLine}Target = {formattedTarget}{Environment.NewLine}Tolerance = +/-{formattedTolerance}";

                            // Wafer Thickness
                            if (Settings.HasWaferThicknesss)
                            {
                                string stringWaferThickness = LengthToStringConverter.ConvertToString(Point.WaferThicknessStat.Mean, Digits, true, "-", totalUnit);
                                var waferThicknessDelta = Point.WaferThicknessStat.Mean == null || Settings.TotalTarget == null ? null : Point.WaferThicknessStat.Mean - Settings.TotalTarget;
                                string stringWaferThicknessDelta = LengthToStringConverter.ConvertToString(waferThicknessDelta, Digits, true, "-", totalUnit);
                                LayersInfo.WaferThickness = $"Value = {Environment.NewLine}{stringWaferThickness}{Environment.NewLine}{Environment.NewLine}Deviation = {Environment.NewLine}{stringWaferThicknessDelta}";
                            }
                            else
                            {
                                LayersInfo.WaferThickness = string.Empty;
                            }

                            break;
                        }
                    case ThicknessResultVM.WaferThickness:
                        {
                            LayersInfo.Title = "Target";
                            _layersUnit.TryGetValue(ThicknessResultVM.TotalLayerName, out var totalUnit);

                            // Wafer Thickness
                            layers.Add(new LayerDetailInfoVM(Layer, Settings?.TotalTarget, Settings?.TotalTolerance, Point.WaferThicknessStat?.Mean, Digits, ThicknessResultVM.WaferThicknessColor, totalUnit));

                            // Total target & tolerance
                            string formattedTarget = LengthToStringConverter.ConvertToString(Settings?.TotalTarget, Digits, true, "-", totalUnit);
                            string formattedTolerance = LengthToleranceToStringConverter.ConvertToString(Settings?.TotalTolerance, Digits, true, "-", totalUnit);
                            LayersInfo.Value = $"{formattedTarget}{Environment.NewLine}+/-{formattedTolerance}";

                            LayersInfo.WaferThickness = string.Empty;
                            LayersInfo.ShowTotalArrow = false;
                            LayersInfo.LeftSideTooltip = null;
                            break;
                        }
                    default:
                        {
                            LayersInfo.Title = "Target";
                            _layersUnit.TryGetValue(Layer, out var unit);

                            var setting = Settings.ThicknessLayers.SingleOrDefault(output => output.Name.Equals(Layer));

                            // Layer
                            Point.ThicknessLayerStats.TryGetValue(Layer, out var stats);
                            layers.Add(new LayerDetailInfoVM(Layer, setting, stats, Digits, _getLayerColor(Layer), unit));

                            // Layer target & tolerance
                            string formattedTarget = LengthToStringConverter.ConvertToString(setting?.Target, Digits, true, "-", unit);
                            string formattedTolerance = LengthToleranceToStringConverter.ConvertToString(setting?.Tolerance, Digits, true, "-", unit);
                            LayersInfo.Value = $"{formattedTarget}{Environment.NewLine}+/-{formattedTolerance}";

                            LayersInfo.WaferThickness = string.Empty;
                            LayersInfo.ShowTotalArrow = false;
                            LayersInfo.LeftSideTooltip = null;
                            break;
                        }
                }

                #region Warp measurement result
                if (Settings.HasWarpMeasure && Settings.WarpTargetMax != null && WarpResultLength != null)
                {
                    warpMax = Settings.WarpTargetMax;

                    WarpResult = LengthToStringConverter.ConvertToString(WarpResultLength, Digits, true, "-", LengthUnit.Micrometer);

                    ResultColor = (0 < WarpResultLength.Value && WarpResultLength.Value < warpMax.Value) ? s_validResultColor : s_invalidResultColor;
                }
                #endregion
            }
            else
            {
                LayersInfo.Title = string.Empty;
                LayersInfo.Value = string.Empty;
                LayersInfo.WaferThickness = string.Empty;
                LayersInfo.LeftSideTooltip = null;
            }

            LayersInfo.LayersDetails = layers;

        }
        #endregion

        public void SetLayersUnit(Dictionary<string, LengthUnit> layersUnit)
        {
            _layersUnit = layersUnit;
        }

    }
}
