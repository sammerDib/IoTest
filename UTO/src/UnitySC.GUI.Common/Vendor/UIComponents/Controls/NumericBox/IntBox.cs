namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class IntBox : NumericBox<int>
    {
        protected override TryParseHandler<int> TypeTryParseHandler => int.TryParse;

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
