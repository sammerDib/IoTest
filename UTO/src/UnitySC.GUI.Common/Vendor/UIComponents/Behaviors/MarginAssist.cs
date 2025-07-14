using System;
using System.Windows;
using System.Windows.Markup;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public class MarginAssist : MarkupExtension
    {
        public double Left { get; set; }

        public double Top { get; set; }

        public double Right { get; set; }

        public double Bottom { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Thickness(Left, Top, Right, Bottom);
        }
    }
}
