namespace UnitySC.Shared.UI.Controls
{
    public class DoubleUpDown : NumericUpDown<double>
    {
        public DoubleUpDown()
        {
            Step = 1;
        }

        protected override double DecrementValue(double value, double increment)
        {
            if (value > Minimum)
            {
                value -= increment;
                if (value < Minimum)
                    value = Minimum;
            }
            return value;
        }

        protected override double IncrementValue(double value, double increment)
        {
            if (value < Maximum)
            {
                value += increment;
                if (value > Maximum)
                    value = Maximum;
            }
            return value;
        }

        protected override void LimitsChanged()
        {
            if (Minimum < 0)
                this.InputMask = Extensions.MaskType.Decimal;
            else
                this.InputMask = Extensions.MaskType.PositiveDecimal;
            InternalValue = Value;
        }
    }
}
