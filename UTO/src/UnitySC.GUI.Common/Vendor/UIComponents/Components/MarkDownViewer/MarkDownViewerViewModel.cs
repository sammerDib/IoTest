using System;

using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

using Markdig;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.MarkDownViewer
{
    public class MarkDownViewerViewModel : Notifier
    {
        public MarkDownViewerViewModel(string content)
        {
            content = Markdown.ToHtml(content);
            var color = Brushes.ForegroundBrush.Color;
            var stringColor = $"rgb({color.R}, {color.G}, {color.B})";
            Content = $"<p style=\"color :{stringColor}; \">{Environment.NewLine}{content}{Environment.NewLine}</p>";
        }

        public string Content { get; set; }
    }
}
