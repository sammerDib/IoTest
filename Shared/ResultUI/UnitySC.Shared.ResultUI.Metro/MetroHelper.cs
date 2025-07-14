using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.UI.Controls;

using Color = System.Windows.Media.Color;

namespace UnitySC.Shared.ResultUI.Metro
{
    public static class MetroHelper
    {
        private const int ShapeSize = 16;

        private static Dictionary<MeasureState, Color> _measureStateToColors;

        public static Color GetColor(this MeasureState measureState)
        {
            if (_measureStateToColors == null)
            {
                _measureStateToColors = new Dictionary<MeasureState, Color>
                {
                    { MeasureState.Success, Color.FromArgb(0xFF, 0x08, 0xB4, 0x08) },
                    { MeasureState.Partial, Color.FromArgb(0xFF, 0xF9, 0x81, 0x02) },
                    { MeasureState.Error, Color.FromArgb(0xFF, 0xD8, 0x12, 0x12) },
                    { MeasureState.NotMeasured, Color.FromArgb(0xFF, 0x97, 0x00, 0x4D) }
                };
            }

            return _measureStateToColors.ContainsKey(measureState) ? _measureStateToColors[measureState] : Colors.Black;
        }

        public static SeriesMarkerPointShapeStyle GetSymbol(MeasureState state)
        {
            var symbol = new SeriesMarkerPointShapeStyle
            {
                Width = ShapeSize,
                Height = ShapeSize,
                UseImageSize = false,
                BodyThickness = 3,
                BorderColor = Color.FromArgb(200, 0x29, 0x29, 0x29),
                BorderWidth = 1
            };
            
            switch (state)
            {
                case MeasureState.Success:

                    symbol.Color1 = ToleranceDisplayer.GoodColorBrush.Color;
                    symbol.Color2 = ToleranceDisplayer.GoodColorBrush.Color;
                    symbol.Color3 = ToleranceDisplayer.GoodColorBrush.Color;
                    symbol.Shape = SeriesMarkerPointShape.Rectangle;
                    symbol.Height = ShapeSize - 2;
                    symbol.Width = ShapeSize - 2;
                    return symbol;

                case MeasureState.Partial:

                    symbol.Color1 = ToleranceDisplayer.WarningColorBrush.Color;
                    symbol.Color2 = ToleranceDisplayer.WarningColorBrush.Color;
                    symbol.Color3 = ToleranceDisplayer.WarningColorBrush.Color;
                    symbol.Shape = SeriesMarkerPointShape.Cross;
                    return symbol;

                case MeasureState.Error:

                    symbol.Color1 = ToleranceDisplayer.BadColorBrush.Color;
                    symbol.Color2 = ToleranceDisplayer.BadColorBrush.Color;
                    symbol.Color3 = ToleranceDisplayer.BadColorBrush.Color;
                    symbol.Shape = SeriesMarkerPointShape.Cross;
                    symbol.Angle = 45;
                    return symbol;

                case MeasureState.NotMeasured:

                    symbol.Color1 = ToleranceDisplayer.NotMeasuredColorBrush.Color;
                    symbol.Color2 = ToleranceDisplayer.NotMeasuredColorBrush.Color;
                    symbol.Color3 = ToleranceDisplayer.NotMeasuredColorBrush.Color;
                    symbol.Shape = SeriesMarkerPointShape.Triangle;
                    return symbol;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public const int MinDigitsDisplayCount = 2;
        public const int MaxDigitsDisplayCount = 5;

        public static int GetDecimalCount(double? value)
        {
            if (!value.HasValue) return MinDigitsDisplayCount;

            string stringValue = value.Value.ToString(CultureInfo.InvariantCulture);
            if (stringValue.Contains("."))
            {
                int index = stringValue.IndexOf('.');
                int count = stringValue.Length - 1 - index;
                if (count < MinDigitsDisplayCount) return MinDigitsDisplayCount;
                if (count > MaxDigitsDisplayCount) return MaxDigitsDisplayCount;
                return count;
            }

            return MinDigitsDisplayCount;
        }

        public static int GetDecimalCount(params double?[] values)
        {
            return values.Select(GetDecimalCount).Max();
        }

        public static List<Point> InterBresenhamLine(int x0, int y0, int x1, int y1)
        {
            var indexList = new List<Point>();

            int x, y, e;
            int dx = x1 - x0;
            int dy = y1 - y0;
            int tmp;

            if (Math.Abs(dy) <= Math.Abs(dx))
            {
                if (x1 < x0)
                {
                    tmp = x0; x0 = x1; x1 = tmp;
                    tmp = y0; y0 = y1; y1 = tmp;
                }

                dx = x1 - x0; dy = y1 - y0;
                if (y0 <= y1)
                {
                    e = -dx;
                    x = x0; y = y0;
                    for (int i = 0; i <= dx; i++)
                    {
                        indexList.Add(new Point(x, y));
                        x++;
                        e += 2 * dy;
                        if (e >= 0)
                        {
                            y++;
                            e -= 2 * dx;
                        }
                    }
                }
                else
                {
                    e = dx;
                    x = x0; y = y0;
                    for (int i = 0; i <= dx; i++)
                    {
                        indexList.Add(new Point(x, y));
                        x++;
                        e += 2 * dy;
                        if (e <= 0)
                        {
                            --y;
                            e += 2 * dx;
                        }
                    }
                }
            }
            else
            {
                if (y1 < y0)
                {
                    tmp = x0; x0 = x1; x1 = tmp;
                    tmp = y0; y0 = y1; y1 = tmp;
                }

                dx = x1 - x0; dy = y1 - y0;
                if (x0 <= x1)
                {
                    e = -dy;
                    x = x0; y = y0;
                    for (int i = 0; i <= dy; i++)
                    {
                        indexList.Add(new Point(x, y));
                        y++;
                        e += 2 * dx;
                        if (e >= 0)
                        {
                            x++;
                            e -= 2 * dy;
                        }
                    }
                }
                else
                {
                    e = dy;
                    x = x0; y = y0;
                    for (int i = 0; i <= dy; i++)
                    {
                        indexList.Add(new Point(x, y));
                        y++;
                        e += 2 * dx;
                        if (e <= 0)
                        {
                            --x;
                            e += 2 * dy;
                        }
                    }
                    // we need to revert point list in order to have x correctly ordered 
                    indexList.Reverse();
                }
            }

            return indexList;
        }

        public static double GetWaferDiameterMillimeters(MeasureResultBase resultBase)
        {
            double? diameter = resultBase?.Wafer?.Diameter?.Millimeters;
            double? tolerance = resultBase?.Wafer?.DiameterTolerance?.Millimeters;
            
            // Default Value
            if (!diameter.HasValue) return 300;
            
            if (tolerance.HasValue)
            {
                return diameter.Value + tolerance.Value;
            }

            return diameter.Value;
        }
    }
}
