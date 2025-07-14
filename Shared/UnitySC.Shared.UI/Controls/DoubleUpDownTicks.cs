using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UnitySC.Shared.UI.Controls
{
    /// <summary>
    /// Un DoubleUpDown qui peut prendre une liste de valeurs pour l'incrément/décrement
    /// </summary>
    public class DoubleUpDownTicks : Xceed.Wpf.Toolkit.DoubleUpDown
    {
        protected override double DecrementValue(double value, double increment)
        {
            if (IncrementTicks == null)
                return base.DecrementValue(value, increment);

            int idx = FindClosestSliderTickIndex(value, out bool isExact);
            if (idx == 0)
                return value;
            else if (value > IncrementTicks.Last())
                return IncrementTicks.Last();
            else
                return IncrementTicks[idx - 1];
        }

        protected override double IncrementValue(double value, double increment)
        {
            if (IncrementTicks == null)
                return base.IncrementValue(value, increment);

            int idx = FindClosestSliderTickIndex(value, out bool isExact);
            if (!isExact)
                return IncrementTicks[idx];
            else if (idx == IncrementTicks.Count() - 1)
                return value;
            else
                return IncrementTicks[idx + 1];
        }

        private int FindClosestSliderTickIndex(double value, out bool isExact)
        {
            int i;
            for (i = 0; i < IncrementTicks.Count(); i++)
            {
                if (IncrementTicks[i] >= value)
                    break;
            }
            if (i >= IncrementTicks.Count())
            {
                isExact = false;
                return i - 1;
            }
            else
            {
                isExact = IncrementTicks[i] == value;
                return i;
            }
        }

        //=================================================================
        // Dependency properties
        //=================================================================
        public IList<double> IncrementTicks
        {
            get { return (IList<double>)GetValue(IncrementTicksProperty); }
            set { SetValue(IncrementTicksProperty, value); }
        }

        public static readonly DependencyProperty IncrementTicksProperty =
            DependencyProperty.Register("IncrementTicks", typeof(IList<double>), typeof(DoubleUpDownTicks), new PropertyMetadata(null));
    }
}
