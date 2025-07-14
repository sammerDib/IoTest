using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls
{
    public class TextSlider : Slider
    {
        // Using a DependencyProperty as the backing store for TickBarText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickBarTextProperty =
            DependencyProperty.Register(nameof(TickText), typeof(string), typeof(TextSlider),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TickTextColorProperty =
            DependencyProperty.Register(nameof(TickTextColor), typeof(Brush), typeof(TextSlider),
                new PropertyMetadata(Brushes.Black));

        /// <summary>
        ///     Text display on the tick
        ///     string slit by ','
        ///     Exemple: text1,text2,text3
        /// </summary>
        public string TickText
        {
            get => (string)GetValue(TickBarTextProperty);
            set => SetValue(TickBarTextProperty, value);
        }

        public Brush TickTextColor
        {
            get => (Brush)GetValue(TickTextColorProperty);
            set => SetValue(TickTextColorProperty, value);
        }
    }

    public class TextTickBar : TickBar
    {
        // Using a DependencyProperty as the backing store for TickBarText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickBarTextProperty =
            DependencyProperty.Register("TickBarText", typeof(string), typeof(TextTickBar),
                new PropertyMetadata(string.Empty));

        /// <summary>
        ///     Text display on the tick
        ///     string slit by ','
        ///     Exemple: text1,text2,text3
        /// </summary>
        public string TickBarText
        {
            get => (string)GetValue(TickBarTextProperty);
            set => SetValue(TickBarTextProperty, value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (!string.IsNullOrEmpty(TickBarText))
            {
                string[] textArray = TickBarText.Split(',');
                FormattedText formattedText;
                var size = new Size(ActualWidth, ActualHeight);
                double num = Maximum - Minimum;
                double num5 = ReservedSpace * 0.5;

                // size.Width can not be negative
                if (size.Width > ReservedSpace)
                {
                    size.Width -= ReservedSpace;
                    double tickFrequencySize = size.Width * TickFrequency / (Maximum - Minimum);
                    int j = 0;
                    for (double i = 0; i <= num; i += TickFrequency)
                    {
                        if (textArray != null && textArray.Count() > j && !string.IsNullOrEmpty(textArray[j]))
                        {
                            var parentSlider = TemplatedParent as TextSlider;
                            var parentTypeFace = new Typeface(parentSlider.FontFamily, parentSlider.FontStyle,
                                parentSlider.FontWeight, parentSlider.FontStretch);
#pragma warning disable CS0618 // Type or member is obsolete
                            formattedText = new FormattedText(textArray[j], CultureInfo.InvariantCulture,
                                FlowDirection.LeftToRight, parentTypeFace, parentSlider.FontSize, Brushes.Black);
#pragma warning restore CS0618 // Type or member is obsolete
                            dc.DrawText(formattedText,
                                new Point(tickFrequencySize * (i / TickFrequency) + num5 - formattedText.Width / 2,
                                    10));
                        }

                        j++;
                    }
                }
            }

            base.OnRender(dc); //This is essential so that tick marks are displayed.
        }
    }
}
