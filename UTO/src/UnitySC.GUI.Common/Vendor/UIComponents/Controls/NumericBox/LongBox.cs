namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class LongBox : NumericBox<long>
    {
        protected override TryParseHandler<long> TypeTryParseHandler => long.TryParse;

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
