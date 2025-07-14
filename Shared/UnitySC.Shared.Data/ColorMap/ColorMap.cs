using System.Drawing;

namespace UnitySC.Shared.Data.ColorMap
{
    public class ColorMap
    {
        public ColorMap(string name, Color[] colors, Bitmap bitmap)
        {
            Name = name;
            Colors = colors;
            Bitmap = bitmap;
        }

        public Color[] Colors { get; private set; }

        public Bitmap Bitmap { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            return $"{Name} ({Colors.Length})";
        }

        public Color GetColorFromValue(float value, float a, float b)
        {
            int colorIndex = ColorMapHelper.GetColorIndexFromValue(value, a, b, Colors.Length);
            return colorIndex < 0 ? Color.Transparent : Colors[colorIndex];
        }
    }
}
