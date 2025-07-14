using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    /// <summary>
    /// This control provides NeedHorizontalResponsive = true if the width of its content is greater than its own width
    /// and NeedVerticalResponsive = true if the height of the content is greater than its own height.
    /// </summary>
    public class ResponsiveContentControl : ContentControl
    {
        #region Apply Default Style

        static ResponsiveContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResponsiveContentControl), new FrameworkPropertyMetadata(typeof(ResponsiveContentControl)));
        }

        #endregion

        private FrameworkElement _container;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SizeChanged += OnSizeChanged;

            _container = GetTemplateChild("PART_StackPanel") as FrameworkElement;
            if (_container != null)
            {
                _container.SizeChanged += ContentSizeChanged;
                _container.Loaded += ContainerLoaded;
            }
            
            ContentSizeChanged(null, null);
        }

        private void ContainerLoaded(object sender, RoutedEventArgs e)
        {
            ContentSizeChanged(null, null);
        }

        private void ContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsArrangeValid || !IsInitialized) return;

            if (_container == null || !_container.IsLoaded) return;

            if (_container.ActualHeight > _lastHeight)
            {
                _lastHeight = _container.ActualHeight;
                NeedVerticalResponsive = _container.ActualHeight > ActualHeight;
            }

            if (_container.ActualWidth > _lastWidth)
            {
                _lastWidth = _container.ActualWidth;
                NeedHorizontalResponsive = _container.ActualWidth > ActualWidth;
            }
        }

        private double _lastHeight;
        private double _lastWidth;

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            NeedVerticalResponsive = false;
            NeedHorizontalResponsive = false;
            _lastHeight = 0;
            _lastWidth = 0;

            ContentSizeChanged(null, null);
        }

        public static readonly DependencyProperty NeedHorizontalResponsiveProperty = DependencyProperty.Register(nameof(NeedHorizontalResponsive), typeof(bool), typeof(ResponsiveContentControl), new PropertyMetadata(default(bool)));

        public bool NeedHorizontalResponsive
        {
            get { return (bool)GetValue(NeedHorizontalResponsiveProperty); }
            set { SetValue(NeedHorizontalResponsiveProperty, value); }
        }

        public static readonly DependencyProperty NeedVerticalResponsiveProperty = DependencyProperty.Register(nameof(NeedVerticalResponsive), typeof(bool), typeof(ResponsiveContentControl), new PropertyMetadata(default(bool)));

        public bool NeedVerticalResponsive
        {
            get { return (bool)GetValue(NeedVerticalResponsiveProperty); }
            set { SetValue(NeedVerticalResponsiveProperty, value); }
        }
    }
}
