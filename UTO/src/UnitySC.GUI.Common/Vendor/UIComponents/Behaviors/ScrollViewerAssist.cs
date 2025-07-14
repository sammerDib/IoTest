using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class ScrollViewerAssist
    {
        public static readonly DependencyProperty IgnorePaddingProperty = DependencyProperty.RegisterAttached(
            "IgnorePadding", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(true));

        public static void SetIgnorePadding(DependencyObject element, bool value) => element.SetValue(IgnorePaddingProperty, value);
        public static bool GetIgnorePadding(DependencyObject element) => (bool)element.GetValue(IgnorePaddingProperty);

        public static readonly DependencyProperty IsAutoHideEnabledProperty = DependencyProperty.RegisterAttached(
            "IsAutoHideEnabled", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(default(bool)));

        public static void SetIsAutoHideEnabled(DependencyObject element, bool value) => element.SetValue(IsAutoHideEnabledProperty, value);

        public static bool GetIsAutoHideEnabled(DependencyObject element) => (bool)element.GetValue(IsAutoHideEnabledProperty);

        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached(
            "SyncHorizontalOffset", typeof(double), typeof(ScrollViewerAssist),
            new PropertyMetadata(default(double), OnSyncHorizontalOffsetChanged));

        private static void OnSyncHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            scrollViewer?.ScrollToHorizontalOffset((double)e.NewValue);
        }

        public static void SetSyncHorizontalOffset(DependencyObject element, double value) => element.SetValue(HorizontalOffsetProperty, value);

        public static double GetSyncHorizontalOffset(DependencyObject element) => (double)element.GetValue(HorizontalOffsetProperty);
    }
}
