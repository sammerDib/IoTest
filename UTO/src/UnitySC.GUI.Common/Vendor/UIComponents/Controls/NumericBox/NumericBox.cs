using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.NumericBox
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public abstract class NumericBox<T> : Control where T : struct, IFormattable, IComparable<T>
    {
        private const string TemplateTextBoxName = "PART_TextBox";

        private TextBox TextBox { get; set; }
        
        private bool _isSyncingTextAndValueProperties;
        private bool _internalValueSet;

        #region Constructors

        static NumericBox()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ControlsResources)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericBox<T>), new FrameworkPropertyMetadata(typeof(NumericBox<T>)));
        }

        protected NumericBox()
        {
            IsKeyboardFocusWithinChanged += NumericBox_IsKeyboardFocusWithinChanged;
        }

        #endregion

        #region Dependency Properties

        #region StringFormat

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            nameof(StringFormat), typeof(string), typeof(NumericBox<T>), new PropertyMetadata(string.Empty, StringFormatChangedCallback));

        private static void StringFormatChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericBox<T> numericBox)
            {
                if (!numericBox.IsInitialized) return;
                numericBox.UpdateTextBox();
            }
        }
        
        [Category("Main")]
        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        #endregion

        #region FormatProvider

        public static readonly DependencyProperty FormatProviderProperty = DependencyProperty.Register(
            nameof(FormatProvider), typeof(IFormatProvider), typeof(NumericBox<T>), new PropertyMetadata(CultureInfo.InvariantCulture, FormatProviderPropertyChangedCallback));

        [Category("Main")]
        public IFormatProvider FormatProvider
        {
            get { return (IFormatProvider)GetValue(FormatProviderProperty); }
            set { SetValue(FormatProviderProperty, value); }
        }

        private static void FormatProviderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NumericBox<T>;
            control?.UpdateTextBox();
        }

        #endregion

        #region Value

        public T? Value
        {
            get { return (T?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value), typeof(T?), typeof(NumericBox<T>),
                new FrameworkPropertyMetadata(default(T?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChangedCallback, null, false,
                    UpdateSourceTrigger.PropertyChanged));

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericBox<T> numericBox)
            {
                if (!numericBox.IsInitialized || numericBox._internalValueSet) return;
                numericBox.UpdateTextBox();
            }
        }

        private void UpdateTextBox()
        {
            if (_isSyncingTextAndValueProperties) return;
            _isSyncingTextAndValueProperties = true;

            // Update the textBox value
            if (TextBox != null)
            {
                TextBox.Text = ConvertValueToText();
            }

            _isSyncingTextAndValueProperties = false;
        }

        #endregion

        #region Increment

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(
            nameof(Increment), typeof(T?), typeof(NumericBox<T>), new PropertyMetadata(default(T?)));

        [Category("Main")]
        public T? Increment
        {
            get { return (T?)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        protected abstract void DoIncrement();

        protected abstract void DoDecrement();

        #endregion

        #region DefaultValue

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(
            nameof(DefaultValue), typeof(T?), typeof(NumericBox<T>), new PropertyMetadata(default(T?)));

        [Category("Main")]
        public T? DefaultValue
        {
            get { return (T?)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        #endregion

        #endregion
        
        #region Event Handlers

        private void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!IsKeyboardFocusWithin || _isSyncingTextAndValueProperties) return;
            if (e.Delta > 0)
            {
                DoIncrement();
                e.Handled = true;
            }
            else if (e.Delta < 0)
            {
                DoDecrement();
                e.Handled = true;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsKeyboardFocusWithin || _isSyncingTextAndValueProperties) return;
            if (e.Key == Key.Up)
            {
                DoIncrement();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                DoDecrement();
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsKeyboardFocusWithin || _isSyncingTextAndValueProperties) return;
            UpdateValue(((TextBox)sender).Text);
        }

        private void NumericBox_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) return;
            UpdateTextBox();
        }

        private void UpdateValue(string text)
        {
            if (_isSyncingTextAndValueProperties) return;

            _isSyncingTextAndValueProperties = true;

            if (text == null)
            {
                SetValueInternal(DefaultValue);
            }
            else
            {
                try
                {
                    var currentValue = ConvertTextToValue(text);
                    if (!Equals(currentValue, Value))
                    {
                        SetValueInternal(currentValue);
                    }
                }
                catch
                {
                    // ignored
                }
            }

            _isSyncingTextAndValueProperties = false;
        }

        private void SetValueInternal(T? value)
        {
            _internalValueSet = true;
            try
            {
                Value = value;
            }
            finally
            {
                _internalValueSet = false;
            }
        }

        #endregion

        #region Parsing

        protected delegate bool TryParseHandler<TKey>(string value, NumberStyles style, IFormatProvider provider, out TKey result);

        protected abstract TryParseHandler<T> TypeTryParseHandler { get; }

        private static bool TryParse(string value, NumberStyles style, IFormatProvider provider, TryParseHandler<T> handler, out T result)
        {
            result = default;
            return !string.IsNullOrEmpty(value) && handler(value, style, provider, out result);
        }

        #endregion

        #region Convert

        private string ConvertValueToText()
        {
            SetIsConversionValid(true);
            if (!Value.HasValue) return string.Empty;
            return Value.Value.ToString(StringFormat, FormatProvider);
        }
        
        private T? ConvertTextToValue(string text)
        {
            if (text == null) return null;

            var currentValueText = ConvertValueToText();
            if (Equals(currentValueText, text))
            {
                SetIsConversionValid(true);
                return Value;
            }

            return ConvertTextToValueCore(currentValueText, text);
        }

        private T? ConvertTextToValueCore(string currentValueText, string text)
        {
            if (!TryParse(text,NumberStyles.Any, FormatProvider, TypeTryParseHandler, out var result1))
            {
                var hasError = true;
                if (!TryParse(currentValueText,NumberStyles.Any, FormatProvider, TypeTryParseHandler, out _))
                {
                    var chars = currentValueText.Where(c => !char.IsDigit(c)).ToList();
                    if (chars.Any())
                    {
                        IEnumerable<char> second = text.Where(c => !char.IsDigit(c)).ToList();
                        if (chars.Except(second).ToList().Count == 0)
                        {
                            foreach (var ch in second)
                            {
                                text = text.Replace(ch.ToString(), string.Empty);
                            }
                            if (TryParse(text,NumberStyles.Any, FormatProvider, TypeTryParseHandler, out result1))
                            {
                                hasError = false;
                            }
                        }
                    }
                }

                SetIsConversionValid(!hasError);

                if (hasError)
                {
                    throw new InvalidDataException("Input string was not in a correct format.");
                }
            }
            else
            {
                SetIsConversionValid(true);
            }

            return result1;
        }

        private static (bool, INotifyConversionErrorInfo) GetNotifyConversionErrorInfo(BindingExpression expression)
        {
            if (expression.ResolvedSource is INotifyConversionErrorInfo source)
            {
                return (true, source);
            }

            if (expression.DataItem is INotifyConversionErrorInfo dataItem)
            {
                return (false, dataItem);
            }

            return (false, null);
        }

        private void SetIsConversionValid(bool valid)
        {
            var bindingExpression = GetBindingExpression(ValueProperty);
            if (bindingExpression == null)
            {
                return;
            }

            var (isSource, conversionErrorInfo) = GetNotifyConversionErrorInfo(bindingExpression);
            
            if (valid)
            {
                conversionErrorInfo?.ClearConversionError(bindingExpression.ResolvedSourcePropertyName);

                // [TLa] If the INotifyConversionErrorInfo implementation is not the direct parent of the expression, clear invalidate the expression to allow the deletion of the error on the control.
                if (!isSource)
                {
                    Validation.ClearInvalid(bindingExpression);
                }
            }
            else
            {
                var errorText = LocalizationManager.GetString(nameof(ControlsResources.CONTROLS_CONVERSION_VALIDATION_ERROR), TextBox?.Text);
                conversionErrorInfo?.AddConversionError(bindingExpression.ResolvedSourcePropertyName, errorText);

                // [TLa] If the INotifyConversionErrorInfo implementation is not the direct parent of the expression, invalidate the expression to allow the error to be displayed on the control.
                if (!isSource)
                {
                    Validation.MarkInvalid(bindingExpression, new ValidationError(new ExceptionValidationRule(), bindingExpression)
                    {
                        ErrorContent = errorText
                    });
                }
            }
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (TextBox != null)
            {
                TextBox.TextChanged -= TextBox_TextChanged;
                TextBox.PreviewKeyDown -= TextBox_KeyDown;
                TextBox.PreviewMouseWheel -= TextBox_PreviewMouseWheel;
            }

            TextBox = GetTemplateChild(TemplateTextBoxName) as TextBox;
            if (TextBox != null)
            {
                UpdateTextBox();
                TextBox.TextChanged += TextBox_TextChanged;
                TextBox.PreviewKeyDown += TextBox_KeyDown;
                TextBox.PreviewMouseWheel += TextBox_PreviewMouseWheel;
            }
        }

        #endregion
    }
}
