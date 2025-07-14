using System.Linq;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

using Brushes = UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared.Brushes;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer
{
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _editor;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor)
        {
            _editor = editor;
        }

        public KnownLayer Layer => KnownLayer.Selection;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();

            foreach (var startOffset in textView.VisualLines.Select(visualLine => visualLine.StartOffset))
            {
                var line = _editor.Document.GetLineByOffset(startOffset);
                var text = _editor.Document.GetText(startOffset, line.Length);

                if (!text.Contains("[ERROR]"))
                {
                    continue;
                }

                var segment = new TextSegment
                {
                    StartOffset = line.Offset,
                    EndOffset = line.EndOffset
                };

                foreach (var r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
                {
                    var border = new Pen(new SolidColorBrush(Color.FromArgb(30, 0xff, 0xff, 0xff)), 1);

                    drawingContext.DrawRoundedRectangle(
                        Brushes.SeverityErrorBrush,
                        border,
                        new Rect(r.Location, new Size(textView.ActualWidth, r.Height)),
                        3,
                        3);
                }
            }
        }
    }
}
