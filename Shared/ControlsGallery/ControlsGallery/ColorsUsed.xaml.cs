using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlsGallery
{
    /// <summary>
    /// Interaction logic for Colors.xaml
    /// </summary>
    public partial class ColorsUsed : UserControl
    {
        public class SortColors : IComparer<Color>
        {
            public int Compare(Color x, Color y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                var xHue = System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B).GetHue();
                var xBrightness = System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B).GetBrightness();
                var xSaturation = System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B).GetSaturation();
                var yHue = System.Drawing.Color.FromArgb(y.A, y.R, y.G, y.B).GetHue();
                var yBrightness = System.Drawing.Color.FromArgb(y.A, y.R, y.G, y.B).GetBrightness();
                var ySaturation = System.Drawing.Color.FromArgb(y.A, y.R, y.G, y.B).GetSaturation();

                if (xSaturation < 0.01 && ySaturation < 0.01)
                    return xBrightness.CompareTo(yBrightness);
                if (xSaturation < 0.01)
                    return -1;
                if (ySaturation < 0.01)
                    return 1;

                if (Math.Abs(xHue - yHue) < 5)
                    return xBrightness.CompareTo(yBrightness);

                return xHue.CompareTo(yHue);
            }
        }

        public class NamedColor
        {
            public Color Value { get; set; }
            public String Name { get; set; }
        }

        public ColorsUsed()
        {
            InitializeComponent();
            List<NamedColor> colorsUsed = new List<NamedColor>();

            foreach (var mergedDictionnary in Application.Current.Resources.MergedDictionaries)
            {
                if (mergedDictionnary.Source.LocalPath.Contains("UnityStylesNew"))
                {
                    foreach (DictionaryEntry dictionaryEntry in mergedDictionnary)
                    {
                        if (dictionaryEntry.Value is Color)
                        {
                            colorsUsed.Add(new NamedColor() { Name = dictionaryEntry.Key.ToString(), Value = (Color)dictionaryEntry.Value });
                        }
                    }
                }
            }

            var sortcolors = new SortColors();

            this.DataContext = (colorsUsed.OrderBy(x => x.Value, sortcolors)).ToList();
        }
    }
}
