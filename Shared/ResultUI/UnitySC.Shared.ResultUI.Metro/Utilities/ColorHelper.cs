using System;
using System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Metro.Utilities
{
    public static class ColorHelper
    {
        public static Color ToMediaColor(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color GetIdealForeground(Color background)
        {
            // In case the color is too transparent
            if (background.A < 75) return Colors.Black;

            const int nThreshold = 105;
            int bgDelta = Convert.ToInt32(background.R * 0.299 + background.G * 0.587 + background.B * 0.114);
            return 255 - bgDelta < nThreshold ? Colors.Black : Colors.White;
        }
    }
}
