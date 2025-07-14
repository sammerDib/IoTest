namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class ByteBox : NumericBox<byte>
    {
        protected override TryParseHandler<byte> TypeTryParseHandler => byte.TryParse;

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
