using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.UI.Controls
{
    public class LabelUnit : Label
    {
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(LabelUnit), new PropertyMetadata(string.Empty));
    }
}
