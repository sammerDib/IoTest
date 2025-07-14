using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class TimePicker : Control
    {
        private const string ElementTextBox = "PART_TextBox";
        private const string ElementButton = "PART_Button";
        private const string ElementPopup = "PART_Popup";

        private bool _lockChangeTime;

        private TimeSelector _timeSelector;
        private Button _dropDownButton;
        private Popup _popUp;

        static TimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(typeof(TimePicker)));
        }

        public TimePicker()
        {
            InitializeTimeSelector();
        }

        public override void OnApplyTemplate()
        {
            if (_popUp != null)
            {
                _popUp.Opened -= PopUp_Opened;
                _popUp.Closed -= PopUp_Closed;
                _popUp.Child = null;
            }
            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= DropDownButton_Click;
            }
            base.OnApplyTemplate();
            _popUp = GetTemplateChild(ElementPopup) as Popup;

            if (_popUp != null)
            {
                _popUp.Opened += PopUp_Opened;
                _popUp.Closed += PopUp_Closed;
                _popUp.Child = _timeSelector;
                if (IsDropDownOpen) _popUp.IsOpen = true;
            }
            _dropDownButton = GetTemplateChild(ElementButton) as Button;
            if (_dropDownButton != null)
            {
                _dropDownButton.Click += DropDownButton_Click;
            }
        }

        #region TimeSelector

        private void InitializeTimeSelector()
        {
            _timeSelector = new TimeSelector();
            _timeSelector.SecondButtonMouseUp += TimeSelector_MinuteButtonMouseUp;
            _timeSelector.SelectedHourChanged += TimeSelector_SelectedHourChanged;
            _timeSelector.SelectedMinuteChanged += TimeSelectorOnSelectedMinuteChanged;
            _timeSelector.SelectedSecondChanged += TimeSelectorOnSelectedSecondChanged;

            _timeSelector.PreviewKeyDown += TimeSelectorOnPreviewKeyDown;
            _timeSelector.HorizontalAlignment = HorizontalAlignment.Left;
            _timeSelector.VerticalAlignment = VerticalAlignment.Top;

            _timeSelector.SetBinding(ForegroundProperty, GetBinding(ForegroundProperty));
            _timeSelector.SetBinding(FlowDirectionProperty, GetBinding(FlowDirectionProperty));
            RenderOptions.SetClearTypeHint(_timeSelector, ClearTypeHint.Enabled);
        }

        private BindingBase GetBinding(DependencyProperty property)
        {
            return new Binding(property.Name)
            {
                Source = this
            };
        }

        private void TimeSelectorOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape && e.Key != Key.Return && e.Key != Key.Space) return;
            IsDropDownOpen = false;
        }

        private void TimeSelectorOnSelectedSecondChanged(object sender, EventArgs e) => Value = new TimeSpan(Value.Hours, Value.Minutes, _timeSelector.SelectedSecond);

        private void TimeSelectorOnSelectedMinuteChanged(object sender, EventArgs e) => Value = new TimeSpan(Value.Hours, _timeSelector.SelectedMinute, Value.Seconds);

        private void TimeSelector_SelectedHourChanged(object sender, EventArgs e) => Value = new TimeSpan(_timeSelector.SelectedHour, Value.Minutes, Value.Seconds);

        private void TimeSelector_MinuteButtonMouseUp(object sender, MouseButtonEventArgs e) => IsDropDownOpen = false;


        #endregion

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePopUp();
        }

        private void PopUp_Closed(object sender, EventArgs e)
        {
            if (IsDropDownOpen) IsDropDownOpen = false;
        }

        private void PopUp_Opened(object sender, EventArgs e)
        {
            if (!IsDropDownOpen) IsDropDownOpen = true;
        }

        private void TogglePopUp()
        {
            IsDropDownOpen = !IsDropDownOpen;
        }

        #region Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(TimeSpan), typeof(TimePicker), new FrameworkPropertyMetadata(default(TimeSpan), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValueCallback));


        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static object CoerceValueCallback(DependencyObject d, object basevalue)
        {
            var control = (TimePicker)d;
            control.DisplayValue((TimeSpan)basevalue);
            return basevalue;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimePicker)d;
            control.DisplayValue((TimeSpan)e.NewValue);
        }

        #endregion

        #region ValueAsString

        private void DisplayValue(TimeSpan time)
        {
            _lockChangeTime = true;

            var formattedString = $"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}";
            ValueAsString = formattedString;

            _timeSelector.SelectedHour = time.Hours;
            _timeSelector.SelectedMinute = time.Minutes;
            _timeSelector.SelectedSecond = time.Seconds;

            _lockChangeTime = false;
        }

        public static readonly DependencyProperty ValueAsStringProperty = DependencyProperty.Register(
            "ValueAsString", typeof(string), typeof(TimePicker), new PropertyMetadata(default(string), OnValueAsStringChanged));

        private static void OnValueAsStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimePicker)d;
            if (control._lockChangeTime) return;

            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            try
            {
                var format = ((string)e.NewValue).Split(':');
                hours = int.Parse(format[0]);
                minutes = int.Parse(format[1]);
                seconds = int.Parse(format[2]);
            }
            catch
            {
                // ignored
            }
            finally
            {
                control.Value = new TimeSpan(hours, minutes, seconds);
            }
        }

        public string ValueAsString
        {
            get { return (string)GetValue(ValueAsStringProperty); }
            set { SetValue(ValueAsStringProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            nameof(IsDropDownOpen), typeof(bool), typeof(TimePicker),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
        {
            if (((TimePicker)d).IsEnabled)
            {
                return baseValue;
            }
            return false;
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tp = (TimePicker)d;
            var newValue = (bool)e.NewValue;

            if (tp._popUp == null || tp._popUp.IsOpen == newValue) return;
            tp._popUp.IsOpen = newValue;
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
    }
}
