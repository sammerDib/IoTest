namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class UShortBox : NumericBox<ushort>
    {
        protected override TryParseHandler<ushort> TypeTryParseHandler => ushort.TryParse;

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
