using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.UI.Controls
{
    public class WaferVisual : DrawingVisual
    {
        private double _width;

        // in pixels
        public double Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                }
            }
        }

        private double _height;

        // in pxels
        public double Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                }
            }
        }

        private Length _edgeExclusionThickness;

        // Edge Exclusion Thickness in mm
        public Length EdgeExclusionThickness
        {
            get { return _edgeExclusionThickness; }
            set
            {
                if (_edgeExclusionThickness != value)
                {
                    _edgeExclusionThickness = value;
                }
            }
        }

        private WaferDimensionalCharacteristic _waferInfos;

        public WaferDimensionalCharacteristic WaferInfos
        {
            get { return _waferInfos; }
            set
            {
                _waferInfos = value;
            }
        }

        public Brush WaferBrush { get; set; }
        public Pen WaferBorderPen { get; set; }
        public Brush EdgeExclusionBrush { get; set; }

        public WaferVisual()
        {
            WaferBrush = new SolidColorBrush(Colors.Gray);
            WaferBorderPen = new Pen(new SolidColorBrush(Colors.Black), 2);
            EdgeExclusionThickness = 0.Millimeters();
            EdgeExclusionBrush = new SolidColorBrush(Colors.Pink);
        }

        public WaferVisual(WaferDimensionalCharacteristic waferInfos, double width, double height) : this()
        {
            _width = width;
            _height = height;
            _waferInfos = waferInfos;
            Render();
        }

        private void Render()
        {
            if (_waferInfos == null)
                return;
            if ((_width == 0) || (_height == 0))
                return;

            using (var drawingContext = this.RenderOpen())
            {
                VisualEdgeMode = EdgeMode.Unspecified;

                double widthDrawing = Width;
                double heightDrawing = Height;
                double radiusDrawing = Math.Min(widthDrawing, heightDrawing) / 2; // Math.Min(parent.ActualWidth, parent.ActualHeight)/2;

                if (radiusDrawing == 0)
                    return;

                Geometry waferGeometry = null;
                if (_waferInfos.WaferShape == WaferShape.NonFlat)
                {
                    waferGeometry = DefiningNonFlatWafer(_waferInfos, radiusDrawing, new Point(widthDrawing / 2, heightDrawing / 2));
                }
                if (_waferInfos.WaferShape == WaferShape.Flat)
                {
                    waferGeometry = DefiningFlatWafer(_waferInfos, radiusDrawing, new Point(widthDrawing / 2, heightDrawing / 2));
                }
                if (_waferInfos.WaferShape == WaferShape.Notch)
                {
                    waferGeometry = DefiningNotchWafer(_waferInfos, radiusDrawing, new Point(widthDrawing / 2, heightDrawing / 2));
                }
                if (_waferInfos.WaferShape == WaferShape.Sample)
                {
                    waferGeometry = DefiningSampleWafer(_waferInfos, widthDrawing, heightDrawing);
                }

                if (waferGeometry == null)
                    return;

                if (EdgeExclusionThickness.Millimeters > 0)
                {
                    if (EdgeExclusionThickness > _waferInfos.Diameter / 2)
                        EdgeExclusionThickness = _waferInfos.Diameter / 2;

                    double edgeExclusionRatio = 1 - EdgeExclusionThickness.Millimeters / (_waferInfos.Diameter.Millimeters / 2);

                    var WaferInGeometry = waferGeometry.Clone();
                    // We scale the wafer to exclude the edge exclusion
                    WaferInGeometry.Transform = new ScaleTransform(edgeExclusionRatio, edgeExclusionRatio, Width / 2, Height / 2);

                    drawingContext.DrawGeometry(EdgeExclusionBrush, WaferBorderPen, waferGeometry);
                    drawingContext.DrawGeometry(WaferBrush, null, WaferInGeometry);
                }
                else
                {
                    // We draw the Wafer without any edge exclusion
                    drawingContext.DrawGeometry(WaferBrush, WaferBorderPen, waferGeometry);
                }
            }
        }

        public static Point PolarToCartesian(double angle, double radius, Point center)
        {
            double angleRad = Math.PI / 180.0 * (angle - 90);
            double x = radius * Math.Cos(angleRad) + center.X;
            double y = radius * Math.Sin(angleRad) + center.Y;
            return new Point(x, y);
        }

        private Geometry DefiningNonFlatWafer(WaferDimensionalCharacteristic wafer, double drawingRadius, Point drawingCenter)
        {
            Geometry g = new EllipseGeometry(drawingCenter, drawingRadius, drawingRadius);

            return g;
        }

        private Geometry DefiningFlatWafer(WaferDimensionalCharacteristic wafer, double drawingRadius, Point drawingCenter)
        {
            var segments = new List<PathSegment>();

            Point startPoint = default;
            double startAngle = 0;

            for (int i = 0; i < wafer.Flats.Count; i++)
            {
                var flat = wafer.Flats[i];
                //double flatAngle = Math.Asin(flat.ChordLength/wafer.Diameter)*180/Math.PI;
                double flatAngle = Math.Asin(flat.ChordLength.Millimeters / wafer.Diameter.Millimeters) * 2 * 180 / Math.PI;
                double startFlatAngle = 180 + flat.Angle.Degrees - flatAngle / 2;
                if (i == 0)
                    startAngle = startFlatAngle;
                double startArcAngle = startFlatAngle + flatAngle;

                double sweepAngle;
                // if we are on the last flat
                if (i == wafer.Flats.Count - 1)
                {
                    sweepAngle = startAngle - startArcAngle;
                }
                else
                {
                    var nextFlat = wafer.Flats[i + 1];
                    double nextFlatAngle = Math.Asin(nextFlat.ChordLength.Millimeters / wafer.Diameter.Millimeters) * 2 * 180 / Math.PI;
                    double nextStartFlatAngle = 180 + nextFlat.Angle.Degrees - nextFlatAngle / 2;
                    sweepAngle = nextStartFlatAngle - startArcAngle;
                }
                if (sweepAngle < 0) sweepAngle += 360;

                bool isLarge = startArcAngle + sweepAngle - startArcAngle > 180;
                var currentStartPoint = PolarToCartesian(startArcAngle, drawingRadius, drawingCenter);
                var currentEndPoint = PolarToCartesian(startArcAngle + sweepAngle, drawingRadius, drawingCenter);
                if (i == 0)
                    startPoint = currentStartPoint;
                // We add a line if it is not the first flat
                if (i > 0)
                    segments.Add(new LineSegment(currentStartPoint, true));
                segments.Add(new ArcSegment(currentEndPoint, new Size(drawingRadius, drawingRadius), 0.0, isLarge, SweepDirection.Clockwise, true));
            }

            var figures = new List<PathFigure>(1);
            var pf = new PathFigure(startPoint, segments, true)
            {
                IsClosed = true
            };
            figures.Add(pf);
            Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);

            return g;
        }

        public void Draw()
        {
            Render();
        }

        private Geometry DefiningNotchWafer(WaferDimensionalCharacteristic wafer, double drawingRadius, Point drawingCenter)
        {
            var segments = new List<PathSegment>();

            var notch = wafer.Notch;

            // We multiply by 2 the real size of the notch in order to make it visible on the drawing
            double notchDepthDrawing = notch.Depth.Millimeters * drawingRadius * 2 / wafer.Diameter.Millimeters * 2;
            //double flatAngle = Math.Asin(flat.ChordLength/wafer.Diameter)*180/Math.PI;
            double notchAngle = Math.Asin(notchDepthDrawing / drawingRadius) * 2 * 180 / Math.PI;
            double startNotchAngle = 180 + notch.Angle.Degrees - notchAngle / 2;

            double sweepAngle = 360 - notchAngle;

            double startArcAngle = startNotchAngle + notchAngle;

            bool isLarge = startNotchAngle + sweepAngle - startArcAngle > 180;
            var arcStartPoint = PolarToCartesian(startArcAngle, drawingRadius, drawingCenter);
            var arcEndPoint = PolarToCartesian(startArcAngle + sweepAngle, drawingRadius, drawingCenter);
            segments.Add(new ArcSegment(arcEndPoint, new Size(drawingRadius, drawingRadius), 0.0, isLarge, SweepDirection.Clockwise, true));
            segments.Add(new ArcSegment(arcStartPoint, new Size(notchDepthDrawing, notchDepthDrawing), 0.0, false, SweepDirection.Counterclockwise, true));

            var figures = new List<PathFigure>(1);
            var pf = new PathFigure(arcStartPoint, segments, true)
            {
                IsClosed = true
            };
            figures.Add(pf);
            Geometry g = new PathGeometry(figures, FillRule.EvenOdd, null);

            return g;
        }

        private Geometry DefiningSampleWafer(WaferDimensionalCharacteristic wafer, double widthDrawing, double heightDrawing)
        {
            double ratio = Math.Min(widthDrawing / wafer.SampleWidth.Millimeters, heightDrawing / wafer.SampleHeight.Millimeters);

            var rect = new Rect(new Point((widthDrawing - wafer.SampleWidth.Millimeters * ratio) / 2, (heightDrawing - wafer.SampleHeight.Millimeters * ratio) / 2),
                                new Point((widthDrawing + wafer.SampleWidth.Millimeters * ratio) / 2, (heightDrawing + wafer.SampleHeight.Millimeters * ratio) / 2));
            Geometry g = new RectangleGeometry(rect);

            return g;
        }
    }
}
