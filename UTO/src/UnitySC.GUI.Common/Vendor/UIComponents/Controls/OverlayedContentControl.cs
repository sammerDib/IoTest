using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class OverlayedContentControl : ContentControl
    {
        #region Apply Default Style

        static OverlayedContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayedContentControl), new FrameworkPropertyMetadata(typeof(OverlayedContentControl)));
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsVisibleChanged += OnIsVisibleChanged;
        }

        #region OnDisplayed

        protected override void OnRender(DrawingContext drawingContext)
        {
            OnDisplayed();
            base.OnRender(drawingContext);
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                OnDisplayed();
            }
        }

        private void OnDisplayed()
        {
            OverlayIsVisible = true;
            HideOverlay();
        }

        #endregion

        #region OverlayIsVisible

        public static readonly DependencyProperty OverlayIsVisibleProperty = DependencyProperty.Register(
            "OverlayIsVisible", typeof(bool), typeof(OverlayedContentControl), new PropertyMetadata(default(bool)));

        public bool OverlayIsVisible
        {
            get { return (bool)GetValue(OverlayIsVisibleProperty); }
            set { SetValue(OverlayIsVisibleProperty, value); }
        }

        #endregion

        #region Overlay

        public static readonly DependencyProperty OverlayProperty = DependencyProperty.Register(
            "Overlay", typeof(object), typeof(OverlayedContentControl), new PropertyMetadata(default(object)));

        public object Overlay
        {
            get { return GetValue(OverlayProperty); }
            set { SetValue(OverlayProperty, value); }
        }

        #endregion

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            ShowOverlay();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            HideOverlay();
            base.OnMouseLeave(e);
        }

        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            ShowOverlay();
            base.OnPreviewTouchDown(e);
        }

        protected override void OnPreviewTouchUp(TouchEventArgs e)
        {
            ShowOverlay();
            base.OnPreviewTouchUp(e);
        }

        private DispatcherTimer _dispatcherTimer;

        private void ShowOverlay()
        {
            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Tick -= dispatcherTimer_Tick;
            }
            OverlayIsVisible = true;
        }

        private void HideOverlay()
        {
            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Tick -= dispatcherTimer_Tick;
            }
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _dispatcherTimer.Tick -= dispatcherTimer_Tick;
            _dispatcherTimer.Stop();
            _dispatcherTimer = null;
            OverlayIsVisible = false;
        }
    }
}
