namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class DecimalBox : NumericBox<decimal>
    {
        protected override TryParseHandler<decimal> TypeTryParseHandler => decimal.TryParse;
        
        protected override void DoIncrement()
        {
            if (Value.HasValue && Increment.HasValue)
            {
                Value += Increment;
            }
        }

        protected override void DoDecrement()
        {
            if (Value.HasValue && Increment.HasValue)
            {
                Value -= Increment;
            }
        }

    }
}
