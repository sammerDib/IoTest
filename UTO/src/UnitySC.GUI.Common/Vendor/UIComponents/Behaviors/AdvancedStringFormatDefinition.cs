using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    /// <summary>
    /// Allows you to define formatted ends of strings allowing a <see cref="TextBlock"/> to display syntactically formatted text with a single binding.
    /// To do this, it is necessary to use the AttachedProperty <see cref="AdvancedStringFormatTextBlockBehavior.DefinitionsProperty"/> on the <see cref="TextBlock"/>.
    /// </summary>
    public class AdvancedStringFormatDefinition
    {
        public bool Bold { get; set; }

        public bool Highlighted { get; set; }

        public string Text { get; }

        public AdvancedStringFormatDefinition(string text)
        {
            Text = text;
        }
    }
}
