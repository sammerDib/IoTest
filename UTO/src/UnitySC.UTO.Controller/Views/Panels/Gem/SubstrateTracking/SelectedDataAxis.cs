using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using LiveCharts;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking
{
    public class SelectedDataAxis : AxisWindow
    {
        private readonly List<double> _values;
        private readonly List<double> _totalSecondsSuppressed;

        public SelectedDataAxis(List<double> values, List<double> totalSecondsSuppressed)
        {
            if (values.Count != totalSecondsSuppressed.Count)
            {
                values = new List<double>();
                totalSecondsSuppressed = new List<double>();
            }
            _values = values;
            _totalSecondsSuppressed = totalSecondsSuppressed;
        }

        public override double MinimumSeparatorWidth => 10;

        public override bool IsHeader(double x)
        {
            return false;
        }

        public override bool IsSeparator(double x)
        {
            return _values.Any(val => Math.Floor(val) + 0.1 > x && Math.Floor(val) - 0.1 < x);
        }

        public override string FormatAxisLabel(double x)
        {
            var value = _values.FirstOrDefault(
                val => Math.Floor(val) + 0.1 > x && Math.Floor(val) - 0.1 < x);
            int valueIndex = _values.IndexOf(value);
            var valueTick = (value + _totalSecondsSuppressed[valueIndex]) * TimeSpan.TicksPerSecond;
            var valueTickLong = (long)valueTick;
            var t = new TimeSpan(valueTickLong);
            return $"{new DateTime(t.Ticks).ToString("T", CultureInfo.CurrentUICulture)}";
        }
    }
}
