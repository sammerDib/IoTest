namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class UIntBox : NumericBox<uint>
    {
        protected override TryParseHandler<uint> TypeTryParseHandler => uint.TryParse;

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
