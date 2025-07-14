namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class ShortBox : NumericBox<short>
    {
        protected override TryParseHandler<short> TypeTryParseHandler => short.TryParse;

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
