namespace UnitySC.Shared.UI.Controls
{
    public class IntegerUpDown : NumericUpDown<int>
    {
        public IntegerUpDown()
        {
            Step = 1;
        }

        protected override int DecrementValue(int value, int increment)
        {
            if (value > Minimum)
            {
                value -= increment;
                if (value < Minimum)
                    value = Minimum;
            }
            return value;
        }

        protected override int IncrementValue(int value, int increment)
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
                this.InputMask = Extensions.MaskType.Integer;
            else
                this.InputMask = Extensions.MaskType.PositiveInteger;
            InternalValue = Value;
        }
    }
}
