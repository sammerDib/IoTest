using System.Windows;

namespace UnitySC.Shared.UI.Extensions
{
    public class TabControlExt
    {
        public static int GetMinTabWidth(DependencyObject obj)
        {
            return (int)obj.GetValue(MinTabWidthProperty);
        }

        public static void SetMinTabWidth(DependencyObject obj, int value)
        {
            obj.SetValue(MinTabWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for MinTabWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinTabWidthProperty =
            DependencyProperty.RegisterAttached("MinTabWidth", typeof(int), typeof(TabControlExt), new PropertyMetadata(0));



    }
}
