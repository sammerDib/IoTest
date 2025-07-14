namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class DoubleBox : NumericBox<double>
    {
        protected override TryParseHandler<double> TypeTryParseHandler => double.TryParse;
        
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
