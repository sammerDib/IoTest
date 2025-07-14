using System;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness
{
    public class LayerDetailInfoVM : ObservableObject
    {
        #region Fields

        private int _digits;
        private readonly Color _color;
        private readonly Color _foreground;

        private readonly Length _target;
        private readonly LengthTolerance _tolerance;

        private Length _value;

        #endregion

        #region Properties

        public string LayerName { get; }

        public LengthUnit Unit { get; }

        public SolidColorBrush Background => new SolidColorBrush(_color);

        public SolidColorBrush Foreground => new SolidColorBrush(_foreground);

        public Length LengthValue => _value;

        public bool IsMeasured { get; }

        public string ToolTip
        {
            get
            {
                string notMeasured = IsMeasured ? string.Empty : " (Not measured)";
                if (_target != null && _tolerance != null)
                {
                    return $"{LayerName}{notMeasured}{Environment.NewLine}Target = {GetFormattedTarget()}{Environment.NewLine}Tolerance = +/-{GetFormattedTolerance()}";
                }
                return $"{LayerName}{notMeasured}{Environment.NewLine}Target = -{Environment.NewLine}Tolerance = -";
            }
        }

        public string Value
        {
            get
            {
                if (IsMeasured)
                {
                    return LengthToStringConverter.ConvertToString(_value, _digits, true, "-", Unit);
                }

                return $"Target = {GetFormattedTarget()}";
            }
        }

        public string Deviation
        {
            get
            {
                if (_target == null)
                {
                    return "No target";
                }

                if (IsMeasured)
                {
                    var deviation = _value != null ? _value - _target : null;
                    return LengthToStringConverter.ConvertToString(deviation, _digits, true, "-", Unit);
                }

                return "-";
            }
        }

        #endregion

        private string GetFormattedTarget() => LengthToStringConverter.ConvertToString(_target, _digits, true, "-", Unit);

        private string GetFormattedTolerance() => LengthToleranceToStringConverter.ConvertToString(_tolerance, _digits, true, "-", Unit);

        public LayerDetailInfoVM(string layerName, ThicknessLengthSettings layerSetting, MetroStatsContainer stats, int digits, Color color, LengthUnit unit) : this(layerName, layerSetting, digits, color, unit)
        {
            _value = stats != null && stats.State != MeasureState.NotMeasured ? stats.Mean : null;
        }

        public LayerDetailInfoVM(string layerName, ThicknessLengthSettings layerSetting, int digits, Color color, LengthUnit unit)
        {
            LayerName = layerName;
            _digits = digits;
            _color = color;
            _foreground = ColorHelper.GetIdealForeground(color);
            Unit = unit;

            _target = layerSetting?.Target;
            _tolerance = layerSetting?.Tolerance;

            IsMeasured = layerSetting?.IsMeasured ?? false;
        }

        public LayerDetailInfoVM(string layerName, Length target, LengthTolerance tolerance, Length value, int digits, Color color, LengthUnit unit)
        {
            LayerName = layerName;
            _digits = digits;
            _color = color;
            _foreground = ColorHelper.GetIdealForeground(color);
            Unit = unit;
            _target = target;
            _tolerance = tolerance;
            _value = value;
            IsMeasured = true;
        }

        public void UpdateDigits(int digits)
        {
            _digits = digits;
            //TODO : RaisePropertyChange(null);
            OnPropertyChanged(nameof(Value));
        }

        public void SetValue(Length length)
        {
            _value = length;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(Deviation));
        }
    }
}
