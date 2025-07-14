using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Sort.Controls
{
    public class SortingButton : Control
    {
        static SortingButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SortingButton), new FrameworkPropertyMetadata(typeof(SortingButton)));
        }

        public static readonly DependencyProperty SortEngineProperty = DependencyProperty.Register(
            nameof(SortEngine), typeof(ISortEngine), typeof(SortingButton), new PropertyMetadata(default(ISortEngine)));

        public ISortEngine SortEngine
        {
            get { return (ISortEngine)GetValue(SortEngineProperty); }
            set { SetValue(SortEngineProperty, value); }
        }
    }
}
