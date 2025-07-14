using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class ButtonsBox : ItemsControl
    {
        private const string ElementToggleButton = "PART_ToggleButton";

        static ButtonsBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonsBox), new FrameworkPropertyMetadata(typeof(ButtonsBox)));
        }

        private ToggleButton _toggleButton;
        private bool _mouseDown;

        #region Overrides of FrameworkElement

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EventManager.RegisterClassHandler(typeof(Button), ButtonBase.ClickEvent, new RoutedEventHandler(OnButtonClick));

            _toggleButton = GetTemplateChild(ElementToggleButton) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.PreviewMouseDown += OnToggleButtonPreviewMouseDown;
                _toggleButton.PreviewMouseUp += OnToggleButtonPreviewMouseUp;
            }
        }

        #endregion

        #region Events

        private void OnToggleButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Prevent the popup from opening if the mouseDown event has not been raised.
            if (!_mouseDown)
            {
                e.Handled = true;
                _toggleButton.ReleaseMouseCapture();
            }

            _mouseDown = false;
        }

        private void OnToggleButtonPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
        }

        /// <summary>
        /// Closes the popup if a button has been clicked inside the popup.
        /// </summary>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content), typeof(object), typeof(ButtonsBox), new PropertyMetadata(default(object)));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            nameof(IsDropDownOpen), typeof(bool), typeof(ButtonsBox), new PropertyMetadata(default(bool)));

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        private void Close()
        {
            if (IsDropDownOpen)
            {
                IsDropDownOpen = false;
            }
        }
    }
}
