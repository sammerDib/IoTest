using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace UnitySC.PM.ANA.Client.Controls.WaferMap
{
    public class DiesVisual : DrawingVisual
    {
        public DiesVisual(WaferMapDisplayControl parentControl)
        {
            _parentControl = parentControl;
        }

        private WaferMapDisplayControl _parentControl;

        public void DrawDies(Brush diesBrush, Brush textBrush)
        {
            DrawingContext drawingContext = RenderOpen();
            if (_parentControl.WaferMap is null)
            {
                drawingContext.Close();
                return;
            }
            var WaferMap = _parentControl.WaferMap;

            var WaferDimentionalCharac = _parentControl.WaferDimentionalCharac;

            var offsetLeft = WaferMap.DieGridTopLeft.X + WaferDimentionalCharac.Diameter.Millimeters / 2;
            var offsetTop = WaferDimentionalCharac.Diameter.Millimeters / 2 - WaferMap.DieGridTopLeft.Y;

            double dieHorizontalPitch = WaferMap.DieDimensions.DieWidth.Millimeters + WaferMap.DieDimensions.StreetWidth.Millimeters;
            double dieVerticalPitch = WaferMap.DieDimensions.DieHeight.Millimeters + WaferMap.DieDimensions.StreetHeight.Millimeters;

            var drawingRatio = _parentControl.ActualWidth / WaferDimentionalCharac.Diameter.Millimeters;

            double dieWidth = WaferMap.DieDimensions.DieWidth.Millimeters * drawingRatio;
            double dieHeight = WaferMap.DieDimensions.DieHeight.Millimeters * drawingRatio;

            double left;
            double top;

            for (int row = 0; row < WaferMap.NbRows; row++)

            {
                top = (offsetTop + row * dieVerticalPitch) * drawingRatio;

                for (int column = 0; column < WaferMap.NbColumns; column++)

                {
                    if (WaferMap.DiesPresence.GetValue(row, column))
                    {
                        left = (offsetLeft + (column * dieHorizontalPitch)) * drawingRatio;

                        var drawingRect = new Rect(left, top, dieWidth, dieHeight);
                        drawingContext.DrawRectangle(diesBrush, null, drawingRect);
                     }
                }
            }
            // Persist the drawing content.
            drawingContext.Close();
        }
    }
}
