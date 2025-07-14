namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    public class FloatBox : NumericBox<float>
    {
        protected override TryParseHandler<float> TypeTryParseHandler => float.TryParse;

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
