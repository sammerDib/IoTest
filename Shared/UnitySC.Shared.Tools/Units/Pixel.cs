using System.Windows;

namespace UnitySC.Shared.Tools.Units
{
    static public class Pixel
    {
        // Converts Length into pixels
        public static double ToPixels(this Length length, Length pixelSize)
        {
            return length.Micrometers / pixelSize.Micrometers;
        }

        public static Point CenterReferentialToTopLeft(Point point, double imageWidth, double imageHeight)
        {
            return new Point(point.X + imageWidth / 2, imageHeight / 2 - point.Y);
        }
    }
}
