using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class AdvancedStringFormatTextBlockBehavior
    {
        public static readonly DependencyProperty DefinitionsProperty = DependencyProperty.RegisterAttached(
            "Definitions", typeof(IEnumerable), typeof(AdvancedStringFormatTextBlockBehavior),
            new PropertyMetadata(default(IEnumerable), DefinitionChangedCallback));

        private static void DefinitionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                textBlock.Inlines.Clear();
                var definitions = GetDefinitions(textBlock);
                if (definitions == null) return;
                foreach (var stringFormatDefinition in definitions.OfType<AdvancedStringFormatDefinition>())
                {
                    var run = new Run(stringFormatDefinition.Text);
                    if (stringFormatDefinition.Bold)
                    {
                        run.FontWeight = FontWeights.Bold;
                    }

                    if (stringFormatDefinition.Highlighted)
                    {
                        run.Foreground = Brushes.HighlightBrush;
                    }

                    textBlock.Inlines.Add(run);
                }
            }
        }

        public static void SetDefinitions(DependencyObject element, IEnumerable value)
        {
            element.SetValue(DefinitionsProperty, value);
        }

        public static IEnumerable GetDefinitions(DependencyObject element)
        {
            return (IEnumerable)element.GetValue(DefinitionsProperty);
        }
    }
}
