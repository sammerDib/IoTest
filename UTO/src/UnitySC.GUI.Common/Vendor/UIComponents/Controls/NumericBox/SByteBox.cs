namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class SByteBox : NumericBox<sbyte>
    {
        protected override TryParseHandler<sbyte> TypeTryParseHandler => sbyte.TryParse;

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
