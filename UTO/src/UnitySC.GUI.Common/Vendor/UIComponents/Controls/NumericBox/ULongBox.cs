namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class ULongBox : NumericBox<ulong>
    {
        protected override TryParseHandler<ulong> TypeTryParseHandler => ulong.TryParse;

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
