using System;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

using Brushes = UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared.Brushes;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.AvalonEdit
{

    public class SearchResultBackgroundRenderer : IBackgroundRenderer
    {
        public TextSegmentCollection<SearchResult> CurrentResults { get; } = new();

        // Draw behind selection
        public KnownLayer Layer => KnownLayer.Selection;

        public SearchResultBackgroundRenderer()
        {
            MarkerBrush = Brushes.SelectionBackgroundBrush;
            MarkerPen = null;
            MarkerCornerRadius = 3.0;
        }

        public Brush MarkerBrush { get; set; }

        public Pen MarkerPen { get; set; }

        public double MarkerCornerRadius { get; set; }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView == null) throw new ArgumentNullException(nameof(textView));
            if (drawingContext == null) throw new ArgumentNullException(nameof(drawingContext));

            if (!textView.VisualLinesValid) return;

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0) return;

            var viewStart = visualLines.First().FirstDocumentLine.Offset;
            var viewEnd = visualLines.Last().LastDocumentLine.EndOffset;

            var markerBrush = MarkerBrush;
            var markerPen = MarkerPen;
            var markerCornerRadius = MarkerCornerRadius;
            var markerPenThickness = markerPen?.Thickness ?? 0;

            foreach (var result in CurrentResults.FindOverlappingSegments(
                         viewStart,
                         viewEnd - viewStart))
            {
                var geoBuilder = new BackgroundGeometryBuilder
                {
                    AlignToWholePixels = true,
                    BorderThickness = markerPenThickness,
                    CornerRadius = markerCornerRadius
                };
                geoBuilder.AddSegment(textView, result);
                var geometry = geoBuilder.CreateGeometry();
                if (geometry != null)
                {
                    drawingContext.DrawGeometry(markerBrush, markerPen, geometry);
                }
            }
        }
    }
}
