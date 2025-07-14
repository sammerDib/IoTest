using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class ChipsItem : ListBoxItem
    {
        static ChipsItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChipsItem), new FrameworkPropertyMetadata(typeof(ChipsItem)));
        }

        #region Overrides of ListBoxItem

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            Focus();
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                CaptureMouse();
                if (IsMouseCaptured)
                {
                    if (e.ButtonState == MouseButtonState.Pressed)
                    {
                        if (!IsPressed) IsPressed = true;
                    }
                    else ReleaseMouseCapture();
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        // Activate selection on MouseUp instead of MouseDown
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured) ReleaseMouseCapture();
            if (IsPressed)
            {
                base.OnMouseLeftButtonDown(e);
                IsPressed = false;
            }
            e.Handled = true;
        }

        #endregion

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!IsMouseCaptured || (Mouse.PrimaryDevice.LeftButton != MouseButtonState.Pressed)) return;
            UpdateIsPressed();
            e.Handled = true;
        }

        private void UpdateIsPressed()
        {
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (position.X >= 0.0 && position.X <= ActualWidth && position.Y >= 0.0 && position.Y <= ActualHeight)
            {
                if (IsPressed) return;
                IsPressed = true;
            }
            else
            {
                if (!IsPressed) return;
                IsPressed = false;
            }
        }

        public static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsPressed), typeof(bool), typeof(ChipsItem), new FrameworkPropertyMetadata(default(bool),
                FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            protected set { SetValue(IsPressedPropertyKey, value); }
        }
    }
}
