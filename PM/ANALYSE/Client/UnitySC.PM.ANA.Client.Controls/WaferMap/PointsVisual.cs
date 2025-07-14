using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Controls.WaferMap
{
    public class PointsVisual : DrawingVisual
    {
        public PointsVisual(FrameworkElement parentControl)
        {
            _parentControl = parentControl;
        }
        
        private FrameworkElement _parentControl;

        public void DrawPoints(List<Point> points, Brush pointsBrush, WaferDimensionalCharacteristic waferDimentionalCharac)
        {
            var displaySize = 4;

            if (points is null)
                return;

            DrawingContext drawingContext = this.RenderOpen();

            var drawingRatio = _parentControl.ActualWidth / waferDimentionalCharac.Diameter.Millimeters;
          
            foreach (var point in points)
            {

                var squarePos = CalculateRectanglePosition((Point)point, drawingRatio, displaySize, displaySize, waferDimentionalCharac);
              
                drawingContext.DrawRectangle(pointsBrush, null, new Rect(squarePos.X, squarePos.Y, displaySize, displaySize));
            }

            // Persist the drawing content.
            drawingContext.Close();
        }




        // Calculates the position of the rectangle in pixels based on the point position in wafer coordinates
        public static Point CalculateRectanglePosition(Point point, double drawingRatio, double rectangleWidth, double rectangleHeight, WaferDimensionalCharacteristic waferDimentionalCharac)
        {
            var left = point.X + waferDimentionalCharac.Diameter.Millimeters / 2;
            var top = waferDimentionalCharac.Diameter.Millimeters / 2 - point.Y;

            return new Point(left * drawingRatio - rectangleWidth / 2, top * drawingRatio - rectangleHeight / 2);
        }

        public void DrawPoints(List<Point> points, Brush pointsBrush, DieDimensionalCharacteristic dieDimentionalCharac)
        {
            var displaySize = 4;

            if (points is null)
                return;

            if (dieDimentionalCharac is null)
                return;

            DrawingContext drawingContext = this.RenderOpen();

           

            var drawingRatio = _parentControl.ActualWidth / (dieDimentionalCharac.DieWidth.Millimeters + dieDimentionalCharac.StreetWidth.Millimeters);

            foreach (var point in points)
            {

                var squarePos = CalculateRectanglePosition((Point)point, drawingRatio, displaySize, displaySize, dieDimentionalCharac);

                drawingContext.DrawRectangle(pointsBrush, null, new Rect(squarePos.X, squarePos.Y, displaySize, displaySize));
            }

            // Persist the drawing content.
            drawingContext.Close();
        }

        // Calculates the position of the rectangle in pixels based on the point position in wafer coordinates
        public static Point CalculateRectanglePosition(Point point, double drawingRatio, double rectangleWidth, double rectangleHeight, DieDimensionalCharacteristic dieDimentionalCharac)
        {
            var left = point.X + dieDimentionalCharac.StreetWidth.Millimeters/2;
            var top = dieDimentionalCharac.DieHeight.Millimeters + dieDimentionalCharac.StreetHeight.Millimeters / 2 - point.Y;

            return new Point(left * drawingRatio - rectangleWidth / 2, top * drawingRatio - rectangleHeight / 2);
        }

    }
}
