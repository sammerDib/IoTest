using System.Windows;
using System.Windows.Controls;

using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class SortingButton : Control
    {
        static SortingButton()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ControlsResources)));
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
