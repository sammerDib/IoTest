using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using UnitySC.Shared.UI.Extensions;

namespace UnitySC.PM.EME.Client.Controls.Extensions
{
    #region Documentation Tags

    /// <summary>
    ///     WPF Maskable TextBox class. Just specify the SimpleTextBoxMaskExt.Mask attached property to a TextBox.
    ///     It protect your TextBox from unwanted non numeric symbols and make it easy to modify your numbers.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Class Information:
    ///	    <list type="bullet">
    ///         <item name="authors">Authors: Ruben Hakopian</item>
    ///         <item name="date">February 2009</item>
    ///         <item name="originalURL">http://www.rubenhak.com/?p=8</item>
    ///     </list>
    /// </para>
    /// </remarks>

    #endregion Documentation Tags

    public class SimpleTextBoxMaskExt
    {
        #region Mask Property

        public static MaskType GetMask(DependencyObject obj)
        {
            return (MaskType)obj.GetValue(MaskProperty);
        }

        public static void SetMask(DependencyObject obj, MaskType value)
        {
            obj.SetValue(MaskProperty, value);
        }

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.RegisterAttached(
                "Mask",
                typeof(MaskType),
                typeof(SimpleTextBoxMaskExt),
                new FrameworkPropertyMetadata(MaskChangedCallback)
                );

        private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            if (_this == null)
                return;

            if (!(e.OldValue is null))
            {
                _this.PreviewTextInput -= TextBox_PreviewTextInput;
                _this.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _this.MouseDoubleClick -= TextBox_MouseDoubleClick;
                DataObject.RemovePastingHandler(_this, (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            if ((MaskType)e.NewValue != MaskType.Any)
            {
                _this.PreviewTextInput += TextBox_PreviewTextInput;
                _this.PreviewKeyDown += TextBox_PreviewKeyDown;
                _this.MouseDoubleClick += TextBox_MouseDoubleClick;
                DataObject.AddPastingHandler(_this, (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            ValidateTextBox(_this);
        }

        private static void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private static void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox)
            {
                (sender as TextBox).SelectAll();
            }
        }

        #endregion Mask Property

        #region Private Static Methods

        private static void ValidateTextBox(TextBox _this)
        {
            if (GetMask(_this) != MaskType.Any)
            {
                //_this.Text = ValidateValue(GetMask(_this), _this.Text, GetMinimumValue(_this), GetMaximumValue(_this));
            }
        }

        private static void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            string clipboard = e.DataObject.GetData(typeof(string)) as string;
            if (!IsValidateValue(GetMask(_this), GetText(_this, clipboard)))
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            string curChar = e.Text;

            bool isValid = IsSymbolValid(GetMask(_this), curChar);

            if (!isValid)
            {
                e.Handled = true;
                return;
            }

            var newText = GetText(_this, curChar);

            if (newText.IndexOf("-") > 0)
            {
                e.Handled = true;
                return;
            }

            // We ensure that there is 0 or 1 decimal point
            if ((newText.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) >= 0) &&
                (newText.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) != newText.LastIndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)))
            {
                e.Handled = true;
                return;
            }

            //_this.Text = text;
            e.Handled = false;
        }

        private static string GetText(TextBox textBox, string input)
        {
            int selectionStart = textBox.SelectionStart;
            if (textBox.Text.Length < selectionStart)
                selectionStart = textBox.Text.Length;

            int selectionLength = textBox.SelectionLength;
            if (textBox.Text.Length < selectionStart + selectionLength)
                selectionLength = textBox.Text.Length - selectionStart;

            var realtext = textBox.Text.Remove(selectionStart, selectionLength);

            int caretIndex = textBox.CaretIndex;
            if (realtext.Length < caretIndex)
                caretIndex = realtext.Length;

            var newtext = realtext.Insert(caretIndex, input);

            return newtext;
        }

        private static bool IsValidateValue(MaskType mask, string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim();
            switch (mask)
            {
                case MaskType.Integer:
                case MaskType.PositiveInteger:
                    try
                    {
                        Convert.ToInt64(value);
                        return true;
                    }
                    catch
                    {
                    }
                    return false;

                case MaskType.Decimal:
                case MaskType.PositiveDecimal:
                    try
                    {
                        Convert.ToDouble(value);

                        return true;
                    }
                    catch
                    {
                    }
                    return false;
            }

            return true;
        }

        private static double ValidateLimits(double min, double max, double value)
        {
            if (!min.Equals(double.NaN))
            {
                if (value < min)
                    return min;
            }

            if (!max.Equals(double.NaN))
            {
                if (value > max)
                    return max;
            }

            return value;
        }

        private static bool IsSymbolValid(MaskType mask, string str)
        {
            switch (mask)
            {
                case MaskType.Any:
                    return true;

                case MaskType.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                case MaskType.PositiveInteger:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return false;
                    break;

                case MaskType.Decimal:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                case MaskType.PositiveDecimal:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) return true;
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return false;
                    break;
            }

            if (mask.Equals(MaskType.Integer) || mask.Equals(MaskType.Decimal) || mask.Equals(MaskType.PositiveInteger) || mask.Equals(MaskType.PositiveDecimal))
            {
                foreach (char ch in str)
                {
                    if (!Char.IsDigit(ch))
                        return false;
                }

                return true;
            }

            return false;
        }

        #endregion Private Static Methods
    }
}
