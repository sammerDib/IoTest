using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Helper
{
    public class TextBlockUtils
    {
        /// <summary>
        /// Gets the value of the AutoTooltipProperty dependency property
        /// </summary>
        public static bool GetAutoTooltip(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoTooltipProperty);
        }

        /// <summary>
        /// Sets the value of the AutoTooltipProperty dependency property
        /// </summary>
        public static void SetAutoTooltip(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoTooltipProperty, value);
        }

        /// <summary>
        /// Identified the attached AutoTooltip property. When true, this will set the TextBlock TextTrimming
        /// property to WordEllipsis, and display a tooltip with the full text whenever the text is trimmed.
        /// </summary>
        public static readonly DependencyProperty AutoTooltipProperty = DependencyProperty.RegisterAttached("AutoTooltip",
                typeof(bool), typeof(TextBlockUtils), new PropertyMetadata(false, OnAutoTooltipPropertyChanged));

        private static void OnAutoTooltipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
                return;

            if (e.NewValue.Equals(true))
            {
                textBlock.TextTrimming = TextTrimming.WordEllipsis;
                ComputeAutoTooltip(textBlock);
                textBlock.SizeChanged += TextBlock_SizeChanged;
                // assume textBlock is your TextBlock
                var dp = DependencyPropertyDescriptor.FromProperty(
                             TextBlock.TextProperty,
                             typeof(TextBlock));
                dp.AddValueChanged(textBlock, (sender, args) =>
                {
                    TextBlock_SizeChanged(textBlock, null);
                });
            }
            else
            {
                textBlock.SizeChanged -= TextBlock_SizeChanged;
            }
        }

        private static void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            ComputeAutoTooltip(textBlock);
        }

        /// <summary>
        /// Assigns the ToolTip for the given TextBlock based on whether the text is trimmed
        /// </summary>
        private static void ComputeAutoTooltip(TextBlock textBlock)
        {
            var parentElement = VisualTreeHelper.GetParent(textBlock) as FrameworkElement;
            if (parentElement != null)
            {
                if (IsTextTrimmed(textBlock))
                {
                    ToolTipService.SetToolTip(textBlock, textBlock.Text);
                }
                else
                {
                    ToolTipService.SetToolTip(textBlock, null);
                }
            }
        }

        private static bool IsTextTrimmed(TextBlock textBlock)
        {
            if (!textBlock.IsArrangeValid)
            {
                return false;
            }

            var typeface = new Typeface(
                textBlock.FontFamily,
                textBlock.FontStyle,
                textBlock.FontWeight,
                textBlock.FontStretch);

            // FormattedText is used to measure the whole width of the text held up by TextBlock container
            var formattedText = new FormattedText(
                textBlock.Text,
                System.Threading.Thread.CurrentThread.CurrentCulture,
                textBlock.FlowDirection,
                typeface,
                textBlock.FontSize,
                textBlock.Foreground,
                VisualTreeHelper.GetDpi(textBlock).PixelsPerDip);


            //formattedText.MaxTextWidth = textBlock.ActualWidth;

            // When the maximum text width of the FormattedText instance is set to the actual
            // width of the textBlock, if the textBlock is being trimmed to fit then the formatted
            // text will report a larger height than the textBlock. Should work whether the
            // textBlock is single or multi-line.
            // The "formattedText.MinWidth > formattedText.MaxTextWidth" check detects if any
            // single line is too long to fit within the text area, this can only happen if there is a
            // long span of text with no spaces.
            return formattedText.Height > textBlock.ActualHeight || formattedText.Width - 1 > textBlock.ActualWidth;
        }
    }
}
